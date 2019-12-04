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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            for (int i=0;i<20;i++)
            {
                for(int j=0;j<20;j++)
                {


                    System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();

                    rectangle.Name = "rectCol" + i.ToString() + "Row" + j.ToString();
                    rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Turquoise);
                    rectangle.Stroke = new SolidColorBrush(System.Windows.Media.Colors.Black);

                    Grid.SetColumn(rectangle, i);
                    Grid.SetRow(rectangle, j);

                    myGrid.Children.Add(rectangle);
                }

            }




        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string selectedRect;
            if (e.OriginalSource is Rectangle)
            {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;

                var mouseWasDownOn = e.Source as FrameworkElement;
                string elementName = mouseWasDownOn.Name;
                selectedRect = elementName.ToString();
                MessageBox.Show(selectedRect.ToString());
                
            }
        }

    }
}
