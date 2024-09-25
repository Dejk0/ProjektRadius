using ReadLocationsCSV;
using FluentAssertions;
namespace ReadLocationsCSVTest
{
  public class UnitTest1
  {
    public string FilePathToCSVTest = @"C:\Users\deakt\source\repos\ProjektRadius\ReadLocationsCSV\Resourse\test.csv";

    [Fact]
    public void GetLenght_From_30angle_Test()
    {
      var reader = new Reader();
      reader.EarthRadiusInMater = 1;
      reader.GetLenght(30).Should().BeApproximately(0.5, 0.000001);
    }
    [Fact]
    public void GetLenght_From_Little_Angle()
    {
      var reader = new Reader();
      reader.GetLenght(0.00001).Should().Be(1.111949266445582);
    }
    [Fact]
    public void GetLenght_From_0angle_Test()
    {
      // Arrange
      var reader = new Reader();
      reader.EarthRadiusInMater = 1;

      // Act
      var result = reader.GetLenght(0);

      // Assert
      result.Should().Be(0);
    }
    [Fact]
    public void GetLenght_From_90angle_Test()
    {
      // Arrange
      var reader = new Reader();
      reader.EarthRadiusInMater = 6371000; // A Föld sugara méterben

      // Act
      var result = reader.GetLenght(90);

      // Assert
      result.Should().BeApproximately(6371000, 0.0000001);
    }
    [Fact]
    public void GetLenght_From_30angle_and_DoubleEarthRadius_Test()
    {
      // Arrange
      var reader = new Reader();
      reader.EarthRadiusInMater = 2;

      // Act
      var result = reader.GetLenght(30);

      // Assert
      result.Should().BeApproximately(1.0, 0.00000000000001);
    }
    [Fact]
    public void GetLenght_From_180angle_Test()
    {
      // Arrange
      var reader = new Reader();
      reader.EarthRadiusInMater = 6371000;

      // Act
      var result = reader.GetLenght(180);

      // Assert
      result.Should().BeApproximately(0, 0.0000001);
    }
    [Fact]
    public void SetLattitudeFromLine_Test()
    {  
      var reader = new Reader();
      string line = "20,01";
      var result = reader.SetLattitudeFromLine(line);
      result.Should().Be(20.01);
    }
    [Fact]
    public void SetLongtitudeFromLine_Test()
    {
      var reader = new Reader();
      string line = "40,02";
      var result = reader.SetLongtitudeFromLine(line);
      result.Should().Be(40.02);
    }
    [Fact]
    public void Reader_SkipsFirstLine_Test()
    {
      string filePath = "testdata.csv";
      string[] testData = { "HeaderLine", "1;20,01;40,02", "1;20,01;40,02", "1;20,01;40,02" };
      File.WriteAllLines(filePath, testData);
      var reader = new Reader();
      reader.FilePath = filePath;
      reader.Read();
      reader.Longtitude.Count.Should().Be(3);
    }
    [Fact]
    public void SplitTheLineTest()
    {
      var reader = new Reader();
      string line = "1;20,01;40,02";
      string[] split = reader.SplitTheLine(line);
      split.Length.Should().Be(3);
    }
    [Fact]
    public void ReaderTest() 
    {
      var reader = new Reader();
      reader.FilePath = "Resourse\\test.csv";
      reader.Read();
      reader.Latitude.Count.Should().Be(6);
    }
    [Fact]
    public void SetTheFirstLongtitudeTest()
    {
      string filePath = "testdata.csv";
      string[] testData = { "HeaderLine", "1;20,01;40,02", "1;20,01;40,02", "1;20,01;40,02" };
      File.WriteAllLines(filePath, testData);
      var reader = new Reader();
      reader.FilePath = filePath;
      reader.Read();
      reader.SetTheAVGLists();
      reader.SetTheFirstLongtitudeAVG();
      reader.Longtitude[0].Should().Be(reader.FirstLongtitude);
    }
    [Fact]
    public void SetTheFirstLatitudeTest()
    {
      string filePath = "testdata.csv";
      string[] testData = { "HeaderLine", "1;20,01;40,02", "1;20,01;40,02", "1;20,01;40,02" };
      File.WriteAllLines(filePath, testData);
      var reader = new Reader();
      reader.FilePath = filePath;
      reader.Read();
      reader.SetTheAVGLists();
      reader.SetTheFirstlatitudeAVG();
      reader.Latitude[0].Should().Be(reader.FirstLatitude);
    }
    [Fact]
    public void SetTheCoordinateFromCSVCountingTest()
    {
      string filePath = "testdata.csv";
      string[] testData = { "HeaderLine", "1;20,01;40,02", "1;20,01;40,02", "1;20,01;40,02" };
      File.WriteAllLines(filePath, testData);
      var reader = new Reader();
      reader.FilePath = filePath;
      reader.SetTheCoordinateFromCSV();
      (reader.Latitude.Count/3).Should().Be(reader.X_Coordinates.Count);
    }
    [Fact]
    public void SetTheCoordinateFromCSVFirstCoordinateTest()
    {
      string filePath = "testdata.csv";
      string[] testData = { "HeaderLine", "1;20,01;40,02", "1;20,02;40,03", "1;20,02;40,05" };
      File.WriteAllLines(filePath, testData);
      var reader = new Reader();
      reader.FilePath = filePath;
      reader.SetTheCoordinateFromCSV();
      reader.X_Coordinates[0].Should().Be(0);
    }
    [Fact]
    public void SetTheCoordinateFromCSVSecondCoordinateTest()
    {
      string filePath = "testdata.csv";
      string[] testData = { "HeaderLine", "1;20,01;40,02", "20;20,02;40,03", "3202;20,02;40,05", "1;20,2;40,02", "20;20,4;40,03", "3202;20,8;40,05" };
      File.WriteAllLines(filePath, testData);
      var reader = new Reader();
      reader.FilePath = filePath;
      reader.SetTheCoordinateFromCSV();
      reader.X_Coordinates[1].Should().NotBe(0);
    }
    [Fact]
    public void SetTheTimeFromCSVWithBigTimeValueTest()
    {
      string filePath = "testdata.csv";
      string[] testData = { "HeaderLine", "30020230202303;20,02;40,05", "30020230202303;20,02;40,05", "30020230202303;20,02;40,05" };
      File.WriteAllLines(filePath, testData);
      var reader = new Reader();
      reader.FilePath = filePath;
      reader.SetTheCoordinateFromCSV();
      reader.Time[0].Should().Be(30020230202303);
    }
    [Fact]
    public void FilePathConstructorFirstXCorrdinateTest()
    {
      string filePath = "Resourse\\test.csv";
      var reader = new Reader(filePath);
      reader.X_Coordinates[0].Should().Be(0);
    }
    [Fact]
    public void FilePathConstructorSecXCorrdinateTest()
    {
      string filePath = "Resourse\\test.csv";
      var reader = new Reader(filePath);
      reader.X_Coordinates[1].Should().NotBe(0);
    }
    [Fact]
    public void Reader_EmptyFile_Test()
    {
      string filePath = "emptyfile.csv";
      string[] testData = { "HeaderLine" };
      File.WriteAllLines(filePath, testData);

      var reader = new Reader();
      reader.FilePath = filePath;
      reader.Read();

      reader.Longtitude.Count.Should().Be(0);
      reader.Latitude.Count.Should().Be(0);
      reader.Time.Count.Should().Be(0);
    }
    public void Reader_InvalidData_Test()
    {
      string filePath = "invaliddata.csv";
      string[] testData = { "HeaderLine", "abc;xyz;123" };
      File.WriteAllLines(filePath, testData);

      var reader = new Reader();
      reader.FilePath = filePath;

      Action act = () => reader.Read();
      act.Should().Throw<FormatException>();
    }
    [Fact]
    public void Reader_NullOrEmptyFilePath_Test()
    {
      var readerWithNullFilePath = new Reader();
      readerWithNullFilePath.FilePath = null;

      var readerWithEmptyFilePath = new Reader();
      readerWithEmptyFilePath.FilePath = "";

      Action actNullFilePath = () => readerWithNullFilePath.Read();
      Action actEmptyFilePath = () => readerWithEmptyFilePath.Read();

      actNullFilePath.Should().Throw<ArgumentNullException>().WithMessage("*FilePath*");
      actEmptyFilePath.Should().Throw<FileNotFoundException>().WithMessage("The file path is invalid.");
    }
    [Fact]
    public void GetLenght_SmallAngle_Test()
    {
      var reader = new Reader();

      var result = reader.GetLenght(0.000001);
      result.Should().BeApproximately(0.111194926644558, 0.000000000000001);
    }
    [Fact]
    public void Reader_MissingLatitudeOrLongitude_Test()
    {
      string filePath = "missinglatlong.csv";
      string[] testData = { "HeaderLine", "1;;40,02", "1;20,01;" };
      File.WriteAllLines(filePath, testData);

      var reader = new Reader();
      reader.FilePath = filePath;

      Action act = () => reader.Read();
      act.Should().Throw<FormatException>();
    }
    [Fact]
    public void CreatNC_FileOutputFormat_Test()
    {
      string filePath = "formattedOutputTest.csv";
      var reader = new Reader();
      reader.X_Coordinates.Add(123.456789);
      reader.Y_Coordinates.Add(987.654321);
      reader.Longtitude.Add(40.02); // These are just placeholders for the test
      reader.Latitude.Add(20.01);
      reader.CreatNC();

      string outputFilePath = "C:\\Users\\deakt\\source\\repos\\ProjektRadius\\ConsoleApp1\\Resourse\\nctest.csv";
      var lines = File.ReadAllLines(outputFilePath);

      lines[0].Should().Be("G01X123.456789Y987.654321");
    }
    [Fact]
    public void GetLenght_LargeAngle_Test()
    {
      var reader = new Reader();
      reader.EarthRadiusInMater = 6371000;

      var result = reader.GetLenght(270);
      result.Should().BeApproximately(-6371000, 0.0000001);
    }
    [Fact]
    public void GetDoubleListAvg_2_3_4_avg_Test()
    {
      var reader = new Reader();
      List<double> testlist = new List<double>();
      testlist.Add(2);
      testlist.Add(3);
      testlist.Add(4);
      List<double> result = reader.GetDoubleListAvg(testlist);
      result[0].Should().BeApproximately(3, 0.01);
    }
    [Fact]
    public void GetDoubleListAvg_2_3_4_4_avg_Test()
    {
      var reader = new Reader();
      List<double> testlist = new List<double>();
      testlist.Add(2);
      testlist.Add(3);
      testlist.Add(4);
      testlist.Add(4);
      testlist.Add(4);
      testlist.Add(4);
      List<double> result = reader.GetDoubleListAvg(testlist);
      result[1].Should().BeApproximately(4, 0.01);
    }
    [Fact]
    public void GetTimeListAvg_2_3_4_avg_Test()
    {
      var reader = new Reader();
      List<double> testlist = new List<double>();
      testlist.Add(2);
      testlist.Add(3);
      testlist.Add(4);
      List<double> result = reader.GetDoubleListAvg(testlist);
      result[0].Should().BeApproximately(3, 0.01);
    }
    [Fact]
    public void GetTimeListAvg_2_3_4_4_avg_Test()
    {
      var reader = new Reader();
      List<long> testlist = new List<long>();
      testlist.Add(2);
      testlist.Add(3);
      testlist.Add(4);
      testlist.Add(4);
      testlist.Add(4);
      testlist.Add(4);
      List<double> result = reader.GetTimeListAvg(testlist);
      result[1].Should().BeApproximately(4, 0.01);
    }
    [Fact]
    public void GetTimeListAvg_with_bignumbers_avg_Test()
    {
      var reader = new Reader();
      List<long> testlist = new List<long>();
      testlist.Add(323131231);
      testlist.Add(323131232);
      testlist.Add(323131233);
     
      List<double> result = reader.GetTimeListAvg(testlist);
      result[0].Should().BeApproximately(323131232, 0.01);
    }
    public void TestListLoading(Reader reader)
    {
      reader.Longtitude.Add(1);
      reader.Longtitude.Add(2);
      reader.Longtitude.Add(3);

      reader.Latitude.Add(4);
      reader.Latitude.Add(5);
      reader.Latitude.Add(6);

      reader.Time.Add(11);
      reader.Time.Add(12);
      reader.Time.Add(13);
    }
    [Fact]
    public void TestingTheTestList()
    {
      var reader = new Reader();
      TestListLoading(reader);
      reader.Longtitude[0].Should().Be(1);
      reader.Longtitude[1].Should().Be(2);
      reader.Longtitude[2].Should().Be(3);

      reader.Latitude[0].Should().Be(4);
      reader.Latitude[1].Should().Be(5);
      reader.Latitude[2].Should().Be(6);

      reader.Time[0].Should().Be(11);
      reader.Time[1].Should().Be(12);
      reader.Time[2].Should().Be(13);
    }
    [Fact]
    public void SetTheAVGListsTest() 
    {
      var reader = new Reader();
      TestListLoading(reader);
      reader.SetTheAVGLists();

      reader.LongtitudeAVG[0].Should().Be(2);
      reader.LatitudeAVG[0].Should().Be(5);
      reader.TimeAVG[0].Should().Be(12);
    
    }
    [Fact]
    public void SetTheAVGListsTest2()
    {
      var reader = new Reader();
      TestListLoading(reader);
      reader.SetTheAVGLists();

      reader.LongtitudeAVG[0].Should().Be(2);
      reader.LatitudeAVG[0].Should().Be(5);
      reader.TimeAVG[0].Should().Be(12);

    }
  }
}