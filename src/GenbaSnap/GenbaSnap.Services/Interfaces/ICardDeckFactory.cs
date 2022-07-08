using GenbaSnap.Domain.Models;

namespace GenbaSnap.Services.Interfaces
{
    public interface ICardDeckFactory
    {
        /// <summary>
        /// Create a new unshuffled deck of cards.
        /// </summary>
        /// <returns></returns>
        public Queue<Card> CreateNewDeck();

        /// <summary>
        /// Create a new shuffled deck of cards.
        /// </summary>
        /// <returns></returns>
        public Queue<Card> CreateShuffledDeck();

    }
}
