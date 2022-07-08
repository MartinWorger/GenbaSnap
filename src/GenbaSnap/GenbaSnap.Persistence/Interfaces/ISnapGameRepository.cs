using GenbaSnap.Domain.Models;

namespace GenbaSnap.Persistence.Interfaces
{
    public interface ISnapGameRepository
    {
        /// <summary>
        /// Retrieve a game.
        /// </summary>
        /// <returns>
        /// Null if no game has been saved.
        /// </returns>
        public SnapGame? GetSnapGame();

        /// <summary>
        /// Save a game.
        /// </summary>
        /// <param name="game"></param>
        public void SaveSnapGame(SnapGame game);
    }
}
