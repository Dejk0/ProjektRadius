using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace DoriGE.Objects.Cubes
{
  public class Cube : ObjectLoader
  {
    public Cube()
    {
      filePath = "Objects\\Cubes\\cube.obj";
      LoadModel2();
      CalculateNormals();
    }
  }
}
