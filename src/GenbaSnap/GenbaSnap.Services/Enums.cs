namespace GenbaSnap.Services
{
    public static class Enums
    {
        public enum SnapCallResult
        {
            /// <summary>
            /// Snap was called successfully and won
            /// </summary>
            Won,
            /// <summary>
            /// Snap was called but player was beaten to it
            /// </summary>
            Lost,
            /// <summary>
            /// Snap was called but wasn't a snap
            /// </summary>
            BadCall,
            /// <summary>
            /// Invalid player number in snap call
            /// </summary>
            InvalidPlayer,
            /// <summary>
            /// No current game or not in a valid playing state
            /// </summary>
            GameNotValid
        }

        public enum PlayCardResult
        {
            /// <summary>
            /// Card was played successfully
            /// </summary>
            Success,
            /// <summary>
            /// It is not the turn of the player
            /// </summary>
            OutOfTurn,
            /// <summary>
            /// The player has no cards remaining
            /// </summary>
            NoCards,
            /// <summary>
            /// No current game or not in a valid playing state
            /// </summary>
            InvalidGameState,
            /// <summary>
            /// Invalid player number trying to play a card
            /// </summary>
            InvalidPlayer,
        }
    }
}
