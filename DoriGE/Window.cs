using System;
using GraphicEngine.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using DoriGE;
using System.Security.Cryptography;
using System.IO;
using DoriGE.Objects;
using DoriGE.Objects.Yoda;
using DoriGE.Objects.Cubes;


namespace GraphicEngine
{
  public class Window : GameWindow
  {
    public DoriGE.Objects.CubePositions cubePositions = new DoriGE.Objects.CubePositions();
    public DoriGE.Objects.Yoda.Yoda yoda = new Yoda();
    public DoriGE.Objects.Cubes.Cube body = new Cube();
    public DoriGE.Objects.Plate.Plate plate = new DoriGE.Objects.Plate.Plate();


    // We need the point lights' positions to draw the lamps and to get light the materials properly
    private readonly Vector3[] _pointLightPositions =
    {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };


    private int yoda_vertexBufferObject;
    private int plate_vertexBufferObject;
    private int _vaoModel_yoda;
    private int _vaoModel_plate;
    private int _vaoModel_cube;
    private int _vaoLamp;

    private Shader _lampShader;
    private Shader _lightingShader;
    private Texture _diffuseMap;
    private Texture _specularMap;
    private Camera _camera;
    private bool _firstMove = true;
    private Vector2 _lastPos;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {      
    }

    protected override void OnLoad()
    {
      base.OnLoad();
      GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
      GL.Enable(EnableCap.DepthTest);
      yoda_vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, yoda_vertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, yoda._vertices.Length * sizeof(float), yoda._vertices, BufferUsageHint.StaticDraw);
      //plate_vertexBufferObject = GL.GenBuffer();     
      //GL.BufferData(BufferTarget.ArrayBuffer, plate._vertices.Length * sizeof(float), plate._vertices, BufferUsageHint.StaticDraw);
      _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
      _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

      //LoadingVaoModel(ref _vaoModel_yoda);
      yoda.LoadingVaoModel(ref _vaoModel_yoda);
      //plate.LoadingVaoModel(ref _vaoModel_plate);

      LoadingVboModel(yoda_vertexBufferObject);
      LoadingVaoLampModel(ref _vaoLamp);

      _diffuseMap = Texture.LoadFromFile("Resources/container2.png");
      _specularMap = Texture.LoadFromFile("Resources/container2_specular.png");

      _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

      CursorState = CursorState.Grabbed;
    }
   private void LoadingVaoModel(ref int _vaoModel)
    {
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
    private void LoadingVboModel(int _vertexBufferObject)
    {
      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, body._vertices.Length * sizeof(float), body._vertices, BufferUsageHint.StaticDraw);
    }

    private void LoadingVaoLampModel(ref int _vaoLampModel)
    {
      _vaoLampModel = GL.GenVertexArray();
      GL.BindVertexArray(_vaoLampModel);
      var positionLocation = _lampShader.GetAttribLocation("aPos");
      GL.EnableVertexAttribArray(positionLocation);
      GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
      float deltatime = (float)e.Time;
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.BindVertexArray(_vaoModel_yoda);

      _diffuseMap.Use(TextureUnit.Texture0);
      _specularMap.Use(TextureUnit.Texture1);
      _lightingShader.Use();

      _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _lightingShader.SetVector3("viewPos", _camera.Position);

      _lightingShader.SetInt("material.diffuse", 0);
      _lightingShader.SetInt("material.specular", 1);
      _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
      _lightingShader.SetFloat("material.shininess", 32.0f);


      _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
      _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
      _lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
      _lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

      // Point lights
      for (int i = 0; i < _pointLightPositions.Length; i++)
      {
        _lightingShader.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
        _lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
        _lightingShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
        _lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
        _lightingShader.SetFloat($"pointLights[{i}].constant", 1.0f);
        _lightingShader.SetFloat($"pointLights[{i}].linear", 0.09f);
        _lightingShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
      }

      // Spot light
      _lightingShader.SetVector3("spotLight.position", _camera.Position);
      _lightingShader.SetVector3("spotLight.direction", _camera.Front);
      _lightingShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
      _lightingShader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
      _lightingShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
      _lightingShader.SetFloat("spotLight.constant", 1.0f);
      _lightingShader.SetFloat("spotLight.linear", 0.09f);
      _lightingShader.SetFloat("spotLight.quadratic", 0.032f);
      _lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
      _lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

      Matrix4 yodaModell = Matrix4.CreateTranslation(yoda.Location);
      yoda.Rotation.X = 20.0f;
      yoda.Rotation.Y = 10.0f;
      yoda.LinearAccelaretion.X = 0.1f;
      yoda.LinearVelocity.X = yoda.LinearVelocity.X + yoda.LinearAccelaretion.X * deltatime;
      yoda.Location.X = yoda.Location.X + yoda.LinearVelocity.X * deltatime;
      yodaModell = yodaModell * Matrix4.CreateRotationX(yoda.Rotation.X);
      yodaModell = yodaModell * Matrix4.CreateRotationY(yoda.Rotation.Y);
      yodaModell = yodaModell * Matrix4.CreateRotationZ(yoda.Rotation.Z);

      _lightingShader.SetMatrix4("model", yodaModell);
      GL.DrawArrays(PrimitiveType.Triangles, 0, yoda._vertices.Length);


      GL.BindVertexArray(_vaoLamp);

      _lampShader.Use();

      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
      // We use a loop to draw all the lights at the proper position
      for (int i = 0; i < _pointLightPositions.Length; i++)
      {
        Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
        lampMatrix = lampMatrix * Matrix4.CreateTranslation(_pointLightPositions[i]);

        _lampShader.SetMatrix4("model", lampMatrix);

        GL.DrawArrays(PrimitiveType.Triangles, 0, body._vertices.Length);
      }

      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      if (!IsFocused)
      {
        return;
      }

      var input = KeyboardState;

      if (input.IsKeyDown(Keys.Escape))
      {
        Close();
      }

      const float cameraSpeed = 1.5f;
      const float sensitivity = 0.2f;

      if (input.IsKeyDown(Keys.W))
      {
        _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
      }
      if (input.IsKeyDown(Keys.S))
      {
        _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
      }
      if (input.IsKeyDown(Keys.A))
      {
        _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
      }
      if (input.IsKeyDown(Keys.D))
      {
        _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
      }
      if (input.IsKeyDown(Keys.Space))
      {
        _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
      }
      if (input.IsKeyDown(Keys.LeftShift))
      {
        _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
      }

      var mouse = MouseState;

      if (_firstMove)
      {
        _lastPos = new Vector2(mouse.X, mouse.Y);
        _firstMove = false;
      }
      else
      {
        var deltaX = mouse.X - _lastPos.X;
        var deltaY = mouse.Y - _lastPos.Y;
        _lastPos = new Vector2(mouse.X, mouse.Y);

        _camera.Yaw += deltaX * sensitivity;
        _camera.Pitch -= deltaY * sensitivity;
      }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      base.OnMouseWheel(e);

      _camera.Fov -= e.OffsetY;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, Size.X, Size.Y);
      _camera.AspectRatio = Size.X / (float)Size.Y;
    }
  }
}
