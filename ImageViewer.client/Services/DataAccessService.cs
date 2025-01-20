using System.ComponentModel;
using System.Net.Http.Json;
using ImageViewer.shared;
using Microsoft.VisualBasic;

namespace ImageViewer.client.Services;

public class DataAccessService {
    private readonly HttpClient _httpClient;
    
    public DataAccessService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

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

    public async Task<ImageMetadataBase> GetImageMetadataAsync(string id) {
        var encodedId = System.Net.WebUtility.UrlEncode(id);
        var data = await _httpClient.GetFromJsonAsync<ImageMetadataBase>($"ImageLibrary/ImageMetadata?id={encodedId}");
        return data!;
    }

    public async Task<ConfigBase> GetConfigAsync() {
        var data = await _httpClient.GetFromJsonAsync<ConfigBase>($"Configuration/Config");
        return data!;
    }
    public async Task<HttpResponseMessage> SetConfigAsync(ConfigBase config) {
        var mess = await _httpClient.PostAsJsonAsync($"Configuration/Config",config);
        return mess;
    }
}