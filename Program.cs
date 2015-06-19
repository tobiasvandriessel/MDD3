// C# example program to demonstrate OpenTK
//
// Steps:
// 1. Create an empty C# console application project in Visual Studio
// 2. Place OpenTK.dll in the directory of the C# source file
// 3. Add System.Drawing and OpenTK as References to the project
// 4. Paste this source code into the C# source file
// 5. Run. You should see a colored triangle. Press ESC to quit.
//
// Copyright (c) 2013 Ashwin Nanjappa
// Released under the MIT License
 
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
 
namespace DDM3
{
    class Game : GameWindow
    {
        static bool klaar = false;
        static double[,] pointCloud;
        static int n = 8000;
        static int vbo_id;
        static int vbo_size;
        static Vector3[] vertices;
        float rotation_speed = 90.0f;
        float angle;

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.On;
        }
 
        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest); //Added, but not sure what this does.

            base.OnLoad(e);
 
            //GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            CreatePointCloud();

            GL.GenBuffers(1, out vbo_id);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_id);
            GL.BufferData(BufferTarget.ArrayBuffer,
                          new IntPtr(8000 * BlittableValueType.StrideOf(vertices)),
                          vertices, BufferUsageHint.StaticDraw);

            klaar = true;

        }
 
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
 
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
 
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 12000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }
 
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
 
            if (Keyboard[Key.Escape])
                Exit();
        }
 
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            if (!Keyboard[OpenTK.Input.Key.Space])
                angle += rotation_speed * (float)e.Time;
 
            //Matrix4 modelview = Matrix4.LookAt(new Vector3(-2500f, 2500f, -7000f), Vector3.UnitZ, Vector3.UnitY); //Vector.Zero
            Matrix4 modelview = Matrix4.LookAt(new Vector3(2500f, 2500f, -8000f), new Vector3(2500f, 2500f, 0), Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            //GL.Rotate(angle, 0.0f, 1.0f, 0.0f);                   //ROTATE

            //GL.Begin(BeginMode.Points);

            //if(klaar)
            //{
            //    GL.Color3(1.0f, 0.0f, 0.0f);
            //    for(int i = 0; i < n; i++)
            //    {
            //        GL.Vertex3(pointCloud[i, 0], pointCloud[i, 1], pointCloud[i, 2]); 
            //    }
            //}
 
            //GL.End();


            //Vector3 scale = new Vector3(4, 4, 4);
            //GL.Scale(scale);

            if(klaar)
            {
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_id);
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, new IntPtr(0));
                //GL.DrawArrays(BeginMode.Points, 0, vbo_size);
                GL.DrawArrays(PrimitiveType.Points, 0, vbo_size);
            }
 
            SwapBuffers();
        }
 
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }

        static void CreatePointCloud()
        {

            vertices = new Vector3[8000];
            int index = 0;
            vbo_size = 8000;


            int n = 8000;
            int k = 500; //Or 1000
            //int radius = 0; //Or 1, 2, 3, 4 cm;
            int d = 4;

            double sizeX = 600; //Or 9, 12, 15 m?
            double sizeY = 600; //Or 4, 3, 2.4 m?

            //double startX = 2500 - sizeX;
            //double startY = 2500 - sizeY;
            //double endX = 2500 + sizeX;
            //double endY = 2500 + sizeY;

            Random rand = new Random();
            pointCloud = new double[n, 3];

            //nu weggecomment
            #region main loop 40 diff
            //different k values
            for (int kVal = 0; kVal < 2; kVal++)
            {
                k += (kVal * 500); //so, 500 or 100

                //different radia
                for (int radius = 0; radius < 5; radius++) //This takes care of the radius
                {
                    //different ratios
                    for (int ratio = 0; ratio < 4; ratio++)
                    {
                        switch (ratio)
                        {
                            case 0:
                                sizeX = 600;
                                sizeY = 600;
                                break;
                            case 1:
                                sizeX = 900;
                                sizeY = 400;
                                break;
                            case 2:
                                sizeX = 1200;
                                sizeY = 300;
                                break;
                            case 3:
                                sizeX = 1500;
                                sizeY = 240;
                                break;
                        }

                        double startX = 2500 - sizeX;
                        double startY = 2500 - sizeY;
                        double endX = 2500 + sizeX;
                        double endY = 2500 + sizeY;

                        #region set points not model
                        //For every point not on the model
                        for (int i = 0; i < n - k; i++)
                        {
                            //pointCloud[i, 0] = rand.NextDouble() * 5000;
                            //pointCloud[i, 1] = rand.NextDouble() * 5000;
                            //pointCloud[i, 2] = rand.NextDouble() * 2000;

                            vertices[i] = new Vector3((float)rand.NextDouble() * 5000, (float)rand.NextDouble() * 5000, (float)rand.NextDouble() * 2000);
                        }
                        #endregion

                        #region set points model
                        //For every point on the model
                        for (int i = n - k; i < n; i++)
                        {
                            //pointCloud[i, 0] = rand.NextDouble() * sizeX + startX;
                            //pointCloud[i, 1] = rand.NextDouble() * sizeY + startY;
                            //pointCloud[i, 2] = 1000;

                            vertices[i] = new Vector3((float)rand.NextDouble() * (float)sizeX + (float)startX, (float)rand.NextDouble() * (float)sizeY + (float)startY, 1000);
                        }
                        #endregion

                        #region ball

                        //Apply the ball to the points on the model.
                        if (radius != 0)
                            for (int i = n - k; i < n; i++)
                            {
                                //for (int j = 0; j < 3; j++)
                                //{
                                //    pointCloud[i, j] += rand.NextDouble() - 0.5 * (double)radius;
                                //}

                                vertices[i].X += (float)rand.NextDouble() - 0.5f * (float)radius;
                                vertices[i].Y += (float)rand.NextDouble() - 0.5f * (float)radius;
                                vertices[i].Z += (float)rand.NextDouble() - 0.5f * (float)radius;

                                
                                
                            }

                        #endregion

                        //for(int a = 0; a < 100; a++)
                        //{

                        //}

                        //klaar = true;

                        break;
                    }
                }
            }
            #endregion
        }

    }
}
