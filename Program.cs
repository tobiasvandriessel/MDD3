﻿// C# example program to demonstrate OpenTK
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
using System.IO;
 
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
        static float rotation_speed = 90.0f;
        static Random rand = new Random();
        static float angle;
        static Vector3[] activeVectors;
        static int[] iterationsNeeded;
        static int kValue;

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.On;
        }
 
        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest); //Added, but not sure what this does.

            base.OnLoad(e);
 
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            iterationsNeeded = new int[40]; //To store the iterations needed by each of the settings

            CreatePointCloud();

            GL.GenBuffers(1, out vbo_id);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_id);
            GL.BufferData(BufferTarget.ArrayBuffer,
                          new IntPtr(8000 * BlittableValueType.StrideOf(vertices)),
                          vertices, BufferUsageHint.StaticDraw);

        }
 
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
 
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
 
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 12000000.0f);
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

            Matrix4 modelview = Matrix4.LookAt(new Vector3(4000f, 4000f, -2000f), new Vector3(2500f, 2500f, 0), Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            //GL.Rotate(angle, 0.0f, 1.0f, 0.0f);                   //ROTATE


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
            FileStream fs = new FileStream("Test2.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);

            Vector3[] newVertices = new Vector3[8000];
            vertices = new Vector3[8000];
            vbo_size = 8000;


            int n = 8000;
            int k = 500; //Or 1000

            double sizeX = 600; //Or 9, 12, 15 m?
            double sizeY = 600; //Or 4, 3, 2.4 m?

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
                    for (int ratio = 0; ratio < 4; ratio++ )
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

                        double startX = 2500 - 0.5 * sizeX;
                        double startY = 2500 - 0.5 * sizeY;
                        double endX = 2500 + 0.5 * sizeX;
                        double endY = 2500 + 0.5 * sizeY;


                        #region set points not model
                        //For every point not on the model
                        for (int i = 0; i < n - k; i++)
                        {
                            newVertices[i] = new Vector3((float)rand.NextDouble() * 5000, (float)rand.NextDouble() * 5000, (float)rand.NextDouble() * 2000);
                        }
                        #endregion

                        #region set points model
                        //For every point on the model
                        for (int i = n - k; i < n; i++)
                        {
                            newVertices[i] = new Vector3((float)rand.NextDouble() * (float)sizeX + (float)startX, (float)rand.NextDouble() * (float)sizeY + (float)startY, 1000);
                        }
                        #endregion

                        #region ball

                        //Apply the ball to the points on the model.
                        if (radius != 0)
                            for (int i = n - k; i < n; i++)
                            {
                                newVertices[i].X += 2 * ((float)rand.NextDouble() - 0.5f) * (float)radius;
                                newVertices[i].Y += 2 * ((float)rand.NextDouble() - 0.5f) * (float)radius;
                                newVertices[i].Z += 2 * ((float)rand.NextDouble() - 0.5f) * (float)radius;



                            }

                        #endregion
                        if(!klaar)
                            klaar = true;
                        vertices = newVertices;

                        int[] iterArray = new int[100];

                        Console.WriteLine("k: {0}, radius: {1}, ratio: {2}", kVal, radius, ratio);
                        for (int a = 0; a < 100; a++)
                        {
                            iterArray[a] = Ransac(vertices, k);
                            Console.WriteLine(iterArray[a]);
                        }
                    }
                }
            }
            #endregion

            sw.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointCloud"></param>
        /// <param name="d"></param>
        /// <returns>amount of iterations needed</returns>
        static int Ransac(Vector3[] pointCloud, int d )
        {
            bool planeFound = false;
            int iterations = 0;

            while (!planeFound)
            {
                int support = 0;
                iterations++;

                Vector3 a, b, c;
                int q = rand.Next(8000);
                a = pointCloud[q];
                int r = rand.Next(8000);
                b = pointCloud[r];
                int s = rand.Next(8000);
                c = pointCloud[s];

                if (q == r || r == s || q == s)
                    continue;

                Vector3 f = new Vector3(a[0] - b[0], a[1] - b[1], a[2] - b[2]);
                Vector3 g = new Vector3(c[0] - b[0], c[1] - b[1], c[2] - b[2]);
                Vector3 cross = new Vector3( ( f[ 1 ] * g[ 2 ] ) - ( f[ 2 ] * g[ 1 ] ) , ( f[ 2 ] * g[ 0 ] ) - ( f[ 0 ] * g[ 2 ] ) , ( f[ 0 ] * g[ 1 ] ) - ( f[ 1 ] * g[ 0 ] ) );
                float det = -1 * (cross[0] * a[0] + cross[1] * a[1] + cross[2] * a[2]);
                float[] plane = new float[4];
                plane[0] = cross[0];
                plane[1] = cross[1];
                plane[2] = cross[2];
                plane[3] = det;

                float div = (float)(Math.Sqrt(plane[0] * plane[0] + plane[1] * plane[1] + plane[2] * plane[2]));

                for (int i = 0; i < 8000 - 3; i++)
                {
                    if (i == q || i == r || i == s)
                        continue;
                    float distance = (plane[0] * pointCloud[i][0] + plane[1] * pointCloud[i][1] + plane[2] * pointCloud[i][2] + plane[3]) / div;
                    if(Math.Abs(distance) <= 4 )
                    {
                        support++;
                        if (support >= 0.9 * d)
                        {
                            planeFound = true;
                            break;
                        }
                    }
                }
            }

            return iterations;
        }
    }
}
