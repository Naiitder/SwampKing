using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private int maximumHealth, currentHealth;
    private int damage;
    
    
    public int MaximumHealth {get{return maximumHealth;} set{maximumHealth = value;}}
    public int CurrentHealth {get{return currentHealth;} set{currentHealth = value;}}
    public int Damage {get{return damage;} set{damage = value;}}
    
}
