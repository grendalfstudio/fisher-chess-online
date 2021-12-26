using FisherChessServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherChessServer.Core
{
    public class Game
    {
        private readonly GameService _gameService;

        public Game(GameService gameService)
        {
            _gameService = gameService;
        }

        public bool IsGameStarted { get; set; }
        public bool CanWhiteCastleQueenside { get; set; }
        public bool CanWhiteCastleKingside { get; set; }
        public bool CanBlackCastleQueenside { get; set; }
        public bool CanBlackCastleKingside { get; set; }
        public PlayerColor CurrentPlayer { get; set; }
        public PlayerState WhitePlayerState { get; private set; }
        public PlayerState BlackPlayerState { get; private set; }

        public void StartGame()
        {
            _gameService.ResetChessboard();
            IsGameStarted = true;
            CanWhiteCastleQueenside = true;
            CanWhiteCastleKingside = true;
            CanBlackCastleQueenside = true;
            CanBlackCastleKingside = true;

            CurrentPlayer = PlayerColor.White;
            WhitePlayerState = BlackPlayerState = PlayerState.Regular;
        }

        public IEnumerable<Cell> FindAvailableCells(Piece piece)
        {
            if (!IsGameStarted || piece.Color != CurrentPlayer)
                return new List<Cell>();

            var availableCells = new List<Cell>(_gameService.FindAvailableCells(piece));
            if (piece.Type == PieceType.King)
            {
                if (_gameService.IsQueensideCastlingAvailable(CurrentPlayer, CanWhiteCastleQueenside, CanBlackCastleQueenside))
                {
                    availableCells.Add(_gameService.GetQueensideRook(piece.Color).Cell!);
                }
                if (_gameService.IsKingsideCastlingAvailable(CurrentPlayer, CanWhiteCastleKingside, CanBlackCastleKingside))
                {
                    availableCells.Add(_gameService.GetKingsideRook(piece.Color).Cell!);
                }
            }

            return availableCells;
        }

        public void MakeMove(Piece piece, Cell cell)
        {
            UpdateCastlingPossibilities(piece);

            Piece? pieceOnTheCell = _gameService.GetPieceAt(cell);
            if (piece.Type == PieceType.King && pieceOnTheCell != null && 
                pieceOnTheCell.Color == piece.Color && pieceOnTheCell.Type == PieceType.Rook)
            {
                _gameService.DoCastling(piece, pieceOnTheCell);
            }
            else
            {
                _gameService.MakeMove(piece, cell);
            }
            WhitePlayerState = _gameService.GetPlayerState(PlayerColor.White);
            BlackPlayerState = _gameService.GetPlayerState(PlayerColor.Black);

            SwitchPlayer();
        }

        public void FinishGame()
        {
            IsGameStarted = false;
        }

        public Piece? GetPieceAt(Cell cell)
        {
            return _gameService.GetPieceAt(cell);
        }

        public IEnumerable<Piece> GetAllPieces()
        {
            return _gameService.GetAllPieces();
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer switch
            {
                PlayerColor.White => PlayerColor.Black,
                PlayerColor.Black => PlayerColor.White,
                _ => throw new NotImplementedException()
            };
        }

        private void UpdateCastlingPossibilities(Piece piece)
        {
            if (piece.Type == PieceType.King)
            {
                switch (piece.Color)
                {
                    case PlayerColor.White:
                        if (CanWhiteCastleQueenside) 
                            CanWhiteCastleQueenside = false;
                        if (CanWhiteCastleKingside) 
                            CanWhiteCastleKingside = false;
                        break;
                    case PlayerColor.Black:
                        if (CanBlackCastleQueenside)
                            CanBlackCastleQueenside = false;
                        if (CanBlackCastleKingside)
                            CanBlackCastleKingside = false;
                        break;
                }
            }
            else if (piece.Type == PieceType.Rook)
            {
                int kingColumn = _gameService.GetKing(piece.Color).Cell!.Column;
                // If rook is on the left having right to castle queenside 
                if (piece.Cell!.Column < kingColumn)
                {
                    switch (piece.Color)
                    {
                        case PlayerColor.White:
                            if (CanWhiteCastleQueenside)
                                CanWhiteCastleQueenside = false;
                            break;
                        case PlayerColor.Black:
                            if (CanBlackCastleQueenside)
                                CanBlackCastleQueenside = false;
                            break;
                    }
                }
                // If rook is on the right having right to castle kingside
                else if (piece.Cell!.Column > kingColumn)
                {
                    switch (piece.Color)
                    {
                        case PlayerColor.White:
                            if (CanWhiteCastleKingside)
                                CanWhiteCastleKingside = false;
                            break;
                        case PlayerColor.Black:
                            if (CanBlackCastleKingside)
                                CanBlackCastleKingside = false;
                            break;
                    }
                }
            }
        }
    }
}
