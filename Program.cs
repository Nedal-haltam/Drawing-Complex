
using Raylib_cs;
using System.Numerics;

namespace Drawing_Complex
{
    internal class Program
    {
        static Random random = new();
        static int w = 800;
        static int h = 600;
        static float angle = 0.0f;
        static float mag = 90.0f;
        static float phase = MathF.PI / 4.0f;
        static int N = 3;

        private static readonly List<Vector2> vecs = [];
        private static readonly List<float> freqs = [];
        private static readonly List<Vector2> pts = [];
        static Vector2 center = new(w / 2, h / 2);
        static void Reset()
        {
            pts.Clear();
            vecs.Clear();
            freqs.Clear();
            for (int i = 1; i < N; i++)
                freqs.Add((float)i / N + random.NextSingle() * 0.5f);
        }
        static void Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.AlwaysRunWindow | ConfigFlags.ResizableWindow);
            int FPS = 60;
            Raylib.SetTargetFPS(FPS);
            Raylib.InitWindow(w, h, "Complex");
            Reset();
            while(!Raylib.WindowShouldClose())
            {
                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    Reset();
                }
                w = Raylib.GetScreenWidth();
                h = Raylib.GetScreenHeight();
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new(0x18, 0x18, 0x18));

                angle += 0.05f;
                //vecs.Add(center);
                //for (int i = 1; i < N; i++)
                //{
                //    vecs.Add(vecs[i - 1] + new Vector2(-mag * MathF.Cos(freqs[i - 1] * angle + (i + 1) * phase), mag * MathF.Sin(freqs[i - 1] * angle + (i + 1) * phase)));
                //}
                //pts.Add(vecs[^1]);
                //for (int i = 1; i < N; i++)
                //    Raylib.DrawLineV(vecs[i - 1], vecs[i], Color.White);
                Vector2 accumulate = new(0, 0);
                for (int i = 1; i < N; i++)
                    accumulate += new Vector2(-mag * MathF.Cos(freqs[i - 1] * angle + (i + 1) * phase), mag * MathF.Sin(freqs[i - 1] * angle + (i + 1) * phase));
                accumulate += center;
                pts.Add(accumulate);

                for (int i = 0; i < pts.Count - 1; i++)
                    Raylib.DrawLineV(pts[i], pts[i + 1], Color.White);


                vecs.Clear();
                Raylib.DrawFPS(0, 0);
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }
    }
}
