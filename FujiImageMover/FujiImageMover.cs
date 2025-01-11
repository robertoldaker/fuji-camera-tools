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

        int numMoved = imageMover.MoveFiles(destinationFolder, fujiRecipes);

        Console.WriteLine($"[{numMoved}] files processed");

    }

}