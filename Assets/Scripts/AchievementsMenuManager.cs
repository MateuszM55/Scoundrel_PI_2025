using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Manages the achievements menu, including displaying and unlocking achievements.
/// </summary>
public class AchievementsMenuManager : MonoBehaviour
{
    /// <summary>
    /// The GameObject representing the achievements menu.
    /// </summary>
    public GameObject achievementsMenu;

    /// <summary>
    /// Reference to the UIManager for navigation back to the main menu.
    /// </summary>
    public UIManager uiManager;

    /// <summary>
    /// List of unlocked achievements.
    /// </summary>
    private List<string> achievements = new List<string>();

    /// <summary>
    /// Reference to the UI text displaying the list of achievements.
    /// </summary>
    public TextMeshProUGUI achievementsText;

    /// <summary>
    /// Initializes the achievements menu and populates it with example achievements.
    /// </summary>
    private void Start()
    {
        // Initialize achievements (example)
        achievements.Add("First Monster Slain");
        achievements.Add("Collected 10 Healing Potions");
        achievements.Add("Beat the game");

        UpdateAchievementsDisplay();
    }

    /// <summary>
    /// Updates the achievements display in the UI.
    /// </summary>
    private void UpdateAchievementsDisplay()
    {
        achievementsText.text = "Achievements:\n";
        foreach (string achievement in achievements)
        {
            achievementsText.text += $"- {achievement}\n";
        }
    }

    /// <summary>
    /// Unlocks a new achievement and updates the display.
    /// </summary>
    /// <param name="achievement">The name of the achievement to unlock.</param>
    public void UnlockAchievement(string achievement)
    {
        if (!achievements.Contains(achievement))
        {
            achievements.Add(achievement);
            UpdateAchievementsDisplay();
        }
    }

    /// <summary>
    /// Closes the achievements menu and navigates back to the main menu.
    /// </summary>
    public void CloseAchievementsMenu()
    {
        achievementsMenu.SetActive(false);
        uiManager.BackToMainMenu();
    }
}
