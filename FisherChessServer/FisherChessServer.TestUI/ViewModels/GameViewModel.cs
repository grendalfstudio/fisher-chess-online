using FisherChessServer.Core.Models;
using FisherChessServer.Core;
using FisherChessServer.TestUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace FisherChessServer.TestUI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        protected readonly GameWindow _gameWindow;
        protected readonly Game _Game;
        private readonly List<PieceViewModel> _pieces;
        private readonly List<PieceViewModel> _promotedPawns;
        protected readonly Timer _oneSecondTimer = new Timer { Interval = 1000 };
        protected readonly List<HighlightViewModel> _highlights = new List<HighlightViewModel>();
        protected PieceViewModel? _choosedPieceViewModel;
        protected Cell _cellToMakeMove = default!;
        private string _whitePlayerState = "";
        private string _blackPlayerState = "";
        private string _whitePlayerHighlight = "#00000000";
        private string _blackPlayerHighlight = "#00000000";
        private TimeSpan _time;
        private bool _isNewGameButtonEnabled;
        private bool _isFinishGameButtonEnabled;
        private RelayCommand? _startGameCommand;
        private RelayCommand? _finishGameCommand;
        private RelayCommand? _findAvailableCellsCommand;
        private RelayCommand? _makeMoveCommand;

        public GameViewModel(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _Game = new Game(new GameService(new Chessboard()));
            _Game.OnPawnPromoting += _Game_OnPawnPromoting;

            _pieces = new List<PieceViewModel>(_Game.GetAllPieces()
                .Select(piece => new PieceViewModel(piece)));
            _pieces.ForEach(pieceViewModel =>
            {
                pieceViewModel.OnImageClicked += PieceViewModel_OnImageClicked;
                _gameWindow.grid.Children.Add(pieceViewModel.Image);
            });
            _promotedPawns = new List<PieceViewModel>();

            _isNewGameButtonEnabled = true;
            _isFinishGameButtonEnabled = false;
            _oneSecondTimer.Elapsed += OneSecond_Elapsed;
        }

        public string WhitePlayerState
        {
            get => _whitePlayerState;
            set
            {
                _whitePlayerState = value;
                OnPropertyChanged();
            }
        }
        public string BlackPlayerState
        {
            get => _blackPlayerState;
            set
            {
                _blackPlayerState = value;
                OnPropertyChanged();
            }
        }
        public string WhitePlayerHighlight
        {
            get => _whitePlayerHighlight;
            set
            {
                _whitePlayerHighlight = value;
                OnPropertyChanged();
            }
        }
        public string BlackPlayerHighlight
        {
            get => _blackPlayerHighlight;
            set
            {
                _blackPlayerHighlight = value;
                OnPropertyChanged();
            }
        }
        public TimeSpan Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }
        public bool IsNewGameButtonEnabled
        {
            get => _isNewGameButtonEnabled;
            set
            {
                _isNewGameButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsFinishGameButtonEnabled
        {
            get => _isFinishGameButtonEnabled;
            set
            {
                _isFinishGameButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public virtual RelayCommand StartGameCommand
        {
            get
            {
                return _startGameCommand ??= new RelayCommand(obj =>
                {
                    // Restore pieces to initial configuration after promotions in previous game
                    _promotedPawns.ForEach(pawnVM =>
                    {
                        _pieces.Remove(pawnVM);
                        _gameWindow.grid.Children.Remove(pawnVM.Image);
                    });
                    _pieces.ForEach(pieceVM => pieceVM.Image.Visibility = Visibility.Visible);

                    _Game.StartGame();

                    WhitePlayerState = _Game.WhitePlayerState.ToString();
                    BlackPlayerState = _Game.BlackPlayerState.ToString();
                    switch (_Game.CurrentPlayer)
                    {
                        case PlayerColor.White:
                            WhitePlayerHighlight = "#FFC8FFC8";
                            BlackPlayerHighlight = "#00000000";
                            break;
                        case PlayerColor.Black:
                            WhitePlayerHighlight = "#00000000";
                            BlackPlayerHighlight = "#FFC8FFC8";
                            break;
                    }

                    Time = TimeSpan.Zero;
                    IsNewGameButtonEnabled = false;
                    IsFinishGameButtonEnabled = true;
                    _oneSecondTimer.Start();
                });
            }
        }

        public virtual RelayCommand FindAvailableCellsCommand
        {
            get
            {
                return _findAvailableCellsCommand ??= new RelayCommand(obj =>
                {
                    Piece choosedPiece = _choosedPieceViewModel!.Piece;
                    IEnumerable<Cell> availableCells = _Game.FindAvailableCells(choosedPiece);

                    _highlights.Add(new HighlightViewModel(HighlightType.ChoosedPiece, choosedPiece.Cell!));
                    _highlights.AddRange(availableCells.Select(cell =>
                    {
                        var highlight = HighlightType.ValidMove;
                        if (_Game.GetPieceAt(cell) != null)
                            highlight = HighlightType.ValidMoveOnPiece;

                        var highlightViewModel = new HighlightViewModel(highlight, cell);
                        highlightViewModel.OnImageClicked += HighlightViewModel_OnImageClicked;

                        return highlightViewModel;
                    }));

                    _highlights.ForEach(highlight => _gameWindow.grid.Children.Add(highlight.Image));

                });
            }
        }

        public virtual RelayCommand MakeMoveCommand
        {
            get
            {
                return _makeMoveCommand ??= new RelayCommand(obj =>
                {
                    _Game.MakeMove(_choosedPieceViewModel!.Piece, _cellToMakeMove);
                    WhitePlayerState = _Game.WhitePlayerState.ToString();
                    BlackPlayerState = _Game.BlackPlayerState.ToString();
                    switch (_Game.CurrentPlayer)
                    {
                        case PlayerColor.White:
                            WhitePlayerHighlight = "#FFC8FFC8";
                            BlackPlayerHighlight = "#00000000";
                            break;
                        case PlayerColor.Black:
                            WhitePlayerHighlight = "#00000000";
                            BlackPlayerHighlight = "#FFC8FFC8";
                            break;
                    }
                    _highlights.ForEach(highlight => _gameWindow.grid.Children.Remove(highlight.Image));
                    _highlights.Clear();

                    if (_Game.WhitePlayerState == PlayerState.Checkmate ||
                        _Game.BlackPlayerState == PlayerState.Checkmate)
                    {
                        FinishGameCommand.Execute(null);
                    }
                });
            }
        }

        public virtual RelayCommand FinishGameCommand
        {
            get
            {
                return _finishGameCommand ??= new RelayCommand(obj =>
                {
                    var messageAnswer = App.ShowMessage("Справді завершити гру?", true);
                    if (messageAnswer == MessageBoxResult.Yes)
                    {
                        _Game.FinishGame();

                        IsNewGameButtonEnabled = true;
                        IsFinishGameButtonEnabled = false;
                        _oneSecondTimer.Stop();
                    }
                });
            }
        }

        #endregion

        private void PieceViewModel_OnImageClicked(object? sender, EventArgs e)
        {
            if (_choosedPieceViewModel == null)
            {
                _choosedPieceViewModel = (PieceViewModel)sender!;
                FindAvailableCellsCommand.Execute(null);
            }
            else
            {
                _choosedPieceViewModel = null;
                _highlights.ForEach(highlight => _gameWindow.grid.Children.Remove(highlight.Image));
                _highlights.Clear();
            }
        }

        private void HighlightViewModel_OnImageClicked(object? sender, EventArgs e)
        {
            _cellToMakeMove = ((HighlightViewModel)sender!).Cell;
            MakeMoveCommand.Execute(null);
        }

        private void _Game_OnPawnPromoting(object? sender, Piece e)
        {
            PieceViewModel pieceViewModel = _pieces.Where(pieceVM => pieceVM.Piece.Cell == e.Cell).Single();
            pieceViewModel.Image.Visibility = Visibility.Collapsed;

            var promotedPieceViewModel = new PieceViewModel(e);
            _pieces.Add(promotedPieceViewModel);
            _promotedPawns.Add(promotedPieceViewModel);

            promotedPieceViewModel.OnImageClicked += PieceViewModel_OnImageClicked;
            _gameWindow.grid.Children.Add(promotedPieceViewModel.Image);
        }

        private void OneSecond_Elapsed(object? sender, ElapsedEventArgs e)
        {
            long second = TimeSpan.TicksPerSecond;
            Time += new TimeSpan(second);
        }
    }
}
