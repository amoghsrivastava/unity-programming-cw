using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCounter : MonoBehaviour
{
    public Text enemiesLeft;
    GameObject[] enemiesInGame;

    public int enemiesL;

    void Update()
    {
        UpdateDetails();
        CheckIfZero();
    }

    void UpdateDetails()
    {
        enemiesInGame = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesLeft.text = enemiesInGame.Length.ToString();

    }

    void CheckIfZero()
    {
        if (enemiesInGame.Length <= 0)
        {
            FindObjectOfType<GameManager>().EndGame(enemiesInGame.Length);
        }
    }
}
