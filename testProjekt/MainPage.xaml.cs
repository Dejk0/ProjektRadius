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
      
      try
      {
        string fileName = "angles.csv";


        string filePath = Path.Combine("/storage/emulated/0/Download/", fileName);
      

        // Fájl írása az AppDataDirectory-ba
        using (StreamWriter sw = new StreamWriter(filePath))
        {
          sw.WriteLine("Time;AlfaAngle;BetaAngle;GammaAngle");          
        }
        Console.WriteLine(File.ReadAllText(filePath));

        // "/data/user/0/com.companyname.testprojekt/files/angles.csv"
        Application.Current.MainPage.DisplayAlert("Success", $"File saved to {filePath}", "OK");
      }
      catch (Exception ex)
      {
         Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
      }
    }
  }
}
