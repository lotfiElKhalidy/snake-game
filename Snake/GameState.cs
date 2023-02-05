using System;
using System.Collections.Generic;

namespace Snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Direction { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Direction> directionChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols)
        {
            this.Rows = rows;
            this.Cols = cols;
            this.Grid = new GridValue[rows, cols];
            this.Direction = Direction.Right;
            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = this.Rows / 2;

            for (int c = 1; c < 4; c++) 
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
     

            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    if (Grid[r,c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
                
            }
        }

        private void AddFood() 
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position position = empty[random.Next(empty.Count)];
            Grid[position.Row, position.Col] = GridValue.Food;
        }

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

       public Position TailPosition()
       {
            return snakePositions.Last.Value;
       }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(Position position)
        {
            snakePositions.AddFirst(position);
            Grid[position.Row, position.Col] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        public Direction GetLastDirection()
        {
            if (directionChanges.Count == 0)
            {
                return Direction;
            }

            return directionChanges.Last.Value;
        }

        public bool CanChangetDirection(Direction newDirection)
        {
            if (directionChanges.Count == 2)
            {
                return false;
            }

            Direction lastDirection = GetLastDirection();
            return newDirection != lastDirection && newDirection != lastDirection.Opposite();
        }

        public void ChangeDirection(Direction direction)
        {
            if (CanChangetDirection(direction))
                directionChanges.AddLast(direction);
        }

        private bool OutsideGrid(Position position)
        {
            return position.Row < 0 || position.Row >= Rows || position.Col < 0 || position.Col >= Cols;
        }

        private GridValue WillHit(Position newHeadPosition)
        {
            if (OutsideGrid(newHeadPosition))
            {
                return GridValue.Outside;
            }

            if (newHeadPosition == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPosition.Row, newHeadPosition.Col];
        }

        public void Move()
        {
            if (directionChanges.Count > 0)
            {
                Direction = directionChanges.First.Value;
                directionChanges.RemoveFirst();
            }

            Position newHeadPos = HeadPosition().Translate(Direction);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
