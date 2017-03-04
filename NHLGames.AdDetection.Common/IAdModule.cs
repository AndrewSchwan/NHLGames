using System;
using System.Windows.Controls;

namespace NHLGames.AdDetection.Common
{
    public interface IAdModule
    {
        /// <summary>
        ///     The title of your module. Shows up in the drop down list.
        /// </summary>
        string Title { get; }


        /// <summary>
        ///     Description of your module. Shows up beside the dropdown box.
        /// </summary>
        string Description { get; }


        //todo: Not actually called anywhere yet
        /// <summary>
        ///     Any settings for your module. Just null if you don't need any.
        ///     You are responsible for detecting and saving changes.
        /// </summary>
        UserControl SettingsControl { get; }

        /// <summary>
        ///     If your module needs any initialization logic, otherwise just leave it as an empty method
        /// </summary>
        void Initialize();

        /// <summary>
        ///     What your module does when an ad starts.
        ///     Note: Stuff is called async, so this might be called before your Initialize or Stop call has finished. You are
        ///     expected to handle this.
        /// </summary>
        void AdStarted();

        /// <summary>
        ///     What your module does when an ad ends
        ///     Note: Stuff is called async, so this might be called before your Initialize or Stop call has finished. You are
        ///     expected to handle this.
        /// </summary>
        void AdEnded();

        
        //todo: Not actually called anywhere yet
        /// <summary>
        ///     Any tear down logic for your module
        /// </summary>
        void Stop();
    }
}