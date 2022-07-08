using GenbaSnap.Domain.Models;
using GenbaSnap.Services.Interfaces;
using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Services.Internal
{
    internal class CardDeckFactory : ICardDeckFactory
    {
        private static Queue<Card> CreateEmptyDeck()
        {
            return new Queue<Card>();
        }

        public Queue<Card> CreateNewDeck()
        {
            var result = CreateEmptyDeck();

            foreach (var suit in (Suit[])Enum.GetValues(typeof(Suit)))
            {
                foreach (var value in (CardValue[])Enum.GetValues(typeof(CardValue)))
                {
                    result.Enqueue(new Card { Suit = suit, Value = value });
                }
            }

            return result;
        }

        public Queue<Card> CreateShuffledDeck()
        {
            var newDeck = CreateNewDeck().ToList();

            var random = new Random();
            var result = CreateEmptyDeck();

            while (newDeck.Any())
            {
                var index = random.Next(newDeck.Count);
                result.Enqueue(newDeck.ElementAt(index));
                newDeck.RemoveAt(index);
            }

            return result;
        }
    }
}
