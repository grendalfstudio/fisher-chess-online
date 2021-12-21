using FisherChessServer.Core.Models.Pieces;

namespace FisherChessServer.Core.Models
{
    public class Chessboard
    {
        public static readonly int Length = 8;

        private readonly Piece _whiteKing;
        private readonly Piece _blackKing;

        public Chessboard()
        {
            #region Black pieces initialisation

            StartBoard[0, 0] = new Rook  (PlayerColor.Black, new Cell(0, 0), this);
            StartBoard[0, 1] = new Knight(PlayerColor.Black, new Cell(0, 1), this);
            StartBoard[0, 2] = new Bishop(PlayerColor.Black, new Cell(0, 2), this);
            StartBoard[0, 3] = new Queen (PlayerColor.Black, new Cell(0, 3), this);
            StartBoard[0, 4] = new King  (PlayerColor.Black, new Cell(0, 4), this);
            StartBoard[0, 5] = new Bishop(PlayerColor.Black, new Cell(0, 5), this);
            StartBoard[0, 6] = new Knight(PlayerColor.Black, new Cell(0, 6), this);
            StartBoard[0, 7] = new Rook  (PlayerColor.Black, new Cell(0, 7), this);

            StartBoard[1, 0] = new Pawn(PlayerColor.Black, new Cell(1, 0), this);
            StartBoard[1, 1] = new Pawn(PlayerColor.Black, new Cell(1, 1), this);
            StartBoard[1, 2] = new Pawn(PlayerColor.Black, new Cell(1, 2), this);
            StartBoard[1, 3] = new Pawn(PlayerColor.Black, new Cell(1, 3), this);
            StartBoard[1, 4] = new Pawn(PlayerColor.Black, new Cell(1, 4), this);
            StartBoard[1, 5] = new Pawn(PlayerColor.Black, new Cell(1, 5), this);
            StartBoard[1, 6] = new Pawn(PlayerColor.Black, new Cell(1, 6), this);
            StartBoard[1, 7] = new Pawn(PlayerColor.Black, new Cell(1, 7), this);

            #endregion

            #region White pieces initialisation

            StartBoard[6, 0] = new Pawn(PlayerColor.White, new Cell(6, 0), this);
            StartBoard[6, 1] = new Pawn(PlayerColor.White, new Cell(6, 1), this);
            StartBoard[6, 2] = new Pawn(PlayerColor.White, new Cell(6, 2), this);
            StartBoard[6, 3] = new Pawn(PlayerColor.White, new Cell(6, 3), this);
            StartBoard[6, 4] = new Pawn(PlayerColor.White, new Cell(6, 4), this);
            StartBoard[6, 5] = new Pawn(PlayerColor.White, new Cell(6, 5), this);
            StartBoard[6, 6] = new Pawn(PlayerColor.White, new Cell(6, 6), this);
            StartBoard[6, 7] = new Pawn(PlayerColor.White, new Cell(6, 7), this);

            StartBoard[7, 0] = new Rook  (PlayerColor.White, new Cell(7, 0), this);
            StartBoard[7, 1] = new Knight(PlayerColor.White, new Cell(7, 1), this);
            StartBoard[7, 2] = new Bishop(PlayerColor.White, new Cell(7, 2), this);
            StartBoard[7, 3] = new Queen (PlayerColor.White, new Cell(7, 3), this);
            StartBoard[7, 4] = new King  (PlayerColor.White, new Cell(7, 4), this);
            StartBoard[7, 5] = new Bishop(PlayerColor.White, new Cell(7, 5), this);
            StartBoard[7, 6] = new Knight(PlayerColor.White, new Cell(7, 6), this);
            StartBoard[7, 7] = new Rook  (PlayerColor.White, new Cell(7, 7), this);

            #endregion

            _whiteKing = StartBoard[7, 4]!;
            _blackKing = StartBoard[0, 4]!;

            Reset();
        }

        public Piece?[,] StartBoard { get; } = new Piece?[Length, Length];
        public Piece?[,] Board { get; } = new Piece?[Length, Length];

        public Piece? this[Cell cell]
        {
            get => cell.IsValid()
                ? Board[cell.Row, cell.Column]
                : null;
            set
            {
                Board[cell.Row, cell.Column] = value;
                if (value != null)
                    value.Cell = cell;
            }
        }

        public Piece GetKing(PlayerColor color)
        {
            return color switch
            {
                PlayerColor.White => _whiteKing,
                PlayerColor.Black => _blackKing,
                _ => throw new NotImplementedException()
            };
        }

        public void MakeMove(Piece piece, Cell cell)
        {
            // If there was piece on the cell, it disappears
            if (this[cell] != null)
                this[cell]!.Cell = null;

            // Piece cell became empty and piece is moved to a new cell
            this[piece.Cell!] = null;
            this[cell] = piece;
        }

        public void Reset()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    var cell = new Cell(i, j);
                    this[cell] = StartBoard[i, j];
                }
            }
        }
    }
}