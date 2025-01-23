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

    /* image library */
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
    public async Task LoadImagesAsync() {
        await _httpClient.GetAsync($"ImageLibrary/LoadImages");
    }

    public async Task RotateClockwiseAsync(string id) {
        var encodedId = System.Net.WebUtility.UrlEncode(id);
        await _httpClient.GetAsync($"ImageLibrary/RotateClockwise?id={encodedId}");
    }

    public async Task RotateAntiClockwiseAsync(string id) {
        var encodedId = System.Net.WebUtility.UrlEncode(id);
        await _httpClient.GetAsync($"ImageLibrary/RotateAntiClockwise?id={encodedId}");
    }

    public async Task DeleteAsync(string id) {
        var encodedId = System.Net.WebUtility.UrlEncode(id);
        await _httpClient.GetAsync($"ImageLibrary/Delete?id={encodedId}");
    }

    /* Configuration */
    public async Task<ConfigBase> GetConfigAsync() {
        var data = await _httpClient.GetFromJsonAsync<ConfigBase>($"Configuration/Config");
        return data!;
    }
    public async Task<HttpResponseMessage> SetConfigAsync(ConfigBase config) {
        var mess = await _httpClient.PostAsJsonAsync($"Configuration/Config",config);
        return mess;
    }

    /* image mover */
    public async Task<(List<string>?,string?)> ListImportFilesAsync() {
        var mess = await _httpClient.GetAsync($"ImageMover/ListFiles");
        if ( mess.IsSuccessStatusCode) {
            var data = await mess.Content.ReadFromJsonAsync<List<string>>();
            return (data!,null);
        } else if ((int) mess.StatusCode == 422) {
            var errorMessage = await mess.Content.ReadAsStringAsync();
            return (null,errorMessage);
        } else {
            return (null,mess.ReasonPhrase);
        }
    }
    public async Task<(string?,string?)> ImportNextAsync() {
        var mess = await _httpClient.GetAsync($"ImageMover/MoveNextFile");
        if ( mess.IsSuccessStatusCode) {
            var data = await mess.Content.ReadAsStringAsync();
            return (data,null);
        } else if ((int) mess.StatusCode == 422) {
            var errorMessage = await mess.Content.ReadAsStringAsync();
            return (null,errorMessage);
        } else {
            return (null,mess.ReasonPhrase);
        }
    }

}