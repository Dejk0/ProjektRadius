using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Storage;
using System.IO;

namespace testProjekt
{
  public partial class MainPage : ContentPage
  {
    int count = 0;

    public MainPage()
    {
      InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {

      string fileName = "example.txt";
      string content = "Hello, this is a test file.";

      // Androidos letöltések mappa elérése
      var downloadsPath = "/storage/emulated/0/Download/";
      var filePath = Path.Combine(downloadsPath, fileName);

      try
      {
        // Fájl létrehozása és írása
        File.WriteAllText(filePath, "asd");

         DisplayAlert("Success", $"File created at: {filePath}", "OK");
      }
      catch (Exception ex)
      {
         DisplayAlert("Error", $"File creation failed: {ex.Message}", "OK");
      }
    }
  }
}
