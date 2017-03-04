using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NHLGames.AdDetection
{
    public interface IAdDetectionEngineDescriptor : INotifyPropertyChanged
    {
        AdDetectionEngineType Type { get; }

        string Description { get; }

        string Name { get; }

        UserControl SettingsControl { get; }
    }
}
