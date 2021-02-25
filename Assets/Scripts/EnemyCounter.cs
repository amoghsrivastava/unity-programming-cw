using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCounter : MonoBehaviour
{
    public Text enemiesLeft;
    GameObject[] enemiesInGame;

    void Update()
    {
        UpdateDetails();
    }

    void UpdateDetails()
    {
        enemiesInGame = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesLeft.text = enemiesInGame.Length.ToString();
    }
}
