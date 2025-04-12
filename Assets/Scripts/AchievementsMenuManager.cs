using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class AchievementsMenuManager : MonoBehaviour
{
    public GameObject achievementsMenu;
    public UIManager uiManager;

    private List<string> achievements = new List<string>();
    public TextMeshProUGUI achievementsText; // Reference to the UI text displaying achievements

    private void Start()
    {
        // Initialize achievements (example)
        achievements.Add("First Monster Slain");
        achievements.Add("Collected 10 Healing Potions");
        achievements.Add("Beat the game");

        UpdateAchievementsDisplay();
    }

    private void UpdateAchievementsDisplay()
    {
        achievementsText.text = "Achievements:\n";
        foreach (string achievement in achievements)
        {
            achievementsText.text += $"- {achievement}\n";
        }
    }

    public void UnlockAchievement(string achievement)
    {
        if (!achievements.Contains(achievement))
        {
            achievements.Add(achievement);
            UpdateAchievementsDisplay();
        }
    }

    public void CloseAchievementsMenu()
    {
        achievementsMenu.SetActive(false);
        uiManager.BackToMainMenu();
    }
}
