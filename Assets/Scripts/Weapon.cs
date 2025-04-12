/// <summary>
/// Represents a weapon in the game with strength and tracking of the last slain monster.
/// </summary>
[System.Serializable]
public class Weapon
{
    /// <summary>
    /// The strength of the weapon, used to determine damage dealt to monsters.
    /// </summary>
    public int strength;

    /// <summary>
    /// The rank of the last monster slain with this weapon.
    /// </summary>
    public int lastSlainMonster;
}
