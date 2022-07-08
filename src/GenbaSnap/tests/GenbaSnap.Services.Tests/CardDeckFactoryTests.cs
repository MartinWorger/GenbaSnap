using GenbaSnap.Services.Internal;
using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Services.Tests
{
    [TestClass]
    public class CardDeckFactoryTests
    {
        [TestMethod]
        public void CreateNewDeck_Creates52Cards()
        {
            // Arrange
            var service = new CardDeckFactory();

            // Act
            var result = service.CreateNewDeck();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(52, result.Count);
        }

        [DataTestMethod]
        [DataRow(Suit.Clubs, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)]
        [DataRow(Suit.Diamonds, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25)]
        [DataRow(Suit.Hearts, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38)]
        [DataRow(Suit.Spades, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51)]
        public void CreateNewDeck_CreatesCardsInFactoryOrder(Suit expectedSuit, params int[] expectedIndexes)
        {
            // Arrange
            var service = new CardDeckFactory();

            // Act
            var result = service.CreateNewDeck();

            // Assert
            Assert.IsNotNull(result);
            foreach (var index in expectedIndexes)
            {
                Assert.AreEqual(expectedSuit, result.ElementAt(index).Suit);
                Assert.AreEqual((CardValue)(index % 13) + 1, result.ElementAt(index).Value);
            }
        }

        [TestMethod]
        public void CreateShuffledDeck_Creates52Cards()
        {
            // Arrange
            var service = new CardDeckFactory();

            // Act
            var result = service.CreateShuffledDeck();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(52, result.Count);
        }

        //We can't add a check that cards are shuffled, as using Random means results are indeterminate,
        //and could theoretically end up in factory order following a shuffle (albeit highly unlikely!)
        //Therefore a one-off visual check is sufficient via a break point in the above test

    }
}
