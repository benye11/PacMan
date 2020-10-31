using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject Player;
    public Text HighScore;
    public Text Levels;
    private int highscore;
    public int level;
    public float chase_time = 10;

    void Awake()
     {
         if (FindObjectsOfType(typeof(GameManager)).Length > 1)
             DestroyImmediate(gameObject);
     }
    void Start() {
        Player = GameObject.FindWithTag("Player");
        highscore = 0;
        level = 1;
        chase_time = 10;
        HighScore.text = "High Score\n" + highscore.ToString();
        Levels.text = "Level " + level.ToString();
        DontDestroyOnLoad(gameObject);
    }

    void UpdateHighScore(int score) {
        if (score > highscore) {
            highscore = score;
            HighScore.text = "High Score\n" + highscore.ToString();
        }
    }

    //this, PacMan will send message.
    void StartLevel() {
        level++;
        chase_time += 5;
    }
}
