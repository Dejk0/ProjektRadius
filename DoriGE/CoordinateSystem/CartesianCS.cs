using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DoriGE.CoordinateSystem
{
  public class CartesianCS
  {
    public OpenTK.Mathematics.Vector3 Location = new OpenTK.Mathematics.Vector3(0, 0, 0);
    public OpenTK.Mathematics.Vector3 LinearVelocity = new OpenTK.Mathematics.Vector3(0, 0, 0);
    public OpenTK.Mathematics.Vector3 LinearAccelaretion = new OpenTK.Mathematics.Vector3(0, 0, 0);
    public OpenTK.Mathematics.Vector3 Rotation = new OpenTK.Mathematics.Vector3(0, 0, 0);
    public OpenTK.Mathematics.Vector3 AngularVelocity = new OpenTK.Mathematics.Vector3(0, 0, 0);
    public OpenTK.Mathematics.Vector3 AngularAcceleration = new OpenTK.Mathematics.Vector3(0, 0, 0);    
  }
}
