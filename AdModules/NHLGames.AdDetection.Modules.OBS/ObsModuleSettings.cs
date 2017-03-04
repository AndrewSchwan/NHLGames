using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NHLGames.AdDetection.Modules.OBS
{
    [DataContract]
    class ObsModuleSettings
    {
        private static readonly string m_fileName = @"./ObsModuleSettings.xml";

        private static ObsModuleSettings _settings;

        private ObsModuleSettings()
        {
            GameSceneChar = "";
            GameSceneAlt = false;
            GameSceneCtrl = false;
            GameSceneShift = false;

            AdSceneChar = "";
            AdSceneAlt = false;
            AdSceneCtrl = false;
            AdSceneShift = false;
        }

        private static ObsModuleSettings Default => new ObsModuleSettings();

        [DataMember]
        public string GameSceneChar { get; set; }

        [DataMember]
        public string AdSceneChar { get; set; }

        [DataMember]
        public bool GameSceneCtrl { get; set; }

        [DataMember]
        public bool GameSceneAlt { get; set; }

        [DataMember]
        public bool GameSceneShift { get; set; }

        [DataMember]
        public bool AdSceneCtrl { get; set; }

        [DataMember]
        public bool AdSceneAlt { get; set; }

        [DataMember]
        public bool AdSceneShift { get; set; }


        public static ObsModuleSettings Load()
        {
            if (_settings != null)
            {
                return _settings;
            }

            try
            {
                if (File.Exists(m_fileName))
                {
                    var s = new DataContractSerializer(typeof(ObsModuleSettings));
                    using (var fs = File.Open(m_fileName, FileMode.Open))
                    {
                        using (var reader = XmlReader.Create(fs))
                        {
                            _settings = (ObsModuleSettings)s.ReadObject(reader);
                            return _settings;
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Unable to load {m_fileName}. Using default config.");
            }

            Save(Default);
            return _settings;
        }

        public static void Save(ObsModuleSettings settings)
        {
            try
            {
                var s = new DataContractSerializer(typeof(ObsModuleSettings));
                using (var fs = File.Open(m_fileName, FileMode.Create))
                {
                    s.WriteObject(fs, settings);
                    _settings = settings;
                }
            }
            catch
            {
                Console.WriteLine($"Unable to save {m_fileName}");
            }
        }
    }
}
