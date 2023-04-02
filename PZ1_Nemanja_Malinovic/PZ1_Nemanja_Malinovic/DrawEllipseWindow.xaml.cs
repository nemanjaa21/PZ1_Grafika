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
    /// Interaction logic for DrawEllipseWindow.xaml
    /// </summary>
    public partial class DrawEllipseWindow : Window
    {
        public Point point { get; set; }
        public MainWindow mainWindow { get; set; }
        public DrawEllipseWindow(Point point, MainWindow mainWindow)
        {
            InitializeComponent();
            this.point = point;
            this.mainWindow = mainWindow;
        }
      
        public Brush elipseColor;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                elipseColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        public Brush textColor;

        private void button_TextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void button_DrawEllipse_Click(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            //preuzimamo vrednosti koje je korisnik uneo za elipsu
            ellipse.Width = double.Parse(tb_RadiusX.Text) * 2;
            ellipse.Height = double.Parse(tb_RadiusY.Text) * 2;
            ellipse.StrokeThickness = double.Parse(tb_Thickness.Text);

            ellipse.Fill = elipseColor;
            ellipse.Stroke = Brushes.Black;
            ellipse.MouseLeftButtonDown += mainWindow.EllipseShape_MouseLeftButtonDown;

            Canvas.SetLeft(ellipse, point.X);
            Canvas.SetTop(ellipse, point.Y);

            mainWindow.canvas.Children.Add(ellipse);
            mainWindow.History.Add(ellipse);
            mainWindow.UndoRedoPosition++;

            //preuzimamo vrednosti koje je korisnik uneo za tekst na elipsi

            TextBlock textBlock = new TextBlock();

            textBlock.Text = tb_AddText.Text;
            textBlock.Foreground = textColor;

            Canvas.SetLeft(textBlock, point.X + double.Parse(tb_RadiusX.Text));
            Canvas.SetTop(textBlock, point.Y + double.Parse(tb_RadiusY.Text));

            mainWindow.canvas.Children.Add(textBlock);
            mainWindow.History.Add(textBlock);
            mainWindow.UndoRedoPosition++;
        }

       

    }
}
