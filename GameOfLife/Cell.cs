using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
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
        private SolidColorBrush turquoise = new SolidColorBrush(Colors.Turquoise);

        private State state;
        private State nextState;

        private int age = 0;

        public Rectangle rectangle;

        public Cell(int x,int y,Rectangle rectangle, State state=State.DEAD)
        {
            X = x;
            Y = y;
            this.rectangle = rectangle;
            State = state;
        }

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

        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public void prepare(Cell[,] grid)
        {
            if (getNbOfAliveNeighbour(grid) == 3)
                nextState = State.ALIVE;
            else if (getNbOfAliveNeighbour(grid) == 2)
                nextState = State;
            else
                nextState = State.DEAD;
        }

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
                rectangle.Fill = turquoise;
            }

        }

    }
}
