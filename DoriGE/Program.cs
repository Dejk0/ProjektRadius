using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GraphicEngine
{
    public static class Program
    {
        public static int Width = 800;
        public static int Heigth = 600;
        private static void Main()
        {

            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(Width, Heigth),
                Title = "D'ori Graphic Engine",
                Flags = ContextFlags.ForwardCompatible,                
            };
            var gameWindowSettings = new GameWindowSettings()
            {
              UpdateFrequency = 60
            };

            using (var window = new Window(gameWindowSettings, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}