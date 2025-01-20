using System.Text.Json;
using ImageViewer.shared;

namespace ImageViewer.server.Services;

public class Config : ConfigBase
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
        
    }

    private void CopyTo(Config config) {
        config.ImageFolder = ImageFolder;
        config.GooglePhotosConfig.Enabled = GooglePhotosConfig.Enabled;
        // copy google photos stuff here
    }

    public void Save() {
        var configFile = getConfigFile();
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(configFile, json);       
    }

    public Dictionary<string,string> CheckModel() {
        var errors = new Dictionary<string,string>();
        if ( !Directory.Exists(ImageFolder) ) {
            errors.Add("imageFolder","Folder does not exist");
        }
        return errors;
    }
}