using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace bouncyballs {
    public static class util {
        public static Vector2f randvec2(float minx, float maxx, float miny, float maxy) {
            Vector2f v = new Vector2f();
            v.X = randfloat(minx, maxx);
            v.Y = randfloat(miny, maxy);
            return v;
        }
        public static Vector2f randvec2(float min, float max) {
            return randvec2(min, max, min, max);
        }

        public static int randint(int min, int max) {
            Random r = new Random((int)(DateTime.Now.Ticks%Int32.MaxValue));
            return (int)(r.NextDouble() * (max - min));
        }

        public static float randfloat(float min, float max) {
            Random r = new Random((int)(DateTime.Now.Ticks&Int32.MaxValue));
            return min + (float)r.NextDouble() * max;
        }

        public static byte randbyte()  {
            Random r = new Random((int)(DateTime.Now.Ticks%Int32.MaxValue));
            return (byte)(r.NextDouble() * 256);
        }

        public static Color hsvtocol(float hue, float sat, float val)
        {
            hue %= 360f;

            while(hue<0) hue += 360;

            if(sat<0f) sat = 0f;
            if(sat>1f) sat = 1f;

            if(val<0f) val = 0f;
            if(val>1f) val = 1f;

            int h = (int)(hue/60f);
            float f = hue/60-h;
            byte p = (byte)(val*(1f-sat) * 255);
            byte q = (byte)(val*(1f-sat*f) * 255);
            byte t = (byte)(val*(1f-sat*(1-f)) * 255);
            
            byte bVal = (byte)(val * 255);
            switch(h) {
                default:
                case 0:
                case 6: return new Color(bVal, t, p);
                case 1: return new Color(q, bVal, p);
                case 2: return new Color(p, bVal, t);
                case 3: return new Color(p, q, bVal);
                case 4: return new Color(t, p, bVal);
                case 5: return new Color(bVal, p, q);
            }
        }

        public static float distance(Vector2f a, Vector2f b) {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static float magnitude(Vector2f vec) {
            return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        }

        public static Vector2f normalise(Vector2f vec) {
            return vec / magnitude(vec);
        }

        public static float dot(Vector2f a, Vector2f b) {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public static Vector2f reflect(Vector2f dir, Vector2f normal) {
            return -2f * dot(dir, normal) * normal + dir;
        }

        public static void drawText(RenderWindow window, string text, Vector2f pos, Font? font = null, uint size = 12) {
            Text T = new Text();
            T.CharacterSize = size;
            T.DisplayedString = text;
            if (font == null) {
                Font arial = new Font("arial.ttf");
                T.Font = arial;
            } else {
                T.Font = font;
            }
            T.Position = pos;
            T.OutlineColor = Color.Black;
            T.FillColor = Color.White;
            window.Draw(T);
            window.Display();
        }
    } // end class
} // end namespace