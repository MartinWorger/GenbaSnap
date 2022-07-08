using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Domain.Models
{
    /// <summary>
    /// A single playing card of specified <see cref="Enums.Suit"/> and <see cref="CardValue"/>
    /// </summary>
    public class Card
    {
        public Suit Suit { get; set; }
        public CardValue Value { get; set; }
    }
}
