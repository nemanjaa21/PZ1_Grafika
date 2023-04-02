using PZ1_Nemanja_Malinovic.Model;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace PZ1_Nemanja_Malinovic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<int, int> a = new Dictionary<int, int>();

        private Importer importer;
        private Dictionary<long, SubstationEntity> substations;
        private Dictionary<long, NodeEntity> nodes;
        private Dictionary<long, SwitchEntity> switches;
        private Dictionary<long, LineEntity> lines;
        private List<Point> pointsList;
        List<object> history;
        int undoRedoPosition;

        public static Dictionary<string, bool> points = new Dictionary<string, bool>();
        public static Dictionary<long, Point> entityIds = new Dictionary<long, Point>();


        Dictionary<string, Tuple<double, double>> nodesPositions;

        public double X;
        public double Y;

        public double xMax { get; set; }
        public double yMin { get; set; }
        public double xMin { get; set; }

        public double yMax { get; set; }
        public List<Point> PointsList { get => pointsList; set => pointsList = value; }
        public List<object> History { get => history; set => history = value; }
        public int UndoRedoPosition { get => undoRedoPosition; set => undoRedoPosition = value; }

        public MainWindow()
        {
            substations = new Dictionary<long, SubstationEntity>();
            nodes = new Dictionary<long, NodeEntity>();
            switches = new Dictionary<long, SwitchEntity>();
            lines = new Dictionary<long, LineEntity>();
            nodesPositions = new Dictionary<string, Tuple<double, double>>();
            PointsList = new List<Point>();

            History = new List<object>();
            UndoRedoPosition = -1;
            InitializeComponent();
        }

        private void LoadEntities()
        {
            substations = importer.GetSubstations();
            nodes = importer.GetNodes();
            switches = importer.GetSwitches();
            lines = importer.GetLines();
        }

        #region zoom

        private Double zoomMax = 12;
        private Double zoomMin = 0.2;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta; // prilagodi zoom speed (e.Delta je mouse spin vrednost)

            if (zoom < zoomMin) { zoom = zoomMin; } // nova min granica
            if (zoom > zoomMax) { zoom = zoomMax;  } // nova max granica

            Point mousePosition = e.GetPosition(canvas);

            if (zoom > 1)
            {
                // transformacija velicine canvas-a na osnovu pozicije mouse-a
                canvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePosition.X, mousePosition.Y);
            }
            else
            {
                // transformacija velicine canvas-a
                canvas.RenderTransform = new ScaleTransform(zoom, zoom); 
            }

        }

        #endregion

        private void CalculateCoordination()
        {
            xMin = Math.Min(Math.Min(substations.Values.Min((item) => item.X), nodes.Values.Min((item) => item.X)), switches.Values.Min((item) => item.X)) - 0.01;
            xMax = Math.Max(Math.Max(substations.Values.Max((item) => item.X), nodes.Values.Max((item) => item.X)), switches.Values.Max((item) => item.X)) + 0.01;
            X = (xMax - xMin) / 300;

            yMin = Math.Min(Math.Min(substations.Values.Min((item) => item.Y), nodes.Values.Min((item) => item.Y)), switches.Values.Min((item) => item.Y)) - 0.01;
            yMax = Math.Max(Math.Max(substations.Values.Max((item) => item.Y), nodes.Values.Max((item) => item.Y)), switches.Values.Max((item) => item.Y)) + 0.01;
            Y = (yMax - yMin) / 300;
        }

        private void Draw()
        {
            if (substations.Count > 0)
            {
                foreach (var substation in substations)
                {
                    DrawNode(substation.Value.X, substation.Value.Y, substation.Value.Id, substation.Value.Name, "no status", new SolidColorBrush(Colors.Green));
                }
            }

            if (nodes.Count > 0)
            {
                foreach (var node in nodes)
                {

                    DrawNode(node.Value.X, node.Value.Y, node.Value.Id, node.Value.Name, "no status", new SolidColorBrush(Colors.Red));
                }
            }

            if (switches.Count > 0)
            {
                foreach (var sw in switches)
                {

                    DrawNode(sw.Value.X, sw.Value.Y, sw.Value.Id, sw.Value.Name, sw.Value.Status, new SolidColorBrush(Colors.DeepSkyBlue));
                }
            }

            FindLines(lines, entityIds);
        }

        private void DrawNode(double xValue, double yValue, long id, string name, string status, SolidColorBrush color)
        {
            double tempX = xMax - xValue;
            int pointX = (int)(tempX / X);

            double tempY = yValue - yMin;
            int pointY = (int)(tempY / Y);

            if (!points.ContainsKey(xValue + "," + yValue))
            {
                Ellipse el = new Ellipse();
                el.Stroke = color;
                el.Fill = color;
                el.Width = 4;
                el.Height = 4;
                el.ToolTip = "Id: " + id.ToString() + "\n" + "Name: " + name + "\n" + "Status: " + status;
                el.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
                Canvas.SetLeft(el, (pointY * 6) - 2);
                Canvas.SetTop(el, (pointX * 6) - 2);
                points[xValue + "," + yValue] = true;

                entityIds[id] = new System.Windows.Point(xValue, yValue);

                canvas.Children.Add(el);
                nodesPositions.Add(id.ToString(), new Tuple<double, double>((pointX * 6) - 2, (pointY * 6) - 2));
            }
            else
            {
                FindNewCoordinates(xValue, yValue, out double xValueNew, out double yValueNew);
                DrawNode(xValueNew, yValueNew, id, name, status, color);
            }
        }

        private void FindNewCoordinates(double xValue, double yValue, out double newX, out double newY)
        {
            if (!points.ContainsKey(xValue + "," + (yValue + 1)))
            {
                newX = xValue;
                newY = yValue + 1;
            }
            else if (!points.ContainsKey(xValue + "," + (yValue - 1)))
            {
                newX = xValue;
                newY = yValue - 1;
            }
            else if (!points.ContainsKey((xValue + 1) + "," + yValue))
            {
                newX = xValue + 1;
                newY = yValue;
            }
            else if (!points.ContainsKey((xValue - 1) + "," + yValue))
            {
                newX = xValue - 1;
                newY = yValue;
            }
            else
            {
                newX = xValue + 1;
                newY = yValue + 1;
            }

        }

        void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.From = 4;
            myDoubleAnimation.To = 40;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            (sender as Ellipse).BeginAnimation(Ellipse.WidthProperty, myDoubleAnimation);
            (sender as Ellipse).BeginAnimation(Ellipse.HeightProperty, myDoubleAnimation);
        }

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Cast sender as a Line object
            System.Windows.Shapes.Line line = sender as System.Windows.Shapes.Line;
           
            // Get the ellipse objects at each end of the line
            Ellipse ellipse1 = GetEllipseAtPoint(line.X1,line.Y1);
            Ellipse ellipse2 = GetEllipseAtPoint(line.X2,line.Y2);

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.From = 4;
            myDoubleAnimation.To = 16;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            ellipse1.BeginAnimation(Ellipse.WidthProperty, myDoubleAnimation);
            ellipse1.BeginAnimation(Ellipse.HeightProperty, myDoubleAnimation);
            ellipse2.BeginAnimation(Ellipse.WidthProperty, myDoubleAnimation);
            ellipse2.BeginAnimation(Ellipse.HeightProperty, myDoubleAnimation);


            //ColorAnimation myColorAnimation = new ColorAnimation();
            //myColorAnimation.From = Colors.Purple;
            //myColorAnimation.To = Colors.Red;
            //myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(5));
            //ellipse1.BeginAnimation(Ellipse.FillProperty, myColorAnimation);
            //ellipse2.BeginAnimation(Ellipse.FillProperty, myColorAnimation);

        }



        private Ellipse GetEllipseAtPoint(double x, double y)
        {
            foreach (var child in canvas.Children)
            {
                if (child is Ellipse ellipse)
                {
                    // Get the center point of the ellipse
                    double centerX = Canvas.GetLeft(ellipse) + ellipse.Width / 2;
                    double centerY = Canvas.GetTop(ellipse) + ellipse.Height / 2;

                    // Calculate the distance between the point and the center of the ellipse
                    double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));

                    // Check if the distance is less than the radius of the ellipse
                    if (distance < ellipse.Width / 2 && distance < ellipse.Height / 2)
                    {
                        return ellipse;
                    }
                }
            }

            return null;
        }



        private void FindLines(Dictionary<long, LineEntity> lines, Dictionary<long, System.Windows.Point> ids)
        {
            
            List<LineEntity> foundedLines2 = new List<LineEntity>();

            foreach (var line in lines)
            {
                if (ids.ContainsKey(line.Value.FirstEnd) && ids.ContainsKey(line.Value.SecondEnd))
                {
                    
                    foundedLines2.Add(line.Value);
                }
            }

            DrawLines(foundedLines2);
        }

        private void DrawLines(List<LineEntity> Lines)
        {
            List<System.Windows.Shapes.Line> miniLines = new List<System.Windows.Shapes.Line>();

            foreach (var ln in Lines)
            {
                System.Windows.Shapes.Line newLine = new System.Windows.Shapes.Line();

                newLine.Y1 = nodesPositions[ln.FirstEnd.ToString()].Item1 + 2; // 2 piksela ispod Y1 koordinate
                newLine.X1 = nodesPositions[ln.FirstEnd.ToString()].Item2 + 2;
                newLine.Y2 = nodesPositions[ln.SecondEnd.ToString()].Item1 + 2;
                newLine.X2 = nodesPositions[ln.FirstEnd.ToString()].Item2 + 2;

                if (!miniLines.Contains(newLine))
                {
                    //ovim osiguravamo da ne crtamo liniju 2put
                    newLine.Stroke = System.Windows.Media.Brushes.DimGray;
                    newLine.StrokeThickness = 1;
                    newLine.MouseLeftButtonDown += Line_MouseLeftButtonDown;
                    newLine.ToolTip = "Id: " + ln.Id.ToString() + "\n";
                    newLine.ToolTip += "Name: " + ln.Name + "\n";
                    newLine.ToolTip += "IsUnderground: " + ln.IsUnderground + "\n";
                    newLine.ToolTip += "Line Type: " + ln.LineType + "\n";
                    newLine.ToolTip += "Conductor Material: " + ln.ConductorMaterial + "\n";
                    newLine.ToolTip += "First end: " + ln.FirstEnd.ToString() + "\n";
                    newLine.ToolTip += "Second end: " + ln.SecondEnd.ToString() + "\n";
                    newLine.ToolTip += "Thermal constant heat: " + ln.ThermalConstantHeat.ToString() + "\n";

                    canvas.Children.Add(newLine);
                    miniLines.Add(newLine);
                }

                System.Windows.Shapes.Line newLine2 = new System.Windows.Shapes.Line();

                newLine2.Y1 = nodesPositions[ln.SecondEnd.ToString()].Item1 + 2;
                newLine2.X1 = nodesPositions[ln.FirstEnd.ToString()].Item2 + 2;
                newLine2.Y2 = nodesPositions[ln.SecondEnd.ToString()].Item1 + 2;
                newLine2.X2 = nodesPositions[ln.SecondEnd.ToString()].Item2 + 2;

                if (!miniLines.Contains(newLine2))
                { //ovim osiguravamo da ne crtamo liniju 2put
                    newLine2.Stroke = System.Windows.Media.Brushes.DimGray;
                    newLine2.StrokeThickness = 1;
                    newLine2.MouseLeftButtonDown += Line_MouseLeftButtonDown;
                    newLine2.ToolTip = "Id: " + ln.Id.ToString() + "\n";
                    newLine2.ToolTip += "Name: " + ln.Name + "\n";
                    newLine2.ToolTip += "IsUnderground: " + ln.IsUnderground + "\n";
                    newLine2.ToolTip += "Line Type: " + ln.LineType + "\n";
                    newLine2.ToolTip += "Conductor Material: " + ln.ConductorMaterial + "\n";
                    newLine2.ToolTip += "First end: " + ln.FirstEnd.ToString() + "\n";
                    newLine2.ToolTip += "Second end: " + ln.SecondEnd.ToString() + "\n";
                    newLine2.ToolTip += "Thermal constant heat: " + ln.ThermalConstantHeat.ToString() + "\n";


                    canvas.Children.Add(newLine2);
                    miniLines.Add(newLine2);
                }
            }
        }

        List<LineEntity> lineEntities = new List<LineEntity>();
        private void Vod_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            string[] deloviToolTipa = ((Path)sender).ToolTip.ToString().Split(' ');
            string id = deloviToolTipa[1];
            LineEntity trenutniVod = lineEntities.Find(v => v.Id == long.Parse(id));
            ((Ellipse)canvas.FindName("e" + trenutniVod.FirstEnd.ToString())).Fill = System.Windows.Media.Brushes.Red;
            ((Ellipse)canvas.FindName("e" + trenutniVod.SecondEnd.ToString())).Fill = System.Windows.Media.Brushes.Red;
        }

        public void EllipseShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           //Izmena elipse
            
                Ellipse ellipse = (Ellipse)sender;
                ChangeEllipseWindow dialog = new ChangeEllipseWindow(ellipse);

                // Display the dialog box and wait for the user to close it.
                bool? result = dialog.ShowDialog();

                // If the user clicked the OK button, update the ellipse properties and values.
                if (result == true)
                {
                    ellipse.Fill = dialog.ellipse.Fill;
                    ellipse.StrokeThickness = dialog.ellipse.StrokeThickness;
                    
                }

            
            
        }
        
        public void PoligonShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Izmena poligona
            
                Polygon polygon = (Polygon)sender;
                ChangePolygonWindow dialog = new ChangePolygonWindow(polygon);

                // Display the dialog box and wait for the user to close it.
                bool? result = dialog.ShowDialog();

                // If the user clicked the OK button, update the polygon properties and values.
                if (result == true)
                {
                    polygon.Fill = dialog.polygon.Fill;
                    polygon.StrokeThickness = dialog.polygon.StrokeThickness;

                }

            

        }

        public void TextShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Izmena teksta
            
                TextBlock text = (TextBlock)sender;
                ChangeTextWindow dialog = new ChangeTextWindow(text);

                // Display the dialog box and wait for the user to close it.
                bool? result = dialog.ShowDialog();

                // If the user clicked the OK button, update the polygon properties and values.
                if (result == true)
                {
                    text.Foreground = dialog.text.Foreground;
                    text.FontSize = dialog.text.FontSize;

                }

            

        }

        private void mouseRightButtonDown_Canvas(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point point = e.GetPosition((IInputElement)sender);
            if ((bool)Ellipse_RadioButton.IsChecked)
            {
                DrawEllipseWindow drawElipseWindow = new DrawEllipseWindow(point, this);
                drawElipseWindow.Show();
            }
            else if ((bool)Polygon_RadioButton.IsChecked)
            {
                PointsList.Add(point);
            }
            else if ((bool)Text_RadioButton.IsChecked)
            {
                TextWindow textWindow = new TextWindow(point, this);
                textWindow.Show();
            }
        }

        private void mouseLeftButtonDown_Canvas(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point point = e.GetPosition((IInputElement)sender);
            if ((bool)Polygon_RadioButton.IsChecked)
            {
                if (this.pointsList.Count >= 3)
                {
                    DrawPolygonWindow drawPolygonWindow = new DrawPolygonWindow(this, PointsList);
                    //pointsList.Clear();
                    drawPolygonWindow.Show();
                }
                else
                {
                    System.Windows.MessageBox.Show("You have to choose at least 3 points for polygon!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
        }

        private void Button_LoadButton_Click(object sender, RoutedEventArgs e)
        {
            importer = new Importer();
            LoadEntities();
            CalculateCoordination();
            Draw();
        }

        private void button_Redo_Click(object sender, RoutedEventArgs e)
        {
            if (History.Count > UndoRedoPosition + 1)
            {
                if (History[UndoRedoPosition + 1] is Ellipse)
                {
                    Ellipse ell = (Ellipse)History[UndoRedoPosition + 1];
                    this.canvas.Children.Add(ell);
                    UndoRedoPosition++;
                }
                else if (History[UndoRedoPosition + 1] is Polygon)
                {
                    Polygon pol = (Polygon)History[UndoRedoPosition + 1];
                    this.canvas.Children.Add(pol);
                    UndoRedoPosition++;
                }
                else if (History[UndoRedoPosition + 1] is TextBlock)
                {
                    TextBlock tb = (TextBlock)History[UndoRedoPosition + 1];
                    this.canvas.Children.Add(tb);
                    UndoRedoPosition++;
                }
            }
        }

        private void button_Undo_Click(object sender, RoutedEventArgs e)
        {
            if (UndoRedoPosition > -1)
            {
                if (History[UndoRedoPosition] is Ellipse)
                {
                    Ellipse ell = (Ellipse)History[UndoRedoPosition];
                    this.canvas.Children.Remove(ell);
                    UndoRedoPosition--;
                }
                else if (History[UndoRedoPosition] is Polygon)
                {
                    Polygon pol = (Polygon)History[UndoRedoPosition];
                    this.canvas.Children.Remove(pol);
                    UndoRedoPosition--;
                }
                else if (History[UndoRedoPosition] is TextBlock)
                {
                    TextBlock tb = (TextBlock)History[UndoRedoPosition];
                    this.canvas.Children.Remove(tb);
                    UndoRedoPosition--;
                }
            }
        }

        private void button_Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in History)
            {
                if (item is Ellipse)
                {
                    Ellipse ell = (Ellipse)item;
                    this.canvas.Children.Remove(ell);
                }
                else if (item is Polygon)
                {
                    Polygon pol = (Polygon)item;
                    this.canvas.Children.Remove(pol);
                }
                else if (item is TextBlock)
                {
                    TextBlock tb = (TextBlock)item;
                    this.canvas.Children.Remove(tb);
                }


            }
        }




    }
}
