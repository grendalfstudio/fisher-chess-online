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
            RandomPiecesRow row = GenerateRandomPiecesPlacement();

            #region Black pieces initialisation

            StartBoard[0, row.Rook1] =   new Rook  (PlayerColor.Black, new Cell(0, row.Rook1), this);
            StartBoard[0, row.Knight1] = new Knight(PlayerColor.Black, new Cell(0, row.Knight1), this);
            StartBoard[0, row.Bishop1] = new Bishop(PlayerColor.Black, new Cell(0, row.Bishop1), this);
            StartBoard[0, row.Queen] =   new Queen (PlayerColor.Black, new Cell(0, row.Queen), this);
            StartBoard[0, row.King] =    new King  (PlayerColor.Black, new Cell(0, row.King), this);
            StartBoard[0, row.Bishop2] = new Bishop(PlayerColor.Black, new Cell(0, row.Bishop2), this);
            StartBoard[0, row.Knight2] = new Knight(PlayerColor.Black, new Cell(0, row.Knight2), this);
            StartBoard[0, row.Rook2] =   new Rook  (PlayerColor.Black, new Cell(0, row.Rook2), this);

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

            StartBoard[7, row.Rook1] =   new Rook  (PlayerColor.White, new Cell(7, row.Rook1), this);
            StartBoard[7, row.Knight1] = new Knight(PlayerColor.White, new Cell(7, row.Knight1), this);
            StartBoard[7, row.Bishop1] = new Bishop(PlayerColor.White, new Cell(7, row.Bishop1), this);
            StartBoard[7, row.Queen] =   new Queen (PlayerColor.White, new Cell(7, row.Queen), this);
            StartBoard[7, row.King] =    new King  (PlayerColor.White, new Cell(7, row.King), this);
            StartBoard[7, row.Bishop2] = new Bishop(PlayerColor.White, new Cell(7, row.Bishop2), this);
            StartBoard[7, row.Knight2] = new Knight(PlayerColor.White, new Cell(7, row.Knight2), this);
            StartBoard[7, row.Rook2] =   new Rook  (PlayerColor.White, new Cell(7, row.Rook2), this);

            #endregion

            _whiteKing = StartBoard[7, row.King]!;
            _blackKing = StartBoard[0, row.King]!;

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

        private static RandomPiecesRow GenerateRandomPiecesPlacement()
        {
            var random = new Random();
            var emptyIndeces = new List<int>();
            var row = new RandomPiecesRow();
            for (int i = 0; i < Length; i++)
            {
                emptyIndeces.Add(i);
            }

            // King possible placements: [.kkkkkk.]
            row.King = random.Next(1, Length - 1);
            emptyIndeces.Remove(row.King);

            // Left rook must be before king [rrrrrk..]
            row.Rook1 = random.Next(0, row.King);
            emptyIndeces.Remove(row.Rook1);

            // Right rook must be after king [.....krr]
            row.Rook2 = random.Next(row.King + 1, Length);
            emptyIndeces.Remove(row.Rook2);

            // Odd bishop must occupy only odd empty cell [b.b.b.b.]
            do
            {
                row.Bishop1 = emptyIndeces[random.Next(emptyIndeces.Count)];
            } while (row.Bishop1 % 2 == 1);
            emptyIndeces.Remove(row.Bishop1);

            // Even bishop must occupy only even empty cell [.b.b.b.b]
            do
            {
                row.Bishop2 = emptyIndeces[random.Next(emptyIndeces.Count)];
            } while (row.Bishop2 % 2 == 0);
            emptyIndeces.Remove(row.Bishop2);

            // First knight must occupy an empty cell
            row.Knight1 = emptyIndeces[random.Next(emptyIndeces.Count)];
            emptyIndeces.Remove(row.Knight1);

            // Second knight must occupy an empty cell
            row.Knight2 = emptyIndeces[random.Next(emptyIndeces.Count)];
            emptyIndeces.Remove(row.Knight2);

            // Queen is placed on the last empty cell
            row.Queen = emptyIndeces.Last();

            return row;
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

        private class RandomPiecesRow
        {
            public int Rook1 { get; set; }
            public int Knight1 { get; set; }
            public int Bishop1 { get; set; }
            public int Queen { get; set; }
            public int King { get; set; }
            public int Bishop2 { get; set; }
            public int Knight2 { get; set; }
            public int Rook2 { get; set; }
        }
    }
}