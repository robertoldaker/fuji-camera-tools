using System.Text.RegularExpressions;
using ImageViewer.shared;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace ImageViewer.server.Services;

public class ImageMetadata : ImageMetadataBase {
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
            this.Aperture = directory.GetDescription(ExifDirectoryBase.TagAperture);
            this.ShutterSpeed = directory.GetDescription(ExifDirectoryBase.TagShutterSpeed);
            this.ExposureBias = directory.GetDescription(ExifDirectoryBase.TagExposureBias);
            this.ISO = directory.GetDescription(ExifDirectoryBase.TagIsoEquivalent);
            this.FocalLength = directory.GetDescription(ExifDirectoryBase.TagFocalLength);
        }

        if ( this.Make == "FUJIFILM" && this.Model == "X-E1") {
            this.FujiCustomSettings = new FujiCustomSettings(directories,imagePath);
        }

    }

    public override string ToString() {
        var cs = FujiCustomSettings != null ? FujiCustomSettings.ToString() : "";
        return $"{DateTime} {Make} {Model}{cs}";
    }
}