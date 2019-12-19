using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    /// <summary>
    /// Cell class that matches with every rectangle
    /// </summary>
    class Cell
    {
        private int x;
        private int y;

        public int Y { get => y; set => y = value; }
        public int X { get => x; set => x = value; }
        public State State { get => state; set => state = value; }
        public State NextState { get => nextState; set => nextState = value; }
        public int Age { get => age; set => age = value; }

        private SolidColorBrush green = new SolidColorBrush(Colors.Green);
        private SolidColorBrush white = new SolidColorBrush(Colors.White);

        private State state;
        private State nextState;

        private int age = 0;

        public Rectangle rectangle;

        /// <summary>
        /// Initialiser of a cell
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="rectangle">rectangle used</param>
        /// <param name="state">beginning state</param>
        public Cell(int x,int y,Rectangle rectangle, State state=State.DEAD)
        {
            X = x;
            Y = y;
            this.rectangle = rectangle;
            State = state;
        }
        /// <summary>
        /// Get number of neighbours alive
        /// </summary>
        /// <param name="grid"> Prend grille en paramètre</param>
        /// <returns></returns>
        public int getNbOfAliveNeighbour(Cell[,] grid)
        {
            int startX = X-1;
            int endX = X+1;
            int startY = Y-1;
            int endY = Y+1;
            int nbOfAliveNeighbour = 0;
            int ri, rj;

            for (int i = startX;i<=endX;i++)
            {
                for(int j = startY;j<=endY;j++)
                {
                    ri = mod(i, grid.GetLength(0));
                    rj = mod(j, grid.GetLength(1));
                    if (!(ri == X && rj == Y))
                        if (grid[ri, rj].State == State.ALIVE)
                            nbOfAliveNeighbour++;
                }
            }
            return nbOfAliveNeighbour;
        }
        /// <summary>
        /// Calculate modulos so we can loop between simulation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        /// <summary>
        /// Change cell's state
        /// </summary>
        /// <param name="grid"></param>
        public void prepare(Cell[,] grid)
        {
            if (getNbOfAliveNeighbour(grid) == 3)
                nextState = State.ALIVE;
            else if (getNbOfAliveNeighbour(grid) == 2)
                nextState = State;
            else
                nextState = State.DEAD;
        }

        /// <summary>
        /// Modify cell's color and age
        /// </summary>
        public void apply()
        {
            state = nextState;
            if (state == State.ALIVE)
            {
                Age++;
                rectangle.Fill = green;
            }
            else
            {
                Age = 0;
                rectangle.Fill = white;
            }

        }

    }
}
