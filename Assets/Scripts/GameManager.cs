using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool gameIsOver = false;

    public GameOverScreen gameoverScreen;

    public void EndGame(int health)
    {
        if (gameIsOver == false)
        {
            gameoverScreen.Setup(health);
        }
        
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
