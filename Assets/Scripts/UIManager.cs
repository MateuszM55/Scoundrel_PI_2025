using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro; // Add this for TextMeshPro support

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public DeckManager deckManager;

    // Add references to card buttons
    public List<Button> cardButtons;

    // Replace Text with TextMeshProUGUI for displaying stats
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI weaponStrengthText;
    public TextMeshProUGUI lastSlainMonsterText;

    private bool canRun = true; // Flag to track if running is allowed

    // Add a reference to the GameObject containing the fight popup buttons
    public GameObject fightPopup;

    // Add references for weapon and fists buttons
    public Button weaponButton;
    public Button fistsButton;

    // Add a reference for the Run button
    public Button runButton;

    private void Start()
    {
        // Shuffle the deck before initializing buttons
        ShuffleDeck();

        // Initialize UI with current values
        UpdateStatsDisplay();

        // Initialize card buttons
        InitializeCardButtons();
    }

    // Method to shuffle the deck
    private void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        deckManager.deck = deckManager.deck.OrderBy(_ => rng.Next()).ToList();
    }

    // Method to initialize card buttons with color and rank
    private void InitializeCardButtons()
    {
        // Draw initial cards from the shuffled deck
        List<Card> initialCards = deckManager.deck.Take(cardButtons.Count).ToList();

        for (int i = 0; i < cardButtons.Count; i++)
        {
            if (i < initialCards.Count)
            {
                // Assign card to button
                Card card = initialCards[i];
                Button button = cardButtons[i];
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TextMeshProUGUI>().text = card.Rank.ToString(); // Display rank
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

    // Method to update the displayed stats
    private void UpdateStatsDisplay()
    {
        healthText.text = $"HP: {gameManager.healthPoints}";
        weaponStrengthText.text = gameManager.weapon != null
            ? $"Weapon Strength: {gameManager.weapon.strength}"
            : "Weapon Strength: None";
        lastSlainMonsterText.text = gameManager.weapon != null
            ? $"Last Slain Monster: {gameManager.weapon.lastSlainMonster}"
            : "Last Slain Monster: None";
    }

    // Modify OnCardClicked to enforce a health cap
    public void OnCardClicked(Card card)
    {
        // Find the button associated with the card and hide it
        Button clickedButton = cardButtons.FirstOrDefault(button => button.GetComponentInChildren<TextMeshProUGUI>().text == card.Rank.ToString());
        if (clickedButton != null)
        {
            clickedButton.gameObject.SetActive(false);
        }

        // Handle card interaction
        switch (card.Type)
        {
            case CardType.HealingPotion:
                gameManager.healthPoints += card.Rank;
                if (gameManager.healthPoints > 20) // Enforce health cap
                {
                    gameManager.healthPoints = 20;
                }
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

        // Update stats display
        UpdateStatsDisplay();

        // Check if card cycling is needed
        CycleCards();

        // Re-enable running
        canRun = true;
    }

    // Modify CycleCards to shuffle and draw four random cards
    public void CycleCards()
    {
        // Check if only one card button is active
        int activeButtons = cardButtons.Count(button => button.gameObject.activeSelf);
        if (activeButtons == 1)
        {
            // Shuffle the deck before drawing new cards
            ShuffleDeck();

            // Draw four new cards from the shuffled deck
            List<Card> newCards = deckManager.deck.Take(4).ToList();

            for (int i = 0; i < cardButtons.Count; i++)
            {
                if (i < newCards.Count)
                {
                    // Update button with new card
                    Card card = newCards[i];
                    Button button = cardButtons[i];
                    button.gameObject.SetActive(true);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = card.Rank.ToString(); // Display rank
                    button.GetComponent<Image>().color = GetCardColor(card.Type);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCardClicked(card));
                }
                else
                {
                    // Hide extra buttons if fewer than 4 cards are available
                    cardButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // Method to handle running
    public void Run()
    {
        if (!canRun)
        {
            Debug.Log("You cannot run twice in a row!");
            return;
        }

        // Move all current cards to the bottom of the deck
        foreach (Button button in cardButtons)
        {
            if (button.gameObject.activeSelf)
            {
                int cardRank = int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text);
                Card card = deckManager.deck.FirstOrDefault(c => c.Rank == cardRank);
                if (card != null)
                {
                    deckManager.deck.Remove(card);
                    deckManager.deck.Add(card); // Add to the bottom of the deck
                }
            }
        }

        // Shuffle the deck and draw a new set of cards
        ShuffleDeck();
        InitializeCardButtons();

        // Disable running until another action is taken
        canRun = false;
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

    // Modify TriggerFight to handle the described behavior
    private void TriggerFight(Card monsterCard)
    {
        int monsterStrength = monsterCard.Rank;

        // Check if the player has a weapon with non-zero strength
        if (gameManager.weapon != null && gameManager.weapon.strength > 0)
        {
            // Disable card buttons and the Run button
            foreach (var button in cardButtons)
            {
                button.interactable = false;
            }
            runButton.interactable = false;

            // Show the fight popup
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

                    // Re-enable card buttons and the Run button
                    foreach (var button in cardButtons)
                    {
                        button.interactable = true;
                    }
                    runButton.interactable = true;

                    // Update stats display
                    UpdateStatsDisplay();
                },
                onFistsSelected: () =>
                {
                    // Fight with fists
                    gameManager.healthPoints -= monsterStrength;
                    Debug.Log($"Fought with fists! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");

                    // Re-enable card buttons and the Run button
                    foreach (var button in cardButtons)
                    {
                        button.interactable = true;
                    }
                    runButton.interactable = true;

                    // Update stats display
                    UpdateStatsDisplay();
                }
            );
        }
        else
        {
            // No weapon equipped or weapon strength is zero, fight with fists
            gameManager.healthPoints -= monsterStrength;
            Debug.Log($"No weapon equipped or weapon strength is zero! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");

            // Update stats display
            UpdateStatsDisplay();
        }
    }

    // Modify ShowFightPopup to use editor-assigned buttons
    private void ShowFightPopup(System.Action onWeaponSelected, System.Action onFistsSelected)
    {
        // Enable the fight popup GameObject
        fightPopup.SetActive(true);

        // Assign actions to the buttons
        weaponButton.onClick.RemoveAllListeners();
        weaponButton.onClick.AddListener(() =>
        {
            onWeaponSelected?.Invoke();
            fightPopup.SetActive(false); // Disable the popup after selection
        });

        fistsButton.onClick.RemoveAllListeners();
        fistsButton.onClick.AddListener(() =>
        {
            onFistsSelected?.Invoke();
            fightPopup.SetActive(false); // Disable the popup after selection
        });
    }
}
