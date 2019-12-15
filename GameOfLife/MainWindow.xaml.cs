﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Cell[,] cells;
        private int nbOfRowCell = 0;

        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();

        }

        private void DrawGrid()
        {
            nbOfRowCell = 20;
            double cellSize = 700 / nbOfRowCell;
            for (int i = 0; i < nbOfRowCell; i++)
            {
                RowDefinition gridRow = new RowDefinition();

                gridRow.Height = new GridLength(cellSize);

                myGrid.RowDefinitions.Add(gridRow);

            }
            int nbOfColumn = Convert.ToInt32(SystemParameters.WorkArea.Width / cellSize);
            for (int i = 0; i < nbOfColumn; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(cellSize);

                myGrid.ColumnDefinitions.Add(gridCol);
            }
            cells = new Cell[nbOfColumn, nbOfRowCell];

            for (int i = 0; i < nbOfColumn; i++)
            {
                for (int j = 0; j < nbOfRowCell; j++)
                {

                    System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                    rectangle.Name = "rectCol" + i.ToString() + "Row" + j.ToString();
                    rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Turquoise);
                    rectangle.Stroke = new SolidColorBrush(System.Windows.Media.Colors.Black);

                    Grid.SetColumn(rectangle, i);
                    Grid.SetRow(rectangle, j);

                    myGrid.Children.Add(rectangle);
                    cells[i, j] = new Cell(i, j, rectangle);

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
                int x = Grid.GetColumn(ClickedRectangle);
                int y = Grid.GetRow(ClickedRectangle);
                cells[x, y].State = State.ALIVE;

            }

            isDown = true;
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            var depObj = myGrid;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is Rectangle)
                {
                    Rectangle rect = (Rectangle)child;
                    rect.Fill= new SolidColorBrush(System.Windows.Media.Colors.Turquoise);
                }
            }
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            evaluate();
        }

        private void evaluate()
        {
            foreach (Cell cell in cells)
            {
                cell.prepare(cells);
            }

            foreach (Cell cell in cells)
            {
                cell.apply();
            }


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
                    int x = Grid.GetColumn(ClickedRectangle);
                    int y = Grid.GetRow(ClickedRectangle);
                    cells[x, y].State = State.ALIVE;

                }
            }

        }

        private void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            
            var slider = sender as Slider;
            double value = slider.Value;
            nbOfRowCell = (int)value;
            myGrid.ColumnDefinitions.Clear();
            myGrid.RowDefinitions.Clear();
            DrawGrid();

        }

    }
}
