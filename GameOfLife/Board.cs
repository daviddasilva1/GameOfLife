using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public int NbOfRowCell { get => nbOfRowCell; set => nbOfRowCell = value; }



        public Cell[,] Cells { get => cells; set => cells = value; }


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
                int col = random.Next(1, nbOfColumnCell);
                int row = random.Next(1, NbOfRowCell);
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
            foreach (Cell cell in Cells)
            {
                cell.rectangle.Fill = color;
                cell.State = State.DEAD;
            }
        }

        /// <summary>
        /// Remove cell if grid is resized
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void removeItem(int x, int y)
        {
            Cells[x, y].State = State.DEAD;

            foreach (Tuple<int, int> cell in positionList.ToList())
            {
                if (cell.Item1 >= x || cell.Item2 >= y)
                {
                    positionList.Remove(cell);
                }
            }
        }

        /// <summary>
        /// When user clicks in one of the pattern buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void setPattern(object sender)
        {
            // Top left position of the template
            Button btn = (Button)sender;
            if (btn.Name == "pattern1")
            {
                positionList.Clear();
                int x = 0, y = 0;
                positionList.Add(new Tuple<int, int>(x, y + 2));
                positionList.Add(new Tuple<int, int>(x + 1, y + 2));
                positionList.Add(new Tuple<int, int>(x + 2, y + 2));
                positionList.Add(new Tuple<int, int>(x + 1, y));
                positionList.Add(new Tuple<int, int>(x + 2, y + 1));
            }
            else if (btn.Name == "pattern2")
            {
                positionList.Clear();
                int x = NbOfColumnCell / 2, y = NbOfRowCell / 2;
                for(int i=0;i<NbOfColumnCell;i++)
                {
                    positionList.Add(new Tuple<int, int>(i, y-1));
                    positionList.Add(new Tuple<int, int>(i, y));
                    positionList.Add(new Tuple<int, int>(i, y+1));

                }


            }
            else
            {
                positionList.Clear();
                int x = NbOfColumnCell / 2, y = NbOfRowCell / 2;
                int xSign = 1;
                int ySign = 1;
                for (int i = 0; i < 4; i++)
                {
                    positionList.Add(new Tuple<int, int>(x + xSign * 2, y - ySign * 1));
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
                if (cell.Item1 > NbOfColumnCell || cell.Item2 > NbOfRowCell)
                {

                }
                else
                {
                    Cells[cell.Item1, cell.Item2].rectangle.Fill = green;
                    Cells[cell.Item1, cell.Item2].State = State.ALIVE;
                }
            }
        }

    }

}
