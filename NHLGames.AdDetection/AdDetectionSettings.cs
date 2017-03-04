using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace NHLGames.AdDetection
{
    [DataContract]
    public class AdDetectionSettings
    {
        private static readonly string m_fileName = @"./AdDetectionSettings.xml";

        private static AdDetectionSettings _settings;

        private AdDetectionSettings()
        {
            IsEnabled = false;
            EnabledModules = new List<string>();
            EngineType = AdDetectionEngineType.PlayerSystemVolume;
        }

        private static AdDetectionSettings Default => new AdDetectionSettings();

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public List<string> EnabledModules { get; set; }

        [DataMember]
        public AdDetectionEngineType EngineType { get; set; }


        public static AdDetectionSettings Load()
        {
            if (_settings != null)
            {
                return _settings;
            }

            try
            {
                if (File.Exists(m_fileName))
                {
                    var s = new DataContractSerializer(typeof (AdDetectionSettings));
                    using (var fs = File.Open(m_fileName, FileMode.Open))
                    {
                        using (var reader = XmlReader.Create(fs))
                        {
                            _settings = (AdDetectionSettings) s.ReadObject(reader);
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

        public static void Save(AdDetectionSettings settings)
        {
            try
            {
                var s = new DataContractSerializer(typeof (AdDetectionSettings));
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