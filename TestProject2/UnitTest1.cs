using FluentAssertions;
using test;
namespace TestProject2
{
  public class UnitTest1
  {
    [Fact]
    public void Test()
    {
      var test = new test.Class1();
      test.sum(2, 2).Should().Be(4);
      
    }
  }
}