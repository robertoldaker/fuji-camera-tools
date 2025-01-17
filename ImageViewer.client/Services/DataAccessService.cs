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

    public string BaseAddress {
        get {
            return _httpClient.BaseAddress!.ToString();
        }
    }

    public async Task<List<MonthsByYear>> GetMonthsByYearAsync() {
        var data = await _httpClient.GetFromJsonAsync<List<MonthsByYear>>("ImageLibrary/MonthsByYear");
        return data!;
    }

    public async Task<List<ImagesByDate>> GetImagesByDateAsync(int year, int month) {
        var data = await _httpClient.GetFromJsonAsync<List<ImagesByDate>>($"ImageLibrary/ImagesByDate?year={year}&month={month}");
        return data!;
    }

    public async Task LoadNewThumbnails(int year, int month) {
        var data = await GetImagesByDateAsync(year, month);
        ImagesByDateLoaded?.Invoke(this,data);
    }

}