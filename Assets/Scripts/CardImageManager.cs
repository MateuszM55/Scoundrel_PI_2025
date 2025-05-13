using UnityEngine;

/// <summary>
/// Manages the assignment of images to cards based on their type and rank.
/// </summary>
public class CardImageManager : MonoBehaviour
{
    // Add references to card sprites
    public Sprite weakPotionSprite;
    public Sprite normalPotionSprite;
    public Sprite powerfulPotionSprite;

    public Sprite weakWeaponSprite;
    public Sprite normalWeaponSprite;
    public Sprite powerfulWeaponSprite;

    public Sprite weakMonsterSprite;
    public Sprite normalMonsterSprite;
    public Sprite powerfulMonsterSprite;

    public Sprite specialMonsterSprite11;
    public Sprite specialMonsterSprite12;
    public Sprite specialMonsterSprite13;
    public Sprite specialMonsterSprite14;

    // Add suit sprites
    public Sprite clubsSuitSprite;
    public Sprite spadesSuitSprite;
    public Sprite diamondsSuitSprite;
    public Sprite heartsSuitSprite;

    /// <summary>
    /// Retrieves the appropriate sprite for a given card.
    /// </summary>
    /// <param name="card">The card to get the sprite for.</param>
    /// <returns>The sprite associated with the card.</returns>
    public Sprite GetCardSprite(Card card)
    {
        switch (card.Type)
        {
            case CardType.HealingPotion:
                return GetPotionSprite(card.Rank);
            case CardType.Weapon:
                return GetWeaponSprite(card.Rank);
            case CardType.Monster:
                return GetMonsterSprite(card.Rank);
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the suit sprite for a given card.
    /// </summary>
    public Sprite GetSuitSprite(Card card)
    {
        switch (card.Suit)
        {
            case "Clubs": return clubsSuitSprite;
            case "Spades": return spadesSuitSprite;
            case "Diamonds": return diamondsSuitSprite;
            case "Hearts": return heartsSuitSprite;
            default: return null;
        }
    }

    private Sprite GetPotionSprite(int rank)
    {
        if (rank <= 4) return weakPotionSprite;
        if (rank >= 8) return powerfulPotionSprite;
        return normalPotionSprite;
    }

    private Sprite GetWeaponSprite(int rank)
    {
        if (rank <= 4) return weakWeaponSprite;
        if (rank >= 8) return powerfulWeaponSprite;
        return normalWeaponSprite;
    }

    private Sprite GetMonsterSprite(int rank)
    {
        if (rank <= 4) return weakMonsterSprite;
        if (rank >= 8 && rank <= 10) return powerfulMonsterSprite;
        if (rank == 11) return specialMonsterSprite11;
        if (rank == 12) return specialMonsterSprite12;
        if (rank == 13) return specialMonsterSprite13;
        if (rank == 14) return specialMonsterSprite14;
        return normalMonsterSprite;
    }
}
