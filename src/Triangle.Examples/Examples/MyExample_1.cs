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

    /// <summary>
    /// Triangulate a polygon with hole and set minimum angle constraint.
    /// </summary>
    public static class MyExample_1
    {
        public static bool Run(bool print = false)//return Z[]
        {
            // Generate the input geometry.
            var poly = CreatePolygon();

            // Set minimum angle quality option.
            var quality = new QualityOptions() { MaximumArea = 0.2 };// { MinimumAngle = 20.0, MaximumArea = 0.1 };

            // Generate mesh using the polygons Triangulate extension method.
            var mesh = poly.Triangulate(quality);

            if (print) SvgImage.Save(mesh, "my-example_new.svg", 500);

            return true;
            
        }

        public static IPolygon CreatePolygon(double h = 0.2)
        {
            
            // Generate the input geometry.
            var poly = new Polygon();

            // Center point.
            var center = new Point(0, 0);
            
            var points = new List<Vertex>(6);
            points.Add(new Vertex(-1, 0));
            points.Add(new Vertex(0, 1));
            points.Add(new Vertex(1, 0));
            points.Add(new Vertex(3, 0));
            points.Add(new Vertex(0, 3));
            points.Add(new Vertex(-3, 0));
            

            //var points = new List<Vertex>(4);
            //points.Add(new Vertex(-2, 0));
            //points.Add(new Vertex(-2, 2));
            //points.Add(new Vertex(2, 2));
            //points.Add(new Vertex(2, 0));




            poly.Add(new Contour(points, 0, false));
            

            return poly;
        }
       
    }
}