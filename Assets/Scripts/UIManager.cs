using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Add this for EventSystems
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

    // Add a reference for the weapon image UI element
    public Image weaponImage;

    // Add a public reference for the default weapon image
    public Sprite defaultWeaponImage;

    // Add private fields to track the game over and win screens
    private GameObject gameOverScreen;
    private GameObject winScreen;

    // Add references for menu and gameplay background sprites
    public Sprite menuBackground;
    public Sprite gameplayBackground;

    // Add a reference for the background image UI element
    public Image backgroundImage;

    /// <summary>
    /// Reference to the CardImageManager instance.
    /// </summary>
    public CardImageManager cardImageManager;

    /// <summary>
    /// Initializes the UIManager at the start of the game.
    /// </summary>
    private void Start()
    {
        // Initialize UI with current values
        UpdateStatsDisplay();

        // Initialize difficulty dropdown
        InitializeDifficultyDropdown();

        // Initialize main menu buttons
        InitializeMainMenu();

        // Set the initial background to the menu background
        if (backgroundImage != null && menuBackground != null)
        {
            backgroundImage.sprite = menuBackground;
        }

        // Update the remaining cards text at the start
        UpdateRemainingCardsText();
    }

    /// <summary>
    /// Updates the remaining cards text in the UI.
    /// </summary>
    private void UpdateRemainingCardsText()
    {
        if (remainingCardsText != null && deckManager != null)
        {
            remainingCardsText.text = $"Remaining Cards: {deckManager.GetRemainingCardsCount()}";
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

        // Regenerate the deck with the new difficulty
        if (deckManager != null)
        {
            deckManager.InitializeDeck(selectedDifficulty);
        }

        // Reinitialize the card buttons and update the UI
        InitializeCardButtons();
        UpdateRemainingCardsText();
    }

    /// <summary>
    /// Initializes the card buttons with the corresponding cards from the deck.
    /// </summary>
    private void InitializeCardButtons()
    {
        // Draw initial cards from the shuffled deck and remove them from the deck  
        List<Card> initialCards = deckManager.deck.Take(cardButtons.Count).ToList();
        deckManager.deck = deckManager.deck.Skip(cardButtons.Count).ToList();

        buttonCardMap.Clear(); // Clear the map before populating it  

        for (int i = 0; i < cardButtons.Count; i++)
        {
            if (i < initialCards.Count)
            {
                // Assign card to button  
                Card card = initialCards[i];
                Button button = cardButtons[i];
                button.gameObject.SetActive(true);

                // Assign the appropriate image to the card button  
                if (cardImageManager != null)
                {
                    Sprite cardSprite = cardImageManager.GetCardSprite(card);
                    if (cardSprite != null)
                    {
                        button.GetComponent<Image>().sprite = cardSprite;
                    }
                }

                // Add or update corner display for the card's value and suit  
                UpdateCardCornerDisplay(button, card);

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
    /// Updates the corner display of a card button: value as text in top-left and bottom-right, suit as image in top-right and bottom-left.
    /// </summary>
    /// <param name="button">The button representing the card.</param>
    /// <param name="card">The card to display.</param>
    private void UpdateCardCornerDisplay(Button button, Card card)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();

        Vector2 topLeftPosition = new Vector2(-buttonRect.rect.width / 2 + 20, buttonRect.rect.height / 2 - 20);
        Vector2 topRightPosition = new Vector2(buttonRect.rect.width / 2 - 20, buttonRect.rect.height / 2 - 20);
        Vector2 bottomLeftPosition = new Vector2(-buttonRect.rect.width / 2 + 20, -buttonRect.rect.height / 2 + 20);
        Vector2 bottomRightPosition = new Vector2(buttonRect.rect.width / 2 - 20, -buttonRect.rect.height / 2 + 20);

        // Top-left: value as text
        TextMeshProUGUI topLeftText = GetOrCreateCornerText(button, "TopLeftText", topLeftPosition);
        topLeftText.text = card.Rank.ToString();
        topLeftText.color = Color.white;

        // Bottom-right: value as text
        TextMeshProUGUI bottomRightText = GetOrCreateCornerText(button, "BottomRightText", bottomRightPosition);
        bottomRightText.text = card.Rank.ToString();
        bottomRightText.color = Color.white;

        // Top-right: suit as image
        SetOrCreateCornerSuitImage(button, "TopRightSuitImage", topRightPosition, card);

        // Bottom-left: suit as image
        SetOrCreateCornerSuitImage(button, "BottomLeftSuitImage", bottomLeftPosition, card);
    }

    /// <summary>
    /// Creates or updates a suit image in a card button corner.
    /// </summary>
    private void SetOrCreateCornerSuitImage(Button button, string name, Vector2 anchoredPosition, Card card)
    {
        // Remove any old text object with the same name (from previous implementation)
        Transform oldText = button.transform.Find(name.Replace("Image", "Text"));
        if (oldText != null) DestroyImmediate(oldText.gameObject);

        // Find or create the image object
        Transform existing = button.transform.Find(name);
        Image img;
        if (existing != null)
        {
            img = existing.GetComponent<Image>();
        }
        else
        {
            GameObject imgObj = new GameObject(name);
            imgObj.transform.SetParent(button.transform);
            RectTransform rect = imgObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(64, 64); // Double the previous size (was 32,32)
            rect.anchoredPosition = anchoredPosition;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            img = imgObj.AddComponent<Image>();
        }
        // Set the sprite
        if (cardImageManager != null)
            img.sprite = cardImageManager.GetSuitSprite(card);
        img.color = Color.white;
        img.enabled = img.sprite != null;
    }

    /// <summary>
    /// Retrieves or creates a corner text object for a card button.
    /// </summary>
    /// <param name="button">The button representing the card.</param>
    /// <param name="name">The name of the text object.</param>
    /// <param name="anchoredPosition">The anchored position of the text object.</param>
    /// <returns>The TextMeshProUGUI component of the corner text object.</returns>
    private TextMeshProUGUI GetOrCreateCornerText(Button button, string name, Vector2 anchoredPosition)
    {
        // Check if the text object already exists
        Transform existingTextTransform = button.transform.Find(name);
        if (existingTextTransform != null)
        {
            return existingTextTransform.GetComponent<TextMeshProUGUI>();
        }

        // Create a new text object
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(button.transform);

        // Configure the RectTransform
        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(60, 30); // Increased size to fit two-digit numbers
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // Add and configure the TextMeshProUGUI component
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = 100; // Increased font size to match previous (larger) style
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black; // Adjust color as needed

        return text;
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
    /// Saves the current game state.
    /// </summary>
    private void SaveGameState()
    {
        GameState gameState = new GameState
        {
            healthPoints = gameManager.healthPoints,
            weapon = gameManager.weapon,
            deck = deckManager.deck,
            currentCardButtons = buttonCardMap.Values.ToList(),
            canRun = canRun
        };
        SaveManager.SaveGame(gameState);
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
            // Remove the card from the deck
            deckManager.RemoveCardFromDeck(card);

            // Hide the button and remove it from the map
            clickedButton.gameObject.SetActive(false);
            buttonCardMap.Remove(clickedButton); 

            bool hasFought = false; // Flag to track if a fight has occurred

            // Handle card interaction
            switch (card.Type)
            {
                case CardType.HealingPotion:
                    Debug.Log("[UIManager] Healing potion card clicked.");
                    if (deckManager.healingPotionCounter >= 1)
                    {
                        UpdateInfoText("Healing effect suppressed due to excessive potion use.");
                    }
                    else
                    {
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
                    }
                    deckManager.healingPotionCounter++;
                    deckManager.LogHealingPotionCounter(); // Log the updated counter value

                    // Play healing potion sound
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayCardTypeSound(CardType.HealingPotion);
                    }
                    break;

                case CardType.Weapon:
                    gameManager.weapon = new Weapon
                    {
                        strength = card.Rank,
                        lastSlainMonster = 0
                    };
                    UpdateInfoText($"Equipped weapon with Strength: {card.Rank}, Last Slain Monster: {gameManager.weapon.lastSlainMonster}");

                    // Update the weapon image
                    UpdateWeaponImage(card); // Ensure the image updates when a weapon card is clicked

                    // Play weapon sound
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayCardTypeSound(CardType.Weapon);
                    }
                    break;

                case CardType.Monster:
                    TriggerFight(card);
                    hasFought = true;
                    break;
            }

            // Debug log to display the number of cards remaining in the deck
            Debug.Log($"Cards remaining in deck: {deckManager.GetRemainingCardsCount()}");

            // Update stats display, including remaining cards
            UpdateStatsDisplay();

            // Check if card cycling is needed
            if (!hasFought)
            {
                CycleCards();
            }

            hasFought = false; // Reset the flag for the next card click    

            // Save the game state after card click
            SaveGameState();
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
            // Re-enable running
            canRun = true;
            // Reset the healing potion counter when cards are redrawn
            Debug.Log("[UIManager] Resetting healingPotionCounter during card redraw.");
            deckManager.healingPotionCounter = 0;
            deckManager.LogHealingPotionCounter(); // Log the reset counter value

            UpdateRemainingCardsText(); // Display remaining cards
            // Identify the remaining card
            Button remainingButton = cardButtons.FirstOrDefault(button => button.gameObject.activeSelf);
            Card remainingCard = null;

            if (remainingButton != null)
            {
                // Attempt to find the remaining card in the map instead of re-parsing
                if (buttonCardMap.TryGetValue(remainingButton, out remainingCard))
                {
                    Debug.Log($"Preserving card: Rank {remainingCard.Rank}, Type {remainingCard.Type}");
                }
                else
                {
                    Debug.LogWarning("Remaining card not found in the button-card map.");
                }
            }

            // Draw new cards excluding the remaining card and remove them from the deck  
            List<Card> newCards = deckManager.deck
               .Where(c => remainingCard == null || c != remainingCard)
               .Take(cardButtons.Count - 1)
               .ToList();

            // Remove the drawn cards from the deck  
            foreach (var card in newCards)
            {
                deckManager.RemoveCardFromDeck(card);
            }

            // Update card buttons
            int newCardIndex = 0;
            for (int i = 0; i < cardButtons.Count; i++)
            {
                Button button = cardButtons[i];
                if (button == remainingButton && remainingCard != null)
                {
                    // Keep the remaining card intact
                    button.gameObject.SetActive(true);
                    UpdateCardCornerDisplay(button, remainingCard); // Update all corners

                    // Update the sprite for the remaining card
                    if (cardImageManager != null)
                    {
                        Sprite cardSprite = cardImageManager.GetCardSprite(remainingCard);
                        if (cardSprite != null)
                        {
                            button.GetComponent<Image>().sprite = cardSprite;
                        }
                    }

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCardClicked(button));

                    // Ensure the preserved card remains mapped correctly
                    buttonCardMap[button] = remainingCard;
                }
                else if (newCardIndex < newCards.Count)
                {
                    // Update button with a new card
                    Card card = newCards[newCardIndex++];
                    button.gameObject.SetActive(true);
                    UpdateCardCornerDisplay(button, card); // Update all corners

                    // Update the sprite for the new card
                    if (cardImageManager != null)
                    {
                        Sprite cardSprite = cardImageManager.GetCardSprite(card);
                        if (cardSprite != null)
                        {
                            button.GetComponent<Image>().sprite = cardSprite;
                        }
                    }

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

        // Check if there are any cards left in the deck or on the table
        int activeButtons = cardButtons.Count(button => button.gameObject.activeSelf);
        int deckCount = deckManager.GetRemainingCardsCount();
        if (activeButtons == 0 && deckCount == 0)
        {
            UpdateInfoText("No cards left to run from!");
            return;
        }

        // Play the running sound effect
        if (audioManager != null)
        {
            Debug.Log("Playing run sound effect.");
            audioManager.PlaySFX(audioManager.runSound);
        }

        // Move all current cards to the bottom of the deck (preserve their Card objects)
        List<Card> cardsToMove = new List<Card>();
        foreach (Button button in cardButtons)
        {
            if (button.gameObject.activeSelf && buttonCardMap.TryGetValue(button, out Card card))
            {
                cardsToMove.Add(card);
            }
        }
        // Remove these cards from the deck if they are still present (avoid duplicates)
        foreach (var card in cardsToMove)
        {
            deckManager.RemoveCardFromDeck(card);
            deckManager.deck.Add(card); // Add to the bottom of the deck
        }

        // If there are fewer cards in the deck than cardButtons, only draw as many as possible
        int cardsToDraw = Mathf.Min(cardButtons.Count, deckManager.deck.Count);
        List<Card> newCards = deckManager.deck.Take(cardsToDraw).ToList();
        deckManager.deck = deckManager.deck.Skip(cardsToDraw).ToList();

        buttonCardMap.Clear();
        for (int i = 0; i < cardButtons.Count; i++)
        {
            if (i < newCards.Count)
            {
                Card card = newCards[i];
                Button button = cardButtons[i];
                button.gameObject.SetActive(true);

                // Assign the appropriate image to the card button
                if (cardImageManager != null)
                {
                    Sprite cardSprite = cardImageManager.GetCardSprite(card);
                    if (cardSprite != null)
                    {
                        button.GetComponent<Image>().sprite = cardSprite;
                    }
                }

                UpdateCardCornerDisplay(button, card);

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCardClicked(button));
                buttonCardMap[button] = card;
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
        }

        // Disable running until another action is taken
        canRun = false;

        UpdateInfoText("You ran away and drew new cards.");
        UpdateRemainingCardsText(); // Update remaining cards text
        UpdateStatsDisplay();

        // Save the game state after running
        SaveGameState();
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

                        gameManager.healthPoints -= monsterStrength;
                        UpdateInfoText($"Used weapon but monster attacked back! Player health reduced by {monsterStrength}.");

                       
                        gameManager.weapon.lastSlainMonster = monsterCard.Rank; // Update last slain monster
                        UpdateStatsDisplay();
                       

                        // Play monster sound effect after weapon choice
                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayCardTypeSound(CardType.Monster);
                        }
                        // Check if the player's health is below 0 and trigger loss
                        if (gameManager.healthPoints <= 0)
                        {
                            HandleGameOver();
                            return;
                        }

                        CheckWinCondition();
                    },
                    onFistsSelected: () =>
                    {
                        gameManager.healthPoints -= monsterStrength;
                        UpdateInfoText($"Fought with fists! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");
                        UpdateStatsDisplay();

                        // Play monster sound effect after fists choice
                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayCardTypeSound(CardType.Monster);
                        }
                        // Check if the player's health is below 0 and trigger loss
                        if (gameManager.healthPoints <= 0)
                        {
                            HandleGameOver();
                            return;
                        }

                        CheckWinCondition();
                    }
                );
                SaveGameState(); // Save after fight
                return; // Exit the method after showing the popup
            }
            else
            {

                gameManager.healthPoints -= monsterStrength;
                UpdateInfoText($"Fought with fists! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");

                

                UpdateStatsDisplay();

                // Play monster sound effect for auto fists fight
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCardTypeSound(CardType.Monster);
                }
                // Check if the player's health is below 0 and trigger loss
                if (gameManager.healthPoints <= 0)
                {
                    HandleGameOver();
                    return;
                }
                CheckWinCondition();
            }
        }
        else
        {
            // No weapon equipped or weapon strength is zero, fight with fists
            gameManager.healthPoints -= monsterStrength;
            UpdateInfoText($"No weapon equipped or weapon strength is zero! Player health reduced by {monsterStrength}. Current HP: {gameManager.healthPoints}");

           

            UpdateStatsDisplay();

            // Play monster sound effect for auto fists fight
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCardTypeSound(CardType.Monster);
            }
            // Check if the player's health is below 0 and trigger loss
            if (gameManager.healthPoints <= 0)
            {
                HandleGameOver();
                return;
            }
            CheckWinCondition();

        }

        CycleCards(); // Cycle cards after the fight only when no popup was shown

        // Save the game state after fight
        SaveGameState();
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
            CycleCards(); // Cycle cards after the choice is made
        });

        fistsButton.onClick.RemoveAllListeners();
        fistsButton.onClick.AddListener(() =>
        {
            onFistsSelected?.Invoke();
            fightPopup.SetActive(false); // Disable the popup after selection
            SetButtonsInteractable(true); // Re-enable all buttons
            CycleCards(); // Cycle cards after the choice is made
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

        // Set initial volume slider value to the default volume from AudioManager
        if (audioManager != null && volumeSlider != null)
        {
            volumeSlider.value = audioManager.GetDefaultVolume();
        }

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

        // Change the background to the gameplay background
        if (backgroundImage != null && gameplayBackground != null)
        {
            backgroundImage.sprite = gameplayBackground;
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

    /// <summary>
    /// Updates the weapon image based on the equipped weapon card.
    /// </summary>
    /// <param name="weaponCard">The weapon card to display.</param>
    private void UpdateWeaponImage(Card weaponCard)
    {
        if (weaponImage != null)
        {
            if (weaponCard != null)
            {
                // Assuming weaponCard.Suit or weaponCard.Rank determines the image
                Sprite weaponSprite = GetWeaponSprite(weaponCard);
                if (weaponSprite != null)
                {
                    weaponImage.sprite = weaponSprite;
                    weaponImage.enabled = true; // Ensure the image is visible
                }
            }
            else
            {
                // Reset to the default weapon image if no weapon is equipped
                weaponImage.sprite = defaultWeaponImage;
                weaponImage.enabled = defaultWeaponImage != null; // Hide the image if no default is set
            }
        }
    }

    /// <summary>
    /// Retrieves the appropriate weapon sprite based on the weapon card.
    /// </summary>
    /// <param name="weaponCard">The weapon card to get the sprite for.</param>
    /// <returns>The sprite associated with the weapon card.</returns>
    private Sprite GetWeaponSprite(Card weaponCard)
    {
        if (cardImageManager == null || weaponCard == null)
        {
            return null; // Return null if CardImageManager or weaponCard is not available
        }

        // Use the CardImageManager to fetch the sprite based on the weapon card's rank
        return cardImageManager.GetCardSprite(weaponCard);
    }

    /// <summary>
    /// Handles the game over state.
    /// </summary>
    public void HandleGameOver()
    {
        // Check if the game over screen already exists
        if (gameOverScreen != null)
        {
            return; // Prevent multiple instances
        }

        // Calculate the score as the negative sum of all undefeated monsters
        int score = -deckManager.deck
            .Where(card => card.Type == CardType.Monster)
            .Sum(card => card.Rank);

        // Hide the game UI
        gameMenu.SetActive(false);

        // Dynamically create the game over screen
        gameOverScreen = new GameObject("GameOverScreen");
        Canvas canvas = gameOverScreen.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        gameOverScreen.AddComponent<CanvasScaler>();
        gameOverScreen.AddComponent<GraphicRaycaster>();

        // Add a background panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(gameOverScreen.transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 200);
        panelRect.anchoredPosition = Vector2.zero;
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black

        // Add "Game Over" text
        GameObject gameOverText = new GameObject("GameOverText");
        gameOverText.transform.SetParent(panel.transform);
        TextMeshProUGUI text = gameOverText.AddComponent<TextMeshProUGUI>();
        text.text = "Game Over";
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;
        RectTransform textRect = gameOverText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(300, 50);
        textRect.anchoredPosition = new Vector2(0, 50);

        // Add "Score" text
        GameObject scoreText = new GameObject("ScoreText");
        scoreText.transform.SetParent(panel.transform);
        TextMeshProUGUI scoreTextComponent = scoreText.AddComponent<TextMeshProUGUI>();
        scoreTextComponent.text = $"Score: {score}"; // Display the calculated score
        scoreTextComponent.fontSize = 24;
        scoreTextComponent.alignment = TextAlignmentOptions.Center;
        RectTransform scoreTextRect = scoreText.GetComponent<RectTransform>();
        scoreTextRect.sizeDelta = new Vector2(300, 50);
        scoreTextRect.anchoredPosition = new Vector2(0, 0); // Position below "Game Over" text

        // Add "Back to Menu" button
        GameObject backButton = new GameObject("BackToMenuButton");
        backButton.transform.SetParent(panel.transform);
        Button button = backButton.AddComponent<Button>();
        Image buttonImage = backButton.AddComponent<Image>();
        buttonImage.color = Color.white; // Button background color
        RectTransform buttonRect = backButton.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200, 50);
        buttonRect.anchoredPosition = new Vector2(0, -50);

        // Add button text dynamically
        GameObject buttonText = new GameObject("ButtonText");
        buttonText.transform.SetParent(backButton.transform);
        TextMeshProUGUI buttonTextComponent = buttonText.AddComponent<TextMeshProUGUI>();
        buttonTextComponent.text = "Back to Menu";
        buttonTextComponent.fontSize = 24;
        buttonTextComponent.alignment = TextAlignmentOptions.Center;
        buttonTextComponent.color = Color.black; // Set the text color to black
        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
        buttonTextRect.sizeDelta = new Vector2(200, 50);
        buttonTextRect.anchoredPosition = Vector2.zero;

        // Add button functionality to hide the game over screen and show the main menu
        button.onClick.AddListener(() =>
        {
            Destroy(gameOverScreen); // Remove the game over screen
            gameOverScreen = null; // Reset the reference
            ToggleMainMenu(true); // Show the main menu
        });

        // Change the background back to the menu background
        if (backgroundImage != null && menuBackground != null)
        {
            backgroundImage.sprite = menuBackground;
        }

        // Reset the game state
        gameManager.ResetGameState();
        deckManager.InitializeDeck(selectedDifficulty); // Reinitialize the deck
        InitializeCardButtons(); // Reinitialize the UI
        UpdateStatsDisplay(); // Update the stats display

        // Reset info text and update remaining cards text
        infoText.text = "info";
        UpdateRemainingCardsText();

        // Reset the weapon image to default
        UpdateWeaponImage(null);

        // Reset the canRun flag to allow running in the next game
        canRun = true;
    }

    /// <summary>
    /// Checks if the player has won the game.
    /// </summary>
    private void CheckWinCondition()
    {
        // Check if all monsters have been selected and the player has positive health
        bool allMonstersSelectedInDeck = !deckManager.deck.Any(card => card.Type == CardType.Monster);
        bool allMonstersSelectedInButtons = !buttonCardMap.Values.Any(card => card.Type == CardType.Monster);

        if (allMonstersSelectedInDeck && allMonstersSelectedInButtons && gameManager.healthPoints > 0)
        {
            HandleWin();
        }
    }

    /// <summary>
    /// Handles the win state.
    /// </summary>
    private void HandleWin()
    {
        // Check if the win screen already exists
        if (winScreen != null)
        {
            return; // Prevent multiple instances
        }

        // Calculate the score as the sum of remaining health and the largest unused healing potion  
        int largestUnusedPotion = deckManager.deck
           .Where(card => card.Type == CardType.HealingPotion)
           .Select(card => card.Rank)
           .DefaultIfEmpty(0) // Ensure a default value of 0 if no elements exist  
           .Max();
        int score = gameManager.healthPoints + largestUnusedPotion;

        // Hide the game UI
        gameMenu.SetActive(false);

        // Dynamically create the win screen
        winScreen = new GameObject("WinScreen");
        Canvas canvas = winScreen.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        winScreen.AddComponent<CanvasScaler>();
        winScreen.AddComponent<GraphicRaycaster>();

        // Add a background panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(winScreen.transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 200);
        panelRect.anchoredPosition = Vector2.zero;
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black

        // Add "You Win" text
        GameObject winText = new GameObject("WinText");
        winText.transform.SetParent(panel.transform);
        TextMeshProUGUI text = winText.AddComponent<TextMeshProUGUI>();
        text.text = "You Win!";
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;
        RectTransform textRect = winText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(300, 50);
        textRect.anchoredPosition = new Vector2(0, 50);

        // Add "Score" text
        GameObject scoreText = new GameObject("ScoreText");
        scoreText.transform.SetParent(panel.transform);
        TextMeshProUGUI scoreTextComponent = scoreText.AddComponent<TextMeshProUGUI>();
        scoreTextComponent.text = $"Score: {score}"; // Display the calculated score
        scoreTextComponent.fontSize = 24;
        scoreTextComponent.alignment = TextAlignmentOptions.Center;
        RectTransform scoreTextRect = scoreText.GetComponent<RectTransform>();
        scoreTextRect.sizeDelta = new Vector2(300, 50);
        scoreTextRect.anchoredPosition = new Vector2(0, 0); // Position below "You Win" text

        // Add "Back to Menu" button
        GameObject backButton = new GameObject("BackToMenuButton");
        backButton.transform.SetParent(panel.transform);
        Button button = backButton.AddComponent<Button>();
        Image buttonImage = backButton.AddComponent<Image>();
        buttonImage.color = Color.white; // Button background color
        RectTransform buttonRect = backButton.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200, 50);
        buttonRect.anchoredPosition = new Vector2(0, -50);

        // Add button text dynamically
        GameObject buttonText = new GameObject("ButtonText");
        buttonText.transform.SetParent(backButton.transform);
        TextMeshProUGUI buttonTextComponent = buttonText.AddComponent<TextMeshProUGUI>();
        buttonTextComponent.text = "Back to Menu";
        buttonTextComponent.fontSize = 24;
        buttonTextComponent.alignment = TextAlignmentOptions.Center;
        buttonTextComponent.color = Color.black;
        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
        buttonTextRect.sizeDelta = new Vector2(200, 50);
        buttonTextRect.anchoredPosition = Vector2.zero;

        // Add button functionality to hide the win screen and show the main menu
        button.onClick.AddListener(() =>
        {
            Destroy(winScreen); // Remove the win screen
            winScreen = null; // Reset the reference
            ToggleMainMenu(true); // Show the main menu
        });

        // Change the background back to the menu background
        if (backgroundImage != null && menuBackground != null)
        {
            backgroundImage.sprite = menuBackground;
        }

        // Reset the game state
        gameManager.ResetGameState();
        deckManager.InitializeDeck(selectedDifficulty); // Reinitialize the deck
        InitializeCardButtons(); // Reinitialize the UI
        UpdateStatsDisplay(); // Update the stats display

        // Reset info text and update remaining cards text
        infoText.text = "info";
        UpdateRemainingCardsText();

        // Reset the weapon image to default
        UpdateWeaponImage(null);

        // Reset the canRun flag to allow running in the next game
        canRun = true;
    }

    /// <summary>
    /// Displays a full-screen panel with a grid showing the entire deck in 4 columns, stretching text to be screen wide.
    /// </summary>
    public void ShowDeckOverview()
    {
        // Create the panel
        GameObject panel = new GameObject("DeckOverviewPanel");
        Canvas canvas = panel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        panel.AddComponent<CanvasScaler>();
        panel.AddComponent<GraphicRaycaster>();

        // Add a semi-transparent background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(panel.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black

        // Add a container for the grid
        GameObject gridObject = new GameObject("DeckGrid");
        gridObject.transform.SetParent(panel.transform, false);
        RectTransform gridRect = gridObject.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0, 0.5f);
        gridRect.anchorMax = new Vector2(1, 0.5f);
        gridRect.pivot = new Vector2(0.5f, 0.5f);
        gridRect.sizeDelta = new Vector2(0, Screen.height - 200);
        gridRect.anchoredPosition = Vector2.zero;

        GridLayoutGroup grid = gridObject.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;
        grid.cellSize = new Vector2((Screen.width - 100) / 4f, 80);
        grid.spacing = new Vector2(10, 10);
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.padding = new RectOffset(10, 10, 10, 10);

        // Add each card as a cell in the grid
        if (deckManager != null && deckManager.deck != null)
        {
            foreach (var card in deckManager.deck)
            {
                GameObject cardCell = new GameObject("CardCell");
                cardCell.transform.SetParent(gridObject.transform, false);

                // Stretch the card cell horizontally
                RectTransform cellRect = cardCell.AddComponent<RectTransform>();
                cellRect.anchorMin = new Vector2(0, 0);
                cellRect.anchorMax = new Vector2(1, 1);
                cellRect.pivot = new Vector2(0.5f, 0.5f);
                cellRect.sizeDelta = Vector2.zero;

                TextMeshProUGUI cardText = cardCell.AddComponent<TextMeshProUGUI>();
                cardText.text = $"{card.Rank} of {card.Suit} ({card.Type})";
                cardText.fontSize = 50;
                cardText.alignment = TextAlignmentOptions.Center;
                cardText.color = Color.white;
                cardText.enableWordWrapping = false;
                cardText.enableAutoSizing = true;
                cardText.rectTransform.anchorMin = new Vector2(0, 0);
                cardText.rectTransform.anchorMax = new Vector2(1, 1);
                cardText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                cardText.rectTransform.offsetMin = Vector2.zero;
                cardText.rectTransform.offsetMax = Vector2.zero;
            }
        }

        // Add a close button
        GameObject closeButton = new GameObject("CloseButton");
        closeButton.transform.SetParent(panel.transform, false);
        RectTransform buttonRect = closeButton.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200, 50);
        buttonRect.anchorMin = new Vector2(0.5f, 0);
        buttonRect.anchorMax = new Vector2(0.5f, 0);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0, 60);

        Button button = closeButton.AddComponent<Button>();
        Image buttonImage = closeButton.AddComponent<Image>();
        buttonImage.color = Color.white;

        // Add button text
        GameObject buttonText = new GameObject("ButtonText");
        buttonText.transform.SetParent(closeButton.transform, false);
        TextMeshProUGUI buttonTextComponent = buttonText.AddComponent<TextMeshProUGUI>();
        buttonTextComponent.text = "Close";
        buttonTextComponent.fontSize = 24;
        buttonTextComponent.alignment = TextAlignmentOptions.Center;
        buttonTextComponent.color = Color.black;
        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
        buttonTextRect.sizeDelta = new Vector2(200, 50);
        buttonTextRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonTextRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonTextRect.pivot = new Vector2(0.5f, 0.5f);
        buttonTextRect.anchoredPosition = Vector2.zero;

        // Add close functionality
        button.onClick.AddListener(() => Destroy(panel));
    }

    /// <summary>
    /// Restores the state of card buttons from a saved game state.
    /// </summary>
    /// <param name="savedCards">The list of cards to restore to the buttons.</param>
    public void RestoreCardButtons(List<Card> savedCards)
    {
        if (savedCards == null)
        {
            Debug.LogError("[UIManager] RestoreCardButtons called with null savedCards.");
            return;
        }

        if (cardButtons == null)
        {
            Debug.LogError("[UIManager] cardButtons is null. Ensure it is properly initialized.");
            return;
        }

        buttonCardMap.Clear(); // Clear the existing map

        for (int i = 0; i < cardButtons.Count; i++)
        {
            if (i < savedCards.Count)
            {
                Card card = savedCards[i];
                Button button = cardButtons[i];
                if (button == null)
                {
                    Debug.LogError($"[UIManager] Button at index {i} is null.");
                    continue;
                }

                button.gameObject.SetActive(true);

                // Update the button's image and corner display
                if (cardImageManager != null)
                {
                    Sprite cardSprite = cardImageManager.GetCardSprite(card);
                    if (cardSprite != null)
                    {
                        button.GetComponent<Image>().sprite = cardSprite;
                    }
                }
                UpdateCardCornerDisplay(button, card);

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCardClicked(button));

                // Map the button to the card
                buttonCardMap[button] = card;
            }
            else
            {
                // Hide extra buttons
                if (cardButtons[i] != null)
                {
                    cardButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Sets the canRun flag based on the saved game state.
    /// </summary>
    /// <param name="value">The value to set for the canRun flag.</param>
    public void SetCanRun(bool value)
    {
        canRun = value;
    }

    /// <summary>
    /// Retrieves the cards currently displayed on the card buttons.
    /// </summary>
    /// <returns>A list of cards currently displayed on the card buttons.</returns>
    public List<Card> GetCurrentCardButtons()
    {
        return buttonCardMap.Values.ToList();
    }

    /// <summary>
    /// Retrieves the current value of the canRun flag.
    /// </summary>
    /// <returns>The current value of the canRun flag.</returns>
    public bool GetCanRun()
    {
        return canRun;
    }
}
