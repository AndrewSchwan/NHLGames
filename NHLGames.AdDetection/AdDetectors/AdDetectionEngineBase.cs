using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NHLGames.AdDetection.Common;

namespace NHLGames.AdDetection.AdDetectors
{
    public abstract class AdDetectionEngineBase
    {

        private List<int> m_mediaPlayerProcesses;

        private List<IAdModule> m_modules = new List<IAdModule>();

        protected abstract int PollPeriodMilliseconds { get; }

        private bool m_previousAdPlayingState;

        private bool m_firstAdCheck;

        protected ReadOnlyCollection<int> MediaPlayerProcesses => new ReadOnlyCollection<int>(m_mediaPlayerProcesses);

        internal Task AddModule(IAdModule module)
        {
            lock (m_modules)
            {
                m_modules.Add(module);
                return Task.Run(() => module.Initialize());
            }
        }

        internal void RemoveModule(string moduleName)
        {
            lock (m_modules)
            {
                var toRemove = m_modules.Where(x => x.Title == moduleName);

                foreach (var adModule in toRemove)
                {
                    m_modules.Remove(adModule);
                }
            }
        }

        internal void Start(List<IAdModule> modules)
        {
            m_previousAdPlayingState = false;
            m_firstAdCheck = true;

            var initializationTasks = modules.Select(AddModule).ToList();

            Task.Run(async () => await LoopForever(initializationTasks));
        }

        public async Task LoopForever(List<Task> initializationTasks)
        {
            try
            {
                Task.WaitAll(initializationTasks.ToArray(), TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Problem initializing tasks: {e.Message}");
            }

            while (true)
            {
                try
                {
                    if (!MediaPlayerIsPlaying())
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        continue;
                    }

                    var newAdPlayingState = IsAdCurrentlyPlaying();

                    if (m_firstAdCheck || newAdPlayingState != m_previousAdPlayingState)
                    {
                        m_firstAdCheck = false;
                        m_previousAdPlayingState = newAdPlayingState;
                        NotifyModules();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected Exception: {e.Message}");
                }

                await Task.Delay(PollPeriodMilliseconds);
            }
        }

        protected abstract bool IsAdCurrentlyPlaying();

        private bool MediaPlayerIsPlaying()
        {
            var vlcProcesses =
                Process.GetProcessesByName("vlc").Where(x => x.MainWindowTitle == @"fd://0 - VLC media player" || x.MainWindowTitle.ToLower().Contains(" @ ")).Select(x => x.Id);

            var mpc64Processes =
                Process.GetProcessesByName("MPC-HC64").Where(x => x.MainWindowTitle == @"stdin" || x.MainWindowTitle.ToLower().Contains(" @ ")).Select(x => x.Id);

            var mpc32Processes =
                Process.GetProcessesByName("MPC-HC").Where(x => x.MainWindowTitle == @"stdin" || x.MainWindowTitle.ToLower().Contains(" @ ")).Select(x => x.Id);

            var mpvProcesses =
                Process.GetProcessesByName("mpv").Select(x => x.Id);

            //Add mpv support here

            m_mediaPlayerProcesses = vlcProcesses.Concat(mpc64Processes).Concat(mpc32Processes).Concat(mpvProcesses).ToList();


            return m_mediaPlayerProcesses.Count != 0;
        }

        private void NotifyModules()
        {
            lock (m_modules)
            {
                foreach (var module in m_modules)
                {
                    if (m_previousAdPlayingState)
                    {
                        Console.WriteLine("Ad Detection: Calling AdStarted on modules.");
                        Task.Run(() => module.AdStarted());
                    }
                    else
                    {
                        Console.WriteLine("Ad Detection: Calling AdEnded on modules.");
                        Task.Run(() => module.AdEnded());
                    }
                }
            }
        }
    }
}