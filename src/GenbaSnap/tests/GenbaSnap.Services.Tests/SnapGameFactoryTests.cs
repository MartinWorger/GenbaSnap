using GenbaSnap.Domain.Models;
using GenbaSnap.Services.Interfaces;
using GenbaSnap.Services.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Services.Tests
{
    [TestClass]
    public class SnapGameFactoryTests
    {
        private ILogger<SnapGameFactory>? logger;
        private Mock<ICardDeckFactory>? mockCardDeckFactory;

        private SnapGameFactory NewSnapGameFactory =>
            new SnapGameFactory(logger!, mockCardDeckFactory!.Object);

        [TestInitialize]
        public void Initialise()
        {
            logger = new NullLogger<SnapGameFactory>();
            mockCardDeckFactory = new Mock<ICardDeckFactory>();
        }

        [TestMethod]
        public void Ctor_WithParamNull_Logger_Throws()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = new SnapGameFactory(null, mockCardDeckFactory!.Object);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }

        [TestMethod]
        public void Ctor_WithParamNull_CardDeckFactory_Throws()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = new SnapGameFactory(logger!, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }

        [TestMethod]
        public void Ctor_CorrectParams_ReturnsFactory()
        {
            // Act
            var factory = NewSnapGameFactory;

            // Assert
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void DealCards_WithTwoPlayers_AndEvenDeal_DealsEvenly()
        {
            // Arrange
            var factory = NewSnapGameFactory;
            var players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1 },
                new PlayerGameState { PlayerNumber = 2 }
            };
            var testDeck = new Queue<Card>();
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Two });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Three });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Four });

            mockCardDeckFactory!
                .Setup(x => x.CreateShuffledDeck())
                .Returns(testDeck);

            // Act
            factory.DealCards(players);

            // Assert
            Assert.AreEqual(2, players[0].FaceDownStack.Count);
            Assert.AreEqual(2, players[1].FaceDownStack.Count);
        }

        [TestMethod]
        public void DealCards_WithThreePlayers_AndEvenDeal_DealsEvenly()
        {
            // Arrange
            var factory = NewSnapGameFactory;
            var players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1 },
                new PlayerGameState { PlayerNumber = 2 },
                new PlayerGameState { PlayerNumber = 3 }
            };
            var testDeck = new Queue<Card>();
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Two });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Three });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Four });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Five });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Six });

            mockCardDeckFactory!
                .Setup(x => x.CreateShuffledDeck())
                .Returns(testDeck);

            // Act
            factory.DealCards(players);

            // Assert
            Assert.AreEqual(2, players[0].FaceDownStack.Count);
            Assert.AreEqual(2, players[1].FaceDownStack.Count);
            Assert.AreEqual(2, players[2].FaceDownStack.Count);
        }

        [TestMethod]
        public void DealCards_WithThreePlayers_AndUnevenDeal_DealsUnevenly()
        {
            // Arrange
            var factory = NewSnapGameFactory;
            var players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1 },
                new PlayerGameState { PlayerNumber = 2 },
                new PlayerGameState { PlayerNumber = 3 }
            };
            var testDeck = new Queue<Card>();
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Two });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Three });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Four });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Five });

            mockCardDeckFactory!
                .Setup(x => x.CreateShuffledDeck())
                .Returns(testDeck);

            // Act
            factory.DealCards(players);

            // Assert
            Assert.AreEqual(2, players[0].FaceDownStack.Count);
            Assert.AreEqual(2, players[1].FaceDownStack.Count);
            Assert.AreEqual(1, players[2].FaceDownStack.Count);
        }

        [TestMethod]
        public void InitialisePlayerGameStates_WithTwoPlayers_ReturnsPlayerGameStates()
        {
            // Arrange
            var factory = NewSnapGameFactory;

            // Act
            var result = factory.InitialisePlayerGameStates(2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void InitialisePlayerGameStates_WithThreePlayers_ReturnsPlayerGameStates()
        {
            // Arrange
            var factory = NewSnapGameFactory;

            // Act
            var result = factory.InitialisePlayerGameStates(3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void CreateSnapGame_WithTooFewPlayers_Throws()
        {
            // Arrange
            var factory = NewSnapGameFactory;

            // Act
            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _ = factory.CreateSnapGame(GameType.SimpleSnap, 1);
            });

            // Assert
            Assert.IsTrue(exception.Message.StartsWith("Must have at least 2 players in a game"));
        }

        [TestMethod]
        public void CreateSnapGame_WithTwoPlayers_CreatesWellFormedGame()
        {
            // Arrange
            var factory = NewSnapGameFactory;
            var testDeck = new Queue<Card>();
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Two });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Three });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Four });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Five });
            testDeck.Enqueue(new Card { Suit = Suit.Clubs, Value = CardValue.Six });

            mockCardDeckFactory!
                .Setup(x => x.CreateShuffledDeck())
                .Returns(testDeck);

            // Act
            var result = factory.CreateSnapGame(GameType.SimpleSnap, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(GameType.SimpleSnap, result.GameType);
            Assert.AreEqual(2, result.PlayerGameStates.Count());
            Assert.AreEqual(GameState.Playing, result.GameState);

            var playerList = result.PlayerGameStates.ToList();
            Assert.AreEqual(1, playerList[0].PlayerNumber);
            Assert.AreEqual(3, playerList[0].FaceDownStack.Count);
            Assert.AreEqual(2, playerList[1].PlayerNumber);
            Assert.AreEqual(3, playerList[1].FaceDownStack.Count);
        }

    }
}
