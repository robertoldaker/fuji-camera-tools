// See https://aka.ms/new-console-template for more information
using FujiImageMover;

public class ImageMover {
    public static int Main(string[] args) {
        try {
            if ( args.Length ==1 && args[0] == "--help") {
                Console.WriteLine("Usage: ImageMover");
                Console.WriteLine("This program moves images from a FujiFilm camera to a destination folder. It is configured via a JSON file located at ~/.FujiImageMover/config.json.");
                Console.WriteLine("The config file should contain the following keys:");
                Console.WriteLine("DestinationFolder: the folder where images will be moved to");
                Console.WriteLine("ApiKey: the Google API key for accessing the Google Sheets API");
                Console.WriteLine("SpreadsheetId: the ID of the Google Sheet containing the FujiFilm recipes");
                return 1;
            }
            var destFolder = "/media/public/Pictures";
            var config = Config.Instance;            
            var mover = new FujiImageMover.FujiImageMover(config);
        } catch( Exception e) {
            Console.WriteLine(e.Message);
            return 1;
        }

        return 0;
    }
}
