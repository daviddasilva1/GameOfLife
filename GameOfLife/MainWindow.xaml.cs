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

        private Board board;

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
            // DrawGrid();
            board = new Board(nbOfRowCell, nbOfColumnCell, myGrid);
            board.InitBoard();
            Speed = 100;
            Iterations = 0;
            Oldest = 0;
            DataContext = this; //so we can do databinding


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
            board.resetCells();
            SetEnableButtonsReset();
            board.PositionList.Clear();
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
        /// When user clicks on Start Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, RoutedEventArgs e)
        {
            PopMin = 10000;
            if (board.PositionList.Count == 0)
            
                {
                    board.setRandomCells();
                    PopMin = 50;
                    PopMax = 50;
                }
                startGame();
                SetEnableButtonsStart();
            
        }
           private void setPattern(object sender, RoutedEventArgs e)
        {
            board.setPattern(sender);
        }
 
        private DispatcherTimer dispatcherTimer;

        /// <summary>
        /// Called at beginning of the game
        /// </summary>
        private void startGame()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Evaluate);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Speed);
            dispatcherTimer.Start();
        }

        /// <summary>
        ///Call bord's method to evaluate each cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Evaluate(object sender, EventArgs e)
        {
            Alive = 0;
            Oldest = 0;

            foreach (Cell cell in board.Cells)
            {
                cell.prepare(board.Cells);
            }

            foreach (Cell cell in board.Cells)
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

                board.Cells[x, y].State = State.ALIVE;
                Tuple<int, int> tuple = new Tuple<int, int>(x, y);
                board.PositionList.Add(tuple);
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
                    board.removeItem(x, y);
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
                    board.Cells[x, y].State = State.ALIVE;
                    Tuple<int, int> tuple = new Tuple<int, int>(x, y);
                    board.PositionList.Add(tuple);
                }
                if (isRight)
                {
                    SolidColorBrush color = (SolidColorBrush)ClickedRectangle.Fill;
                    if (color.Color.Equals(Colors.Green))
                    {
                        ClickedRectangle.Fill = white;
                        int x = Grid.GetColumn(ClickedRectangle);
                        int y = Grid.GetRow(ClickedRectangle);
                        board.removeItem(x, y);   
                    }
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
            board.NbOfRowCell = (int)value;
            myGrid.ColumnDefinitions.Clear();
            myGrid.RowDefinitions.Clear();
            board.InitBoard();

        }

    }
}
