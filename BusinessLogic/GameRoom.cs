using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using HangmanServer.Contracts;
using HangmanServer.DTOs;

namespace HangmanServer.BusinessLogic 
{
    public enum GameState
    {
        WaitingForPlayers,
        GuesserTurn,
        CreatorTurn,
        Finished
    }

    public class GameRoom
    {
        private const int MaxMistakesAllowed = 6;
        private const int TurnDurationSeconds = 60;

        public int MatchId { get; private set; }
        private readonly int _creatorId;
        private readonly int _challengerId;
        private readonly GameContextDTO _context;

        private GameState _currentState;
        private int _currentMistakes;
        private readonly HashSet<char> _guessedLetters; 
        private int _revealedLettersCount;
        private char _lastGuessedLetter;

        private IGameCallback _creatorCallback;
        private IGameCallback _challengerCallback;

        private readonly object _syncLock = new object();
        private readonly Timer _turnTimer;
        private int _secondsLeft;

        public event Action<int, GameEndDTO> RoomFinished;

        public GameRoom(GameContextDTO context)
        {
            MatchId = context.MatchId;
            _context = context;
            _creatorId = context.CreatorId;
            _challengerId = context.ChallengerId;

            _currentState = GameState.WaitingForPlayers;
            _currentMistakes = 0;
            _guessedLetters = new HashSet<char>();
            _revealedLettersCount = 0;

            _turnTimer = new Timer(1000);
            _turnTimer.Elapsed += OnTimerTick;
        }

        public void BroadcastChatMessage(string senderUsername, string message)
        {
            lock (_syncLock)
            {
                if (_currentState == GameState.Finished)
                    return;

                if (_creatorCallback != null)
                {
                    try { 
                        _creatorCallback.OnChatMessageReceived(senderUsername, message); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al enviar mensaje de chat al creador: {ex.Message}");
                    }
                }

                if (_challengerCallback != null)
                {
                    try { 
                        _challengerCallback.OnChatMessageReceived(senderUsername, message); 
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine($"Error al enviar mensaje de chat al retador: {ex.Message}");

                    }
                }
            }
        }

        public void Join(int userId, IGameCallback callback)
        {
            lock (_syncLock)
            {
                if (_currentState != GameState.WaitingForPlayers)
                {
                    throw new InvalidOperationException("La partida ya ha comenzado o ha finalizado. No puedes unirte.");
                }

                if (userId == _creatorId)
                {
                    _creatorCallback = callback;
                }
                else if (userId == _challengerId)
                {
                    _challengerCallback = callback;
                }
                else
                {
                    throw new UnauthorizedAccessException("El usuario no pertenece a esta partida.");
                }

                if (_creatorCallback != null && _challengerCallback != null)
                {
                    StartGame();
                }
            }
        }

        public void ProcessGuess(int userId, char guessLetter)
        {
            lock (_syncLock)
            {
                if (_currentState != GameState.GuesserTurn)
                {
                    throw new InvalidOperationException("No es el turno del adivinador.");
                }

                if (userId != _challengerId)
                {
                    throw new UnauthorizedAccessException("Solo el adivinador puede proponer letras.");
                }

                guessLetter = char.ToUpper(guessLetter); 

                if (_guessedLetters.Contains(guessLetter))
                {
                    return;
                }

                _guessedLetters.Add(guessLetter);
                _lastGuessedLetter = guessLetter;

                _currentState = GameState.CreatorTurn;
                ResetAndStartTimer();

                try
                {
                    _creatorCallback.OnGuessReceived(guessLetter);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"Timeout al notificar al creador en la partida {MatchId}: {ex.Message}");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error de conexión con el creador en la partida {MatchId}: {ex.Message}");
                }

                try
                {
                    _challengerCallback.OnGuessReceived(guessLetter);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"Timeout al notificar al retador en la partida {MatchId}: {ex.Message}");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error de conexión con el retador en la partida {MatchId}: {ex.Message}");
                }
            }
        }

        public void ProcessTurnResult(int userId, bool isCorrect, int[] correctPositions)
        {
            lock (_syncLock)
            {
                if (_currentState != GameState.CreatorTurn)
                {
                    throw new InvalidOperationException("No es el turno del creador para evaluar.");
                }

                if (userId != _creatorId)
                {
                    throw new UnauthorizedAccessException("Solo el creador puede evaluar la letra.");
                }

                string secretWord = _context.SecretWord.ToUpper();
                char guessedLetter = char.ToUpper(_lastGuessedLetter);

                List<int> actualPositions = new List<int>();
                for (int i = 0; i < secretWord.Length; i++)
                {
                    if (secretWord[i] == guessedLetter)
                    {
                        actualPositions.Add(i);
                    }
                }

                bool hasError = (actualPositions.Count > 0 && !isCorrect) ||
                                (actualPositions.Count == 0 && isCorrect) ||
                                (isCorrect && (correctPositions == null || actualPositions.Count != correctPositions.Length || !actualPositions.All(correctPositions.Contains)));

                if (hasError)
                {
                    _creatorCallback.OnEvaluationError();
                    return;
                }

                if (!isCorrect)
                {
                    _currentMistakes++;
                }
                else if (correctPositions != null)
                {
                    _revealedLettersCount += correctPositions.Length;
                }

                var turnResult = new TurnResultDTO
                {
                    IsCorrect = isCorrect,
                    GuessedLetter = _lastGuessedLetter,
                    CorrectPositions = correctPositions,
                    CurrentMistakes = _currentMistakes
                };

                NotifyTurnResultToPlayers(turnResult);

                CheckWinConditions();
            }
        }

        private void NotifyTurnResultToPlayers(TurnResultDTO turnResult)
        {
            try
            {
                _creatorCallback.OnTurnResultReceived(turnResult);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout al notificar al creador en la partida {MatchId}: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Error de conexión con el creador en la partida {MatchId}: {ex.Message}");
            }

            try
            {
                _challengerCallback.OnTurnResultReceived(turnResult);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout al notificar al retador en la partida {MatchId}: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Error de conexión con el retador en la partida {MatchId}: {ex.Message}");
            }
        }
        private void StartGame()
        {
            _currentState = GameState.GuesserTurn;

            _creatorCallback.OnGameStarted(_context);
            _challengerCallback.OnGameStarted(_context);

            ResetAndStartTimer();
        }

        private void CheckWinConditions()
        {
            if (_currentMistakes >= MaxMistakesAllowed)
            {
                EndGame(new GameEndDTO
                {
                    Reason = MatchEndReason.MaxMistakesReached,
                    WinnerId = _creatorId, 
                    PenalizedUserId = 0
                });
            }
            else if (_revealedLettersCount >= _context.WordLength)
            {
                EndGame(new GameEndDTO
                {
                    Reason = MatchEndReason.WordGuessed,
                    WinnerId = _challengerId, 
                    PenalizedUserId = 0
                });
            }
            else
            {
                _currentState = GameState.GuesserTurn;
                ResetAndStartTimer();
            }
        }

        public void LeaveMatch(int userId)
        {
            lock (_syncLock)
            {
                if (_currentState == GameState.Finished)
                {
                    return;
                }

                int winnerId = (userId == _creatorId) ? _challengerId : _creatorId;

                EndGame(new GameEndDTO
                {
                    Reason = MatchEndReason.Abandoned,
                    WinnerId = winnerId,
                    PenalizedUserId = userId 
                });
            }
        }

        private void ResetAndStartTimer()
        {
            _secondsLeft = TurnDurationSeconds;
            _turnTimer.Start();
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            lock (_syncLock)
            {
                if (_currentState == GameState.Finished)
                {
                    _turnTimer.Stop();
                    return;
                }

                _secondsLeft--;

                try
                {
                    _creatorCallback.OnTimerTick(_secondsLeft);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"Timeout al actualizar reloj del creador: {ex.Message}");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al actualizar reloj del creador: {ex.Message}");
                }

                try
                {
                    _challengerCallback.OnTimerTick(_secondsLeft);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"Timeout al actualizar reloj del retador: {ex.Message}");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al actualizar reloj del retador: {ex.Message}");
                }

                if (_secondsLeft <= 0)
                {
                    HandleTimeout();
                }
            }
        }

        private void HandleTimeout()
        {
            _turnTimer.Stop();

            int penalizedUser = _currentState == GameState.GuesserTurn ? _challengerId : _creatorId;
            int winnerUser = _currentState == GameState.GuesserTurn ? _creatorId : _challengerId;

            _currentState = GameState.Finished;

            var endResult = new GameEndDTO
            {
                Reason = MatchEndReason.Timeout,
                PenalizedUserId = penalizedUser,
                WinnerId = winnerUser
            };

            EndGame(endResult);
        }

        private void EndGame(GameEndDTO result)
        {
            _currentState = GameState.Finished;
            _turnTimer.Stop();

            try
            {
                _creatorCallback.OnGameEnded(result);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout EndGame Creador: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Error EndGame Creador: {ex.Message}");
            }

            try
            {
                _challengerCallback.OnGameEnded(result);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout EndGame Retador: {ex.Message}");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Error EndGame Retador: {ex.Message}");
            }

            RoomFinished?.Invoke(MatchId, result);
        }
    }
}