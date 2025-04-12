using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Field for health points
    public int healthPoints;

    // Field for weapon
    public Weapon weapon;

    // Initialize default values in Start or Awake
    private void Start()
    {
        healthPoints = 20; // Example default value
        weapon = new Weapon { strength = 10, lastSlainMonster = 0 }; // Example default weapon
    }
}

// Weapon class to encapsulate weapon properties
public class Weapon
{
    public int strength;
    public int lastSlainMonster = 0; // Replaced durability with lastSlainMonster
}
