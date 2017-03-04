using System.ComponentModel.Composition;
using System.Windows.Controls;
using NHLGames.AdDetection.Common;

namespace TemplateModule
{
    /// <summary>
    ///     A template of a sample module
    ///     See Spotify module for an implemented example.
    /// </summary>
    /// <remarks>
    ///     Export attribute is required so that it is loaded by the main program.
    ///     You must also output to NhlGames\AdModules\ModuleName folder to be loaded properly.
    /// </remarks>
    [Export(typeof (IAdModule))]
    public class SampleModule : IAdModule
    {
        /// <summary>
        ///     The title of your module.
        /// </summary>
        public string Title => "Sample Module";

        /// <summary>
        ///     A description of your module.
        /// </summary>
        public string Description => "A template module that does nothing!";


        /// <summary>
        ///     An optional WPF control if you need configurable options.
        ///     You are responsible for listening to changes and auto-saving them.
        /// </summary>
        public UserControl SettingsControl => null;

        public void Initialize()
        {
            //Initialization code that your module needs to function for it to work
            //You are responsible for making sure your plugin is initialized in your AdStart/Stop methods.
        }

        public void AdStarted()
        {
            //The action to perform when an ad is started.
            //You are responsible for making sure your plugin is initialized in your AdStart/Stop methods.
        }

        public void AdEnded()
        {
            //The action to perform when an ad ends..
            //You are responsible for making sure your plugin is initialized in your AdStart/Stop methods.
        }

        public void Stop()
        {
            //The action to perform when your module is disabled (Not disposed, it can be Initialized again after if it's enabled.
        }
    }
}