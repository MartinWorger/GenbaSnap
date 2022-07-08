using GenbaSnap.Domain.Models;
using GenbaSnap.Persistence.Interfaces;
using GenbaSnap.Services.Interfaces;
using GenbaSnap.Services.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Services.Tests
{
    [TestClass]
    public class SnapGameServiceTests
    {
        private ILogger<SnapGameService>? logger;
        private Mock<ISnapGameFactory>? mockSnapGameFactory;
        private Mock<ISnapGameRepository>? mockSnapGameRepository;

        private SnapGameService NewSnapGameService =>
            new SnapGameService(logger!, mockSnapGameFactory!.Object, mockSnapGameRepository!.Object);

        private void SetupFactoryCreateResult(SnapGame snapGame) =>
            mockSnapGameFactory!
                .Setup(x => x.CreateSnapGame(It.IsAny<GameType>(), It.IsAny<int>()))
                .Returns(snapGame);

        private void SetupRepositoryGetResult(SnapGame? snapGame) =>
            mockSnapGameRepository!
                .Setup(x => x.GetSnapGame())
                .Returns(snapGame);

        [TestInitialize]
        public void Initialise()
        {
            logger = new NullLogger<SnapGameService>();
            mockSnapGameFactory = new Mock<ISnapGameFactory>();
            mockSnapGameRepository = new Mock<ISnapGameRepository>();
        }

        [TestMethod]
        public void Ctor_WithParamNull_Logger_Throws()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = new SnapGameService(null, mockSnapGameFactory!.Object, mockSnapGameRepository!.Object);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }

        [TestMethod]
        public void Ctor_WithParamNull_SnapGameFactory_Throws()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = new SnapGameService(logger!, null, mockSnapGameRepository!.Object);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }

        [TestMethod]
        public void Ctor_WithParamNull_SnapGameRepository_Throws()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = new SnapGameService(logger!, mockSnapGameFactory!.Object, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }

        [TestMethod]
        public void Ctor_CorrectParams_ReturnsService()
        {
            // Act
            var service = NewSnapGameService;

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void IsSnap_WhenNoCurrentGame_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;

            // Act
            var result = service.IsSnap();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSnap_WhenCurrentGameNotInPlayingState_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Initialising
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.IsSnap();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSnap_WhenNoCardsFaceUp_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Playing, 
                PlayerGameStates = new List<PlayerGameState>
                { 
                    new PlayerGameState(),
                    new PlayerGameState()
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.IsSnap();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSnap_WhenCardsFaceUp_ButNoMatches_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { FaceUpStack = testDeck1 },
                    new PlayerGameState { FaceUpStack = testDeck2 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.IsSnap();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSnap_WhenCardsFaceUp_AndMatches_ReturnsTrue()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Ace });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { FaceUpStack = testDeck1 },
                    new PlayerGameState { FaceUpStack = testDeck2 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.IsSnap();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CreateGame_CallsFactory_SavesGame_ReturnsGameState()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Playing
            };
            SetupFactoryCreateResult(testGame);

            // Act
            var result = service.CreateGame(2);

            // Assert
            Assert.AreEqual(GameState.Playing, result);
            mockSnapGameFactory!
                .Verify(x => x.CreateSnapGame(It.IsAny<GameType>(), It.IsAny<int>())
                , Times.Once);
            mockSnapGameRepository!
                .Verify(x => x.SaveSnapGame(It.IsAny<SnapGame>())
                , Times.Once);
        }

        [TestMethod]
        public void GetGameState_WhenNoCurrentGame_ReturnsNull()
        {
            // Arrange
            var service = NewSnapGameService;

            // Act
            var result = service.GetGameState();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetGameState_WhenCurrentGame_ReturnsGameState()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Playing
            };
            SetupFactoryCreateResult(testGame);
            service.CreateGame(2);

            // Act
            var result = service.GetGameState();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(GameState.Playing, result);
        }

        [TestMethod]
        public void GetPlayerTurn_WhenNoCurrentGame_ReturnsNull()
        {
            // Arrange
            var service = NewSnapGameService;

            // Act
            var result = service.GetPlayerTurn();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetPlayerTurn_WhenCurrentGame_ReturnsPlayerTurn()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1
            };
            SetupFactoryCreateResult(testGame);
            service.CreateGame(2);

            // Act
            var result = service.GetPlayerTurn();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CallSnap_WhenNoCurrentGame_ReturnsGameNotValid()
        {
            // Arrange
            var service = NewSnapGameService;

            // Act
            var result = service.CallSnap(1);

            // Assert
            Assert.AreEqual(Enums.SnapCallResult.GameNotValid, result);
        }

        [TestMethod]
        public void CallSnap_WithInvalidPlayerNumber_ReturnsInvalidPlayer()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.CallSnap(99);

            // Assert
            Assert.AreEqual(Enums.SnapCallResult.InvalidPlayer, result);
        }

        [TestMethod]
        public void CallSnap_WhenNotSnap_ReturnsBadCall()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.CallSnap(1);

            // Assert
            Assert.AreEqual(Enums.SnapCallResult.BadCall, result);
        }

        [TestMethod]
        public void CallSnap_WhenSnap_ReturnsWon()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Ace });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.CallSnap(1);

            // Assert
            Assert.AreEqual(Enums.SnapCallResult.Won, result);
        }

        [TestMethod]
        public void PlayCard_WhenNoCurrentGame_ReturnsInvalidGameState()
        {
            // Arrange
            var service = NewSnapGameService;

            // Act
            var result = service.PlayCard(1);

            // Assert
            Assert.AreEqual(Enums.PlayCardResult.InvalidGameState, result);
        }

        [TestMethod]
        public void PlayCard_WithInvalidPlayerNumber_ReturnsInvalidPlayer()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.PlayCard(99);

            // Assert
            Assert.AreEqual(Enums.PlayCardResult.InvalidPlayer, result);
        }

        [TestMethod]
        public void PlayCard_WhenGameNotPlaying_ReturnsInvalidGameState()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Finished,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.PlayCard(1);

            // Assert
            Assert.AreEqual(Enums.PlayCardResult.InvalidGameState, result);
        }

        [TestMethod]
        public void PlayCard_WhenOutOfTurn_ReturnsOutOfTurn()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.PlayCard(2);

            // Assert
            Assert.AreEqual(Enums.PlayCardResult.OutOfTurn, result);
        }

        [TestMethod]
        public void PlayCard_WhenPlayerStateMissing_ReturnsInvalidPlayer()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 3, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.PlayCard(1);

            // Assert
            Assert.AreEqual(Enums.PlayCardResult.InvalidPlayer, result);
        }

        [TestMethod]
        public void PlayCard_WhenPlayerHasNoFaceDownCards_ReturnsInvalidPlayer()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            var testDeck4 = new Stack<Card>();
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.PlayCard(1);

            // Assert
            Assert.AreEqual(Enums.PlayCardResult.NoCards, result);
        }

        [TestMethod]
        public void PlayCard_WhenPlayerHasFaceDownCards_PlaysCard_ReturnsSuccess()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);

            // Act
            var result = service.PlayCard(1);

            // Assert
            Assert.AreEqual(Enums.PlayCardResult.Success, result);
            Assert.IsTrue(testGame.PlayerGameStates.First(x => x.PlayerNumber == 1).FaceUpStack.Any());
            Assert.IsFalse(testGame.PlayerGameStates.First(x => x.PlayerNumber == 1).FaceDownStack.Any());
        }

        [TestMethod]
        public void PlayerNumberValid_WhenPlayerNumberLessThanOne_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;

            // Act
            var result = service.PlayerNumberValid(0);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PlayerNumberValid_WhenGameNull_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;

            // Act
            var result = service.PlayerNumberValid(1);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PlayerNumberValid_WhenPlayerNumberGreaterThanPlayers_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1 },
                    new PlayerGameState { PlayerNumber = 2 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            var result = service.PlayerNumberValid(3);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PlayerNumberValid_WhenValidPlayerNumber_ReturnsTrue()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            var result = service.PlayerNumberValid(2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TryGetPlayerGameState_WhenInvalidPlayerNumber_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1 },
                    new PlayerGameState { PlayerNumber = 2 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            var result = service.TryGetPlayerGameState(99, out var _);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryGetPlayerGameState_WhenNoPlayerGameState_ReturnsFalse()
        {
            // Arrange
            var service = NewSnapGameService;
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 3 },
                    new PlayerGameState { PlayerNumber = 2 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            var result = service.TryGetPlayerGameState(1, out var _);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryGetPlayerGameState_WhenValidPlayerNumber_ReturnsTrue()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            var result = service.TryGetPlayerGameState(1, out var playerGameState);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(playerGameState);
            Assert.AreEqual(1, playerGameState.PlayerNumber);
        }

        [TestMethod]
        public void SetNextPlayer_WhenNoPayableCards_DoesNothing()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            var testDeck4 = new Stack<Card>();
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            service.SetNextPlayer();
            var nextPlayer = service.GetPlayerTurn();

            // Assert
            Assert.AreEqual(1, nextPlayer);
        }

        [TestMethod]
        public void SetNextPlayer_WhenPayableCards_NoWrap_ChoosesNextPlayer()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            var testDeck2 = new Stack<Card>();
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            service.SetNextPlayer();
            var nextPlayer = service.GetPlayerTurn();

            // Assert
            Assert.AreEqual(2, nextPlayer);
        }

        [TestMethod]
        public void SetNextPlayer_WhenPayableCards_WithWrap_ChoosesNextPlayer()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            var testDeck2 = new Stack<Card>();
            var testDeck3 = new Stack<Card>();
            testDeck3.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Queen });
            var testDeck4 = new Stack<Card>();
            testDeck4.Push(new Card { Suit = Suit.Hearts, Value = CardValue.King });
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 2,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            service.SetNextPlayer();
            var nextPlayer = service.GetPlayerTurn();

            // Assert
            Assert.AreEqual(1, nextPlayer);
        }

        [TestMethod]
        public void TakeSnapCards_WhenNoSnapCards_DoesNothing()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Two });
            var testDeck3 = new Stack<Card>();
            var testDeck4 = new Stack<Card>();
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            service.TakeSnapCards(1);

            // Assert
            Assert.AreEqual(0, testGame.PlayerGameStates.First(x => x.PlayerNumber == 1).FaceDownStack.Count);

        }

        [TestMethod]
        public void TakeSnapCards_WhenSnapCards_TakesCards()
        {
            // Arrange
            var service = NewSnapGameService;
            var testDeck1 = new Stack<Card>();
            testDeck1.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Ace });
            var testDeck2 = new Stack<Card>();
            testDeck2.Push(new Card { Suit = Suit.Hearts, Value = CardValue.Ace });
            var testDeck3 = new Stack<Card>();
            var testDeck4 = new Stack<Card>();
            var testDeck5 = new Stack<Card>();
            testDeck5.Push(new Card { Suit = Suit.Clubs, Value = CardValue.Two });
            var testDeck6 = new Stack<Card>();
            var testGame = new SnapGame
            {
                GameState = GameState.Playing,
                PlayerTurn = 1,
                PlayerGameStates = new List<PlayerGameState>
                {
                    new PlayerGameState { PlayerNumber = 1, FaceUpStack = testDeck1, FaceDownStack = testDeck3 },
                    new PlayerGameState { PlayerNumber = 2, FaceUpStack = testDeck2, FaceDownStack = testDeck4 },
                    new PlayerGameState { PlayerNumber = 3, FaceUpStack = testDeck5, FaceDownStack = testDeck6 },
                    new PlayerGameState { PlayerNumber = 4 }
                }
            };
            SetupRepositoryGetResult(testGame);
            service.FetchGame();

            // Act
            service.TakeSnapCards(1);

            // Assert
            Assert.AreEqual(2, testGame.PlayerGameStates.First(x => x.PlayerNumber == 1).FaceDownStack.Count);

        }
    }
}
