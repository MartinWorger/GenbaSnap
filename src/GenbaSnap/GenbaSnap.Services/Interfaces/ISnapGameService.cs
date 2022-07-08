using static GenbaSnap.Domain.Enums;
using static GenbaSnap.Services.Enums;

namespace GenbaSnap.Services.Interfaces
{
    /// <summary>
    /// Main service for playing games of snap.
    /// </summary>
    public interface ISnapGameService
    {
        /// <summary>
        /// Create a new game of snap.
        /// </summary>
        /// <param name="numberOfPlayers"></param>
        /// <returns></returns>
        public GameState CreateGame(int numberOfPlayers);

        /// <summary>
        /// Get the current game state.
        /// </summary>
        /// <returns></returns>
        public GameState? GetGameState();

        /// <summary>
        /// Are there two matching cards showing?
        /// </summary>
        /// <returns></returns>
        public bool IsSnap();

        /// <summary>
        /// Whose turn is it?
        /// </summary>
        /// <returns></returns>
        public int? GetPlayerTurn();

        /// <summary>
        /// Call snap.
        /// </summary>
        /// <param name="playerNumber"></param>
        /// <returns></returns>
        public SnapCallResult CallSnap(int playerNumber);

        /// <summary>
        /// Play the next card in the player's deck.
        /// </summary>
        /// <param name="playerNumber"></param>
        /// <returns></returns>
        public PlayCardResult PlayCard(int playerNumber);
    }
}
