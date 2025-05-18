
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
        static bool stop = false;
        private static readonly DrawingMode m = DrawingMode.WithBrushLines;
        private static readonly int N = 3;
        private static readonly float mag = 150;

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
                if (vecs.Count > 0)
                {
                    for (int i = 1; i < N; i++)
                    {
                        Vector2 temp = vecs[i];
                        temp.X = vecs[i - 1].X + -mag * MathF.Cos(freqs[i - 1] * angle);
                        temp.Y = vecs[i - 1].Y + mag * MathF.Sin(freqs[i - 1] * angle);
                        vecs[i] = temp;
                    }
                }
                else
                {
                    vecs.Add(center);
                    for (int i = 1; i < N; i++)
                    {
                        vecs.Add(vecs[i - 1] + new Vector2(-mag * MathF.Cos(freqs[i - 1] * angle), mag * MathF.Sin(freqs[i - 1] * angle)));
                    }
                }
                accumulate = vecs[^1];
            }
            else if (m == DrawingMode.WithOutBrushLines)
            {
                for (int i = 1; i < N; i++)
                    accumulate += new Vector2(-mag * MathF.Cos(freqs[i - 1] * angle), mag * MathF.Sin(freqs[i - 1] * angle));
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
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                stop = !stop;
                if (stop)
                    UpdateVecs(m);
            }
            if (!stop)
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
        static void Main(string[] args)
        {
            int FPS = 300;
            Raylib.SetConfigFlags(ConfigFlags.AlwaysRunWindow | ConfigFlags.ResizableWindow);
            Raylib.SetTargetFPS(FPS);
            Raylib.InitWindow(w, h, "Complex");

            Reset();
            while(!Raylib.WindowShouldClose())
            {
                UpdateState();
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new(0x18, 0x18, 0x18));

                if (!stop)
                    UpdateVecs(m);
                Render(m);

                Raylib.DrawFPS(0, 0);
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }
    }
}
