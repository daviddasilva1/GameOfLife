using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Cell
    {
        private int x;
        private int y;

        public int Y { get => y; set => y = value; }
        public int X { get => x; set => x = value; }
        public State State { get => state; set => state = value; }

        private State state;

        public Cell(int x,int y, State state=State.DEAD)
        {
            X = x;
            Y = y;
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
            if (X + 1 > grid.GetLength(0))
                endX = X;
            if (Y - 1 < 0)
                startY = Y;
            if (Y + 1 > grid.GetLength(1))
                endY = Y;

            for(int i = startX;i<=endX;i++)
            {
                for(int j = startY;j<endY;j++)
                {
                    if (i != X && j != Y)
                        if (grid[i, j].State == State.ALIVE)
                            nbOfAliveNeighbour++;
                        
                }
            }
            return nbOfAliveNeighbour;
        }

    }
}
