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

        //attributes declarations
        private bool isLeft = false;
        private bool isRight = false;
        private Cell[,] cells;
        private int nbOfRowCell = 17;
        private int nbOfColumnCell;
        //list of tuple containing coordinates of our Cells
        List<Tuple<int, int>> positionList = new List<Tuple<int, int>>();

        //all colors used on our grid
        private SolidColorBrush green = new SolidColorBrush(Colors.Green);
        private SolidColorBrush white = new SolidColorBrush(Colors.White);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);

        /*
 * all the properties so we do databinding
*/
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
        /// <summary>
        /// Works with binding,called when values are changed.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
            Speed = 100;
            Iterations = 0;
            Oldest = 0;
            DataContext = this; //so we can do databinding


        }

        /// <summary>
        /// Draw grid using parameters
        /// </summary>
        private void DrawGrid()
        {
            double cellSize = 700 / nbOfRowCell;

            //draw all rows
            for (int i = 0; i < nbOfRowCell; i++)
            {
                RowDefinition gridRow = new RowDefinition();

                gridRow.Height = new GridLength(cellSize);

                myGrid.RowDefinitions.Add(gridRow);

            }
            nbOfColumnCell = Convert.ToInt32(SystemParameters.WorkArea.Width / cellSize);
            //draw all columns
            for (int i = 0; i < nbOfColumnCell; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(cellSize);

                myGrid.ColumnDefinitions.Add(gridCol);
            }

            //create 2d array
            cells = new Cell[nbOfColumnCell, nbOfRowCell];

            //rectangle's creation
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

            //loop on the List so we check when grid is resized if we have to delete some cells or not
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




        /// <summary>
        /// When user clicks on Reset Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetClick(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Iterations = 0;
            PopMin=0;
            PopMax = 0;
            Alive = 0;
            Oldest = 0;
            resetCells();
            SetEnableButtonsReset();
            positionList.Clear();
        }


        /// <summary>
        /// When user clicks Pause Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseClick(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Pause.IsEnabled = false;
            Start.IsEnabled = true;
            sliderSpeed.IsEnabled = true;
            sliderResize.IsEnabled = false;
        }


        /// <summary>
        /// Switch state of sliders
        /// </summary>
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

        /// <summary>
        /// Change state of buttons when clicking on Start
        /// </summary>
        private void SetEnableButtonsStart()
        {
            SetEnableSliders();
            SetEnableButtonsPattern();
            Start.IsEnabled = false;
            Pause.IsEnabled = true;
            Reset.IsEnabled = true;
        }

        /// <summary>
        /// Change state of buttons when clicking on Reset
        /// </summary>
        private void SetEnableButtonsReset()
        {
            SetEnableSliders();
            SetEnableButtonsPattern();
            Pause.IsEnabled = false;
            Reset.IsEnabled = false;
            Start.IsEnabled = true;
        }

        /// <summary>
        /// Switch state of pattern buttons
        /// </summary>
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

        /// <summary>
        /// Reset all cells
        /// </summary>
        public void resetCells()
        {
            var color = white;
            foreach (Cell cell in cells)
            {
                cell.rectangle.Fill = color;
                cell.State = State.DEAD;           
            }
        }


        /// <summary>
        /// When user clicks on Start Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, RoutedEventArgs e)
        {
            PopMin = 10000;
            if(positionList.Count==0)
            {
                setRandomCells();
            }
            startGame();
            SetEnableButtonsStart();           
        }

        /// <summary>
        /// Set random cells if user did not use pattern or creates cells
        /// </summary>
        private void setRandomCells()
        {
            Random random = new Random();
            for(int i=0;i<50;i++)
            {
                int col = random.Next(1, 20);
                int row = random.Next(1, 17);
                positionList.Add(new Tuple<int, int>(col, row));
            }

            foreach (Tuple<int, int> cell in positionList)
            {
              
                    cells[cell.Item1, cell.Item2].rectangle.Fill = green;
                    cells[cell.Item1, cell.Item2].State = State.ALIVE;
                
            }
        }

        /// <summary>
        /// When user clicks in one of the pattern buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Called at beginning of the game
        /// </summary>
        private void startGame()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(evaluate);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Speed);
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Evalute cell property and state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            Iterations++;
        }



        /// <summary>
        /// When user clicks with left mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftDown(object sender, MouseButtonEventArgs e)
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
            }
            isLeft = true;

        }

        /// <summary>
        /// When user clicks with the right mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// When user releases Left Mouse 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isLeft = false;
        }

        /// <summary>
        /// When user releases Right Mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseRightUp(object sender, MouseButtonEventArgs e)
        {
            isRight = false;
        }

        /// <summary>
        /// When user clicks and moves with mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.OriginalSource is Rectangle)
            {
                Rectangle ClickedRectangle = (Rectangle)e.OriginalSource;
                if (isLeft)
                {
                    ClickedRectangle.Fill = green;
                    int x = Grid.GetColumn(ClickedRectangle);
                    int y = Grid.GetRow(ClickedRectangle);
                    cells[x, y].State = State.ALIVE;
                    Tuple<int, int> tuple = new Tuple<int, int>(x, y);
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


        /// <summary>
        /// Remove cell if grid is resized
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
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


        /// <summary>
        /// When user stops dragging on slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
