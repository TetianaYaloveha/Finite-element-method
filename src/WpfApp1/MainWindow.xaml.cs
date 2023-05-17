using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HelixToolkit.Wpf;
using MathNet.Numerics;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Content = Draw3DPlot();
        }
        private HelixViewport3D Draw3DPlot()
        {
            var myViewport = new HelixViewport3D();
            myViewport.Children.Add(new DefaultLights());
           // myViewport.Children.Add(new PointLight(Colors.White, new Point3D(0, 0, 10)));

            var builder = new MeshBuilder();
            // Add points to the builder
            builder.AddSphere(new Point3D(1, 0, 0), 0.1);
            builder.AddSphere(new Point3D(0, 1, 0), 0.2);
            builder.AddSphere(new Point3D(0, 0, 1), 0.3);
            var mesh = builder.ToMesh();

            var gradient = new LinearGradientBrush();
            gradient.GradientStops.Add(new GradientStop(Colors.Red, 0));
            gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.5));
            gradient.GradientStops.Add(new GradientStop(Colors.Green, 1));
            gradient.MappingMode = BrushMappingMode.Absolute;
            var material = new DiffuseMaterial(gradient);
            // Apply the material to the mesh
            mesh.TextureCoordinates = new PointCollection(mesh.Positions.Select(p => new Point(p.X, p.Y)));
            

            var model = new GeometryModel3D(mesh, material);

            var visual = new ModelVisual3D() { Content = model };

            myViewport.Children.Add(visual);

            var camera = new PerspectiveCamera
            {
                Position = new Point3D(0, 0, 0),
                LookDirection = new Vector3D(1, 1, -1),
                UpDirection = new Vector3D(0, 0, 5),
                FieldOfView = 60
            };
            myViewport.Camera = camera;
            myViewport.Background = Brushes.White;
            myViewport.ShowCoordinateSystem = true;
            myViewport.CoordinateSystemHorizontalPosition = 0;
            myViewport.CoordinateSystemVerticalPosition = 0;
            myViewport.Width = 300;
            myViewport.Height = 400;
            return myViewport;
        }
    }
}
