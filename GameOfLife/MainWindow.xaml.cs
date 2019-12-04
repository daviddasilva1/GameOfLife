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
        private bool isDown = false;

        public MainWindow()
        {
            InitializeComponent();

            double nbOfRowCell = 40;
            double cellSize = 700 / nbOfRowCell;
            for(int i=0;i< nbOfRowCell; i++)
            {
                RowDefinition gridRow = new RowDefinition();

                gridRow.Height = new GridLength(cellSize);

                myGrid.RowDefinitions.Add(gridRow);

            }
            int nbOfColumn = Convert.ToInt32(SystemParameters.WorkArea.Width / cellSize);
            for(int i=0;i< nbOfColumn; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(cellSize);

                myGrid.ColumnDefinitions.Add(gridCol);
            }


            for (int i=0;i< nbOfColumn; i++)
            {
                for (int j=0;j< nbOfRowCell; j++)
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
            //string selectedRect;
            if (e.OriginalSource is Rectangle)
            {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;

                //var mouseWasDownOn = e.Source as FrameworkElement;
                //string elementName = mouseWasDownOn.Name;
                //selectedRect = elementName.ToString();
                ClickedRectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                // MessageBox.Show(selectedRect.ToString());

            }

            isDown = true;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDown = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if(isDown)
            {
                //string selectedRect;
                if (e.OriginalSource is Rectangle)
                {
                    Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;

                    //var mouseWasDownOn = e.Source as FrameworkElement;
                    //string elementName = mouseWasDownOn.Name;
                    //selectedRect = elementName.ToString();
                    ClickedRectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                    // MessageBox.Show(selectedRect.ToString());

                }
            }

        }

    }
}
