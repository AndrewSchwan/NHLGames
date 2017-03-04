using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using NHLGames.AdDetection.AdDetectors;
using NHLGames.AdDetection.Common;
using Prism.Commands;
using Prism.Mvvm;

namespace NHLGames.AdDetection
{
    public class AdDetectorViewModel : BindableBase
    {
        private bool m_adDetectingEnabled;

        private IAdDetectionEngineDescriptor m_selectedAdDetectionEngineDescriptor;

        private ObservableCollection<IAdDetectionEngineDescriptor> m_adDetectionEngines;

        private AdDetectionEngineBase m_currentEngine;

        private ObservableCollection<IAdModule> m_disabledModules;

        private DelegateCommand<IAdModule> m_disableModuleCommand;

        private ObservableCollection<IAdModule> m_enabledModules;

        private DelegateCommand<IAdModule> m_enableModuleCommand;

        private AdDetectionEngineType m_engineType;

        private Dictionary<string, IAdModule> m_modules;


        private IAdModule m_selectedModule;

        private AdDetectionSettings m_settings;

        public UserControl SettingsControl;

        public AdDetectorViewModel()
        {
            EnableModuleCommand.ObservesProperty(() => SelectedModule);
            DisableModuleCommand.ObservesProperty(() => SelectedModule);

            AdDetectionEngines = new ObservableCollection<IAdDetectionEngineDescriptor>
            {
                new VolumeAdDetectionDescriptor(),
                new ScreenAdDetectionDescriptor()
            };

            ImportModules();

            LoadConfig();

            RemoveUninstalledModulesFromConfig();


            SettingsControl = new AdDetectorUserControl(this);

            switch (m_engineType)
            {
                case AdDetectionEngineType.FullScreenImage:
                    m_currentEngine = new ScreenAdDetectionEngine();
                    m_currentEngine.Start(m_modules.Where(x => m_settings.EnabledModules.Contains(x.Key)).Select(x => x.Value).ToList());
                    break;
                case AdDetectionEngineType.PlayerSystemVolume:
                    m_currentEngine = new VolumeAdDetectionEngine();
                    m_currentEngine.Start(m_modules.Where(x => m_settings.EnabledModules.Contains(x.Key)).Select(x => x.Value).ToList());
                    break;
            }
        }

        public IAdDetectionEngineDescriptor SelectedAdDetectionEngineDescriptor
        {
            get { return m_selectedAdDetectionEngineDescriptor; }
            set
            {
                if (SetProperty(ref m_selectedAdDetectionEngineDescriptor, value))
                {
                    AdDetectionEngineChanged();
                }
            }
        }

        public ObservableCollection<IAdDetectionEngineDescriptor> AdDetectionEngines
        {
            get { return m_adDetectionEngines; }
            set { SetProperty(ref m_adDetectionEngines, value); }
        }

        public DelegateCommand<IAdModule> EnableModuleCommand
            =>
                m_enableModuleCommand =
                    m_enableModuleCommand ?? new DelegateCommand<IAdModule>(EnableModule, CanEnableModuleExecute);

        public DelegateCommand<IAdModule> DisableModuleCommand
            =>
                m_disableModuleCommand =
                    m_disableModuleCommand ?? new DelegateCommand<IAdModule>(DisableModule, CanDisableModuleExecute);


        public AdDetectionEngineType EngineType
        {
            get { return m_engineType; }
            set { SetProperty(ref m_engineType, value); }
        }

        public IAdModule SelectedModule
        {
            get { return m_selectedModule; }
            set
            {
                SetProperty(ref m_selectedModule, null);

                SetProperty(ref m_selectedModule, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IAdModule> EnabledModules
        {
            get { return m_enabledModules; }
            set { SetProperty(ref m_enabledModules, value); }
        }

        public ObservableCollection<IAdModule> DisabledModules
        {
            get { return m_disabledModules; }
            set { SetProperty(ref m_disabledModules, value); }
        }

        public bool AdDetectingEnabled
        {
            get { return m_adDetectingEnabled; }
            set
            {
                if (SetProperty(ref m_adDetectingEnabled, value))
                {
                    EnableChanged();
                }
            }
        }

        private void AdDetectionEngineChanged()
        {
            if (SelectedAdDetectionEngineDescriptor != null &&
                SelectedAdDetectionEngineDescriptor.Type != m_settings.EngineType)
            {
                m_settings.EngineType = SelectedAdDetectionEngineDescriptor.Type;

                AdDetectionSettings.Save(m_settings);
            }
        }

        public void LoadConfig()
        {
            m_settings = AdDetectionSettings.Load();

            m_adDetectingEnabled = m_settings.IsEnabled;

            m_enabledModules =
                new ObservableCollection<IAdModule>(
                    m_modules.Where(x => m_settings.EnabledModules.Contains(x.Key)).Select(x => x.Value));
            m_disabledModules =
                new ObservableCollection<IAdModule>(
                    m_modules.Where(x => !m_settings.EnabledModules.Contains(x.Key)).Select(x => x.Value));

            m_selectedAdDetectionEngineDescriptor = AdDetectionEngines.FirstOrDefault(x => x.Type == m_settings.EngineType);
        }


        private bool CanDisableModuleExecute(IAdModule x)
        {
            return x != null && m_enabledModules.Contains(x);
        }

        private bool CanEnableModuleExecute(IAdModule x)
        {
            return x != null && m_disabledModules.Contains(x);
        }

        private void EnableModule(IAdModule module)
        {
            DisabledModules.Remove(module);
            EnabledModules.Add(module);

            m_settings.EnabledModules = EnabledModules.Select(x => x.Title).ToList();
            AdDetectionSettings.Save(m_settings);
        }

        private void DisableModule(IAdModule module)
        {
            EnabledModules.Remove(module);
            DisabledModules.Add(module);

            m_settings.EnabledModules = EnabledModules.Select(x => x.Title).ToList();
            AdDetectionSettings.Save(m_settings);
        }

        private void RemoveUninstalledModulesFromConfig()
        {
            var removedModules =
                m_settings.EnabledModules.Where(enabledModule => !m_modules.ContainsKey(enabledModule)).ToList();

            m_settings.EnabledModules.RemoveAll(x => removedModules.Contains(x));
            AdDetectionSettings.Save(m_settings);
        }

        private void ImportModules()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var modulesDirectory = Directory.GetDirectories(assemblyDirectory + @"\AdModules");


            var catalog = new AggregateCatalog();
            foreach (var module in modulesDirectory)
            {
                catalog.Catalogs.Add(
                    new DirectoryCatalog(module));
            }

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            m_modules = container.GetExportedValues<IAdModule>().ToDictionary(k => k.Title, v => v);
        }

        private void EnableChanged()
        {
            if (m_settings.IsEnabled != AdDetectingEnabled)
            {
                m_settings.IsEnabled = AdDetectingEnabled;

                AdDetectionSettings.Save(m_settings);
            }
        }
    }
}