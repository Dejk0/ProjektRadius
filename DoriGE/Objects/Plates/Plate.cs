using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using GraphicEngine.Common;
using OpenTK.Graphics.OpenGL4;

namespace DoriGE.Objects.Plate
{
  public class Plate : ObjectLoader
  {
    public Plate()
    {
      filePath = "Objects\\Plates\\plate.obj";
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
