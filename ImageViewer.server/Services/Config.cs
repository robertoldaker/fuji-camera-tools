using System.Text.Json;

namespace ImageViewer.server.Services;

public class Config
    {

        private static string getConfigFile() {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configDir = Path.Combine(homeDirectory, ".ImageViewer");
            Directory.CreateDirectory(configDir);
            string configFile = Path.Combine(configDir, "config.json");
            return configFile;
        }

        public static Config CreateConfig() {
            var configFile = getConfigFile();
            Config? instance = null;
            if ( File.Exists(configFile)) {
                string json = File.ReadAllText(configFile);
                instance = JsonSerializer.Deserialize<Config>(json);
                if ( instance == null ) {
                    throw new Exception($"Failed to deserialize config");
                } 
            } else {
                instance = new Config();
                string json = JsonSerializer.Serialize(instance, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText(configFile, json);
            }
            return instance;
        }

        public Config() {
            // set defaults
            ImageFolder = "";
            GooglePhotosConfig = new GooglePhotosConfigClass();
            //
        }

        public string ImageFolder { get; set; }
        public GooglePhotosConfigClass GooglePhotosConfig { get; set; }

        public class GooglePhotosConfigClass {
            public GooglePhotosConfigClass() {
                Enabled = false;
            }
            public bool Enabled { get; set; }
            // add google photos stuff here
        }

        private void CopyTo(Config config) {
            config.ImageFolder = ImageFolder;
            config.GooglePhotosConfig.Enabled = GooglePhotosConfig.Enabled;
            // copy google photos stuff here
        }
    }