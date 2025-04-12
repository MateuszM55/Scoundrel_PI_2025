using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "game_save.json");

    public static void SaveGame(GameState gameState)
    {
        string json = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log($"Game saved to {SaveFilePath}");
    }

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

[System.Serializable]
public class GameState
{
    public int healthPoints;
    public Weapon weapon;
}
