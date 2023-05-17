namespace TriangleNet.Examples
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Text.RegularExpressions;
    using TriangleNet.Geometry;
    using TriangleNet.Meshing;
    using TriangleNet.Rendering.Text;
    using TriangleNet.Topology;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;
    using System.IO;

    using HelixToolkit;





    /// <summary>
    /// Triangulate a polygon with hole and set minimum angle constraint.
    /// </summary>
    public static class MyExample
    {
        public static bool Run(bool print = false)//return Z[]
        {
            // Generate the input geometry.
            var poly = CreatePolygon();

            // Set minimum angle quality option.
            var quality = new QualityOptions() { MaximumArea = 0.8 };// { MinimumAngle = 20.0, MaximumArea = 0.1 };

            // Generate mesh using the polygons Triangulate extension method.
            var mesh = poly.Triangulate(quality);

            if (print) SvgImage.Save(mesh, "my-example.svg", 500);
            
            double[] Z = Result((Mesh)mesh);
            Console.WriteLine("-------X------- " + "\t" + "------Y------"+ "\t" + "-------Z-------");

            //using (StreamWriter writer = new StreamWriter("D:\\Універ\\4 курс\\ЧММФ\\src\\solution_points.txt"))
            //{
            //    foreach(var el in mesh.Triangles)
            //    {
            //        Console.WriteLine(el.GetVertexID);
            //    }
            //}

            using (StreamWriter writer = new StreamWriter("D:\\Універ\\4 курс\\ЧММФ\\src\\solution_points.txt"))
            {
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    Console.WriteLine(String.Format("{0,22} {1,22} {2,22}", mesh.Vertices.ToArray()[i].X, mesh.Vertices.ToArray()[i].Y, Z[i]));
                    writer.WriteLine(mesh.Vertices.ToArray()[i].X + " " + mesh.Vertices.ToArray()[i].Y + " " + Z[i]);
                }
            }

            return mesh.Triangles.Count > 0;
        }

        public static double[] Result(Mesh mesh, double f = 2, double d = 0, double a11 = 1, double a22 = 1)
        {
            int iterCounter = 1;
            double[,] MainMatrix = new double[mesh.Vertices.Count(), mesh.Vertices.Count()];
            double[] B = new double[mesh.Vertices.Count()];
            foreach( var triangle in mesh.Triangles)
            {
                int i, j, m;
                i = Array.IndexOf(mesh.Vertices.ToArray(), triangle.GetVertex(0));
                j = Array.IndexOf(mesh.Vertices.ToArray(), triangle.GetVertex(1));
                m = Array.IndexOf(mesh.Vertices.ToArray(), triangle.GetVertex(2));
                Vertex i_ver, j_ver, m_ver;
                i_ver = triangle.GetVertex(0);
                j_ver = triangle.GetVertex(1);
                m_ver = triangle.GetVertex(2);
                double S = 0.0;
                S = Math.Abs((i_ver.X * j_ver.Y + j_ver.X * m_ver.Y + m_ver.X * i_ver.Y) -
                    (i_ver.Y * j_ver.X + j_ver.Y * m_ver.X + m_ver.Y * i_ver.X));
                double a_i, b_i, c_i,
                    a_j, b_j, c_j,
                    a_m, b_m, c_m;
                a_i = j_ver.X * m_ver.Y - m_ver.X * j_ver.Y;
                b_i = j_ver.Y - m_ver.Y;
                c_i = m_ver.X - j_ver.X;

                a_j = i_ver.X * m_ver.Y - m_ver.X * i_ver.Y;
                b_j = m_ver.Y - i_ver.Y;
                c_j = i_ver.X - m_ver.X;

                a_m = j_ver.X * i_ver.Y - i_ver.X * j_ver.Y;
                b_m = i_ver.Y - j_ver.Y;
                c_m = j_ver.X - i_ver.X;

                double[] B_arr = new double[3];
                B_arr[0] = 1.0 / 6 * f * S;
                B_arr[1] = 1.0 / 6 * f * S;
                B_arr[2] = 1.0 / 6 * f * S;
                Console.WriteLine($"\nStep {iterCounter}\nB array:");
                for (int i_ = 0; i_ < B_arr.Length; i_++)
                {
                    Console.WriteLine(B_arr[i_]);
                }
                B[i] += B_arr[0];// 1.0 / 6 * f * S;
                B[j] += B_arr[1];// 1.0 / 6 * f * S;
                B[m] += B_arr[2];// 1.0 / 6 * f * S;

                double[,] Me_matr = new double[3, 3];
                Me_matr[0,0] = d * 1.0 / 12 * S;
                Me_matr[0,1] = d * 1.0 / 24 * S;
                Me_matr[0,2] = d * 1.0 / 24 * S;
                Me_matr[1,0] = d * 1.0 / 24 * S;
                Me_matr[1,1] = d * 1.0 / 12 * S;
                Me_matr[1,2] = d * 1.0 / 24 * S;
                Me_matr[2,0] = d * 1.0 / 24 * S;
                Me_matr[2,1] = d * 1.0 / 24 * S;
                Me_matr[2,2] = d * 1.0 / 12 * S;
                Console.WriteLine("\nMe Matrix ");
                for (int i_ = 0; i_ < 3; i_++)
                {
                    for (int j_ = 0; j_ < 3; j_++)
                    {
                        Console.Write(Me_matr[i_, j_] + "   ");
                    }
                    Console.WriteLine();
                }
                //Me
                MainMatrix[i, i] += Me_matr[0, 0];
                MainMatrix[i, j] += Me_matr[0, 1];
                MainMatrix[i, m] += Me_matr[0, 2];
                MainMatrix[j, i] += Me_matr[1, 0];
                MainMatrix[j, j] += Me_matr[1, 1];
                MainMatrix[j, m] += Me_matr[1, 2];
                MainMatrix[m, i] += Me_matr[2, 0];
                MainMatrix[m, j] += Me_matr[2, 1];
                MainMatrix[m, m] += Me_matr[2, 2];

                //Ke
                double[,] Ke_matr = new double[3, 3];
                Ke_matr[0, 0] = (a11 * Math.Pow(b_i, 2) + a22 * Math.Pow(c_i, 2)) / ( S);
                Ke_matr[0, 1] = (a11 * b_i * b_j + a22 * c_i * c_j) / ( S);
                Ke_matr[0, 2] = (a11 * b_i * b_m + a22 * c_i * c_m) / (S);
                Ke_matr[1, 0] = (a11 * b_j * b_i + a22 * c_j * c_i) / (S);
                Ke_matr[1, 1] = (a11 * Math.Pow(b_j, 2) + a22 * Math.Pow(c_j, 2)) / (S);
                Ke_matr[1, 2] = (a11 * b_j * b_m + a22 * c_j * c_m) / ( S);
                Ke_matr[2, 0] = (a11 * b_m * b_i + a22 * c_m * c_i) / (S);
                Ke_matr[2, 1] = (a11 * b_m * b_j + a22 * c_m * c_j) / (S);
                Ke_matr[2, 2] = (a11 * Math.Pow(b_m, 2) + a22 * Math.Pow(c_m, 2)) / (S);
                Console.WriteLine("\nKe Matrix ");
                for (int i_ = 0; i_ < 3; i_++)
                {
                    for (int j_ = 0; j_ < 3; j_++)
                    {
                        Console.Write(Ke_matr[i_, j_] + "   ");
                    }
                    Console.WriteLine();
                }

                MainMatrix[i, i] += Ke_matr[0, 0];
                MainMatrix[i, j] += Ke_matr[0, 1];
                MainMatrix[i, m] += Ke_matr[0, 2];
                MainMatrix[j, i] += Ke_matr[1, 0];
                MainMatrix[j, j] += Ke_matr[1, 1];//
                MainMatrix[j, m] += Ke_matr[1, 2];
                MainMatrix[m, i] += Ke_matr[2, 0];
                MainMatrix[m, j] += Ke_matr[2, 1];
                MainMatrix[m, m] += Ke_matr[2, 2];
                iterCounter++;
            }

            
            double eps = 0.000006;
            Console.WriteLine();
            for(int i = 0; i < mesh.Vertices.Count; i++)
            {
                var x_i = mesh.Vertices.ToList<Vertex>()[i].X;
                var y_i = mesh.Vertices.ToList<Vertex>()[i].Y;

                if ((Math.Abs(x_i - 2) < eps) )
                    MainMatrix[i, i] = 1e15;
                //if ((Math.Abs(y_i - 2) < eps))
                //    MainMatrix[i, i] = 1e15;
                //if ((Math.Abs(y_i) < eps))
                //    MainMatrix[i, i] = 1e15;
                if ((Math.Abs(x_i + 2) < eps))
                    MainMatrix[i, i] = 1e15;
            }
            using (StreamWriter writer = new StreamWriter("D:\\Універ\\4 курс\\ЧММФ\\src\\Triangle.Examples\\Examples\\matrix.txt"))
            {
                writer.WriteLine("MAIN MARTIX! ");
                for (int i_ = 0; i_ < mesh.Vertices.Count; i_++)
                {
                    for (int j_ = 0; j_ < mesh.Vertices.Count; j_++)
                    {
                        writer.Write(String.Format("{0,22}", MainMatrix[i_, j_])) ;
                    }
                    writer.WriteLine();
                }
            }
            /*for(int i = 0; i < mesh.Vertices.Count; i++)
            {
                for (int j = 0; j < mesh.Vertices.Count; j++)
                {
                    Console.Write(MainMatrix[i, j] + "   ");
                }
                Console.WriteLine();
            }*/
            return LES(MainMatrix, B);
        }
        public static double[] LES(double[,] M, double[] b)
        {
            var m =  Matrix<double>.Build.DenseOfArray(M);
            var v = Vector<double>.Build.DenseOfArray(b);
            return m.Solve(v).ToArray();
        }
        public static IPolygon CreatePolygon(double h = 0.2)
        {
            
            // Generate the input geometry.
            var poly = new Polygon();

            // Center point.
            var center = new Point(0, 0);
            /*
            var points = new List<Vertex>(6);
            points.Add(new Vertex(-1, 0));
            points.Add(new Vertex(0, 1));
            points.Add(new Vertex(1, 0));
            points.Add(new Vertex(3, 0));
            points.Add(new Vertex(0, 3));
            points.Add(new Vertex(-3, 0));
            */
            var points = new List<Vertex>(4);
            points.Add(new Vertex(-2, 0));
            points.Add(new Vertex(-2, 2));
            points.Add(new Vertex(2, 2));
            points.Add(new Vertex(2, 0));




            poly.Add(new Contour(points, 0, false));
            

            return poly;
        }

    } //MainMatrix[i, i] += 1.0 / 12 * S;
      //MainMatrix[i, j] += 1.0 / 24 * S;
      //MainMatrix[i, m] += 1.0 / 24 * S;
      //MainMatrix[j, i] += 1.0 / 24 * S;
      //MainMatrix[j, j] += 1.0 / 12 * S;
      //MainMatrix[j, m] += 1.0 / 24 * S;
      //MainMatrix[m, i] += 1.0 / 24 * S;
      //MainMatrix[m, j] += 1.0 / 24 * S;
      //MainMatrix[m, m] += 1.0 / 12 * S;
    /*
      MainMatrix[i, i] += (a11 * Math.Pow(b_i, 2) + a22 * Math.Pow(c_i, 2)) / (2 * S);
              MainMatrix[i, j] += (a11 * b_i * b_j + a22 * c_i * c_j) / (2 * S);
              MainMatrix[i, m] += (a11 * b_i * b_m + a22 * c_i * c_m) / (2 * S);
              MainMatrix[j, i] += (a11 * b_j * b_i + a22 * c_j * c_i) / (2 * S);
              MainMatrix[j, j] += (a11 * Math.Pow(b_j, 2) + a22 * Math.Pow(c_j, 2)) / (2 * S);//
              MainMatrix[j, m] += (a11 * b_j * b_m + a22 * c_j * c_m) / (2 * S);
              MainMatrix[m, i] += (a11 * b_m * b_i + a22 * c_m * c_i) / (2 * S);
              MainMatrix[m, j] += (a11 * b_m * b_j + a22 * c_m * c_j) / (2 * S);
              MainMatrix[m, m] += (a11 * Math.Pow( b_m, 2) + a22 * Math.Pow(c_m, 2)) / (2 * S);
    */
}