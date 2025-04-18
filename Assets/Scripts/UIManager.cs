using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro; // Add this for TextMeshPro support

/// <summary>
/// Manages the user interface elements and interactions in the game.
/// </summary>
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
    public TextMeshProUGUI highScoreText; // Reference to the UI text displaying the high score
    public TextMeshProUGUI remainingCardsText; // Reference to the UI text displaying remaining cards

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

    // Add a dropdown for difficulty selection
    public TMP_Dropdown difficultyDropdown;

    // Add a field to store the selected difficulty
    private string selectedDifficulty = "Normal";

    // Add a reference to the AudioManager
    public AudioManager audioManager;

    // Add a reference for background music
    public AudioClip backgroundMusic;

    /// <summary>
    /// Initializes the UIManager at the start of the game.
    /// </summary>
    private void Start()
    {
        // Initialize difficulty dropdown
        InitializeDifficultyDropdown();

        // Shuffle the deck before initializing buttons
        ShuffleDeck();

        // Initialize UI with current values
        UpdateStatsDisplay();

        // Initialize card buttons
        InitializeCardButtons();

        // Initialize main menu buttons
        InitializeMainMenu();

        // Play background music
        if (audioManager != null && backgroundMusic != null)
        {
            audioManager.PlayMusic(backgroundMusic);
        }
    }

    /// <summary>
    /// Initializes the difficulty dropdown with options and sets the default value.
    /// </summary>
    private void InitializeDifficultyDropdown()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
            difficultyDropdown.options = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("Easy"),
                new TMP_Dropdown.OptionData("Normal"),
                new TMP_Dropdown.OptionData("Hard")
            };
            difficultyDropdown.value = 1; // Default to "Normal"
        }
    }

    /// <summary>
    /// Handles changes to the selected difficulty in the dropdown.
    /// </summary>
    /// <param name="index">The index of the selected difficulty.</param>
    private void OnDifficultyChanged(int index)
    {
        selectedDifficulty = difficultyDropdown.options[index].text;
        Debug.Log($"Difficulty changed to: {selectedDifficulty}");

        // Reinitialize the deck with the selected difficulty
        ShuffleDeck();
    }

    /// <summary>
    /// Shuffles the deck and updates the card buttons.
    /// </summary>
    public void ShuffleDeck()
    {
        deckManager.InitializeDeck(selectedDifficulty);
        InitializeCardButtons();

        // Update the remaining cards text
        UpdateStatsDisplay();
    }

    /// <summary>
    /// Initializes the card buttons with the corresponding cards from the deck.
    /// </summary>
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

        // Update the remaining cards text
        UpdateStatsDisplay();
    }

    /// <summary>
    /// Updates the displayed stats on the UI.
    /// </summary>
    private void UpdateStatsDisplay()
    {
        healthText.text = $"HP: {gameManager.healthPoints}";
        weaponStrengthText.text = gameManager.weapon != null
            ? $"Weapon Strength: {gameManager.weapon.strength}"
            : "Weapon Strength: None";
        lastSlainMonsterText.text = gameManager.weapon != null
            ? $"Last Slain Monster: {gameManager.weapon.lastSlainMonster}"
            : "Last Slain Monster: None";
        highScoreText.text = $"High Score: {gameManager.highScore}"; // Display the high score
        remainingCardsText.text = $"Remaining Cards: {deckManager.GetRemainingCardsCount()}"; // Display remaining cards
    }

    /// <summary>
    /// Updates the info text with a new message.
    /// </summary>
    /// <param name="message">The message to display.</param>
    private void UpdateInfoText(string message)
    {
        infoText.text = ""; // Clear the existing text
        infoText.text += message + "\n"; // Append the new message
    }

    /// <summary>
    /// Handles the logic when a card button is clicked.
    /// </summary>
    /// <param name="clickedButton">The button that was clicked.</param>
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

            // Update stats display, including remaining cards
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

    /// <summary>
    /// Cycles the cards, preserving the remaining card if applicable.
    /// </summary>
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
                // Attempt to find the remaining card in the deck
                int remainingCardRank = int.Parse(remainingButton.GetComponentInChildren<TextMeshProUGUI>().text);
                remainingCard = deckManager.deck.FirstOrDefault(c => c.Rank == remainingCardRank);

                if (remainingCard == null)
                {
                    Debug.LogWarning("Remaining card not found in the deck.");
                }
            }

            // Shuffle the deck before drawing new cards
            ShuffleDeck();

            // Draw new cards excluding the remaining card
            List<Card> newCards = deckManager.deck
                .Where(c => remainingCard == null || c != remainingCard)
                .Take(cardButtons.Count - 1)
                .ToList();

            // Update card buttons
            int newCardIndex = 0;
            for (int i = 0; i < cardButtons.Count; i++)
            {
                Button button = cardButtons[i];
                if (button == remainingButton && remainingCard != null)
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

            // Update the remaining cards text
            UpdateStatsDisplay();
        }
    }

    /// <summary>
    /// Handles the logic for running away and drawing new cards.
    /// </summary>
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

    /// <summary>
    /// Gets the color associated with a specific card type.
    /// </summary>
    /// <param name="type">The type of the card.</param>
    /// <returns>The color associated with the card type.</returns>
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

    /// <summary>
    /// Sets the interactable state of all buttons.
    /// </summary>
    /// <param name="interactable">Whether the buttons should be interactable.</param>
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in cardButtons)
        {
            button.interactable = interactable;
        }
        runButton.interactable = interactable;
    }

    /// <summary>
    /// Triggers a fight with a monster card.
    /// </summary>
    /// <param name="monsterCard">The monster card to fight.</param>
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

    /// <summary>
    /// Displays the fight popup and assigns actions to the buttons.
    /// </summary>
    /// <param name="onWeaponSelected">Action to perform when the weapon button is selected.</param>
    /// <param name="onFistsSelected">Action to perform when the fists button is selected.</param>
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

    /// <summary>
    /// Initializes the main menu and assigns button listeners.
    /// </summary>
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

    /// <summary>
    /// Toggles the visibility of the main menu.
    /// </summary>
    /// <param name="isVisible">Whether the main menu should be visible.</param>
    public void ToggleMainMenu(bool isVisible)
    {
        mainMenu.SetActive(isVisible);
    }

    /// <summary>
    /// Handles the logic for continuing a saved game.
    /// </summary>
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

    /// <summary>
    /// Handles the logic for starting a new game.
    /// </summary>
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

    /// <summary>
    /// Handles the logic for opening the achievements menu.
    /// </summary>
    private void OnAchievementsClicked()
    {
        mainMenu.SetActive(false);
        achievementsMenu.SetActive(true);
    }

    /// <summary>
    /// Handles changes to the volume slider.
    /// </summary>
    /// <param name="value">The new volume value.</param>
    private void OnVolumeChanged(float value)
    {
        // Use AudioManager to adjust volume
        if (audioManager != null)
        {
            audioManager.SetVolume(value);
        }
    }

    /// <summary>
    /// Handles the logic for quitting the application.
    /// </summary>
    private void OnQuitClicked()
    {
        GameState currentState = gameManager.GetGameState();
        SaveManager.SaveGame(currentState);
        Application.Quit();
    }

    /// <summary>
    /// Returns to the main menu from the achievements menu.
    /// </summary>
    public void BackToMainMenu()
    {
        achievementsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Handles the logic for winning the game.
    /// </summary>
    public void WinGame()
    {
        // Placeholder for win logic
        Debug.Log("WinGame method called.");
    }

    /// <summary>
    /// Handles the logic for losing the game.
    /// </summary>
    public void LoseGame()
    {
        // Placeholder for lose logic
        Debug.Log("LoseGame method called.");
    }
}
