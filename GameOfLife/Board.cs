using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Board
    {
        private Cell[,] cells;
        private int nbOfRowCell = 17;
        private int nbOfColumnCell;
        //list of tuple containing coordinates of our Cells
        List<Tuple<int, int>> positionList = new List<Tuple<int, int>>();

        public List<Tuple<int, int>> PositionList { get => positionList; }

        public int NbOfColumnCell { get => nbOfColumnCell; }

        public int NbOfRowCell { get => nbOfRowCell; }



        public Cell[,] Cell { get => cells; set => cells = value; }


        //all colors used on our grid
        private SolidColorBrush green = new SolidColorBrush(Colors.Green);
        private SolidColorBrush white = new SolidColorBrush(Colors.White);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);

        private Grid grid;

        public Board(int nbRow,int nbCol,Grid grid)
        {
            this.nbOfRowCell = nbRow;
            this.nbOfColumnCell = nbCol;
            this.grid = grid;

            InitBoard();
        }

        public void InitBoard()
        {
            double cellSize = 700 / nbOfRowCell;

            //draw all rows
            for (int i = 0; i < nbOfRowCell; i++)
            {
                RowDefinition gridRow = new RowDefinition();

                gridRow.Height = new GridLength(cellSize);

                grid.RowDefinitions.Add(gridRow);

            }
            nbOfColumnCell = Convert.ToInt32(SystemParameters.WorkArea.Width / cellSize);
            //draw all columns
            for (int i = 0; i < nbOfColumnCell; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(cellSize);

                grid.ColumnDefinitions.Add(gridCol);
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

                    grid.Children.Add(rectangle);
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
        /// Set random cells if user did not use pattern or creates cells
        /// </summary>
        public void setRandomCells()
        {
            Random random = new Random();
            for (int i = 0; i < 50; i++)
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

    }
}
