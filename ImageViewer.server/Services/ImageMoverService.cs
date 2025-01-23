namespace ImageViewer.server.Services;

public class ImageMoverService
{
    private readonly Config _config;
    private readonly ImageLibraryService _imageLibraryService;
    private FujiRecipes? _fujiRecipes = null;

    public ImageMoverService(Config config, ImageLibraryService imageLibraryService)
    {
        _config = config;
        _imageLibraryService = imageLibraryService;
    }

    public List<String> ListFiles() {
        // see if we can find an attached sd-card        
        var imageMover = ImageMoverFactory.Create();
        
        getDestinationFolder();

        // if we have the FujiFilm recipe enabled, get all recipes
        if ( _config.SetFujiFilmRecipe.Enabled ) {
            // get all fujifilm recipes we know about via the Google Sheets API
            _fujiRecipes = new FujiRecipes(_config.SetFujiFilmRecipe.ApiKey, _config.SetFujiFilmRecipe.SpreadsheetId);
        }

        return imageMover.ListFiles();
    }

    private string getDestinationFolder() {
        var destinationFolder = _config.ImageFolder;
        if ( string.IsNullOrEmpty(destinationFolder) ) {
            throw new System.Exception($"Please set a destination folder to move the files to");
        }
        if ( !Directory.Exists(destinationFolder) ) {
            throw new System.Exception($"Destination folder [{destinationFolder}] does not exist");
        }
        return destinationFolder;
    }

    public string MoveNextFile() {
        var destinationFolder = getDestinationFolder();
        var imageMover = ImageMoverFactory.Create();
        var movedFile = imageMover.MoveNextFile(destinationFolder,_fujiRecipes);
        //
        _imageLibraryService.NewImage(movedFile);
        //
        return movedFile;
    }
}
