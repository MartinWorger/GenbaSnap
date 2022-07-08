namespace GenbaSnap.Domain.Models
{
    public class PlayerGameState
    {
        public int PlayerNumber { get; set; }
        public Stack<Card> FaceDownStack { get; set; } = new Stack<Card>();
        public Stack<Card> FaceUpStack { get; init; } = new Stack<Card>();
    }
}
