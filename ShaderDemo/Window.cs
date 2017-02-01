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
        // PickSSB defaultPickSSB;
        Shader shader;
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

            //defaultPickSSB = new PickSSB();
            //defaultPickSSB.DepthValue = float.MaxValue;
            //defaultPickSSB.PickIndex = -1;

            shader = new Shader();

            shader.AttachSource(File.ReadAllText("C:/Users/Anwender/Source/Repos/ShaderDemo/ShaderDemo/Shader/vs.glsl"), ShaderType.VertexShader);
            shader.AttachSource(File.ReadAllText("C:/Users/Anwender/Source/Repos/ShaderDemo/ShaderDemo/Shader/fr.glsl"), ShaderType.FragmentShader);

            shader.Compile();

            shader.Use();

            shader.LookUpAttributeLocation("vertex_pick_index", "vertex_position", "vertex_normal", "vertex_tex_coord", "vertex_color");
            shader.LookUpUniformLocation("mvp_matrix", "mouse_x", "mouse_y", "main_texture");

            GL.GenBuffers(1, out SSB);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSB);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr)(4 * sizeof(float)), ref PickSSB.Default, BufferUsageHint.DynamicCopy);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, SSB);

            GL.UseProgram(0);
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

            shader.Use();

            GL.UniformMatrix4(shader.GetUniformLocation("mvp_matrix"), false, ref MVPMatrix);
            GL.Uniform1(shader.GetUniformLocation("mouse_x"), Mouse.X);
            GL.Uniform1(shader.GetUniformLocation("mouse_y"), Height - Mouse.Y);
            

            PickSSB output = new PickSSB();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSB);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, SSB);
            GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, (IntPtr)0, (IntPtr)(8), ref output);

            if (output.PickIndex != -1)
            {
                Console.Out.WriteLine(output.PickIndex);
                Picker.invoke(output.PickIndex);
            }

            GL.BufferData(BufferTarget.ShaderStorageBuffer, (IntPtr)(2 * sizeof(float)), ref PickSSB.Default, BufferUsageHint.DynamicCopy);

            GL.UseProgram(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(0.0f, 0.8f, 0.8f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use(); ;

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.Uniform1(SamplerLocation, 0);

            m.draw(shader.GetAttributeLocation("vertex_position"), shader.GetAttributeLocation("vertex_normal"), shader.GetAttributeLocation("vertex_color"),
                shader.GetAttributeLocation("vertex_tex_coord"), shader.GetAttributeLocation("vertex_pick_index"));

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
    public static PickSSB Default = new PickSSB(1, 2);

    public PickSSB(int PickIndex, float DepthValue)
    {
        this.PickIndex = PickIndex;
        this.DepthValue = DepthValue;
    }
}