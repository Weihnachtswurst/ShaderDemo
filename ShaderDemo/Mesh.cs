using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace ShaderDemo
{
    public class Mesh
    {
        public List<float> VertexData { get; set; }
        public List<float> NormalData { get; set; }
        public List<float> ColorData { get; set; }
        public List<float> TexCoordData { get; set; }
        public List<int> PickColorData { get; set; }
        public int ColorBufferID { get; set; }
        public int VertexBufferID { get; set; }
        public int NormalBufferID { get; set; }
        public int TexCoordBufferID { get; set; }
        public int PickColorBufferID { get; set; }
        public int VertexCount { get; set; }
        public int PickIndex { get; set; }

        public Mesh()
        {
            VertexData = new List<float>();
            NormalData = new List<float>();
            ColorData = new List<float>();
            TexCoordData = new List<float>();
            PickColorData = new List<int>();
            ColorBufferID = -1;
            VertexBufferID = -1;
            VertexCount = 0;
            PickIndex = Picker.register(click);
        }

        private void click()
        {
            List<float> list = new List<float>(ColorData.Count);

            for (int i = 0; i < list.Count; i += 4)
            {
                list.AddRange(new float[] { 0, 1, 0, 1 });
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(list.Count * sizeof(float)), list.ToArray(), BufferUsageHint.StaticDraw);
        }
    }
}
