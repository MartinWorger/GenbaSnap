using GenbaSnap.Services.Interfaces;
using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.ConsoleApp
{
    internal class GameEngine : IGameEngine
    {
        private readonly ICommandService _commandService;
        private readonly ISnapGameService _snapGameService;

        private readonly Random _random;

        public GameEngine(
            ICommandService commandService,
            ISnapGameService snapGameService
            )
        {
            _commandService = commandService;
            _snapGameService = snapGameService;

            _random = new Random();
        }

        public void PlayGame()
        {
            var numberOfPlayers = 9;
            var numberOfTurns = 0;
            var numberOfSnapCalls = 0;

            GameState? gameState = _snapGameService.CreateGame(numberOfPlayers);

            while (gameState == GameState.Playing)
            {
                if (RandomFactor(0.05))
                {
                    if (RandomFactor(0.5))
                    {
                        //Occasionally make a random snap call
                        _snapGameService.CallSnap(RandomPlayer(numberOfPlayers));
                        numberOfSnapCalls++;
                    }
                    else
                    {
                        //Or try to play a card at random
                        var randomPlayResult = _snapGameService.PlayCard(RandomPlayer(numberOfPlayers));
                        if (randomPlayResult == Services.Enums.PlayCardResult.Success)
                        {
                            numberOfTurns++;
                        }
                    }
                }
                else
                {
                    var isSnap = _snapGameService.IsSnap();
                    if (isSnap && RandomFactor(0.8))
                    {
                        _snapGameService.CallSnap(RandomPlayer(numberOfPlayers));
                        numberOfSnapCalls++;
                    }
                    else
                    {
                        var playerTurn = _snapGameService.GetPlayerTurn();
                        if (playerTurn != null)
                        {
                            _snapGameService.PlayCard(playerTurn.Value);
                            numberOfTurns++;
                        }
                    }
                }

                gameState = _snapGameService.GetGameState();
                numberOfTurns++;

                Thread.Sleep(200);
            }

            _commandService.WriteLine();
            _commandService.WriteLine($"Game completed after {numberOfTurns} turns and {numberOfSnapCalls} snap calls");
            _commandService.WriteLine("Press any key to end");
            _commandService.ReadKey();
        }

        /// <summary>
        /// Returns true some of the time depending on <paramref name="likelihood"/> in the range 0.0 to 1.0
        /// </summary>
        /// <param name="likelihood"></param>
        /// <returns></returns>
        private bool RandomFactor(double likelihood)
        {
            var factor = _random.NextDouble();
            return likelihood > factor;
        }

        /// <summary>
        /// Returns a random player number.
        /// </summary>
        /// <param name="numberOfPlayers"></param>
        /// <returns></returns>
        private int RandomPlayer(int numberOfPlayers)
        {
            return _random.Next(1, numberOfPlayers + 1);
        }
    }
}
