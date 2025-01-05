using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace FujiImageMover;

public class FujiRecipes
{
    private static readonly string _apiKey = "AIzaSyB23aqe8dw163f-JUHJv8LGmM5pOfTSXxU";
    private static readonly string _spreadsheetId = "1WuwX4YVBygbWoAp3LQcL0xqqv1ljOdd2I1FnIM5wkvk";
    public FujiRecipes()
    {
        // Set up the Google Sheets API client
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            ApiKey = _apiKey,
            ApplicationName = "Google Sheets API .NET Quickstart"
        });

        // Open the spreadsheet
        var spreadsheetId = _spreadsheetId;
        var range = "Recipes!A1:N50"; // Adjust the range as needed
        var response = service.Spreadsheets.Values.Get(spreadsheetId, range).Execute();

        // Process the rows
        foreach (var row in response.Values)
        {
            foreach (var cell in row)
            {
                Console.WriteLine(cell);
            }
        }
    }

    public string getName(FujiCustomSettings fcs)
    {
        return "???";
    }
}