using GenbaSnap.Domain.Models;
using GenbaSnap.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GenbaSnap.Services.Internal
{
    internal class SnapGameFactory : ISnapGameFactory
    {
        private readonly ILogger<SnapGameFactory> _logger;
        private readonly ICardDeckFactory _cardDeckFactory;

        public SnapGameFactory(ILogger<SnapGameFactory> logger, ICardDeckFactory cardDeckFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cardDeckFactory = cardDeckFactory ?? throw new ArgumentNullException(nameof(cardDeckFactory));
        }

        public SnapGame CreateSnapGame(Domain.Enums.GameType gameType, int noPlayers)
        {
            _logger.LogDebug("Creating new Snap game");

            if (noPlayers < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(noPlayers), "Must have at least 2 players in a game");
            }

            var result = new SnapGame
            {
                GameType = gameType,
                GameState = Domain.Enums.GameState.Initialising,
                PlayerGameStates = InitialisePlayerGameStates(noPlayers),
                PlayerTurn = 1
            };

            DealCards(result.PlayerGameStates);

            result.GameState = Domain.Enums.GameState.Playing;

            _logger.LogDebug("Created new Snap game");

            return result;
        }

        /// <summary>
        /// Initalises a collection of <see cref="PlayerGameState"/>
        /// </summary>
        /// <param name="noPlayers"></param>
        /// <returns></returns>
        internal IEnumerable<PlayerGameState> InitialisePlayerGameStates(int noPlayers)
        {
            var playerGameStates = new List<PlayerGameState>();
            for (var index = 0; index < noPlayers; index++)
            {
                playerGameStates.Add(new PlayerGameState { PlayerNumber = index + 1 });
            }

            return playerGameStates;
        }

        /// <summary>
        /// Deals a new shuffled deck of cards into players' Face Down Stacks.
        /// </summary>
        /// <param name="playerGameStates"></param>
        internal void DealCards(IEnumerable<PlayerGameState> playerGameStates)
        {
            var noPlayers = playerGameStates.Count();
            var shuffledPack = _cardDeckFactory.CreateShuffledDeck();
            var dealPlayer = 1;
            do
            {
                var nextCard = shuffledPack.Dequeue();
                var playerState = playerGameStates.First(x => x.PlayerNumber == dealPlayer);
                playerState.FaceDownStack.Push(nextCard);
                dealPlayer++;
                if (dealPlayer > noPlayers)
                {
                    dealPlayer = 1;
                }
            } while (shuffledPack.Any());
        }
    }
}
