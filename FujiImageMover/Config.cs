using System.Net.NetworkInformation;
using System.Text.Json;

namespace FujiImageMover
{
    public class Config
    {
        private static Config _instance=null;
        private static object _instanceLock = new object();

        public static Config Instance {
            get {
                lock(_instanceLock) {
                    if (_instance == null) {
                        var configFile = getConfigFile();
                        if ( File.Exists(configFile)) {
                            string json = File.ReadAllText(configFile);
                            _instance = JsonSerializer.Deserialize<Config>(json);
                            if ( _instance == null ) {
                                throw new Exception("Failed to deserialize config");
                            }
                        } else {
                            _instance = new Config();
                            // save serialized config
                            string json = JsonSerializer.Serialize(_instance, new JsonSerializerOptions() { WriteIndented = true });
                            File.WriteAllText(configFile, json);
                            throw new Exception($"New config file created at [{configFile}]. Please edit the file and re-run the program");
                        }
                    }
                    return _instance;
                }
            }
        }

        private static string getConfigFile() {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configDir = Path.Combine(homeDirectory, ".FujiImageMover");
            Directory.CreateDirectory(configDir);
            string configFile = Path.Combine(configDir, "config.json");
            return configFile;
        }

        public Config() {
            // set defaults
            DestinationFolder = "";
            ApiKey = "";
            SpreadsheetId = "";
        }

        public string DestinationFolder { get; set; }
        public string ApiKey { get; set; }
        public string SpreadsheetId { get; set; }
    }

}