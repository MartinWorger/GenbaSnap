using GenbaSnap.Domain.Models;
using GenbaSnap.Persistence.Interfaces;
using GenbaSnap.Services.Interfaces;
using Microsoft.Extensions.Logging;
using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Services.Internal
{
    internal class SnapGameService : ISnapGameService
    {
        private readonly ILogger<SnapGameService> _logger;
        private readonly ISnapGameFactory _snapGameFactory;
        private readonly ISnapGameRepository _snapGameRepository;

        internal SnapGame? game;

        public SnapGameService(
            ILogger<SnapGameService> logger,
            ISnapGameFactory snapGameFactory,
            ISnapGameRepository snapGameRepository
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snapGameFactory = snapGameFactory ?? throw new ArgumentNullException(nameof(snapGameFactory));
            _snapGameRepository = snapGameRepository ?? throw new ArgumentNullException(nameof(snapGameRepository));
        }

        public GameState CreateGame(int numberOfPlayers)
        {
            _logger.LogInformation("Starting new snap game for {numberOfPlayers} players", numberOfPlayers);
            game = _snapGameFactory.CreateSnapGame(GameType.SimpleSnap, numberOfPlayers);
            SaveGame();

            return game.GameState;
        }

        public GameState? GetGameState()
        {
            return game?.GameState;
        }

        public int? GetPlayerTurn()
        {
            return game?.PlayerTurn;
        }

        public Enums.SnapCallResult CallSnap(int playerNumber)
        {
            _logger.LogInformation("!!! SNAP !!!");

            if (FetchGame())
            { 
                if (!PlayerNumberValid(playerNumber))
                {
                    _logger.LogWarning("Bad snap call by invalid player {playerNumber}", playerNumber);
                    return Enums.SnapCallResult.InvalidPlayer;
                }

                if (!IsSnap())
                {
                    _logger.LogWarning("Bad snap call by player {playerNumber}", playerNumber);
                    return Enums.SnapCallResult.BadCall;
                }

                _logger.LogInformation("Successful snap call by player {playerNumber}", playerNumber);

                //It is a successful snap
                TakeSnapCards(playerNumber);
                UpdateGameStatus();
                SaveGame();

                return Enums.SnapCallResult.Won;
            }

            _logger.LogWarning("Bad snap call when game not in valid state");
            return Enums.SnapCallResult.GameNotValid;
        }

        public bool IsSnap()
        {
            if (FetchGame())
            {
                if (game!.GameState != GameState.Playing)
                {
                    return false;
                }

                var topCardValues = new List<CardValue>();
                foreach (var player in game!.PlayerGameStates.Where(x => x.FaceUpStack.Any()))
                {
                    topCardValues.Add(player.FaceUpStack.Peek().Value);
                }

                var isSnap = topCardValues
                    .GroupBy(x => x)
                    .Any(x => x.Count() > 1);

                if (isSnap)
                {
                    _logger.LogDebug("*** It's a snap ***");
                }

                return isSnap;
            }

            return false;
        }

        public Enums.PlayCardResult PlayCard(int playerNumber)
        {
            if (FetchGame())
            {
                if (!TryGetPlayerGameState(playerNumber, out var playerGameState))
                {
                    _logger.LogWarning("Play card attempt by invalid player {playerNumber}", playerNumber);
                    return Enums.PlayCardResult.InvalidPlayer;
                }

                if (game!.GameState != GameState.Playing)
                {
                    _logger.LogWarning("Play card attempt by player {playerNumber} when game in invalid state {game.GameState}", playerNumber, game.GameState);
                    return Enums.PlayCardResult.InvalidGameState;
                }

                if (game.PlayerTurn != playerNumber)
                {
                    _logger.LogWarning("Play card attempt by player {playerNumber} out of turn", playerNumber);
                    return Enums.PlayCardResult.OutOfTurn;
                }

                if (!playerGameState!.FaceDownStack.Any())
                {
                    _logger.LogWarning("Play card attempt by player {playerNumber} who has no cards left", playerNumber);
                    return Enums.PlayCardResult.NoCards;
                }

                //Play the card
                var card = playerGameState!.FaceDownStack.Pop();
                playerGameState.FaceUpStack.Push(card);
                _logger.LogInformation("{card.Value} of {card.Suit} was played by player {playerNumber}", card.Value, card.Suit, playerNumber);
                if (!playerGameState.FaceDownStack.Any())
                {
                    _logger.LogInformation("Player {playerNumber} has now run out of cards to play", playerNumber);
                }

                SetNextPlayer();
                UpdateGameStatus();
                SaveGame();
                return Enums.PlayCardResult.Success;
            }

            _logger.LogWarning("Play card attempt by player {playerNumber} when game does not exist", playerNumber);
            return Enums.PlayCardResult.InvalidGameState;

        }

        internal bool FetchGame()
        {
            var savedGame = _snapGameRepository.GetSnapGame();

            if (savedGame != null)
            {
                game = savedGame;
            }
            return savedGame != null;
        }

        internal void SaveGame()
        {
            if (game != null)
            {
                _snapGameRepository.SaveSnapGame(game!);
            }
        }

        /// <summary>
        /// Player number is valid if we have a game, the player is in the game, and has any cards left
        /// </summary>
        /// <param name="playerNumber"></param>
        /// <returns></returns>
        internal bool PlayerNumberValid(int playerNumber)
        {
            return 
                game != null && 
                playerNumber > 0 && 
                game.PlayerGameStates.Any(x => x.PlayerNumber == playerNumber && (x.FaceDownStack.Any() || x.FaceUpStack.Any()));
        }

        internal bool TryGetPlayerGameState(int playerNumber, out PlayerGameState? playerGameState)
        {
            playerGameState = null;

            if (!PlayerNumberValid(playerNumber))
            {
                return false;
            }

            playerGameState = game!.PlayerGameStates.FirstOrDefault(x => x.PlayerNumber == playerNumber);
            return playerGameState != null;
        }

        /// <summary>
        /// Updates the game with the player number next to take a turn,
        /// based on who has cards remaining
        /// </summary>
        internal void SetNextPlayer()
        {
            var currentPlayer = game!.PlayerTurn;
            var allPlayers = game.PlayerGameStates.ToList();
            var playersWithCards = allPlayers.Where(x => x.FaceDownStack.Any());

            if (playersWithCards.Count() > 1)
            {
                var nextPlayer = currentPlayer;
                do
                {
                    nextPlayer++;
                    if (nextPlayer > allPlayers.Count)
                    {
                        nextPlayer = 1;
                    }
                } while (!playersWithCards.Any(x => x.PlayerNumber == nextPlayer));

                game.PlayerTurn = nextPlayer;
            }
        }

        internal void TakeSnapCards(int playerNumber)
        {
            if (TryGetPlayerGameState(playerNumber, out PlayerGameState? playerGameState))
            {
                var beforeCount = playerGameState!.FaceDownStack.Count;
                var playerStates = game!.PlayerGameStates.ToList();

                var topCardValues = new List<CardValue>();
                foreach (var player in playerStates.Where(x => x.FaceUpStack.Any()))
                {
                    topCardValues.Add(player.FaceUpStack.Peek().Value);
                }

                var snapValues = topCardValues
                    .GroupBy(x => x)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key);

                var snapStacks = playerStates
                    .Where(x => x.FaceUpStack.Any() && snapValues.Contains(x.FaceUpStack.Peek().Value))
                    .Select(x => x.FaceUpStack);
                var reverseStack = new Stack<Card>(playerGameState!.FaceDownStack.Reverse());
                foreach (var snapStack in snapStacks)
                {
                    while (snapStack.Any())
                    {
                        var card = snapStack.Pop();
                        reverseStack.Push(card);
                    }
                }
                playerGameState!.FaceDownStack = new Stack<Card>(reverseStack.Reverse());
                var afterCount = playerGameState!.FaceDownStack.Count;
                var cardCount = afterCount - beforeCount;
                _logger.LogInformation("Player {playerNumber} has picked up {cardCount} cards and now has {afterCount}", playerNumber, cardCount, afterCount);
            }
        }

        internal void UpdateGameStatus()
        {
            //If only one player has cards left that are face down then they have won
            if (game!.PlayerGameStates.Count(x => x.FaceDownStack.Any()) <= 1)
            {
                var winner = game.PlayerGameStates.FirstOrDefault(x => x.FaceDownStack.Any());
                if (winner != null)
                {
                    var playerNumber = winner.PlayerNumber;
                    _logger.LogInformation("Game has been won by player {playerNumber}", playerNumber);
                }
                game.GameState = GameState.Finished;
            }
        }

    }
}
