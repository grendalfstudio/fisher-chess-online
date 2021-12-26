using FisherChessServer.Core.Models;
using FisherChessServer.Core.Models.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FisherChessServer.Core
{
    public class GameService
    {
        private const int QueensideCastlingKingPosition = 2;
        private const int QueensideCastlingRookPosition = 3;
        private const int KingsideCastlingKingPosition = 6;
        private const int KingsideCastlingRookPosition = 5;

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

        public void DoCastling(Piece king, Piece rook)
        {
            Cell castledKingCell = default!;
            Cell castledRookCell = default!;
            if (rook.Cell!.Column < king.Cell!.Column)
            {
                castledKingCell = new Cell(king.Cell!.Row, QueensideCastlingKingPosition);
                castledRookCell = new Cell(rook.Cell!.Row, QueensideCastlingRookPosition);
            }
            else if (rook.Cell!.Column > king.Cell!.Column)
            {
                castledKingCell = new Cell(king.Cell!.Row, KingsideCastlingKingPosition);
                castledRookCell = new Cell(rook.Cell!.Row, KingsideCastlingRookPosition);
            }
            _chessboard[king.Cell] = null;
            _chessboard[rook.Cell] = null;
            _chessboard[castledKingCell] = king;
            _chessboard[castledRookCell] = rook;
            king.Cell = castledKingCell;
            rook.Cell = castledRookCell;
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

        public Piece GetKing(PlayerColor color)
        {
            return _chessboard.GetKing(color);
        }

        public Piece GetQueensideRook(PlayerColor color)
        {
            return _chessboard.GetQueensideRook(color);
        }

        public Piece GetKingsideRook(PlayerColor color)
        {
            return _chessboard.GetKingsideRook(color);
        }

        public IEnumerable<Piece> GetAllPieces()
        {
            return _chessboard.GetAllPieces();
        }

        public bool IsQueensideCastlingAvailable(PlayerColor color, bool canWhiteCastleQueenside, bool canBlackCastleQueenside)
        {
            Piece king = GetKing(color);
            Piece rook = GetQueensideRook(color);

            switch (king.Color)
            {
                case PlayerColor.White:
                    if (!canWhiteCastleQueenside)
                        return false;
                    break;
                case PlayerColor.Black:
                    if (!canBlackCastleQueenside)
                        return false;
                    break;
            }

            if (IsKingRouteInterrupted(king, rook, QueensideCastlingKingPosition) ||
                IsRookRouteInterrupted(rook, king, QueensideCastlingRookPosition))
            {
                return false;
            }

            return true;
        }

        public bool IsKingsideCastlingAvailable(PlayerColor color, bool canWhiteCastleKingside, bool canBlackCastleKingside)
        {
            Piece king = GetKing(color);
            Piece rook = GetKingsideRook(color);

            switch (king.Color)
            {
                case PlayerColor.White:
                    if (!canWhiteCastleKingside)
                        return false;
                    break;
                case PlayerColor.Black:
                    if (!canBlackCastleKingside)
                        return false;
                    break;
            }

            if (IsKingRouteInterrupted(king, rook, KingsideCastlingKingPosition) ||
                IsRookRouteInterrupted(rook, king, KingsideCastlingRookPosition))
            {
                return false;
            }

            return true;
        }

        private bool IsKingRouteInterrupted(Piece king, Piece rook, int kingCastlingPosition)
{
            king.HasToRaiseOnCellChangeEvent = false;
            SaveChessboard();
            bool isKingRouteInterrupted = false;
            int startPosition = Math.Min(king.Cell!.Column, kingCastlingPosition);
            int endPosition = Math.Max(king.Cell!.Column, kingCastlingPosition);
            for (int i = startPosition; i <= endPosition; i++)
            {
                var cell = new Cell(king.Cell!.Row, i);
                if (_chessboard[cell] != null && _chessboard[cell] != king && _chessboard[cell] != rook)
                {
                    isKingRouteInterrupted = true;
                    break;
                }

                if (_chessboard[cell] != king)
                {
                    MakeMove(king, cell);
                    if (IsCheck(king.Color))
                    {
                        isKingRouteInterrupted = true;
                        // Cannot break here because we have to restore a board
                    }
                    RestoreChessboard();
                }
                else if (IsCheck(king.Color))
                {
                    isKingRouteInterrupted = true;
                    break;
                }
            }
            king.HasToRaiseOnCellChangeEvent = true;

            return isKingRouteInterrupted;
        }

        private bool IsRookRouteInterrupted(Piece rook, Piece king, int rookCastlingPosition)
        {
            bool isRookRouteInterrupted = false;
            int startPosition = Math.Min(rook.Cell!.Column, rookCastlingPosition);
            int endPosition = Math.Max(rook.Cell!.Column, rookCastlingPosition);
            for (int i = startPosition; i <= endPosition; i++)
            {
                var cell = new Cell(rook.Cell!.Row, i);
                if (_chessboard[cell] != null && _chessboard[cell] != rook && _chessboard[cell] != king)
                {
                    isRookRouteInterrupted = true;
                    break;
                }
            }

            return isRookRouteInterrupted;
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
