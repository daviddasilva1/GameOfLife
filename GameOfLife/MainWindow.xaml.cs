using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool isLeft = false;
        private bool isRight = false;
        private Cell[,] cells;
        private int nbOfRowCell = 17;
        private int nbOfColumnCell;
        List<Tuple<int, int>> positionList = new List<Tuple<int, int>>();


       // private HashSet<Tuple<int, int>> tupleList = new HashSet<Tuple<int, int>>();

       // private int nbIterations = 0;

        //private int nbAliveCells = 0;
        //private int nbOfCellMax =0;
        //private int nbOfCellMin = 0;
        //private int oldestCellAge = 0;

        private SolidColorBrush green = new SolidColorBrush(Colors.Green);
        private SolidColorBrush white = new SolidColorBrush(Colors.White);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);


        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
            Speed = 100;
            Iterations = 0;
            Oldest = 0;
            

            DataContext = this;


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
                    Rectangle rectangle = new Rectangle();
                    rectangle.Name = "rectCol" + i.ToString() + "Row" + j.ToString();
                    rectangle.Fill = white;
                    rectangle.Stroke = black;
                    Grid.SetColumn(rectangle, i);
                    Grid.SetRow(rectangle, j);

                    myGrid.Children.Add(rectangle);
                    cells[i, j] = new Cell(i, j, rectangle);
                }
            }
            updateStats();

            foreach (Tuple<int, int> cell in positionList.ToList())
            {
                if (cell.Item1 >= nbOfColumnCell || cell.Item2 >= nbOfRowCell)
                {
                    positionList.RemoveAll(x => x.Item1 == cell.Item1 && x.Item2 == cell.Item2);
                }
                else
                {
                    cells[cell.Item1, cell.Item2].rectangle.Fill = green;
                    cells[cell.Item1, cell.Item2].State = State.ALIVE;
                }

            }

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //string selectedRect;
            if (e.OriginalSource is Rectangle)
            {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                ClickedRectangle.Fill = green;

                int x = Grid.GetColumn(ClickedRectangle);
                int y = Grid.GetRow(ClickedRectangle);

                cells[x, y].State = State.ALIVE;
                Tuple<int, int> tuple = new Tuple<int, int>(x, y);
                positionList.Add(tuple);
                updateStats();
            }
            isLeft = true;

        }



        private void ResetClick(object sender, RoutedEventArgs e)
        {
           
            dispatcherTimer.Stop();
            // nbIterations = 0;
            Iterations = 0;
            //nbOfCellMin = 0;
            PopMin=0;
            //nbOfCellMax = 0;
            PopMax = 0;
            //nbAliveCells = 0;
            Alive = 0;
            Oldest = 0;
            updateStats();
            resetCells();
            SetEnableButtonsReset();
            positionList.Clear();
        }


        private void PauseClick(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Pause.IsEnabled = false;
            Start.IsEnabled = true;
            sliderSpeed.IsEnabled = true;
            sliderResize.IsEnabled = false;


        }

        private void SetEnableSliders()
        {
            if(sliderResize.IsEnabled)
            {
                sliderResize.IsEnabled = false;
                sliderSpeed.IsEnabled = false;
            }
            else
            {
                sliderResize.IsEnabled = true;
                sliderSpeed.IsEnabled = true;
            }


        }
        private void SetEnableButtonsStart()
        {
            SetEnableSliders();
            SetEnableButtonsPattern();
            Start.IsEnabled = false;
            Pause.IsEnabled = true;
            Reset.IsEnabled = true;
        }

        private void SetEnableButtonsReset()
        {
            SetEnableSliders();
            SetEnableButtonsPattern();
            Pause.IsEnabled = false;
            Reset.IsEnabled = false;
            Start.IsEnabled = true;
        }

        private void SetEnableButtonsPattern()
        {
            if(pattern1.IsEnabled)
            {
                pattern1.IsEnabled = false;
                pattern2.IsEnabled = false;
                pattern3.IsEnabled = false;
            }
            else
            {
                pattern1.IsEnabled = true;
                pattern2.IsEnabled = true;
                pattern3.IsEnabled = true;
            }
        }



        public void resetCells()
        {
            var color = white;
            foreach (Cell cell in cells)
            {
                cell.rectangle.Fill = color;
                cell.State = State.DEAD;
                
            }

        }

/*private void SliderSpeedValueChanged(object sender, RoutedEventArgs e)
        {
            lblSpeed.Content = "Vitesse de cycle : " + (int)sliderSpeed.Value + "ms";
        }*/

        private void StartClick(object sender, RoutedEventArgs e)
        {
            PopMin = 10000;
            startGame();
            SetEnableButtonsStart();
            
        }



        private void setPattern(object sender, RoutedEventArgs e)
        {
             // Top left position of the template
            Button btn = (Button)sender;
            if(btn.Name == "pattern1")
            {
                positionList.Clear();
                int x = 0, y = 0;
                positionList.Add(new Tuple<int, int>(x, y + 2));
                positionList.Add(new Tuple<int, int>(x + 1, y + 2));
                positionList.Add(new Tuple<int, int>(x + 2, y + 2));
                positionList.Add(new Tuple<int, int>(x + 1, y));
                positionList.Add(new Tuple<int, int>(x + 2, y + 1));
            }
            else if(btn.Name == "pattern2")
            {
                positionList.Clear();
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
                positionList.Clear();
                int x = nbOfColumnCell / 2 , y = nbOfRowCell / 2;
                int xSign = 1;
                int ySign = 1;
                for(int i=0;i<4;i++)
                {
                    positionList.Add(new Tuple<int, int>(x + xSign* 2, y - ySign*1));
                    positionList.Add(new Tuple<int, int>(x + xSign * 3, y - ySign * 1));
                    positionList.Add(new Tuple<int, int>(x + xSign * 4, y - ySign * 1));
                    positionList.Add(new Tuple<int, int>(x + xSign * 1, y - ySign * 2));
                    positionList.Add(new Tuple<int, int>(x + xSign * 1, y - ySign * 3));
                    positionList.Add(new Tuple<int, int>(x + xSign * 1, y - ySign * 4));
                    positionList.Add(new Tuple<int, int>(x + xSign * 2, y - ySign * 6));
                    positionList.Add(new Tuple<int, int>(x + xSign * 3, y - ySign * 6));
                    positionList.Add(new Tuple<int, int>(x + xSign * 4, y - ySign * 6));
                    positionList.Add(new Tuple<int, int>(x + xSign * 6, y - ySign * 2));
                    positionList.Add(new Tuple<int, int>(x + xSign * 6, y - ySign * 3));
                    positionList.Add(new Tuple<int, int>(x + xSign * 6, y - ySign * 4));
                    if (i == 0)
                        xSign = -1;
                    else if (i == 1)
                        ySign = -1;
                    else if (i == 2)
                        xSign = 1;
                }

            }

            resetCells();
            foreach (Tuple<int, int> cell in positionList)
            {
                if (cell.Item1 > nbOfColumnCell || cell.Item2 > nbOfRowCell)
                {

                }
                else
                {
                    cells[cell.Item1, cell.Item2].rectangle.Fill = green;
                    cells[cell.Item1, cell.Item2].State = State.ALIVE;
                }

            }

        }

        private DispatcherTimer dispatcherTimer;
        private void startGame()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(evaluate);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Speed);
            dispatcherTimer.Start();
        }

        private void evaluate(object sender, EventArgs e)
        {
            Alive = 0;
            Oldest = 0;

            foreach (Cell cell in cells)
            {
                cell.prepare(cells);
            }

            foreach (Cell cell in cells)
            {
                cell.apply();
                if (cell.State == State.ALIVE)
                    Alive++;
                if (cell.Age > Oldest)
                    Oldest = cell.Age;
            }
            if (Alive > PopMax)
                PopMax = Alive;
            if (Alive < PopMin)
                PopMin = Alive;
            //  nbIterations++;
            Iterations++;
            updateStats();
        }

        private void updateStats()
        {
           // lblIterations.Content = "N° itérations : " + nbIterations;
            //lblAlive.Content = "Taille de population : " + nbAliveCells;
           // lblMin.Content = "Population min. :" + nbOfCellMin;
            //lblMax.Content = "Population max. :" + nbOfCellMax;
            //lblOldest.Content = "Oldest cell's age : " + oldestCellAge;
            //pyramide des ages des cellules,
            

        }


        private void Grid_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            //string selectedRect;
            if (e.OriginalSource is Rectangle)
            {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                SolidColorBrush color = (SolidColorBrush)ClickedRectangle.Fill;
                if (color.Color.Equals(Colors.Green))
                {
                    ClickedRectangle.Fill = white;
                    int x = Grid.GetColumn(ClickedRectangle);
                    int y = Grid.GetRow(ClickedRectangle);
                    removeItem(x, y);

                }
            }

            isRight = true;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isLeft = false;
        }


        private void Grid_MouseRightUp(object sender, MouseButtonEventArgs e)
        {
            isRight = false;
        }


        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.OriginalSource is Rectangle)
            {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                if (isLeft)
                {
                    //var mouseWasDownOn = e.Source as FrameworkElement;
                    //string elementName = mouseWasDownOn.Name;
                    //selectedRect = elementName.ToString();
                    ClickedRectangle.Fill = green;
                    // MessageBox.Show(selectedRect.ToString());
                    int x = Grid.GetColumn(ClickedRectangle);
                    int y = Grid.GetRow(ClickedRectangle);
                    cells[x, y].State = State.ALIVE;
                    Tuple<int, int> tuple = new Tuple<int, int>(x, y);
                    //nbOfCellMin++;
                    //nbAliveCells++;
                    updateStats();
                    positionList.Add(tuple);
                }
                if (isRight)
                {
                    SolidColorBrush color = (SolidColorBrush)ClickedRectangle.Fill;
                    if (color.Color.Equals(Colors.Green))
                    {
                        ClickedRectangle.Fill = white;
                        int x = Grid.GetColumn(ClickedRectangle);
                        int y = Grid.GetRow(ClickedRectangle);

                        removeItem(x, y);
     
                    }
                }
            }

        }

        private void removeItem(int x,int y)
        {
            cells[x, y].State = State.DEAD;

            foreach (Tuple<int, int> cell in positionList.ToList())
            {
                if (cell.Item1 >= x || cell.Item2 >= y)
                {
                    positionList.Remove(cell);
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


        private int _speed;
        public int Speed
        {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _oldest;
        public int Oldest
        {
            get { return _oldest; }
            set
            {
                if (_oldest != value)
                {
                    _oldest = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _iterations;
        public int Iterations
        {
            get { return _iterations; }
            set
            {
                if (_iterations != value)
                {
                    _iterations = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _alive;
        public int Alive
        {
            get { return _alive; }
            set
            {
                if (_alive != value)
                {
                    _alive = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _popMin;
        public int PopMin
        {
            get { return _popMin; }
            set
            {
                if (_popMin != value)
                {
                    _popMin = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _popMax;
        public int PopMax
        {
            get { return _popMax; }
            set
            {
                if (_popMax != value)
                {
                    _popMax = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
