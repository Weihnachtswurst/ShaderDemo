using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDemo
{
    static class Picker
    {
        static Dictionary<Color, Action> Table;
        const int Threashold = 100;
        static int R = 0;
        static int G = 0;
        static int B = 0;

        private static Color NextColor()
        {
            R++;
            if (R > Threashold)
            {
                R = 0;
                G++;
                if (G > Threashold)
                {
                    G = 0;
                    B++;
                    if (B > Threashold)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Out.WriteLine("Too many registered Colors!");
                        Console.ResetColor();

                        reset();
                    }
                }
            }

            return new Color((float)R / Threashold, (float)G / Threashold, (float)B / Threashold, 0);
        }

        public static Color Register(Action method)
        {
            Color color = NextColor();
            Table.Add(color, method);
            return color;
        }

        public static void Unregister(Action method)
        {
            Table.Remove(Table.First(kvp => kvp.Value == method).Key);
        }

        public static void Evaluate(float[] value)
        {
            Table[new Color(value[0], value[1], value[2], 0)]();
        }

        public static void reset()
        {
            Table.Clear();
            R = 0;
            G = 0;
            B = 0;
        }

    }

    class Color
    {
        float[] color { get; set; }

        public Color() : this(0, 0, 0, 0) { }

        public Color(float[] color) : this(color[0], color[1], color[2], color[3]) { }
        
        public Color(float x, float y, float z, float w)
        {
            color = new float[4];
            color[0] = x;
            color[1] = y;
            color[2] = z;
            color[3] = w;
        }

        public float x
        {
            get { return color[0]; }
            set { color[0] = value; }
        }

        public float y
        {
            get { return color[1]; }
            set { color[1] = value; }
        }

        public float z
        {
            get { return color[2]; }
            set { color[2] = value; }
        }

        public float w
        {
            get { return color[3]; }
            set { color[3] = value; }
        }

        public float[] data
        {
            get { return color; }
            set { color = value; }
        }

        public float this[int key]
        {
            get
            {
                return color[key];
            }
            set
            {
                color[key] = value;
            }
        } 

        public override int GetHashCode()
        {
            int i1 = BitConverter.ToInt32(BitConverter.GetBytes(color[0]), 0);
            int i2 = BitConverter.ToInt32(BitConverter.GetBytes(color[1]), 0);
            int i3 = BitConverter.ToInt32(BitConverter.GetBytes(color[2]), 0);
            int i4 = BitConverter.ToInt32(BitConverter.GetBytes(color[3]), 0);

            return i1 ^ i2 ^ i3 ^ i4;
        }
    }
}
