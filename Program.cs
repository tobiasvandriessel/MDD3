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
        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.On;
        }
 
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
 
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }
 
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
 
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
 
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
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
 
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
 
            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
 
            GL.Begin(BeginMode.Triangles);
 
            GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 4.0f);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 4.0f);
            GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(0.0f, 1.0f, 4.0f);
 
            GL.End();
 
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
            double[,] randArray = new double[n, 3];
            double[,] pointCloud = new double[n, 3];

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
                                sizeY = 2400;
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
                                for (int j = 0; j < 3; j++)
                                {
                                    randArray[i, j] = rand.NextDouble();

                                    switch (j)
                                    {
                                        case 0:
                                            pointCloud[i, j] = randArray[i, j] * 5000;
                                            break;
                                        case 1:
                                            pointCloud[i, j] = randArray[i, j] * 5000;
                                            break;
                                        case 2:
                                            pointCloud[i, j] = randArray[i, j] * 2000;
                                            break;
                                    }
                                }
                            }
                            #endregion

                        #region set points model
                            //For every point on the model
                            for (int i = n - k; i < n; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    randArray[i, j] = rand.NextDouble();

                                    switch (j)
                                    {
                                        case 0:
                                            pointCloud[i, j] = randArray[i, j] * (double)sizeX + (double)startX;
                                            break;
                                        case 1:
                                            pointCloud[i, j] = randArray[i, j] * (double)sizeY + (double)startY;
                                            break;
                                        case 2:
                                            pointCloud[i, j] = 1000;
                                            break;
                                    }
                                }
                            }
                            #endregion

                        #region ball
                            double[,] randBall = new double[n - k, 3];

                            //Apply the ball to the points on the model.
                            if (radius != 0)
                                for (int i = n - k; i < n; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        randBall[i, j] = rand.NextDouble() - 0.5 * (double)radius;

                                        pointCloud[i, j] += randBall[i, j];
                                    }
                                }

                            #endregion

                        for(int a = 0; a < 100; a++)
                        {
                                
                        }
                    }
                }
            }
            #endregion




        }
    }
}
