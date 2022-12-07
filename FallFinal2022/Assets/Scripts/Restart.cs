using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public bool gameOver = false;

    public Text gameOverText;

    public Text staticText;

    public static bool staticVar = false;

    // Update is called once per frame

    void Update()

    {

        if (Input.GetKey(KeyCode.X))
        {
            gameOverText.text = "The gameOver boolean is true! Press R to Restart";
            gameOver = true;
            staticVar = true;

        }

        if (Input.GetKey(KeyCode.R))

        {
            if (gameOver == true)
            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (Input.GetKey(KeyCode.F))
        {

            staticVar = false;

        }

       if (staticVar == true)
       {
           staticText.text = "The static variable is true!";
       }

       if (staticVar == false)
       {
           staticText.text = "The static variable is false!";
       }

        if (Input.GetKey(KeyCode.L))
        {
            SceneManager.LoadScene("SampleScene 1");
        }

    }
}
