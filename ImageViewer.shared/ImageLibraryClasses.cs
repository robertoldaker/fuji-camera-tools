using System.Reflection;

namespace ImageViewer.shared;

public class ImagesByDate {
    public ImagesByDate(DateTime date) {
        Date = date;
        Images = new List<ImageInfo>();
    }
    public DateTime Date {get; set;}
    public List<ImageInfo> Images {get; set;}
}

public class ImageInfo {
    public ImageInfo(string id, bool inGooglePhotos) {
        Id = id;
        InGooglePhotos = inGooglePhotos;
    }
    public string Id {get; set;}
    public bool InGooglePhotos {get; set;}
}

public class MonthsByYear {
    public MonthsByYear(int year) {
        Year = year;
        Months = new List<int>();
    }
    public int Year {get; set;}
    public List<int> Months {get; set;}
}


public class ImageMetadataBase {
    public DateTime DateTime {get; set;}
    public string? Make {get; set;}
    public string? Model {get; set;}
    public string? Aperture {get; set;}
    public string? ShutterSpeed {get; set;}
    public string? ExposureBias {get; set;}
    public string? ISO {get; set;}
    public string? FocalLength {get; set;}
    public int Width {get; set;}
    public int Height {get; set;}
    public FujiCustomSettingsBase? FujiCustomSettings {get; set;}
}

public enum FilmSimulation {
    Std=0,
    Velvia=1,
    Astia=2,
    ProNegHi=3,
    ProNegStd=4,
    Monochrome=5,
    MonochromeYFilter=6,
    MonochromeRFilter=7,
    MonochromeGFilter=8,
    Sepia=9
}

public enum WhiteBalance {
    Auto=0,
    Custom=1,
    Kelvin=2,
    Daylight=3,
    Shade=4,
    Fluorescent1=5,
    Fluorescent2=6,
    Fluorescent3=7,
    Incandescent=8,
    Underwater=9
}

public enum DynamicRange {
    Auto=0,
    DR100=1,
    DR200=2,
    DR400=3
}


public class FujiCustomSettingsBase {
    public FujiCustomSettingsBase() {
        WhiteBalanceShift = new int[2] { 0,0 };
    }
    public string? FilmRecipe { get; set; }
    public FilmSimulation FilmSimulation { get; set; }
    public WhiteBalance WhiteBalance { get; set; }
    public DynamicRange DynamicRange { get; set; }
    public int[] WhiteBalanceShift { get; set; }
    public int WhiteBalanceTemp { get; set; }
    public int Color { get; set; }
    public int Sharpness {get; set; }
    public int ShadowTone {get; set; }
    public int HighlightTone {get; set; }

}
 

