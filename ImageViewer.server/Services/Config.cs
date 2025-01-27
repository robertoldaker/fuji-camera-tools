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

    private static Config? _instance = null;

    public static Config CreateConfig() {
        var configFile = getConfigFile();
        if ( File.Exists(configFile)) {
            string json = File.ReadAllText(configFile);
            _instance = JsonSerializer.Deserialize<Config>(json);
            if ( _instance == null ) {
                throw new Exception($"Failed to deserialize config");
            } 
        } else {
            _instance = new Config();
            string json = JsonSerializer.Serialize(_instance, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(configFile, json);
        }
        return _instance;
    }

    public Config() {
        
    }

    private void CopyTo(Config config) {
        config.ImageFolder = ImageFolder;
        config.ExternalEditor = ExternalEditor;
        // copy google photos stuff here
        config.GooglePhotosConfig.Enabled = GooglePhotosConfig.Enabled;
        // set fuji film recipe stuff
        config.SetFujiFilmRecipe.Enabled = SetFujiFilmRecipe.Enabled;
        config.SetFujiFilmRecipe.ApiKey = SetFujiFilmRecipe.ApiKey;
        config.SetFujiFilmRecipe.SpreadsheetId = SetFujiFilmRecipe.SpreadsheetId;
    }

    public void Save() {
        var configFile = getConfigFile();
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(configFile, json);
        //
        this.CopyTo(_instance!);
    }

    public Dictionary<string,string> CheckModel() {
        var errors = new Dictionary<string,string>();
        if ( !Directory.Exists(ImageFolder) ) {
            errors.Add("imageFolder","Folder does not exist");
        }
        return errors;
    }
}