using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Snake
{
    public class Position
    {
        public int Row;
        public int Col;

        public Position(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public Position Translate(Direction direction)
        {
            return new Position(direction.RowOffset + this.Row, direction.ColOffset + this.Col);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
