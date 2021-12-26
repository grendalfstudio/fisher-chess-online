using FisherChessServer.Core.Models.Pieces;

namespace FisherChessServer.Core.Models
{
    public class Chessboard
    {
        public static readonly int Length = 8;

        private readonly PlayerPieces _whitePlayerPieces;
        private readonly PlayerPieces _blackPlayerPieces;

        public Chessboard()
        {
            _whitePlayerPieces = new PlayerPieces(PlayerColor.White, this);
            _blackPlayerPieces = new PlayerPieces(PlayerColor.Black, this);

            #region Black pieces placement

            this[new Cell(0, 0)] = _blackPlayerPieces.Rook1;
            this[new Cell(0, 1)] = _blackPlayerPieces.Knight1;
            this[new Cell(0, 2)] = _blackPlayerPieces.Bishop1;
            this[new Cell(0, 3)] = _blackPlayerPieces.Queen;
            this[new Cell(0, 4)] = _blackPlayerPieces.King;
            this[new Cell(0, 5)] = _blackPlayerPieces.Bishop2;
            this[new Cell(0, 6)] = _blackPlayerPieces.Knight2;
            this[new Cell(0, 7)] = _blackPlayerPieces.Rook2;

            for (int i = 0; i < Length; i++)
            {
                this[new Cell(1, i)] = _blackPlayerPieces.Pawns[i];
            }

            #endregion

            #region White pieces placement

            for (int i = 0; i < Length; i++)
            {
                this[new Cell(6, i)] = _whitePlayerPieces.Pawns[i];
            }

            this[new Cell(7, 0)] = _whitePlayerPieces.Rook1;
            this[new Cell(7, 1)] = _whitePlayerPieces.Knight1;
            this[new Cell(7, 2)] = _whitePlayerPieces.Bishop1;
            this[new Cell(7, 3)] = _whitePlayerPieces.Queen;
            this[new Cell(7, 4)] = _whitePlayerPieces.King;
            this[new Cell(7, 5)] = _whitePlayerPieces.Bishop2;
            this[new Cell(7, 6)] = _whitePlayerPieces.Knight2;
            this[new Cell(7, 7)] = _whitePlayerPieces.Rook2;

            #endregion
        }

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
                PlayerColor.White => _whitePlayerPieces.King,
                PlayerColor.Black => _blackPlayerPieces.King,
                _ => throw new NotImplementedException()
            };
        }

        public Piece GetQueensideRook(PlayerColor color)
        {
            return color switch
            {
                PlayerColor.White => _whitePlayerPieces.Rook1,
                PlayerColor.Black => _blackPlayerPieces.Rook1,
                _ => throw new NotImplementedException()
            };
        }

        public Piece GetKingsideRook(PlayerColor color)
        {
            return color switch
            {
                PlayerColor.White => _whitePlayerPieces.Rook2,
                PlayerColor.Black => _blackPlayerPieces.Rook2,
                _ => throw new NotImplementedException()
            };
        }
        
        public IEnumerable<Piece> GetAllPieces()
        {
            return _whitePlayerPieces.GetPieces().Concat(_blackPlayerPieces.GetPieces());
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
            RandomPiecesRow row = GenerateRandomPiecesPlacement();

            #region Black pieces placement

            this[new Cell(0, row.Rook1)] =   _blackPlayerPieces.Rook1;
            this[new Cell(0, row.Knight1)] = _blackPlayerPieces.Knight1;
            this[new Cell(0, row.Bishop1)] = _blackPlayerPieces.Bishop1;
            this[new Cell(0, row.Queen)] =   _blackPlayerPieces.Queen;
            this[new Cell(0, row.King)] =    _blackPlayerPieces.King;
            this[new Cell(0, row.Bishop2)] = _blackPlayerPieces.Bishop2;
            this[new Cell(0, row.Knight2)] = _blackPlayerPieces.Knight2;
            this[new Cell(0, row.Rook2)] =   _blackPlayerPieces.Rook2;

            for (int i = 0; i < Length; i++)
            {
                this[new Cell(1, i)] = _blackPlayerPieces.Pawns[i];
            }

            for (int i = 2; i < 6; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    this[new Cell(i, j)] = null;
                }
            }

            #endregion

            #region White pieces placement

            for (int i = 0; i < Length; i++)
            {
                this[new Cell(6, i)] = _whitePlayerPieces.Pawns[i];
            }

            this[new Cell(7, row.Rook1)] =   _whitePlayerPieces.Rook1;
            this[new Cell(7, row.Knight1)] = _whitePlayerPieces.Knight1;
            this[new Cell(7, row.Bishop1)] = _whitePlayerPieces.Bishop1;
            this[new Cell(7, row.Queen)] =   _whitePlayerPieces.Queen;
            this[new Cell(7, row.King)] =    _whitePlayerPieces.King;
            this[new Cell(7, row.Bishop2)] = _whitePlayerPieces.Bishop2;
            this[new Cell(7, row.Knight2)] = _whitePlayerPieces.Knight2;
            this[new Cell(7, row.Rook2)] =   _whitePlayerPieces.Rook2;

            #endregion
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