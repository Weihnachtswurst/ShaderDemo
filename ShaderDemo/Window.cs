using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;

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
        PickSSB defaultPickSSB;
        int PickColorLocation;
        int PositionLocation;
        int NormalLocation;
        int TexCoordLocation;
        int ColorLocation;
        int UniformMouseXLocation;
        int UniformMouseYLocation;
        int UniformMVPMatrixLocation;
        int Program;
        int SSB;

        int SamplerLocation;
        int TextureID;
        
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

            TextureID = loadImage("C:/Users/Anwender/Desktop/test.png");

            // Setup openGL
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // Create MVP-Matrices and Camera Data
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.5f, 10000.0f);
            WorldMatrix = new Matrix4();
            ModelviewMatrix = new Matrix4();
            CameraPosition = new Vector3(0.5f, 0.5f, 0);

            defaultPickSSB = new PickSSB();
            defaultPickSSB.DepthValue = float.MaxValue;
            defaultPickSSB.PickIndex = -1;

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
            PickColorLocation = GL.GetAttribLocation(Program, "vertex_pick_index");
            PositionLocation = GL.GetAttribLocation(Program, "vertex_position");
            NormalLocation = GL.GetAttribLocation(Program, "vertex_normal");
            TexCoordLocation = GL.GetAttribLocation(Program, "vertex_tex_coord");
            ColorLocation = GL.GetAttribLocation(Program, "vertex_color");
            UniformMVPMatrixLocation = GL.GetUniformLocation(Program, "mvp_matrix");
            UniformMouseXLocation = GL.GetUniformLocation(Program, "mouse_x");
            UniformMouseYLocation = GL.GetUniformLocation(Program, "mouse_y");
            SamplerLocation = GL.GetUniformLocation(Program, "main_texture");


            GL.GenBuffers(1, out SSB);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSB);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr)(4 * sizeof(float)), ref defaultPickSSB, BufferUsageHint.DynamicCopy);
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
            Vector3 lookAtVector = new Vector3(0, 1, 0);
            
            GL.UseProgram(Program);
 
            GL.UniformMatrix4(UniformMVPMatrixLocation, false, ref MVPMatrix);
            GL.Uniform1(UniformMouseXLocation, Mouse.X);
            GL.Uniform1(UniformMouseYLocation, Height - Mouse.Y);
            
            //float[] output = new float[4];
            PickSSB output = new PickSSB();
            // int output = -1;
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSB);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, SSB);
            GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, (IntPtr)0, (IntPtr)(8), ref output);

            if (output.PickIndex != -1)
            {
                Console.Out.WriteLine(output.PickIndex);
                Picker.invoke(output.PickIndex);
            }

            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr)(2 * sizeof(float)), ref defaultPickSSB, BufferUsageHint.DynamicCopy);

            GL.UseProgram(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(0.0f, 0.8f, 0.8f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(Program);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.Uniform1(SamplerLocation, 0);

            m.draw(PositionLocation, NormalLocation, ColorLocation, TexCoordLocation, PickColorLocation);

            GL.UseProgram(0);

            SwapBuffers();
        }

        int loadImage(Bitmap image)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        int loadImage(string filename)
        {
            try
            {
                Bitmap file = new Bitmap(filename);
                return loadImage(file);
            }
            catch (FileNotFoundException e)
            {
                return -1;
            }
        }
    }
}


public struct PickSSB
{
    public int PickIndex;
    public float DepthValue;
}