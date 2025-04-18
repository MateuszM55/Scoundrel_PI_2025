using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the deck of cards, including generation, shuffling, and drawing.
/// </summary>
public class DeckManager : MonoBehaviour
{
    /// <summary>
    /// The current deck of cards.
    /// </summary>
    public List<Card> deck;

    /// <summary>
    /// Initializes the deck when the object is awakened.
    /// </summary>
    private void Awake()
    {
        deck = GenerateDeck("Normal");
    }

    /// <summary>
    /// Initializes the deck based on the specified difficulty and shuffles it.
    /// </summary>
    /// <param name="difficulty">The difficulty level to use for deck generation.</param>
    public void InitializeDeck(string difficulty = "Normal")
    {
        // Regenerate the deck based on difficulty
        deck = GenerateDeck(difficulty);

        // Shuffle the deck
        System.Random rng = new System.Random();
        deck = deck.OrderBy(_ => rng.Next()).ToList();
    }

    /// <summary>
    /// Gets the number of remaining cards in the deck.
    /// </summary>
    /// <returns>The number of cards left in the deck.</returns>
    public int GetRemainingCardsCount()
    {
        return deck.Count;
    }

    /// <summary>
    /// Draws a specified number of cards from the top of the deck.
    /// </summary>
    /// <param name="count">The number of cards to draw.</param>
    public void DrawCards(int count)
    {
        if (deck.Count >= count)
        {
            deck.RemoveRange(0, count);
        }
        else
        {
            deck.Clear();
        }
    }

    /// <summary>
    /// Generates a new deck of cards based on the specified difficulty.
    /// </summary>
    /// <param name="difficulty">The difficulty level to use for deck generation.</param>
    /// <returns>A list of cards representing the generated deck.</returns>
    private List<Card> GenerateDeck(string difficulty)
    {
        List<Card> newDeck = new List<Card>();

        string[] suits = { "Clubs", "Spades", "Diamonds", "Hearts" };
        for (int rank = 2; rank <= 14; rank++) // 2 to Ace (14)
        {
            foreach (string suit in suits)
            {
                // Apply difficulty-specific filters
                if (difficulty == "Easy" && suit == "Spades" && rank == 14) // Exclude black aces
                    continue;

                if (difficulty == "Hard" && suit == "Hearts" && rank == 10) // Exclude healing potions with value 10
                    continue;

                CardType type = DetermineCardType(suit);
                newDeck.Add(new Card { Suit = suit, Rank = rank, Type = type });
            }
        }

        return newDeck;
    }

    /// <summary>
    /// Determines the type of a card based on its suit.
    /// </summary>
    /// <param name="suit">The suit of the card.</param>
    /// <returns>The type of the card.</returns>
    private CardType DetermineCardType(string suit)
    {
        return suit switch
        {
            "Clubs" or "Spades" => CardType.Monster,
            "Diamonds" => CardType.Weapon,
            "Hearts" => CardType.HealingPotion,
            _ => CardType.None
        };
    }
}

/// <summary>
/// Represents a card in the deck, including its suit, rank, and type.
/// </summary>
public class Card
{
    /// <summary>
    /// The suit of the card (e.g., Clubs, Spades, Diamonds, Hearts).
    /// </summary>
    public string Suit { get; set; }

    /// <summary>
    /// The rank of the card (e.g., 2 to Ace).
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// The type of the card (e.g., Monster, Weapon, HealingPotion).
    /// </summary>
    public CardType Type { get; set; }
}

/// <summary>
/// Represents the type of a card in the deck.
/// </summary>
public enum CardType
{
    /// <summary>
    /// No specific type.
    /// </summary>
    None,

    /// <summary>
    /// A monster card.
    /// </summary>
    Monster,

    /// <summary>
    /// A weapon card.
    /// </summary>
    Weapon,

    /// <summary>
    /// A healing potion card.
    /// </summary>
    HealingPotion
}
