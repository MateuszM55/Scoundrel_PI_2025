using UnityEngine;

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
    /// The highest score achieved by the player.
    /// </summary>
    public int highScore;

    /// <summary>
    /// Initializes default values for the game manager at the start of the game.
    /// </summary>
    private void Start()
    {
        healthPoints = 20; // Example default value
        weapon = new Weapon { strength = 0, lastSlainMonster = 0 }; // Example default weapon
        highScore = 0; // Initialize high score
    }

    /// <summary>
    /// Resets the game state to its initial values.
    /// </summary>
    public void InitializeGame()
    {
        // Reset health points and weapon
        healthPoints = 20;
        weapon = null;
        highScore = 0; // Reset high score

        // Add any additional initialization logic here
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
            highScore = highScore // Include high score in the game state
        };
    }               

    /// <summary>
    /// Loads a saved game state into the game manager.
    /// </summary>
    /// <param name="gameState">The saved game state to load.</param>
    public void LoadGameState(GameState gameState)
    {
        healthPoints = gameState.healthPoints;
        weapon = gameState.weapon != null ? new Weapon { strength = gameState.weapon.strength, lastSlainMonster = gameState.weapon.lastSlainMonster } : null;
        highScore = gameState.highScore; // Restore high score
    }

    /// <summary>
    /// Updates the high score if the new score is greater than the current high score.
    /// </summary>
    /// <param name="newScore">The new score to compare against the current high score.</param>
    public void UpdateHighScore(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
        }
    }
}
