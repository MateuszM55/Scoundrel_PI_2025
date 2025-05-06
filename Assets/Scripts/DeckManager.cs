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
    /// Tracks the number of healing potions used in the current set of 4 cards.
    /// </summary>
    public int healingPotionCounter = 0;

    // Add a method to log the current value of the healingPotionCounter
    public void LogHealingPotionCounter()
    {
        Debug.Log($"[DeckManager] Current healingPotionCounter: {healingPotionCounter}");
    }

    /// <summary>
    /// Initializes the deck when the object is awakened.
    /// </summary>
    private void Awake()
    {
        deck = GenerateDeck("Normal");

        // Shuffle the deck after generation
        ShuffleDeck();

        // Debug: Print the entire deck
        PrintDeck();
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
        ShuffleDeck();

        // Debug: Print the entire deck
        PrintDeck();
    }

    /// <summary>
    /// Shuffles the current deck.
    /// </summary>
    private void ShuffleDeck()
    {
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
    /// Removes a specific card from the deck.
    /// </summary>
    /// <param name="card">The card to remove.</param>
    public void RemoveCardFromDeck(Card card)
    {
        if (deck.Contains(card))
        {
            deck.Remove(card);
            Debug.Log($"Card removed: {card.Rank} of {card.Suit}");

            // Play sound effect based on card type
            if (AudioManager.Instance != null && card.Type != CardType.Monster)
            {
                AudioManager.Instance.PlayCardTypeSound(card.Type);
            }
        }
        else
        {
            Debug.LogWarning("Attempted to remove a card that is not in the deck.");
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
                // Exclude diamonds with rank 11 or higher
                if (suit == "Diamonds" && rank >= 11)
                    continue;

                // Exclude jokers (not represented in this range), red face cards, and red aces
                if ((suit == "Diamonds" || suit == "Hearts") && (rank >= 11 || rank == 14)) // Red face cards and red aces
                    continue;

                // Apply difficulty-specific filters
                if (difficulty == "Easy" && (suit == "Spades" || suit == "Clubs") && rank == 14) // Exclude black aces for Easy
                    continue;

                if (difficulty == "Hard" && suit == "Hearts" && rank == 10) // Exclude healing potions with value 10 for Hard
                    continue;

                CardType type = DetermineCardType(suit);
                newDeck.Add(new Card { Suit = suit, Rank = rank, Type = type });
            }
        }

        // Ensure no duplicate cards and consistent properties
        return newDeck.Distinct().ToList();
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

    /// <summary>
    /// Prints the entire deck to the console for debugging purposes.
    /// </summary>
    private void PrintDeck()
    {
        Debug.Log("Current Deck:");
        foreach (var card in deck)
        {
            Debug.Log($"Card: {card.Rank} of {card.Suit}, Type: {card.Type}");
        }
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
