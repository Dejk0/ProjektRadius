using ReadLocationsCSV;
namespace ConsoleApp1
{
  internal class Program
  {
    static void Main(string[] args)
    {
      Reader reader = new Reader("Resourse\\locations.csv");
      reader.CreatNC();
    }
  }
}
