using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text HighScore;
    public Text Levels;
    //private int highscore;
    //public int level;
    //public float ChaseTime;

    public StoreValues values;

    void Start() {
        //Player = GameObject.FindWithTag("Player");
        values = new StoreValues();
        if (StoreValues.initialized == false) {
            StoreValues.highscore = 0;
            StoreValues.level = 1;
            StoreValues.ChaseTime = 0.2f;
            StoreValues.initialized = true;
        }
        else {
            //StoreValues.level++;
            //StoreValues.ChaseTime += 0.2f;
        }
        HighScore.text = "High Score\n" + StoreValues.highscore.ToString();
        Levels.text = "Level " + StoreValues.level.ToString();
        //DontDestroyOnLoad(gameObject);
    }

    void UpdateHighScore(int score) {
        if (score > StoreValues.highscore) {
            StoreValues.highscore = score;
            HighScore.text = "High Score\n" + StoreValues.highscore.ToString();
        }
    }

    //this, PacMan will send message.
    void StartLevel() {
        StoreValues.level++;
        StoreValues.ChaseTime += 0.2f;
    }

    void RestartGame() {
        StoreValues.initialized = false;
        StoreValues.highscore = 0;
        StoreValues.level = 1;
        StoreValues.ChaseTime = 0.2f;
    }
}

public class StoreValues {
    public static int highscore;
    public static int level;
    public static float ChaseTime;
    public static bool initialized = false;
}
