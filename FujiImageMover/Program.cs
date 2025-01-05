// See https://aka.ms/new-console-template for more information
public class ImageMover {
    public static int Main(string[] args) {
        try {
            var mover = new FujiImageMover.FujiImageMover();
        } catch( Exception e) {
            Console.WriteLine(e.Message);
            return 1;
        }

        return 0;
    }
}
