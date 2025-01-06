using System.Runtime.Versioning;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace FujiImageMover;

public class FujiRecipes
{
    private Dictionary<FujiCustomSettings,string> _recipes = new Dictionary<FujiCustomSettings,string>();
    public FujiRecipes(string apiKey, string spreadsheetId)
    {
        // Set up the Google Sheets API client
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            ApiKey = apiKey,
            ApplicationName = "Google Sheets API .NET Quickstart"
        });

        // Open the spreadsheet containing the recipes
        var range = "Recipes!A1:N50"; // Adjust the range as needed
        var response = service.Spreadsheets.Values.Get(spreadsheetId, range).Execute();
        processRows(response.Values);
    }

    public string FindMatch(FujiCustomSettings fcs)
    {
        foreach( var recipe in _recipes.Keys)
        {
            if (FujiCustomSettings.AreEqual(fcs, recipe))
            {
                return _recipes[recipe];
            }
        }
        return "";
    }

    private void processRows(IList<IList<object>> values)
    {
        int nameIx = 0,
            filmSimulationIdx = 2,
            dynamicRangeIdx = 4,
            colourIdx = 5,
            sharpnessIdx = 6,
            highlightToneIdx = 7,
            shadowToneIdx = 8,
            wbIdx = 10,
            wbTempIdx=11,
            rShiftIdx=12,
            bShiftRdx=13;
        
        bool headerRow = true;
        foreach (var row in values)
        {
            if (headerRow)
            {
                headerRow = false;
            }
            else
            {                
                if ( row.Count > 13)
                {
                    var fr = new FujiCustomSettings();
                    var name = (string) row[nameIx];
                    fr.FilmSimulation = (FilmSimulation) Enum.Parse(typeof(FilmSimulation), (string) row[filmSimulationIdx]);
                    fr.DynamicRange = (DynamicRange) Enum.Parse(typeof(DynamicRange), (string) row[dynamicRangeIdx]);
                    fr.Color = getIntValue(row[colourIdx]);
                    fr.Sharpness = getIntValue(row[sharpnessIdx]);
                    fr.HighlightTone = getIntValue(row[highlightToneIdx]);
                    fr.ShadowTone = getIntValue(row[shadowToneIdx]);
                    fr.WhiteBalance = (WhiteBalance) Enum.Parse(typeof(WhiteBalance), (string) row[wbIdx]);
                    fr.WhiteBalanceTemp = string.IsNullOrEmpty(((string) row[wbTempIdx])) ? 0 : getIntValue(row[wbTempIdx]);
                    fr.WhiteBalanceShift[0] = getIntValue(row[rShiftIdx]);
                    fr.WhiteBalanceShift[1] = getIntValue(row[bShiftRdx]);
                    _recipes.Add(fr,name);
                }
            }
        }
    }

    private int getIntValue(object? value)
    {
        return value == null ? throw new Exception("Unexpected null value found") : int.Parse((string) value);
    }

}