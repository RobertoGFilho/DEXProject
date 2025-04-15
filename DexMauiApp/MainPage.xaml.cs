using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DexMauiApp;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public MainPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        var byteArray = Encoding.ASCII.GetBytes("vendsys:NFsZGmHAGWJSZ#RuvdiV");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        _httpClient.BaseAddress = new Uri("https://localhost:7297/");
    }

    private async void OnSendMachineAClicked(object sender, EventArgs e)
    {
        await SendDex("ID101MACHINE123*VA1019999*PA101COKE*PA102250*PA20110*PA2022500*", 'A');
    }

    private async void OnSendMachineBClicked(object sender, EventArgs e)
    {
        await SendDex("ID101MACHINE456*VA1011234*PA101PEPSI*PA102300*PA2015*PA2021500*", 'B');
    }

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
            var response = await _httpClient.PostAsync("dex/vdi-dex", contentBody);
            var result = await response.Content.ReadAsStringAsync();
            ResultLabel.Text = response.IsSuccessStatusCode ? "✅ Success!" : $"❌ Error: {result}";
        }
        catch (Exception ex)
        {
            ResultLabel.Text = $"❌ Exception: {ex.Message}";
        }
    }
}