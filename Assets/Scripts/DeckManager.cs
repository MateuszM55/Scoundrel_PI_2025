using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck;

    private void Awake()
    {
        deck = GenerateDeck("Normal");
    }

    public void InitializeDeck(string difficulty = "Normal")
    {
        // Regenerate the deck based on difficulty
        deck = GenerateDeck(difficulty);

        // Shuffle the deck
        System.Random rng = new System.Random();
        deck = deck.OrderBy(_ => rng.Next()).ToList();
    }

    public int GetRemainingCardsCount()
    {
        return deck.Count;
    }

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

public class Card
{
    public string Suit { get; set; }
    public int Rank { get; set; }
    public CardType Type { get; set; }
}

public enum CardType
{
    None,
    Monster,
    Weapon,
    HealingPotion
}
