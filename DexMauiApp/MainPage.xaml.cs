using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;

namespace DexMauiApp;

public partial class MainPage : ContentPage
{
    // HTTP client for making API requests
    private readonly HttpClient _httpClient;

    public MainPage()
    {
        InitializeComponent();
        // Initialize HTTP client with Basic Authentication
        _httpClient = new HttpClient();
        var byteArray = Encoding.ASCII.GetBytes("vendsys:NFsZGmHAGWJSZ#RuvdiV");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        _httpClient.BaseAddress = new Uri("https://localhost:7297/");
    }

    // Event handler for Machine A button click
    // Picks a DEX file and sends it identified as Machine A
    private async void OnSendMachineAClicked(object sender, EventArgs e)
    {
        var dexContent = await PickDexFileAsync();
        if (!string.IsNullOrWhiteSpace(dexContent))
            await SendDex(dexContent, 'A');
        else
            ResultLabel.Text = "❌ Arquivo inválido ou não selecionado.";
    }

    // Event handler for Machine B button click
    // Picks a DEX file and sends it identified as Machine B
    private async void OnSendMachineBClicked(object sender, EventArgs e)
    {
        var dexContent = await PickDexFileAsync();
        if (!string.IsNullOrWhiteSpace(dexContent))
            await SendDex(dexContent, 'B');
        else
            ResultLabel.Text = "❌ Arquivo inválido ou não selecionado.";
    }

    // Opens file picker to select a DEX file
    // Returns the file content as string or null if cancelled
    private async Task<string?> PickDexFileAsync()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione o arquivo DEX",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".txt" } },
                    { DevicePlatform.Android, new[] { "text/plain" } },
                    { DevicePlatform.iOS, new[] { "public.plain-text" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.plain-text" } }
                })
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao ler o arquivo: {ex.Message}", "OK");
        }

        return null;
    }

    // Sends DEX file content to the API
    // Parameters:
    // - content: The DEX file content
    // - machine: Machine identifier ('A' or 'B')
    private async Task SendDex(string content, char machine)
    {
        var payload = new
        {
            fileContent = content,
            machine = machine
        };

        var json = JsonConvert.SerializeObject(payload);
        var contentBody = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            // Send POST request to the API
            var response = await _httpClient.PostAsync("dex/vdi-dex", contentBody);
            var result = await response.Content.ReadAsStringAsync();
            ResultLabel.Text = response.IsSuccessStatusCode ? "✅ Enviado com sucesso!" : $"❌ Erro: {result}";
        }
        catch (Exception ex)
        {
            ResultLabel.Text = $"❌ Exceção: {ex.Message}";
        }
    }
}