using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KakaoTalkAdBlockPlus.Settings
{
    /// <summary>설정을 JSON 파일에 저장한다.</summary>
    public sealed class JsonSettingsStore
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(SettingsDocument));

        private readonly string _filePath;

        public JsonSettingsStore(string filePath)
        {
            _filePath = filePath;
        }

        public AppSettings Load()
        {
            if (!File.Exists(_filePath)) return AppSettings.Default;

            try
            {
                using var stream = File.OpenRead(_filePath);
                var document = (SettingsDocument)Serializer.ReadObject(stream);
                return new AppSettings(CheckInterval.FromMilliseconds(document.CheckIntervalMs));
            }
            catch (SerializationException)
            {
                return AppSettings.Default;
            }
        }

        public void Save(AppSettings settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_filePath)));
            using var stream = File.Create(_filePath);
            using var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, ownsStream: false, indent: true);
            Serializer.WriteObject(writer, new SettingsDocument { CheckIntervalMs = settings.CheckInterval.Milliseconds });
        }

        [DataContract]
        internal sealed class SettingsDocument
        {
            [DataMember(Name = "checkIntervalMs")]
            public int CheckIntervalMs { get; set; }
        }
    }
}
