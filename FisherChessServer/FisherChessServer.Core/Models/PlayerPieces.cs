using FisherChessServer.Core.Models.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherChessServer.Core.Models
{
    public class PlayerPieces
    {
        public PlayerPieces(PlayerColor color, Chessboard chessboard)
        {
            Rook1 = new Rook(color, null, chessboard);
            Knight1 = new Knight(color, null, chessboard);
            Bishop1 = new Bishop(color, null, chessboard);
            Queen = new Queen(color, null, chessboard);
            King = new King(color, null, chessboard);
            Bishop2 = new Bishop(color, null, chessboard);
            Knight2 = new Knight(color, null, chessboard);
            Rook2 = new Rook(color, null, chessboard);

            Pawns = new Pawn[Chessboard.Length];
            for (int i = 0; i < Pawns.Length; i++)
            {
                Pawns[i] = new Pawn(color, null, chessboard);
            }
        }

        public Piece Rook1 { get; }
        public Piece Knight1 { get; }
        public Piece Bishop1 { get; }
        public Piece Queen { get; }
        public Piece King { get; }
        public Piece Bishop2 { get; }
        public Piece Knight2 { get; }
        public Piece Rook2 { get; }

        public Piece[] Pawns { get; }

        public IEnumerable<Piece> GetPieces()
        {
            var pieces = new List<Piece>
            {
                Rook1,
                Knight1,
                Bishop1,
                Queen,
                King,
                Bishop2,
                Knight2,
                Rook2
            };
            pieces.AddRange(Pawns);

            return pieces;
        }
    }
}
