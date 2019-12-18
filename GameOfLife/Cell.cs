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
        private SolidColorBrush white = new SolidColorBrush(Colors.White);

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
            if (X - 1 < 0)
                startX = X;
            if (X + 1 > grid.GetLength(0)-1)
                endX = X;
            if (Y - 1 < 0)
                startY = Y;
            if (Y + 1 > grid.GetLength(1)-1)
                endY = Y;

            for(int i = startX;i<=endX;i++)
            {
                for(int j = startY;j<=endY;j++)
                {
                    if (!(i == X && j == Y))
                        if (grid[i, j].State == State.ALIVE)
                            nbOfAliveNeighbour++;
                        
                }
            }
            return nbOfAliveNeighbour;
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
                rectangle.Fill = white;
            }

        }

    }
}
