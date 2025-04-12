using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck;

    private void Start()
    {
        deck = GenerateDeck();
    }

    private List<Card> GenerateDeck()
    {
        List<Card> newDeck = new List<Card>();

        string[] suits = { "Clubs", "Spades", "Diamonds", "Hearts" };
        for (int rank = 2; rank <= 14; rank++) // 2 to Ace (14)
        {
            foreach (string suit in suits)
            {
                // Skip red face cards and red aces
                if ((suit == "Diamonds" || suit == "Hearts") && (rank > 10 || rank == 14))
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
