using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public DeckManager deckManager;

    // Add references to card buttons
    public List<Button> cardButtons;

    // Method to handle card cycling
    public void CycleCards()
    {
        // Check if only one card button is active
        int activeButtons = cardButtons.Count(button => button.gameObject.activeSelf);
        if (activeButtons == 1)
        {
            // Draw three new cards from the deck
            List<Card> newCards = deckManager.deck.Take(3).ToList();

            for (int i = 0; i < cardButtons.Count; i++)
            {
                if (i < newCards.Count)
                {
                    // Update button with new card
                    Card card = newCards[i];
                    Button button = cardButtons[i];
                    button.gameObject.SetActive(true);
                    button.GetComponentInChildren<Text>().text = $"{card.Rank}";
                    button.GetComponent<Image>().color = GetCardColor(card.Type);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCardClicked(card));
                }
                else
                {
                    // Hide extra buttons
                    cardButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // Method to get color based on card type
    private Color GetCardColor(CardType type)
    {
        switch (type)
        {
            case CardType.Monster:
                return Color.black;
            case CardType.HealingPotion:
                return Color.red;
            case CardType.Weapon:
                return Color.gray;
            default:
                return Color.white;
        }
    }

    // Modify OnCardClicked to handle button disappearance
    public void OnCardClicked(Card card)
    {
        // Find the button associated with the card and hide it
        Button clickedButton = cardButtons.FirstOrDefault(button => button.GetComponentInChildren<Text>().text == $"{card.Rank}");
        if (clickedButton != null)
        {
            clickedButton.gameObject.SetActive(false);
        }

        // Handle card interaction
        switch (card.Type)
        {
            case CardType.HealingPotion:
                gameManager.healthPoints += card.Rank;
                Debug.Log($"Healed for {card.Rank} HP. Current HP: {gameManager.healthPoints}");
                break;

            case CardType.Weapon:
                gameManager.weapon = new Weapon
                {
                    strength = card.Rank,
                    lastSlainMonster = 0
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

        // Check if card cycling is needed
        CycleCards();
    }

    // Placeholder for fight logic
    private void TriggerFight(Card monsterCard)
    {
        int monsterStrength = monsterCard.Rank;

        if (gameManager.weapon != null)
        {
            // Show pop-up with two buttons: 'Weapon' and 'Fists'
            ShowFightPopup(
                onWeaponSelected: () =>
                {
                    // Use weapon to fight
                    int damage = Mathf.Min(gameManager.weapon.strength, monsterStrength);
                    monsterStrength -= damage;
                    Debug.Log($"Used weapon! Monster strength reduced by {damage}. Remaining monster strength: {monsterStrength}");
                    if (monsterStrength > 0)
                    {
                        gameManager.healthPoints -= monsterStrength;
                        Debug.Log($"Monster attacked back! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
                    }
                },
                onFistsSelected: () =>
                {
                    // Fight with fists
                    gameManager.healthPoints -= monsterStrength;
                    Debug.Log($"Fought with fists! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
                }
            );
        }
        else
        {
            // No weapon equipped, fight with fists
            gameManager.healthPoints -= monsterStrength;
            Debug.Log($"No weapon equipped! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
        }
    }

    private void ShowFightPopup(System.Action onWeaponSelected, System.Action onFistsSelected)
    {
        // Placeholder for pop-up UI logic
        Debug.Log("Displaying fight pop-up with options: Weapon or Fists.");

        // Simulate button clicks for demonstration purposes
        // Replace this with actual UI button logic
        bool weaponButtonClicked = true; // Simulate weapon button click
        if (weaponButtonClicked)
        {
            onWeaponSelected?.Invoke();
        }
        else
        {
            onFistsSelected?.Invoke();
        }
    }
}
