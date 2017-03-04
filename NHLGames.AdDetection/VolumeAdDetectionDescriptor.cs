using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Prism.Mvvm;

namespace NHLGames.AdDetection
{
    public class VolumeAdDetectionDescriptor : BindableBase, IAdDetectionEngineDescriptor
    {
        public AdDetectionEngineType Type => AdDetectionEngineType.PlayerSystemVolume;

        public string Description
            =>
                "Detects ads based on the volume of your VLC/MPC instances playing games. If all of them are not outputting any sound it will assume an ad started. The limitation is that it can't tell if you've muted your media player."
            ;

        public string Name => "Volume detection";
        public UserControl SettingsControl => null;
    }
}
