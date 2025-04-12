using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public DeckManager deckManager;

    // Method to handle card interaction
    public void OnCardClicked(Card card)
    {
        switch (card.Type)
        {
            case CardType.HealingPotion:
                gameManager.healthPoints += card.Rank; // Add card rank to health points
                Debug.Log($"Healed for {card.Rank} HP. Current HP: {gameManager.healthPoints}");
                break;

            case CardType.Weapon:
                gameManager.weapon = new Weapon
                {
                    strength = card.Rank,
                    lastSlainMonster = 0 // Initialize lastSlainMonster to default value
                };
                Debug.Log($"Equipped weapon with Strength: {card.Rank}, Last Slain Monster: {gameManager.weapon.lastSlainMonster}");
                break;

            case CardType.Monster:
                TriggerFight(card);
                break;

            default:
                Debug.Log("Unknown card type.");
                break;
        }
    }

    // Placeholder for fight logic
    private void TriggerFight(Card monsterCard)
    {
        Debug.Log($"Encountered a monster with rank {monsterCard.Rank}. Fight logic goes here.");
    }
}
