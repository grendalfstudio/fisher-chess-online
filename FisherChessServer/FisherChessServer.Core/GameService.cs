using FisherChessServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherChessServer.Core
{
    public class GameService
    {
        private readonly Chessboard _chessboard;
        private readonly Chessboard _savedChessboard = new Chessboard();

        public GameService(Chessboard chessboard)
        {
            _chessboard = chessboard;
        }

        public IEnumerable<Cell> FindAvailableCells(Piece piece)
        {
            List<Cell> availableCells = piece.GetAvailableCells().ToList();

            piece.HasToRaiseOnCellChangeEvent = false;
            SaveChessboard();
            foreach (Cell cell in piece.GetAvailableCells())
            {
                MakeMove(piece, cell);
                if (IsCheck(piece.Color))
                    availableCells.Remove(cell);

                RestoreChessboard();
            }
            piece.HasToRaiseOnCellChangeEvent = true;

            return availableCells;
        }

        public void MakeMove(Piece piece, Cell cell)
        {
            _chessboard.MakeMove(piece, cell);
        }

        public PlayerState GetPlayerState(PlayerColor player)
        {
            if (IsCheckmate(player))
                return PlayerState.Checkmate;
            else if (IsCheck(player))
                return PlayerState.Check;
            else if (IsStalemate(player))
                return PlayerState.Stalemate;
            else
                return PlayerState.Regular;
        }

        public void ResetChessboard()
        {
            _chessboard.Reset();
        }

        public Piece? GetPieceAt(Cell cell)
        {
            return _chessboard[cell];
        }

        public IEnumerable<Piece> GetAllPieces()
        {
            return _chessboard.GetAllPieces();
        }

        private bool IsCheck(PlayerColor player)
        {
            var opponentPlayerAvailableCells = new List<Cell>();
            foreach (Piece? piece in _chessboard.Board)
            {
                // If piece is opponent's piece
                if (piece != null && piece.Color != player)
                    opponentPlayerAvailableCells.AddRange(piece.GetAvailableCells());
            }

            Piece king = _chessboard.GetKing(player);
            return opponentPlayerAvailableCells.Contains(king.Cell!);
        }

        private bool IsStalemate(PlayerColor player)
        {
            var currentPlayerAvailableCells = new List<Cell>();
            foreach (Piece? piece in _chessboard.Board)
            {
                // If piece is a friendly piece
                if (piece != null && piece.Color == player)
                {
                    piece.HasToRaiseOnCellChangeEvent = false;
                    SaveChessboard();
                    foreach (Cell cell in piece.GetAvailableCells())
                    {
                        MakeMove(piece, cell);
                        if (!IsCheck(player))
                            currentPlayerAvailableCells.Add(cell);
                        RestoreChessboard();
                    }
                    piece.HasToRaiseOnCellChangeEvent = true;
                }
            }

            return currentPlayerAvailableCells.Count == 0;
        }

        private bool IsCheckmate(PlayerColor player)
        {
            if (!IsCheck(player))
                return false;

            return IsStalemate(player);
        }

        private void SaveChessboard()
        {
            for (int i = 0; i < Chessboard.Length; i++)
            {
                for (int j = 0; j < Chessboard.Length; j++)
                {
                    var cell = new Cell(i, j);
                    _savedChessboard[cell] = _chessboard[cell];
                }
            }
        }

        private void RestoreChessboard()
        {
            for (int i = 0; i < Chessboard.Length; i++)
            {
                for (int j = 0; j < Chessboard.Length; j++)
                {
                    var cell = new Cell(i, j);
                    _chessboard[cell] = _savedChessboard[cell];
                }
            }
        }
    }
}
