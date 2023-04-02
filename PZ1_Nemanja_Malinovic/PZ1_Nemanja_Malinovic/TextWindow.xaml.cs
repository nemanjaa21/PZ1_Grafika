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
    /// Interaction logic for TextWindow.xaml
    /// </summary>
    public partial class TextWindow : Window
    {
        Point point;
        MainWindow mainWindow;

        public TextWindow(Point point, MainWindow mainWindow)
        {
            InitializeComponent();
            this.point = point;
            this.mainWindow = mainWindow;
        }

        FontDialog dig = new FontDialog();
        private void butonText_TextFont_Click(object sender, RoutedEventArgs e)
        {

            if (dig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FontFamilyConverter ffc = new FontFamilyConverter();

                tb_AddText.FontSize = dig.Font.Size;
                tb_AddText.FontFamily = (FontFamily)ffc.ConvertFromString(dig.Font.Name);

                tb_AddText.FontFamily = new FontFamily(dig.Font.Name);
                tb_AddText.FontSize = dig.Font.Size * 98.0 / 72.0;
                tb_AddText.FontWeight = dig.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                tb_AddText.FontStyle = dig.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        Brush textColor;
        private void buttonText_TextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));

            }
        }

        private void button_AddText_Click(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = new TextBlock();

            textBlock.Text = tb_AddText.Text;
            textBlock.Foreground = textColor;
            textBlock.FontSize = dig.Font.Size;
            textBlock.MouseLeftButtonDown += mainWindow.TextShape_MouseLeftButtonDown;

            Canvas.SetLeft(textBlock, point.X);
            Canvas.SetTop(textBlock, point.Y);

            mainWindow.canvas.Children.Add(textBlock);
            mainWindow.History.Add(textBlock);
            mainWindow.UndoRedoPosition++;
        }
    }
}
