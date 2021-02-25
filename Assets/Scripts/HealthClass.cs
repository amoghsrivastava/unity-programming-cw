using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthClass
{
    private int maxHealth;
    private int health;

    // Declaring fields
    public int MaxHealth { 
        get { return maxHealth; } 
        set { maxHealth = value; } 
    }

    public int CurrentHealth
    {
        get { return health; }
        set { health = value; }
    }

    // Class constructor
    public HealthClass(int _maxHealth)
    {
        MaxHealth = _maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void ReduceHealth(int damage)
    {
        CurrentHealth -= damage;
    }

    // Returns a floating point number of the current health
    public float CalculateCurrentHealth()
    {
        return (float)CurrentHealth / (float)MaxHealth;
    }
}
