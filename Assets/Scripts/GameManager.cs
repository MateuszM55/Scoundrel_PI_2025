using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Field for health points
    public int healthPoints;

    // Field for weapon
    public Weapon weapon;

    // Field to track the highest score
    public int highScore;

    // Initialize default values in Start or Awake
    private void Start()
    {
        healthPoints = 20; // Example default value
        weapon = new Weapon { strength = 0, lastSlainMonster = 0 }; // Example default weapon
        highScore = 0; // Initialize high score
    }

    public void InitializeGame()
    {
        // Reset health points and weapon
        healthPoints = 20;
        weapon = null;
        highScore = 0; // Reset high score

        // Add any additional initialization logic here
    }

    public GameState GetGameState()
    {
        return new GameState
        {
            healthPoints = healthPoints,
            weapon = weapon != null ? new Weapon { strength = weapon.strength, lastSlainMonster = weapon.lastSlainMonster } : null,
            highScore = highScore // Include high score in the game state
        };
    }

    public void LoadGameState(GameState gameState)
    {
        healthPoints = gameState.healthPoints;
        weapon = gameState.weapon != null ? new Weapon { strength = gameState.weapon.strength, lastSlainMonster = gameState.weapon.lastSlainMonster } : null;
        highScore = gameState.highScore; // Restore high score
    }

    // Method to update the high score
    public void UpdateHighScore(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
        }
    }
}
