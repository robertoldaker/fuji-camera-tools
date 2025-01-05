using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace FujiImageMover;

public class ImageMetadata {
    public ImageMetadata(string imagePath) {
        
        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);

        var subIfdDirectory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        if (subIfdDirectory != null)
        {
            this.Make = subIfdDirectory.GetDescription(ExifDirectoryBase.TagMake);
            this.Model = subIfdDirectory.GetDescription(ExifDirectoryBase.TagModel);
        }
        var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        if (directory != null)
        {
            // query the tag's value
            if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime)) {
                this.DateTime = dateTime;
            }
        }

        if ( this.Make == "FUJIFILM" && this.Model == "X-E1") {
            this.FujiCustomSettings = new FujiCustomSettings(directories);
        }

    }

    public DateTime DateTime {get; private set;}

    public string? Make {get; private set;}

    public string? Model {get; private set; }

    public FujiCustomSettings? FujiCustomSettings {get; private set;}

    public override string ToString() {
        var cs = FujiCustomSettings != null ? FujiCustomSettings.ToString() : "";
        return $"{DateTime} {Make} {Model}{cs}";
    }
}