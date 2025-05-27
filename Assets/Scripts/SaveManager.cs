using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Converts a list of cards to an array of strings for JSON serialization.
    /// </summary>
    /// <param name="cards">The list of cards to convert.</param>
    /// <returns>An array of strings representing the cards.</returns>
    private static string[] ConvertCardsToStrings(List<Card> cards)
    {
        return cards.Select(card => $"{card.Rank}|{card.Suit}|{card.Type}").ToArray();
    }

    /// <summary>
    /// Converts an array of strings back to a list of cards.
    /// </summary>
    /// <param name="cardStrings">The array of strings to convert.</param>
    /// <returns>A list of cards represented by the strings.</returns>
    private static List<Card> ConvertStringsToCards(string[] cardStrings)
    {
        return cardStrings.Select(cardString =>
        {
            var parts = cardString.Split('|');
            return new Card
            {
                Rank = int.Parse(parts[0]),
                Suit = parts[1],
                Type = (CardType)System.Enum.Parse(typeof(CardType), parts[2]) // Fixed Enum usage
            };
        }).ToList();
    }

    /// <summary>
    /// Saves the current game state to a file in JSON format.
    /// </summary>
    /// <param name="gameState">The game state to save.</param>
    public static void SaveGame(GameState gameState)
    {
        // Convert the deck and current card buttons to strings
        gameState.deckStrings = ConvertCardsToStrings(gameState.deck);
        gameState.currentCardButtonStrings = ConvertCardsToStrings(gameState.currentCardButtons);

        // Clear the original lists to avoid duplication in JSON
        gameState.deck = null;
        gameState.currentCardButtons = null;

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
            var gameState = JsonUtility.FromJson<GameState>(json);

            // Convert strings back to cards
            gameState.deck = ConvertStringsToCards(gameState.deckStrings);
            gameState.currentCardButtons = ConvertStringsToCards(gameState.currentCardButtonStrings);

            return gameState;
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
    /// The player's deck of cards.
    /// </summary>
    public List<Card> deck;

    /// <summary>
    /// The player's deck of cards as strings for serialization.
    /// </summary>
    public string[] deckStrings;

    /// <summary>
    /// The current card buttons displayed to the player.
    /// </summary>
    public List<Card> currentCardButtons;

    /// <summary>
    /// The current card buttons as strings for serialization.
    /// </summary>
    public string[] currentCardButtonStrings;

    /// <summary>
    /// Indicates whether the player can run.
    /// </summary>
    public bool canRun;
}
