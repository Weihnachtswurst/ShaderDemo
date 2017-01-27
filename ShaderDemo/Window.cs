using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Input;

namespace ShaderDemo
{
    class Window : GameWindow
    {
        Model m = new Model();
        Matrix4 ProjectionMatrix;
        Matrix4 WorldMatrix;
        Matrix4 ModelviewMatrix;
        Vector3 CameraPosition;
        float[] SSBDefaultData = { 0.0f, 0.0f, 0.0f, float.MaxValue };
        int PickColorLocation;
        int PositionLocation;
        int ColorLocation;
        int UniformMouseXLocation;
        int UniformMouseYLocation;
        int UniformMVPMatrixLocation;
        int Program;
        int SSB;
        
        public Window(int width, int height) : base(width, height) {}

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.5f, 10000.0f);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int status_code;
            string info;

            Console.Out.WriteLine(GL.GetString(StringName.Version));
                        
            m.loadFromOBJ("C:/Users/Anwender/Downloads/lpm_yard.obj");
            m.init();

            // Setup openGL
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // Create MVP-Matrices and Camera Data
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.5f, 10000.0f);
            WorldMatrix = new Matrix4();
            ModelviewMatrix = new Matrix4();
            CameraPosition = new Vector3(0.5f, 0.5f, 0);

            // Read shaders from file
            // string VertexSource = File.ReadAllText("Shader/vs.glsl");
            // string FragmentSource = File.ReadAllText("Shader/fr.glsl");

            string VertexSource = File.ReadAllText("C:/Users/Anwender/Source/Repos/ShaderDemo/ShaderDemo/Shader/vs.glsl");
            string FragmentSource = File.ReadAllText("C:/Users/Anwender/Source/Repos/ShaderDemo/ShaderDemo/Shader/fr.glsl");

            // Create Shaders
            int VertexID = GL.CreateShader(ShaderType.VertexShader);
            int FragmentID = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(VertexID, VertexSource);
            GL.CompileShader(VertexID);
            GL.GetShaderInfoLog(VertexID, out info);
            GL.GetShader(VertexID, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
                throw new ApplicationException(info);

            // Compile fragment shader
            GL.ShaderSource(FragmentID, FragmentSource);
            GL.CompileShader(FragmentID);
            GL.GetShaderInfoLog(FragmentID, out info);
            GL.GetShader(FragmentID, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
                throw new ApplicationException(info);

            // Create and Link Program
            Program = GL.CreateProgram();
            GL.AttachShader(Program, FragmentID);
            GL.AttachShader(Program, VertexID);

            GL.BindFragDataLocation(Program, 0, "color");

            GL.LinkProgram(Program);

            GL.UseProgram(Program);

            // Get Buffer Locations
            PickColorLocation = GL.GetAttribLocation(Program, "vertex_pick_color");
            PositionLocation = GL.GetAttribLocation(Program, "vertex_position");
            ColorLocation = GL.GetAttribLocation(Program, "vertex_color");
            UniformMVPMatrixLocation = GL.GetUniformLocation(Program, "mvp_matrix");
            UniformMouseXLocation = GL.GetUniformLocation(Program, "mouse_x");
            UniformMouseYLocation = GL.GetUniformLocation(Program, "mouse_y");
            
            GL.GenBuffers(1, out SSB);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSB);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr)(4 * sizeof(float)), SSBDefaultData, BufferUsageHint.DynamicCopy);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, SSB);

            GL.UseProgram(0);

            // Load Models
            //model.loadFromOBJ("Models/lpm_yard.obj", "Models/lpm_yard.mtl");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            #region GetKeyboardInput
            KeyboardState keystate = OpenTK.Input.Keyboard.GetState();

            if (keystate.IsKeyDown(Key.W))
            {
                CameraPosition.Z -= 0.02f;
            } 
            if (keystate.IsKeyDown(Key.S))
            {
                CameraPosition.Z += 0.02f;
            } 
            if (keystate.IsKeyDown(Key.A))
            {
                CameraPosition.X -= 0.02f;
            } 
            if (keystate.IsKeyDown(Key.D))
            {
                CameraPosition.X += 0.02f;
            }
            if (keystate.IsKeyDown(Key.Space))
            {
                CameraPosition.Y += 0.02f;
            } 
            if (keystate.IsKeyDown(Key.ShiftLeft))
            {
                CameraPosition.Y -= 0.02f;
            }
            #endregion GetKeyboardInput

            WorldMatrix = Matrix4.CreateTranslation(-CameraPosition);
            ModelviewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -2.0f);
            Matrix4 MVPMatrix = ModelviewMatrix * WorldMatrix * ProjectionMatrix;

            GL.UseProgram(Program);
 
            GL.UniformMatrix4(UniformMVPMatrixLocation, false, ref MVPMatrix);
            GL.Uniform1(UniformMouseXLocation, Mouse.X);
            GL.Uniform1(UniformMouseYLocation, Height - Mouse.Y);

            float[] output = new float[4];
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSB);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, SSB);
            GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, (IntPtr)0, (IntPtr)(4 * sizeof(float)), output);

            if (output[3] != float.MaxValue)
            {
                Console.Out.WriteLine(output[0] + "/" + output[1] + "/" + output[2] + "/" + output[3]);
            }

            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr)(4 * sizeof(float)), SSBDefaultData, BufferUsageHint.DynamicCopy);

            GL.UseProgram(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(0.0f, 0.8f, 0.8f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(Program);

            m.draw(PositionLocation, ColorLocation, PickColorLocation);

            GL.UseProgram(0);

            SwapBuffers();
        }
    }
}
