using System;
using System.Diagnostics;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using NHLGames.AdDetection.Common;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UserControl = System.Windows.Controls.UserControl;

namespace NHLGames.AdDetection.Modules.OBS
{
    /// <summary>
    ///     A template of a sample module
    ///     See Spotify module for an implemented example.
    /// </summary>
    /// <remarks>
    ///     Export attribute is required so that it is loaded by the main program.
    ///     You must also output to NhlGames\AdModules\ModuleName folder to be loaded properly.
    /// </remarks>
    [Export(typeof(IAdModule))]
    public class ObsModule : IAdModule
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private readonly ObsModuleViewModel m_viewModel;
        public ObsModule()
        {
            m_viewModel = new ObsModuleViewModel();
        }

        public string Title => "OBS Scene Changer";

        public string Description => "Emulates pressing the keys you configured when an ad starts and stops to automatically change scenes for you in OBS Studio.";


        public UserControl SettingsControl => m_viewModel.SettingsControl;

        public void Initialize(){ }

        public void AdStarted()
        {
            var settings = ObsModuleSettings.Load();

            string toSend = String.Empty;
            bool modifier = false;
            if (settings.AdSceneCtrl)
            {
                toSend += "^";
                modifier = true;
            }
            if (settings.AdSceneAlt)
            {
                toSend += "%";
                modifier = true;
            }
            if (settings.AdSceneShift)
            {
                toSend += "+";
                modifier = true;
            }

            if (modifier)
            {
                toSend += "{";
                toSend += settings.AdSceneChar;
                toSend += "}";
            }
            else
            {
                toSend += settings.AdSceneChar;
            }

            if (!String.IsNullOrEmpty(toSend))
            {
                IntPtr? obs = hookOBS();
                IntPtr? curr = GetForegroundWindow();
                if (obs != null)
                {
                    Console.WriteLine("OBS: Changing to Ad Scene");
                    SetForegroundWindow((IntPtr)obs);
                    SendKeys.SendWait(toSend);
                    SetForegroundWindow((IntPtr)curr);
                }
                else
                {
                    Console.WriteLine("Error: OBS was not located");
                }

            }
        }

        public void AdEnded()
        {
            var settings = ObsModuleSettings.Load();

            string toSend = String.Empty;
            bool modifier = false;
            if (settings.GameSceneCtrl)
            {
                toSend += "^";
                modifier = true;
            }
            if (settings.GameSceneAlt)
            {
                toSend += "%";
                modifier = true;
            }
            if (settings.GameSceneShift)
            {
                toSend += "+";
                modifier = true;
            }

            if (modifier)
            {
                toSend += "{";
                toSend += settings.GameSceneChar;
                toSend += "}";
            }
            else
            {
                toSend += settings.GameSceneChar;
            }
            

            if (!String.IsNullOrEmpty(toSend))
            {
                IntPtr? obs = hookOBS();
                IntPtr? curr = GetForegroundWindow();
                if (obs != null)
                {
                    Console.WriteLine("OBS: Changing to Game Scene");
                    SetForegroundWindow((IntPtr)obs);
                    SendKeys.SendWait(toSend);
                    SetForegroundWindow((IntPtr)curr);
                }
                else
                {
                    Console.WriteLine("Error: OBS was not located");
                }
                
            }
        }

        public void Stop() { }

        private IntPtr? hookOBS()
        {
            List<string> processNames = new List<string>{ "obs32", "obs64" };
            Process[] pList;
            for (int i = 0; i < processNames.Count; i++)
            {
                pList = Process.GetProcessesByName(processNames[i]);
                if (pList.Length != 0)
                {
                    return pList[0].MainWindowHandle;
                }
            }
            return null;
        }
    }
}