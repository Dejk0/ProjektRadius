using GraphicEngine.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace DoriGE.Objects.Yoda
{
  public class Yoda : ObjectLoader
  {
    public Yoda()        
    { 
      filePath = "Objects\\Yoda\\yoda.obj";
      LoadModel();
      CalculateNormals();      
    }

    public void LoadingVaoModel(ref int _vaoModel)
    {
      var _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
      var _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
      _vaoModel = GL.GenVertexArray();
      GL.BindVertexArray(_vaoModel);
      {
        var positionLocation = _lightingShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        var normalLocation = _lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
      }

    }
  }
}
