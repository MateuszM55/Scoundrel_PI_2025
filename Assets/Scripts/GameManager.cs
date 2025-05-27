using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the core game logic, including player stats, weapon, and high score.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The player's current health points.
    /// </summary>
    public int healthPoints;

    /// <summary>
    /// The weapon currently equipped by the player.
    /// </summary>
    public Weapon weapon;

    /// <summary>
    /// Reference to the UIManager instance.
    /// </summary>
    public UIManager uiManager;

    /// <summary>
    /// Reference to the DeckManager instance.
    /// </summary>
    public DeckManager deckManager;

    /// <summary>
    /// Initializes default values for the game manager at the start of the game.
    /// </summary>
    private void Start()
    {
        healthPoints = 20; // Example default value
        weapon = new Weapon { strength = 0, lastSlainMonster = 0 }; // Example default weapon

        // Initialize references to UIManager and DeckManager
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        if (deckManager == null)
        {
            deckManager = FindObjectOfType<DeckManager>();
        }
    }

    /// <summary>
    /// Resets the game state to its initial values.
    /// </summary>
    public void ResetGameState()
    {
        healthPoints = 20; // Reset health to the initial value
        weapon = null; // Remove any equipped weapon
        // Add any other properties that need to be reset
        Debug.Log("[GameManager] Game state has been reset.");
    }

    /// <summary>
    /// Retrieves the current game state.
    /// </summary>
    /// <returns>A <see cref="GameState"/> object representing the current game state.</returns>
    public GameState GetGameState()
    {
        return new GameState
        {
            healthPoints = healthPoints,
            weapon = weapon != null ? new Weapon { strength = weapon.strength, lastSlainMonster = weapon.lastSlainMonster } : null,
            deck = deckManager != null ? new List<Card>(deckManager.deck) : new List<Card>(), // Save the current deck
            currentCardButtons = uiManager != null ? new List<Card>(uiManager.GetCurrentCardButtons()) : new List<Card>(), // Save current card buttons
            canRun = uiManager != null && uiManager.GetCanRun() // Save the canRun flag
        };
    }               

    /// <summary>
    /// Loads a saved game state into the game manager.
    /// </summary>
    /// <param name="gameState">The saved game state to load.</param>
    public void LoadGameState(GameState gameState)
    {
        if (gameState == null)
        {
            Debug.LogError("[GameManager] LoadGameState called with null gameState.");
            return;
        }

        healthPoints = gameState.healthPoints;
        weapon = gameState.weapon != null ? new Weapon { strength = gameState.weapon.strength, lastSlainMonster = gameState.weapon.lastSlainMonster } : null;

        if (deckManager != null)
        {
            deckManager.deck = gameState.deck; // Restore deck
        }
        else
        {
            Debug.LogError("[GameManager] deckManager is null. Ensure it is properly initialized.");
        }

        if (uiManager != null)
        {
            if (gameState.currentCardButtons != null)
            {
                uiManager.RestoreCardButtons(gameState.currentCardButtons); // Restore card buttons
            }
            else
            {
                Debug.LogError("[GameManager] currentCardButtons in gameState is null.");
            }

            uiManager.SetCanRun(gameState.canRun); // Restore canRun flag
        }
        else
        {
            Debug.LogError("[GameManager] uiManager is null. Ensure it is properly initialized.");
        }
    }
}
