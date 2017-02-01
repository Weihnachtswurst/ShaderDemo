namespace ShaderDemo
{
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
