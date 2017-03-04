using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace NHLGames.AdDetection.Modules.OBS
{
    public class ObsModuleViewModel : BindableBase
    {
        private ObsModuleSettings m_settings;
        private string m_gameSceneChar;
        private string m_adSceneChar;

        public ObsModuleControl SettingsControl { get; private set; }

        public string GameSceneChar
        {
            get { return m_gameSceneChar; }
            set
            {
                if (SetProperty(ref m_gameSceneChar, value))
                {
                    OnConfigChanged();
                }
            }
        }


        public string AdSceneChar
        {
            get { return m_adSceneChar; }
            set
            {
                if (SetProperty(ref m_adSceneChar, value))
                {
                    OnConfigChanged();
                }
            }
        }

        private void OnConfigChanged()
        {
            m_settings.GameSceneChar = GameSceneChar;
            m_settings.GameSceneCtrl = GameSceneCtrl;
            m_settings.GameSceneAlt = GameSceneAlt;
            m_settings.GameSceneShift = GameSceneShift;

            m_settings.AdSceneChar = AdSceneChar;
            m_settings.AdSceneCtrl = AdSceneCtrl;
            m_settings.AdSceneAlt = AdSceneAlt;
            m_settings.AdSceneShift = AdSceneShift;

            ObsModuleSettings.Save(m_settings);
        }

        public ObsModuleViewModel()
        {
            LoadSettings();

            SettingsControl = new ObsModuleControl(this);
        }

        private void LoadSettings()
        {
            m_settings = ObsModuleSettings.Load();

            m_gameSceneChar = m_settings.GameSceneChar;
            m_gameSceneCtrl = m_settings.GameSceneCtrl;
            m_gameSceneAlt = m_settings.GameSceneAlt;
            m_gameSceneShift = m_settings.GameSceneShift;

            m_adSceneChar = m_settings.AdSceneChar;
            m_adSceneCtrl = m_settings.AdSceneCtrl;
            m_adSceneAlt = m_settings.AdSceneAlt;
            m_adSceneShift = m_settings.AdSceneShift;
        }





        private bool m_gameSceneCtrl;
        public bool GameSceneCtrl
        {
            get { return m_gameSceneCtrl; }
            set
            {
                if (SetProperty(ref m_gameSceneCtrl, value))
                {
                    OnConfigChanged();
                }
            }
        }

        private bool m_gameSceneAlt;
        public bool GameSceneAlt
        {
            get { return m_gameSceneAlt; }
            set
            {
                if (SetProperty(ref m_gameSceneAlt, value))
                {
                    OnConfigChanged();
                }
            }
        }

        private bool m_gameSceneShift;
        public bool GameSceneShift
        {
            get { return m_gameSceneShift; }
            set
            {
                if (SetProperty(ref m_gameSceneShift, value))
                {
                    OnConfigChanged();
                }
            }
        }

        private bool m_adSceneCtrl;
        public bool AdSceneCtrl
        {
            get { return m_adSceneCtrl; }
            set
            {
                if (SetProperty(ref m_adSceneCtrl, value))
                {
                    OnConfigChanged();
                }
            }
        }

        private bool m_adSceneAlt;
        public bool AdSceneAlt
        {
            get { return m_adSceneAlt; }
            set
            {
                if (SetProperty(ref m_adSceneAlt, value))
                {
                    OnConfigChanged();
                }
            }
        }

        private bool m_adSceneShift;
        public bool AdSceneShift
        {
            get { return m_adSceneShift; }
            set
            {
                if (SetProperty(ref m_adSceneShift, value))
                {
                    OnConfigChanged();
                }
            }
        }
    }
}
