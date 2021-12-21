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
        public PlayerColor CurrentPlayer { get; set; }
        public PlayerState WhitePlayerState { get; private set; }
        public PlayerState BlackPlayerState { get; private set; }

        public void StartGame()
        {
            _gameService.ResetChessboard();
            IsGameStarted = true;

            CurrentPlayer = PlayerColor.White;
            WhitePlayerState = BlackPlayerState = PlayerState.Regular;
        }

        public IEnumerable<Cell> FindAvailableCells(Piece piece)
        {
            return IsGameStarted && piece.Color == CurrentPlayer
                ? _gameService.FindAvailableCells(piece)
                : new List<Cell>();
        }

        public void MakeMove(Piece piece, Cell cell)
        {
            _gameService.MakeMove(piece, cell);
            WhitePlayerState = _gameService.GetPlayerState(PlayerColor.White);
            BlackPlayerState = _gameService.GetPlayerState(PlayerColor.Black);

            SwitchPlayer();
        }

        public void FinishGame()
        {
            IsGameStarted = false;
        }

        public Piece GetPieceAt(Cell cell)
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
    }
}
