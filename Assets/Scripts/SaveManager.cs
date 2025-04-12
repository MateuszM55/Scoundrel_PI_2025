using System.IO;
using UnityEngine;

/// <summary>
/// Provides functionality to save and load the game state to and from a file.
/// </summary>
public static class SaveManager
{
    /// <summary>
    /// The file path where the game state is saved.
    /// </summary>
    private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "game_save.json");

    /// <summary>
    /// Saves the current game state to a file in JSON format.
    /// </summary>
    /// <param name="gameState">The game state to save.</param>
    public static void SaveGame(GameState gameState)
    {   
        string json = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log($"Game saved to {SaveFilePath}");
    }

    /// <summary>
    /// Loads the game state from a file.
    /// </summary>
    /// <returns>The loaded game state, or null if no save file exists.</returns>
    public static GameState LoadGame()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            return JsonUtility.FromJson<GameState>(json);
        }
        Debug.LogWarning("No save file found.");
        return null;
    }
}

/// <summary>
/// Represents the state of the game, including player stats and progress.
/// </summary>
[System.Serializable]
public class GameState
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
    public int highScore; // Add high score to the game state
}
