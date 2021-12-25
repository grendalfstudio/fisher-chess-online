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

            PawnA = new Pawn(color, null, chessboard);
            PawnB = new Pawn(color, null, chessboard);
            PawnC = new Pawn(color, null, chessboard);
            PawnD = new Pawn(color, null, chessboard);
            PawnE = new Pawn(color, null, chessboard);
            PawnF = new Pawn(color, null, chessboard);
            PawnG = new Pawn(color, null, chessboard);
            PawnH = new Pawn(color, null, chessboard);
        }

        public Piece Rook1 { get; }
        public Piece Knight1 { get; }
        public Piece Bishop1 { get; }
        public Piece Queen { get; }
        public Piece King { get; }
        public Piece Bishop2 { get; }
        public Piece Knight2 { get; }
        public Piece Rook2 { get; }

        public Piece PawnA { get; }
        public Piece PawnB { get; }
        public Piece PawnC { get; }
        public Piece PawnD { get; }
        public Piece PawnE { get; }
        public Piece PawnF { get; }
        public Piece PawnG { get; }
        public Piece PawnH { get; }

        public IEnumerable<Piece> GetPieces()
        {
            var pieces = new List<Piece>();

            pieces.Add(Rook1);
            pieces.Add(Knight1);
            pieces.Add(Bishop1);
            pieces.Add(Queen);
            pieces.Add(King);
            pieces.Add(Bishop2);
            pieces.Add(Knight2);
            pieces.Add(Rook2);

            pieces.Add(PawnA);
            pieces.Add(PawnB);
            pieces.Add(PawnC);
            pieces.Add(PawnD);
            pieces.Add(PawnE);
            pieces.Add(PawnF);
            pieces.Add(PawnG);
            pieces.Add(PawnH);

            return pieces;
        }
    }
}
