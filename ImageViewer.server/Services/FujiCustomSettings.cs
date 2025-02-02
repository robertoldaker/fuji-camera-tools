using System.Reflection;
using System.Text.RegularExpressions;
using ImageViewer.shared;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif.Makernotes;

namespace ImageViewer.server.Services;

public class FujiCustomSettings : FujiCustomSettingsBase {
    private static Regex _regExRecipe = new Regex(@"\((.+)\)\.JPG$");

    public FujiCustomSettings(IEnumerable<MetadataExtractor.Directory> directories, string imagePath) {
        this.WhiteBalanceShift = new int[2];
        setFields(directories);
        var match = _regExRecipe.Match(imagePath);
        if ( match.Success) {
            this.FilmRecipe = match.Groups[1].Value;
        }

    }

    public FujiCustomSettings() {
        this.WhiteBalanceShift = new int[2];
    }

    private void setFilmMode(FujifilmMakernoteDirectory mn) {
        if ( mn.TryGetInt16(FujifilmMakernoteDirectory.TagFilmMode, out short fm) ) {
            if ( fm == 0 ) {
                this.FilmSimulation = FilmSimulation.Std;
            } else if ( fm == 1280 ) {
                this.FilmSimulation = FilmSimulation.ProNegStd;
            } else if ( fm == 512) {
                this.FilmSimulation = FilmSimulation.Velvia;
            } else if ( fm == 1281) {
                this.FilmSimulation = FilmSimulation.ProNegHi;
            } else if ( fm == 288 ) {
                this.FilmSimulation = FilmSimulation.Astia;
            }
        } else {
            var cs = mn.GetInt16(FujifilmMakernoteDirectory.TagColorSaturation);
            if ( cs == 768 ) {
                this.FilmSimulation = FilmSimulation.Monochrome;
            } else if ( cs == 769) {
                this.FilmSimulation = FilmSimulation.MonochromeRFilter;
            } else if ( cs == 770) {
                this.FilmSimulation = FilmSimulation.MonochromeYFilter;
            } else if ( cs == 771) {
                this.FilmSimulation = FilmSimulation.MonochromeGFilter;
            } else if ( cs == 784) {
                this.FilmSimulation = FilmSimulation.Sepia;
            } else {
                this.FilmSimulation = FilmSimulation.Std;
            }
        }
    }

    private void setWhiteBalance(FujifilmMakernoteDirectory mn)  {
        if ( mn.TryGetInt16(FujifilmMakernoteDirectory.TagWhiteBalance, out short wb)) {
            if ( wb == 0) {
                this.WhiteBalance = WhiteBalance.Auto;
            } else if ( wb == 3840 ) {
                this.WhiteBalance = WhiteBalance.Custom;
            } else if ( wb == 4080 ) {
                this.WhiteBalance = WhiteBalance.Kelvin;
                if ( mn.TryGetInt32(FujifilmMakernoteDirectory.TagColorTemperature, out int ct) ) {
                    this.WhiteBalanceTemp = ct;
                }
            } else if ( wb == 256) {
                this.WhiteBalance = WhiteBalance.Daylight;
            } else if ( wb == 512 ) {
                this.WhiteBalance = WhiteBalance.Shade;
            } else if ( wb == 768 ) {
                this.WhiteBalance = WhiteBalance.Fluorescent1;
            } else if ( wb == 769 ) {
                this.WhiteBalance = WhiteBalance.Fluorescent2;
            } else if ( wb == 770 ) {
                this.WhiteBalance = WhiteBalance.Fluorescent3;
            } else if ( wb == 1024) {
                this.WhiteBalance = WhiteBalance.Incandescent;
            } else if ( wb == 1536) {
                this.WhiteBalance = WhiteBalance.Underwater;
            }
            var wbft = mn.GetInt32Array(FujifilmMakernoteDirectory.TagWhiteBalanceFineTune);
            if ( wbft!=null && wbft.Length>=2) {
                this.WhiteBalanceShift[0] = wbft[0]/20;
                this.WhiteBalanceShift[1] = wbft[1]/20;
            }
        }
    }

    private void setDynamicRange(FujifilmMakernoteDirectory mn) {
        if ( mn.TryGetInt16(FujifilmMakernoteDirectory.TagDevelopmentDynamicRange, out short ddr) ) {
            if ( ddr == 100 ) {
                this.DynamicRange = DynamicRange.DR100;
            } else if ( ddr == 200) {
                this.DynamicRange = DynamicRange.DR200;
            } else if ( ddr == 400) {
                this.DynamicRange = DynamicRange.DR400;
            }
        } else {
            this.DynamicRange = DynamicRange.Auto;
        }
    }

    private void setColor(FujifilmMakernoteDirectory mn) {
        if ( mn.TryGetInt16(FujifilmMakernoteDirectory.TagColorSaturation, out short cs) ) {
            if ( cs == 1024) {
                this.Color = -2;
            } else if ( cs == 384) {
                this.Color = -1;
            } else if ( cs == 128) {
                this.Color = 1;
            } else if ( cs == 256 ) {
                this.Color = 2;
            } else if ( cs == 0 ) {
                this.Color = 0;
            }
        } 
    }

    private void setSharpness(FujifilmMakernoteDirectory mn) {
        if ( mn.TryGetInt16(FujifilmMakernoteDirectory.TagSharpness, out short sh) ) {
            if ( sh == 2 ) {
                this.Sharpness = -2;
            } else if ( sh == 130) {
                this.Sharpness = -1;
            } else if ( sh == 3 ) {
                this.Sharpness = 0;
            } else if ( sh == 132) {
                this.Sharpness = 1;
            } else if ( sh == 4 ) {
                this.Sharpness = 2;
            }
        } 
    }

    private void setShadowTone(FujifilmMakernoteDirectory mn) {
        if ( mn.TryGetInt16(4160, out short st) ) {
            this.ShadowTone = -st/16;
        } 
    }

    private void setHighlightTone(FujifilmMakernoteDirectory mn) {
        if ( mn.TryGetInt16(4161, out short st) ) {
            this.HighlightTone = -st/16;
        } 
    }

    private void setNoiseReduction(FujifilmMakernoteDirectory mn) {        
        if ( mn.TryGetInt16(4110, out short sh) ) {
            if ( sh == 512 ) {
                this.NoiseReduction=-2;
            } else if ( sh == 640 ) {
                this.NoiseReduction = -1;                
            } else if ( sh == 0) {
                this.NoiseReduction = 0;
            } else if ( sh == 384 ) {
                this.NoiseReduction = 1;
            } else if ( sh == 256 ) {
                this.NoiseReduction = 2;
            }
        } 
    }

    private void setFields(IEnumerable<MetadataExtractor.Directory> directories) {
        //
        var mn = directories.OfType<FujifilmMakernoteDirectory>().FirstOrDefault();
        if ( mn==null ) {
            return;
        }

        setFilmMode(mn);

        setWhiteBalance(mn);

        setDynamicRange(mn);

        setColor(mn);

        setSharpness(mn);

        setHighlightTone(mn);

        setShadowTone(mn);

        setNoiseReduction(mn);

    }

    public static bool IsRecipeMatch(FujiCustomSettingsBase image, FujiCustomSettingsBase recipe)
    {
        if ( image.FilmSimulation!=recipe.FilmSimulation) {
            return false;
        // Auto dynmaic range can cause dynamic range to be set to something explicit so ignore if recipe has not
        // nailed this down to 100, 200 or 400
        } else if ( recipe.DynamicRange != DynamicRange.Auto && image.DynamicRange!=recipe.DynamicRange) {
            return false;
        } else if ( image.WhiteBalance!=recipe.WhiteBalance) {
            return false;
        } else if ( image.WhiteBalance == WhiteBalance.Kelvin && recipe.WhiteBalance == WhiteBalance.Kelvin) {
            if ( image.WhiteBalanceTemp!=recipe.WhiteBalanceTemp) {
                return false;
            }
        } else if ( image.WhiteBalanceShift[0]!=recipe.WhiteBalanceShift[0]) {
            return false;
        } else if ( image.WhiteBalanceShift[1]!=recipe.WhiteBalanceShift[1]) {
            return false;
        } else if ( image.Color!=recipe.Color) {
            return false;
        } else if ( image.Sharpness!=recipe.Sharpness) {
            return false;
        } else if ( image.HighlightTone!=recipe.HighlightTone) {
            return false;
        } else if ( image.ShadowTone!=recipe.ShadowTone) {
            return false;
        } else if ( image.NoiseReduction!=recipe.NoiseReduction) {
            return false;
        }
        return true;
    }

    public override string ToString() {
        var fcs = this;
        var wbTemp = fcs.WhiteBalance == WhiteBalance.Kelvin ? $" ({fcs.WhiteBalanceTemp})" : "";
        return @$"
Dynamic range={fcs.DynamicRange}
Film simulation={fcs.FilmSimulation}
White balance={fcs.WhiteBalance} {wbTemp}[{fcs.WhiteBalanceShift[0]} {fcs.WhiteBalanceShift[1]}]
Color={fcs.Color}
Sharpness={fcs.Sharpness}
Highlight tone={fcs.HighlightTone}
Shadow tone={fcs.ShadowTone}
Noise reduction={fcs.NoiseReduction}
";
    }

}