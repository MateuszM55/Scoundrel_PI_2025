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

    // Add a reference for the info text
    public TextMeshProUGUI infoText;

    private bool canRun = true; // Flag to track if running is allowed

    // Add a reference to the GameObject containing the fight popup buttons
    public GameObject fightPopup;

    // Add references for weapon and fists buttons
    public Button weaponButton;
    public Button fistsButton;

    // Add a reference for the Run button
    public Button runButton;

    // Add a dictionary to map buttons to their corresponding cards
    private Dictionary<Button, Card> buttonCardMap = new Dictionary<Button, Card>();

    // Add references for the main menu and its elements
    public GameObject mainMenu;
    public Button continueButton;
    public Button newGameButton;
    public Button achievementsButton;
    public Slider volumeSlider;

    // Add a reference for the Quit button
    public Button quitButton;

    // Add a reference for the achievements menu
    public GameObject achievementsMenu;

    // Add a reference for the game menu
    public GameObject gameMenu;

    private void Start()
    {
        // Shuffle the deck before initializing buttons
        ShuffleDeck();

        // Initialize UI with current values
        UpdateStatsDisplay();

        // Initialize card buttons
        InitializeCardButtons();

        // Initialize main menu buttons
        InitializeMainMenu();
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

        buttonCardMap.Clear(); // Clear the map before populating it

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

                // Use a local variable to correctly capture the card reference
                Card capturedCard = card;
                button.onClick.AddListener(() => OnCardClicked(button)); // Pass the button itself

                // Map the button to the card
                buttonCardMap[button] = card;
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

    // Helper method to update the info text
    private void UpdateInfoText(string message)
    {
        infoText.text = ""; // Clear the existing text
        infoText.text += message + "\n"; // Append the new message
    }

    // Modify OnCardClicked to enforce a health cap and initialize lastSlainMonster when equipping a weapon
    public void OnCardClicked(Button clickedButton)
    {
        // Ensure the clicked button exists in the map
        if (buttonCardMap.TryGetValue(clickedButton, out Card card))
        {
            // Hide the button and remove it from the map
            clickedButton.gameObject.SetActive(false);
            buttonCardMap.Remove(clickedButton);

            // Handle card interaction
            switch (card.Type)
            {
                case CardType.HealingPotion:
                    gameManager.healthPoints += card.Rank;
                    if (gameManager.healthPoints > 20) // Enforce health cap
                    {
                        gameManager.healthPoints = 20;
                        UpdateInfoText("Healed to full health.");
                    }
                    else
                    {
                        UpdateInfoText($"Healed for {card.Rank} HP. Current HP: {gameManager.healthPoints}");
                    }
                    break;

                case CardType.Weapon:
                    gameManager.weapon = new Weapon
                    {
                        strength = card.Rank,
                        lastSlainMonster = 0
                    };
                    UpdateInfoText($"Equipped weapon with Strength: {card.Rank}, Last Slain Monster: {gameManager.weapon.lastSlainMonster}");
                    break;

                case CardType.Monster:
                    TriggerFight(card);
                    break;

                default:
                    UpdateInfoText("Unknown card type.");
                    break;
            }

            // Update stats display
            UpdateStatsDisplay();

            // Check if card cycling is needed
            CycleCards();

            // Re-enable running
            canRun = true;
        }
        else
        {
            // Debugging log
            Debug.LogError($"Error: Clicked button {clickedButton.name} not found in the map.");
            UpdateInfoText("Error: Clicked button not found in the map.");
        }
    }

    // Modify CycleCards to preserve the remaining card
    public void CycleCards()
    {
        // Check if only one card button is active
        int activeButtons = cardButtons.Count(button => button.gameObject.activeSelf);
        if (activeButtons == 1)
        {
            // Identify the remaining card
            Button remainingButton = cardButtons.FirstOrDefault(button => button.gameObject.activeSelf);
            Card remainingCard = null;

            if (remainingButton != null)
            {
                int remainingCardRank = int.Parse(remainingButton.GetComponentInChildren<TextMeshProUGUI>().text);
                remainingCard = deckManager.deck.FirstOrDefault(c => c.Rank == remainingCardRank);
            }

            // Shuffle the deck before drawing new cards
            ShuffleDeck();

            // Draw new cards excluding the remaining card
            List<Card> newCards = deckManager.deck
                .Where(c => remainingCard == null || c != remainingCard)
                .Take(3)
                .ToList();

            // Update card buttons
            int newCardIndex = 0;
            for (int i = 0; i < cardButtons.Count; i++)
            {
                Button button = cardButtons[i];
                if (button == remainingButton)
                {
                    // Keep the remaining card intact
                    button.gameObject.SetActive(true);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = remainingCard.Rank.ToString();
                    button.GetComponent<Image>().color = GetCardColor(remainingCard.Type);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCardClicked(button));

                    // Debugging log
                    Debug.Log($"Preserved button {button.name} with card {remainingCard.Rank} of type {remainingCard.Type}");
                }
                else if (newCardIndex < newCards.Count)
                {
                    // Update button with a new card
                    Card card = newCards[newCardIndex++];
                    button.gameObject.SetActive(true);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = card.Rank.ToString();
                    button.GetComponent<Image>().color = GetCardColor(card.Type);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCardClicked(button));

                    // Map the button to the new card
                    buttonCardMap[button] = card;

                }
                else
                {
                    // Hide extra buttons
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

    // Method to handle running
    public void Run()
    {
        if (!canRun)
        {
            UpdateInfoText("You cannot run twice in a row!");
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

        UpdateInfoText("You ran away and drew new cards.");
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

    // Method to disable all buttons
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in cardButtons)
        {
            button.interactable = interactable;
        }
        runButton.interactable = interactable;
    }

    // Modify TriggerFight to show the fight popup
    private void TriggerFight(Card monsterCard)
    {
        int monsterStrength = monsterCard.Rank;

        // Check if the player has a weapon with non-zero strength
        if (gameManager.weapon != null && gameManager.weapon.strength > 0)
        {
            // Allow fighting any monster if lastSlainMonster is 0
            if (gameManager.weapon.lastSlainMonster == 0 || monsterStrength <= gameManager.weapon.lastSlainMonster)
            {
                // Show the fight popup to choose between weapon and fists
                ShowFightPopup(
                    onWeaponSelected: () =>
                    {
                        int damage = Mathf.Min(gameManager.weapon.strength, monsterStrength);
                        monsterStrength -= damage;
                        UpdateInfoText($"Used weapon! Monster strength reduced by {damage}. Remaining monster strength: {monsterStrength}");

                        gameManager.healthPoints -= monsterStrength;
                        UpdateInfoText($"Monster attacked back! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
                        gameManager.weapon.lastSlainMonster = monsterCard.Rank; // Update last slain monster
                        UpdateInfoText($"Monster slain! Last slain monster updated to {gameManager.weapon.lastSlainMonster}");
                        UpdateStatsDisplay();
                    },
                    onFistsSelected: () =>
                    {
                        gameManager.healthPoints -= monsterStrength;
                        UpdateInfoText($"Fought with fists! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
                        UpdateStatsDisplay();
                    }
                );
            }
            else
            {
                UpdateInfoText($"Monster is too strong! Cannot use weapon. Fighting with fists instead.");
                gameManager.healthPoints -= monsterStrength;
                UpdateInfoText($"Fought with fists! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
                UpdateStatsDisplay();
            }
        }
        else
        {
            // No weapon equipped or weapon strength is zero, fight with fists
            gameManager.healthPoints -= monsterStrength;
            UpdateInfoText($"No weapon equipped or weapon strength is zero! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
            UpdateStatsDisplay();
        }
    }

    // Modify ShowFightPopup to disable other buttons when the popup is shown
    private void ShowFightPopup(System.Action onWeaponSelected, System.Action onFistsSelected)
    {
        if (fightPopup == null)
        {
            Debug.LogError("Fight popup is not assigned in the inspector.");
            return;
        }

        // Disable all other buttons
        SetButtonsInteractable(false);

        // Enable the fight popup GameObject
        fightPopup.SetActive(true);

        // Assign actions to the buttons
        weaponButton.onClick.RemoveAllListeners();
        weaponButton.onClick.AddListener(() =>
        {
            onWeaponSelected?.Invoke();
            fightPopup.SetActive(false); // Disable the popup after selection
            SetButtonsInteractable(true); // Re-enable all buttons
        });

        fistsButton.onClick.RemoveAllListeners();
        fistsButton.onClick.AddListener(() =>
        {
            onFistsSelected?.Invoke();
            fightPopup.SetActive(false); // Disable the popup after selection
            SetButtonsInteractable(true); // Re-enable all buttons
        });
    }

    private void InitializeMainMenu()
    {
        // Assign button listeners
        continueButton.onClick.AddListener(OnContinueClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        achievementsButton.onClick.AddListener(OnAchievementsClicked);

        // Assign listener for the Quit button
        quitButton.onClick.AddListener(OnQuitClicked);

        // Set initial volume slider value
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void ToggleMainMenu(bool isVisible)
    {
        mainMenu.SetActive(isVisible);
    }

    private void OnContinueClicked()
    {
        ToggleMainMenu(false);
        GameState loadedState = SaveManager.LoadGame();
        if (loadedState != null)
        {
            gameManager.LoadGameState(loadedState);
        }
        else
        {
            Debug.LogWarning("No saved game to continue.");
        }
    }

    private void OnNewGameClicked()
    {
        ToggleMainMenu(false);

        // Show the game menu
        if (gameMenu != null)
        {
            gameMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("Game menu GameObject is not assigned in the inspector.");
        }
    }

    private void OnAchievementsClicked()
    {
        mainMenu.SetActive(false);
        achievementsMenu.SetActive(true);
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    // Add a method to handle quitting the application
    private void OnQuitClicked()
    {
        GameState currentState = gameManager.GetGameState();
        SaveManager.SaveGame(currentState);
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        achievementsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
