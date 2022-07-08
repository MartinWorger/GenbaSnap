using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Domain.Models
{
    public class SnapGame
    {
        public SnapGame()
        {
            PlayerGameStates = new List<PlayerGameState>();
        }

        /// <summary>
        /// The variant of Snap being played.
        /// </summary>
        public GameType GameType { get; set; }

        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState GameState { get; set; }

        /// <summary>
        /// The player whose turn it is.
        /// </summary>
        public int PlayerTurn { get; set; }

        /// <summary>
        /// Game state for each player.
        /// </summary>
        public IEnumerable<PlayerGameState> PlayerGameStates { get; set; }

        /// <summary>
        /// Snap pool.
        /// </summary>
        public Queue<Card> SnapPool { get; init; } = new Queue<Card>();
    }
}
