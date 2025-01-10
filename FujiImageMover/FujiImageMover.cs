using System.Data.SqlTypes;
using System.Runtime.InteropServices.Marshalling;

namespace FujiImageMover;

public class FujiImageMover {

    public FujiImageMover(Config config) {

        // see if we can find an attached sd-card
        var imageMover = ImageMoverFactory.Create();

        var destinationFolder = config.DestinationFolder;
        if ( string.IsNullOrEmpty(destinationFolder) ) {
            throw new System.Exception($"Please set a destination folder to move the files to");
        }
        if ( !Directory.Exists(destinationFolder) ) {
            throw new System.Exception($"Destination folder [{destinationFolder}] does not exist");
        }
        

        FujiRecipes? fujiRecipes = null;
        // if we have the FujiFilm recipe enabled, get all recipes
        if ( config.SetFujiFilmRecipe.Enabled ) {
            // get all fujifilm recipes we know about via the Google Sheets API
            fujiRecipes = new FujiRecipes(config.SetFujiFilmRecipe.ApiKey, config.SetFujiFilmRecipe.SpreadsheetId);
        }

        // get all image files from the source folder        
        var imageFiles = imageMover.ListFiles();
        foreach (var file in imageFiles) {
            var md = new ImageMetadata(file);
            string recipeName = "";
            if ( fujiRecipes!=null && md.FujiCustomSettings != null ) {
                recipeName = fujiRecipes.FindMatch(md.FujiCustomSettings);
            }
            // mov file to new destination
            imageMover.Move(file, md, config.DestinationFolder, recipeName);
        }
        Console.WriteLine($"[{imageFiles.Count()}] files processed");

    }

    private IEnumerable<string> ListFilesWithExtension(string directory, string extension) {
        return Directory.GetFiles(directory, $"*{extension}", SearchOption.AllDirectories);
    }
}