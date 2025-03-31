
using Raylib_cs;
using System.Numerics;
using System.Reflection.Metadata;

namespace Drawing_Complex
{
    internal class Program
    {
        static Random random = new();
        static int w = 1000;
        static int h = 750;
        static float angle = 0.0f;
        static float mag = 110.0f;
        static float phase = MathF.PI / 4.0f;
        static int N = 3;

        private static readonly List<Vector2> vecs = [];
        private static readonly List<float> freqs = [];
        private static readonly List<Vector2> pts = [];
        static void Reset()
        {
            pts.Clear();
            vecs.Clear();
            freqs.Clear();
            for (int i = 1; i < N; i++)
                freqs.Add((float)i / N + random.NextSingle() * 0.5f);
        }
        enum DrawingMode
        {
            WithBrushLines, WithOutBrushLines,
        }
        static void UpdateVecs(DrawingMode m)
        {
            Vector2 center = new(w / 2, h / 2);
            Vector2 accumulate = new();
            if (m == DrawingMode.WithBrushLines)
            {
                vecs.Add(center);
                for (int i = 1; i < N; i++)
                {
                    vecs.Add(vecs[i - 1] + new Vector2(-mag * MathF.Cos(freqs[i - 1] * angle + (i + 1) * phase), mag * MathF.Sin(freqs[i - 1] * angle + (i + 1) * phase)));
                }
                accumulate = vecs[^1];
            }
            else if (m == DrawingMode.WithOutBrushLines)
            {
                for (int i = 1; i < N; i++)
                    accumulate += new Vector2(-mag * MathF.Cos(freqs[i - 1] * angle + (i + 1) * phase), mag * MathF.Sin(freqs[i - 1] * angle + (i + 1) * phase));
                accumulate += center;
            }
            pts.Add(accumulate);
        }
        static void UpdateState()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.R) || Raylib.IsWindowResized())
            {
                Reset();
            }
            w = Raylib.GetScreenWidth();
            h = Raylib.GetScreenHeight();
            angle += 0.05f;
        }
        static void Render(DrawingMode m)
        {
            if (m == DrawingMode.WithBrushLines)
                for (int i = 1; i < N; i++)
                    Raylib.DrawLineV(vecs[i - 1], vecs[i], Color.White);

            for (int i = 0; i < pts.Count - 1; i++)
                Raylib.DrawLineV(pts[i], pts[i + 1], Color.White);
        }
        static void TakeScreenShotAndSave(int i)
        {
            // ffmpeg -framerate 60 -i image%d.png -c:v libx264 -pix_fmt yuv420p output.mp4
            string dirpath = $"./pics/";
            string filepath = $"image{i}.png";
            string destination = dirpath + $"{filepath}";

            if (!Directory.Exists(dirpath))
                Directory.CreateDirectory(dirpath);


            Image image = Raylib.LoadImageFromScreen();
            Raylib.ExportImage(image, filepath);
            Raylib.UnloadImage(image);
            while (!File.Exists(filepath)) ;
            File.Move(filepath, destination);
        }
        static void Main(string[] args)
        {
            int i = 0;
            Raylib.SetConfigFlags(ConfigFlags.AlwaysRunWindow | ConfigFlags.ResizableWindow);
            int FPS = 0;
            Raylib.SetTargetFPS(FPS);

            bool generate = false;
            int sec = 10;
            int duration = sec * 60;
            Raylib.InitWindow(w, h, "Complex");
            DrawingMode m = DrawingMode.WithBrushLines;
            Reset();
            while(!Raylib.WindowShouldClose())
            {
                UpdateState();
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new(0x18, 0x18, 0x18));

                UpdateVecs(m);
                Render(m);

                    vecs.Clear();
                Raylib.DrawFPS(0, 0);
                Raylib.EndDrawing();
                // TODO: specify a duration to produce produce image for and then pipe to ffmpeg
                if (generate && i < duration)
                    TakeScreenShotAndSave(i++);
            }
            Raylib.CloseWindow();
        }
    }
}
