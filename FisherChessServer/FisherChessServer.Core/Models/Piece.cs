using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherChessServer.Core.Models
{
    public abstract class Piece
    {
        private Cell? _cell;

        protected Piece(PieceType type, PlayerColor color, Cell cell, Chessboard chessboard)
        {
            Type = type;
            Color = color;
            Cell = cell;
            Chessboard = chessboard;
        }

        public event EventHandler<Cell?>? OnCellChange;

        protected enum MoveDirection
        {
            Up, Right, Down, Left,
            UpLeft, UpRight, DownRight, DownLeft,
            UpUpLeft, UpUpRight, RightRightUp, RightRightDown,
            DownDownRight, DownDownLeft, LeftLeftDown, LeftLeftUp
        }

        public PieceType Type { get; }
        public PlayerColor Color { get; }
        public Cell? Cell
        {
            get => _cell;
            set
            {
                _cell = value;
                if (HasToRaiseOnCellChangeEvent)
                    OnCellChange?.Invoke(this, Cell);
            }
        }
        public bool HasToRaiseOnCellChangeEvent { get; set; } = true;

        protected Chessboard Chessboard { get; }

        public abstract IEnumerable<Cell> GetAvailableCells();

        protected virtual IEnumerable<Cell> FindAvailableCellsInDirection(MoveDirection direction)
        {
            if (Cell == null)
                return new List<Cell>();

            var availableCells = new List<Cell>();
            var currentCell = GetNextCell(direction, Cell);

            while (IsCellAvailable(currentCell))
            {
                availableCells.Add(currentCell);

                // If cell is empty
                if (Chessboard[currentCell] == null)
                    currentCell = GetNextCell(direction, currentCell);
                else
                    break;
            }

            return availableCells;
        }

        protected bool IsCellAvailable(Cell cell)
        {
            if (!cell.IsValid())
                return false;

            // Returns true if cell is empty or contains opponent's piece
            return Chessboard[cell] == null || Chessboard[cell]?.Color != Color;
        }

        protected static Cell GetNextCell(MoveDirection direction, Cell currentCell)
        {
            return direction switch
            {
                MoveDirection.Up => new Cell(currentCell.Row - 1, currentCell.Column),
                MoveDirection.Right => new Cell(currentCell.Row, currentCell.Column + 1),
                MoveDirection.Down => new Cell(currentCell.Row + 1, currentCell.Column),
                MoveDirection.Left => new Cell(currentCell.Row, currentCell.Column - 1),
                MoveDirection.UpLeft => new Cell(currentCell.Row - 1, currentCell.Column - 1),
                MoveDirection.UpRight => new Cell(currentCell.Row - 1, currentCell.Column + 1),
                MoveDirection.DownRight => new Cell(currentCell.Row + 1, currentCell.Column + 1),
                MoveDirection.DownLeft => new Cell(currentCell.Row + 1, currentCell.Column - 1),
                MoveDirection.UpUpLeft => new Cell(currentCell.Row - 2, currentCell.Column - 1),
                MoveDirection.UpUpRight => new Cell(currentCell.Row - 2, currentCell.Column + 1),
                MoveDirection.RightRightUp => new Cell(currentCell.Row - 1, currentCell.Column + 2),
                MoveDirection.RightRightDown => new Cell(currentCell.Row + 1, currentCell.Column + 2),
                MoveDirection.DownDownRight => new Cell(currentCell.Row + 2, currentCell.Column + 1),
                MoveDirection.DownDownLeft => new Cell(currentCell.Row + 2, currentCell.Column - 1),
                MoveDirection.LeftLeftDown => new Cell(currentCell.Row + 1, currentCell.Column - 2),
                MoveDirection.LeftLeftUp => new Cell(currentCell.Row - 1, currentCell.Column - 2),
                _ => throw new NotImplementedException()
            };
        }
    }
}
