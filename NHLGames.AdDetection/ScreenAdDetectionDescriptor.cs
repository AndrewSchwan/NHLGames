using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Prism.Mvvm;

namespace NHLGames.AdDetection
{
    public class ScreenAdDetectionDescriptor : BindableBase, IAdDetectionEngineDescriptor
    {
        public AdDetectionEngineType Type => AdDetectionEngineType.FullScreenImage;

        public string Description => "Must be playing in fullscreen. If any of your monitors are on the 'Commercial Break in Progress' " +
                                     "'The Game Has Ended' or similar screens it will assume an ad started.";

        public string Name => "Fullscreen detection";
        public UserControl SettingsControl => null;
    }
}
