using UnityEngine;

public class AchievementsMenuManager : MonoBehaviour
{
    public GameObject achievementsMenu;
    public UIManager uiManager;

    public void CloseAchievementsMenu()
    {
        achievementsMenu.SetActive(false);
        uiManager.BackToMainMenu();
    }
}
