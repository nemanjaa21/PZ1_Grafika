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
    /// Interaction logic for ChangeEllipseWindow.xaml
    /// </summary>
    public partial class ChangeEllipseWindow : Window
    {
        public Brush elipseColor;
        public Ellipse ellipse;
        public ChangeEllipseWindow(Ellipse ellipse)
        {
            InitializeComponent();
            this.ellipse = ellipse;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                elipseColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void button_ChangeEllipse_Click(object sender, RoutedEventArgs e)
        {
            ellipse.StrokeThickness = double.Parse(tb_Thickness.Text);
            ellipse.Stroke = Brushes.Black;
            ellipse.Fill = elipseColor;
        }
    }
}
