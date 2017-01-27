namespace ShaderDemo
{
    using System.IO;
    using ModelData;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using OpenTK.Graphics.OpenGL4;
    using System.Globalization;

    class Model
    {
        private Dictionary<string, Material> materials;
        private List<float[]> vertices;
        private List<float[]> normals;
        private List<float[]> textureVertices;
        private List<OBJObject> objects;
        
        public Model()
        {
            materials = new Dictionary<string, Material>();
            vertices = new List<float[]>();
            normals = new List<float[]>();
            textureVertices = new List<float[]>();
            objects = new List<OBJObject>();
            // objects.Add(new OBJObject()); // Add Default Object
        }

        public void init()
        {
            foreach (var o in objects)
            {
                o.VertexBufferID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, o.VertexBufferID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(o.VertexData.Count * sizeof(float)), o.VertexData.ToArray(), BufferUsageHint.StaticDraw);

                o.ColorBufferID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, o.ColorBufferID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(o.ColorData.Count * sizeof(float)), o.ColorData.ToArray(), BufferUsageHint.StaticDraw);

                o.PickColorBufferID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, o.PickColorBufferID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(o.PickColorData.Count * sizeof(float)), o.PickColorData.ToArray(), BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }

        public void draw(int positionLocation, int colorLocation, int pickColorLocation)
        {
            if (positionLocation == -1)
                Console.Out.WriteLine("Invalid Position Location");

            if (colorLocation == -1)
                Console.Out.WriteLine("Invalid Color Location");

            if (pickColorLocation == -1)
                Console.Out.WriteLine("Invalid Pick Color Location");

            foreach (var o in objects)
            {
                GL.EnableVertexAttribArray(positionLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, o.VertexBufferID);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.EnableVertexAttribArray(colorLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, o.ColorBufferID);
                GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, 0, 0);

                GL.EnableVertexAttribArray(pickColorLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, o.PickColorBufferID);
                GL.VertexAttribPointer(pickColorLocation, 4, VertexAttribPointerType.Float, false, 0, 0);

                GL.DrawArrays(PrimitiveType.Triangles, 0, o.VertexCount);
                GL.DisableVertexAttribArray(colorLocation);
                GL.DisableVertexAttribArray(pickColorLocation);
                GL.DisableVertexAttribArray(positionLocation);
            }
        }

        public void loadFromOBJ(string path)
        {
            Material curMaterial = new Material("");
            string directory = path.Substring(0, path.LastIndexOf('/') + 1);

            foreach (var line in File.ReadLines(path))
            {
                if (line.StartsWith("v "))
                {
                    vertices.Add(Vector3FromLine(line));
                }
                else if (line.StartsWith("vn "))
                {
                    normals.Add(Vector3FromLine(line));
                }
                else if (line.StartsWith("vt "))
                {
                    textureVertices.Add(Vector3FromLine(line));
                }
                else if (line.StartsWith("f "))
                {
                    string[] s = line.Split('/', ' ');

                    int v1 = int.Parse(s[1]) - 1;
                    int v2 = int.Parse(s[4]) - 1;
                    int v3 = int.Parse(s[7]) - 1;

                    objects.Last().VertexCount += 3;

                    objects.Last().VertexData.AddRange(vertices[v1]);
                    objects.Last().VertexData.AddRange(vertices[v2]);
                    objects.Last().VertexData.AddRange(vertices[v3]);

                    objects.Last().ColorData.AddRange(curMaterial.DiffuseColor);
                    objects.Last().ColorData.AddRange(curMaterial.DiffuseColor);
                    objects.Last().ColorData.AddRange(curMaterial.DiffuseColor);

                    objects.Last().PickColorData.AddRange(new float[] { 1.0f, 0.0f, 0.0f, 1.0f});
                    objects.Last().PickColorData.AddRange(new float[] { 1.0f, 0.0f, 0.0f, 1.0f });
                    objects.Last().PickColorData.AddRange(new float[] { 1.0f, 0.0f, 0.0f, 1.0f });
                }
                else if (line.StartsWith("o "))
                {
                    objects.Add(new OBJObject());
                }
                else if (line.StartsWith("g "))
                {
                    Console.Out.WriteLine("Groups are not supported!");
                }
                else if (line.StartsWith("usemtl"))
                {
                    curMaterial = materials[line.Substring(7, line.Length - 7)];
                }
                else if (line.StartsWith("mtllib"))
                {
                    readMTL(directory + line.Substring(7, line.Length - 7));
                }
            }
        }

        private void readMTL(string path)
        {
            Material curMaterial = new Material("");
            
            foreach (var line in File.ReadLines(path))
            {
                if (line.StartsWith("newmtl"))
                {
                    string temp = line.Substring(7, line.Length - 7);

                    curMaterial = new Material(temp);
                    materials.Add(temp, curMaterial);
                }
                else if (line.StartsWith("Kd"))
                {
                    curMaterial.DiffuseColor = Vector4FromLine(line);
                }
                else if (line.StartsWith("Ka"))
                {
                    curMaterial.AmbientColor = Vector4FromLine(line);
                }
                else if (line.StartsWith("Ks"))
                {
                    curMaterial.SpecularColor = Vector4FromLine(line);
                }
                else if (line.StartsWith("Ke"))
                {
                    curMaterial.EmissiveColor = Vector4FromLine(line);
                }
                else if (line.StartsWith("Ns"))
                {
                    curMaterial.SpecularExponent = float.Parse(line.Substring(3, line.Length - 3));
                }
                else if (line.StartsWith("d"))
                {
                    curMaterial.Alpha = float.Parse(line.Substring(2, line.Length - 2));
                }
            }
        }

        private float[] Vector3FromLine(string line)
        {
            string[] elements = line.Split(' ');
            float[] vector = new float[3];

            vector[0] = float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat);
            vector[1] = float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat);
            vector[2] = float.Parse(elements[3], CultureInfo.InvariantCulture.NumberFormat);

            return vector;
        }

        private float[] Vector4FromLine(string line)
        {
            string[] elements = line.Split(' ');
            float[] vector = new float[4];

            vector[0] = float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat);
            vector[1] = float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat);
            vector[2] = float.Parse(elements[3], CultureInfo.InvariantCulture.NumberFormat);
            if (elements.Length > 4)
                vector[3] = float.Parse(elements[4], CultureInfo.InvariantCulture.NumberFormat);
            else
                vector[3] = 1.0f;

            return vector;
        }
    }
}

namespace ModelData
{
    using System.Collections.Generic;
    using System.Text;

    public class OBJObject
    {
        public List<float> VertexData { get; set; }
        public List<float> ColorData { get; set; }
        public List<float> PickColorData { get; set; }
        public int ColorBufferID { get; set; }
        public int VertexBufferID { get; set; }
        public int PickColorBufferID { get; set; }
        public int VertexCount { get; set; }

        public OBJObject()
        {
            VertexData = new List<float>();
            ColorData = new List<float>();
            PickColorData = new List<float>();
            ColorBufferID = -1;
            VertexBufferID = -1;
            VertexCount = 0;
        }
    }

    public class Material
    {
        private string name;
        public float Alpha { get; set; }
        public float SpecularExponent { get; set; }
        public float[] DiffuseColor { get; set; }
        public float[] AmbientColor { get; set; }
        public float[] SpecularColor { get; set; }
        public float[] EmissiveColor { get; set; }

        public Material(string name)
        {
            this.name = name;
            Alpha = 1;
            SpecularExponent = 1;
            DiffuseColor = new float[4];
            AmbientColor = new float[4];
            SpecularColor = new float[4];
            EmissiveColor = new float[4];
        }
    }
}


