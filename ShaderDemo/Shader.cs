using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace ShaderDemo
{
    class Shader
    {
        Dictionary<string, int> UniformLocations = new Dictionary<string, int>();
        Dictionary<string, int> AttributeLocations = new Dictionary<string, int>();
        int ProgramID = -1;
        int VertexID = -1;
        int FragmentID = -1;

        public void AttachSource(string source, ShaderType type)
        {
            string info;
            int statusCode;
            int shaderID;

            shaderID = GL.CreateShader(type);

            // Compile vertex shader
            GL.ShaderSource(shaderID, source);
            GL.CompileShader(shaderID);
            GL.GetShaderInfoLog(shaderID, out info);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            switch (type)
            {
                case ShaderType.VertexShader:
                    VertexID = shaderID;
                    break;
                case ShaderType.FragmentShader:
                    FragmentID = shaderID;
                    break;
            }
        }

        public void Compile()
        {
            ProgramID = GL.CreateProgram();
            GL.AttachShader(ProgramID, FragmentID);
            GL.AttachShader(ProgramID, VertexID);

            GL.BindFragDataLocation(ProgramID, 0, "color");

            GL.LinkProgram(ProgramID);
        }

        public void LookUpUniformLocation(params string[] uniformName)
        {
            foreach(var name in uniformName)
            {
                LookUpUniformLocation(name);
            }
        }

        public void LookUpUniformLocation(string uniformName)
        {
            int location = GL.GetUniformLocation(ProgramID, uniformName);
            UniformLocations.Add(uniformName, location);
        }

        public void LookUpAttributeLocation(params string[] attributeName)
        {
            foreach (var name in attributeName)
            {
                LookUpAttributeLocation(name);
            }
        }

        public void LookUpAttributeLocation(string attributeName)
        {
            int location = GL.GetAttribLocation(ProgramID, attributeName);
            AttributeLocations.Add(attributeName, location);
        }

        public int GetUniformLocation(string uniformName)
        {
            return UniformLocations[uniformName];
        }

        public int GetAttributeLocation(string attributeName)
        {
            return AttributeLocations[attributeName];
        }

        public void Use()
        {
            GL.UseProgram(ProgramID);
        }
    }
}
