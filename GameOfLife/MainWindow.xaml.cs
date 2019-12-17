using System;
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
        private int nbOfRowCell = 20;
        private int nbOfColumnCell;


        private HashSet<Tuple<int, int>> tupleList = new HashSet<Tuple<int, int>>();

        private int nbIterations = 0;

        private int nbAliveCells = 0;


        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();

        }

        private void DrawGrid()
        {
            double cellSize = 700 / nbOfRowCell;
            for (int i = 0; i < nbOfRowCell; i++)
            {
                RowDefinition gridRow = new RowDefinition();

                gridRow.Height = new GridLength(cellSize);

                myGrid.RowDefinitions.Add(gridRow);

            }
            nbOfColumnCell = Convert.ToInt32(SystemParameters.WorkArea.Width / cellSize);
            for (int i = 0; i < nbOfColumnCell; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(cellSize);

                myGrid.ColumnDefinitions.Add(gridCol);
            }
            cells = new Cell[nbOfColumnCell, nbOfRowCell];

            for (int i = 0; i < nbOfColumnCell; i++)
            {
                for (int j = 0; j < nbOfRowCell; j++)
                {

                    System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                    rectangle.Name = "rectCol" + i.ToString() + "Row" + j.ToString();
                    rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Turquoise);
                    HashSet<Tuple<int, int>>.Enumerator e = tupleList.GetEnumerator();
                    while (e.MoveNext())
                    {
                        Console.WriteLine(e.Current.Item1 + ";" + e.Current.Item2);
                        if (e.Current.Item1 == i && e.Current.Item2 == j)
                        {
                            Console.WriteLine("ici");
                            rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);

                        }



                    }
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
                Tuple<int, int> tuple = new Tuple<int, int>(x, y);
                tupleList.Add(tuple);

            }

            isDown = true;
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            tupleList.Clear();

            var depObj = myGrid;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is Rectangle)
                {
                    Rectangle rect = (Rectangle)child;
                    rect.Fill= new SolidColorBrush(System.Windows.Media.Colors.White);
                }
            }
        }

        public void resetCells()
        {
            var color = new SolidColorBrush(Colors.White);
            foreach (Cell cell in cells)
            {
                cell.rectangle.Fill = color;
                cell.State = State.DEAD;

            }
        }

        private void SliderSpeedValueChanged(object sender, RoutedEventArgs e)
        {
            lblSpeed.Content = "Vitesse de cycle : " + (int)sliderSpeed.Value + "ms";
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            startGame();
        }


        /// eventuellement faire une seule méthode pour les 3 template en passant le numéro du template en args
        private void setTemplate(object sender, RoutedEventArgs e)
        {
             // Top left position of the template
            List<Tuple<int,int>> positionList = new List<Tuple<int, int>>();
            Button btn = (Button)sender;
            if(btn.Name == "template1")
            {
                int x = 0, y = 0;
                positionList.Add(new Tuple<int, int>(x, y + 2));
                positionList.Add(new Tuple<int, int>(x + 1, y + 2));
                positionList.Add(new Tuple<int, int>(x + 2, y + 2));
                positionList.Add(new Tuple<int, int>(x + 1, y));
                positionList.Add(new Tuple<int, int>(x + 2, y + 1));
            }
            else if(btn.Name == "template2")
            {
                int x = nbOfColumnCell/2-5, y = nbOfRowCell/2;
                positionList.Add(new Tuple<int, int>(x, y));
                positionList.Add(new Tuple<int, int>(x + 1,y));
                positionList.Add(new Tuple<int, int>(x + 2, y));
                positionList.Add(new Tuple<int, int>(x + 3, y));
                positionList.Add(new Tuple<int, int>(x + 4, y));
                positionList.Add(new Tuple<int, int>(x + 5, y));
                positionList.Add(new Tuple<int, int>(x + 6, y));
                positionList.Add(new Tuple<int, int>(x + 7, y));
                positionList.Add(new Tuple<int, int>(x + 8, y));
                positionList.Add(new Tuple<int, int>(x + 9, y));

            }
            else
            {

            }

            resetCells();
            foreach(Tuple<int, int> cell in positionList)
            {
                cells[cell.Item1,cell.Item2].rectangle.Fill = new SolidColorBrush(Colors.Green);
                cells[cell.Item1, cell.Item2].State = State.ALIVE;
            }

        }

        private void setTemplate2(object sender, RoutedEventArgs e)
        {

        }

        private void setTemplate3(object sender, RoutedEventArgs e)
        {

        }

        private void startGame()
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(evaluate);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)sliderSpeed.Value);
            dispatcherTimer.Start();
        }

        private void evaluate(object sender, EventArgs e)
        {
            
            foreach (Cell cell in cells)
            {
                cell.prepare(cells);
            }

            foreach (Cell cell in cells)
            {
                cell.apply();
                if (cell.State == State.ALIVE)
                    nbAliveCells++;
            }
            nbIterations++;
            updateStats();
        }

        private void updateStats()
        {
            lblIterations.Content = "N° itérations : " + nbIterations;
            lblAlive.Content = "N° alive cells : " + nbAliveCells; //Pas sur que ça marche !!!!!!!!!!!!!!!!!!!!!!!!!!!!

            nbAliveCells = 0;

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
                    Tuple<int, int> tuple = new Tuple<int, int>(x, y);
                    tupleList.Add(tuple);

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
