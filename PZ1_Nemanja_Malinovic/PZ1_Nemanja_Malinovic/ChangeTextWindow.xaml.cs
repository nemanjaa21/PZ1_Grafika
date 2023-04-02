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
    /// Interaction logic for ChangeTextWindow.xaml
    /// </summary>
    public partial class ChangeTextWindow : Window
    {
        public Brush textColor;
        public TextBlock text;
        public ChangeTextWindow(TextBlock text)
        {
            InitializeComponent();
            this.text = text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void button_ChangeText_Click(object sender, RoutedEventArgs e)
        {
            text.Foreground = textColor;
            text.FontSize = double.Parse(tb_Size.Text);
        }
    }
}
