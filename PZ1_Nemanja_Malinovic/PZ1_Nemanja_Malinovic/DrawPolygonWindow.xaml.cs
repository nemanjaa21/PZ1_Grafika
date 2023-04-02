using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PZ1_Nemanja_Malinovic
{
    /// <summary>
    /// Interaction logic for DrawPolygonWindow.xaml
    /// </summary>
    public partial class DrawPolygonWindow : Window
    {
        public MainWindow mainWindow;
        List<Point> pointList;
        public DrawPolygonWindow(MainWindow main, List<Point> points)
        {
            InitializeComponent();
            this.mainWindow = main;
            this.pointList = points;
        }

        Brush polygonColor;

        private void button_PolygonColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                polygonColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));

            }
        }

        Brush textColor;
        private void button_TextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));

            }
        }

        private void button_DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            Polygon polygon = new Polygon();

            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = double.Parse(tb_cThickness.Text);

            foreach (var item in pointList)
            {

                polygon.Points.Add(item);

            }

            polygon.Fill = polygonColor;
            polygon.MouseLeftButtonDown += mainWindow.PoligonShape_MouseLeftButtonDown;

            mainWindow.canvas.Children.Add(polygon);
            mainWindow.PointsList.Clear();
            mainWindow.History.Add(polygon);
            mainWindow.UndoRedoPosition++;

            TextBlock textBlock = new TextBlock();

            textBlock.Text = tb_AddText.Text;
            textBlock.Foreground = textColor;

            //ovo je da bi nasli gde da stavimo nas tekst
            double x = 0;
            double y = 0;
            foreach (var point in polygon.Points)
            {
                x += point.X;
                y += point.Y;
            }

            x /= polygon.Points.Count;
            y /= polygon.Points.Count;

            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);

            mainWindow.canvas.Children.Add(textBlock);
            mainWindow.History.Add(textBlock);
            mainWindow.UndoRedoPosition++;
            
        }


    }
}
