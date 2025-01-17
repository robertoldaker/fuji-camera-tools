using System.ComponentModel;
using System.Net.Http.Json;
using ImageViewer.shared;
using Microsoft.VisualBasic;

namespace ImageViewer.client.Services;

public class DataAccessService {
    public List<MonthsByYear>? MonthsByYear { get; set; }
    private readonly HttpClient _httpClient;
    
    public DataAccessService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public event EventHandler<List<ImagesByDate>>? ImagesByDateLoaded;

    public List<ImagesByDate>? ImagesByDateList {get; set;}
    public int Year {get; set;}
    public int Month {get; set;}

    public string BaseAddress {
        get {
            return _httpClient.BaseAddress!.ToString();
        }
    }

    public async Task<List<MonthsByYear>> GetMonthsByYearAsync() {
        var data = await _httpClient.GetFromJsonAsync<List<MonthsByYear>>("ImageLibrary/MonthsByYear");
        return data!;
    }

    private async Task<List<ImagesByDate>> GetImagesByDateAsync(int year, int month) {
        var data = await _httpClient.GetFromJsonAsync<List<ImagesByDate>>($"ImageLibrary/ImagesByDate?year={year}&month={month}");
        return data!;
    }

    public async Task LoadNewThumbnails(int year, int month) {
        ImagesByDateList = await GetImagesByDateAsync(year, month);
        Year = year;
        Month = month;
        ImagesByDateLoaded?.Invoke(this,ImagesByDateList);
    }

}