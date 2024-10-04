using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLocationsCSV
{
  public class Reader
  {
    public List<double> Longtitude = new List<double>();
    public List<double> Latitude = new List<double>();
    public List<long> Time = new List<long>();
    public List<double> LongtitudeAVG = new List<double>();
    public List<double> LatitudeAVG = new List<double>();
    public List<double> TimeAVG = new List<double>();
    public List<double> X_Coordinates = new List<double>();
    public List<double> Y_Coordinates = new List<double>();
    public double FirstLongtitude;
    public double FirstLatitude;
    public string FilePath { get; set; }
    public StreamReader reader { get; set; }

    public double EarthRadiusInMater = 6371000.0;
    public Reader()
    {

    }
    public Reader(string filepath)
    {
      FilePath = filepath;
      SetTheCoordinateFromCSV();
    }
    public void SetTheCoordinateFromCSV()
    {
      Read();
      SetTheAVGLists();
      SetTheFirstlatitudeAVG();
      SetTheFirstLongtitudeAVG();
      for (int i = 0; i < LongtitudeAVG.Count; i++)
      {
        double longti = FirstLongtitude - LongtitudeAVG[i];
        double lat = FirstLatitude - LatitudeAVG[i];
        X_Coordinates.Add(GetLenght(longti));
        Y_Coordinates.Add(GetLenght(lat));
      }
    }
    public void CreatNC()
    {
      using (StreamWriter writer = new StreamWriter("C:\\Users\\deakt\\source\\repos\\ProjektRadius\\JustRunTheCreatNCForTesting\\Resourse\\nctest.csv"))
      {
        for (int i = 0; i < LongtitudeAVG.Count; i++)
        {
          string line = "G01X" + X_Coordinates[i].ToString().Replace(",", ".") + "Y" + Y_Coordinates[i].ToString().Replace(",", ".");
          writer.WriteLine(line);
        }
      }
    }
    public void Read()
    {
      if (FilePath == null)
      {
        throw new ArgumentNullException("*FilePath*");
      }
      else if (FilePath == "")
      {
        throw new FileNotFoundException("The file path is invalid.");
      }
      
      reader = new StreamReader(FilePath);
      using (reader)
      {
        reader.ReadLine();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          string[] split = SplitTheLine(line);

          Time.Add(SetTimeFromLine(split[0]));
          Longtitude.Add(SetLongtitudeFromLine(split[1]));
          Latitude.Add(SetLattitudeFromLine(split[2]));
        }      
    }
    }
    public void SetTheAVGLists()
    {
      LongtitudeAVG = GetDoubleListAvg(Longtitude);
      LatitudeAVG = GetDoubleListAvg(Latitude);
      TimeAVG = GetTimeListAvg(Time); 
    }
    public List<double> GetDoubleListAvg(List<double> list)
    {
      List<double> avg = new List<double>();  
      for (int i = 0; i <= list.Count-3; i=i+3)
      {
        double[] longavg = { list[i], list[i+1], list[i+2] };
        avg.Add(longavg.Average());
      }
     return avg;
    }
    public List<double> GetTimeListAvg(List<long> list)
    {
      List<double> avg = new List<double>();
      for (int i = 0; i <= list.Count - 3; i = i + 3)
      {
        double[] longavg = { list[i], list[i + 1], list[i + 2] };
        avg.Add(longavg.Average());
      }
      return avg;
    }
    public void SetTheFirstLongtitudeAVG()
    {
      FirstLongtitude = LongtitudeAVG[0];
    }
    public void SetTheFirstlatitudeAVG()
    {
      FirstLatitude = LatitudeAVG[0];
    }
    public void SetTheFirstTimeAVG()
    {
      FirstLatitude = LatitudeAVG[0];
    }
    public double GetLenght(double angle)
    {
      angle = angle * Math.PI / 180.0;
      return Math.Sin(angle) * EarthRadiusInMater;
    }
    public long SetTimeFromLine(string line)
    {
      return long.Parse(line);
    }
    public double SetLattitudeFromLine(string line)
    {
      return double.Parse(line.Replace(".", ","));
    }
    public double SetLongtitudeFromLine(string line)
    {
      return double.Parse(line.Replace(".", ","));
    }
    public string[] SplitTheLine(string line)
    {
      string[] split = line.Split(';');
      return split;
    }
  }
}
