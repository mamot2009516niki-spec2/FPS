using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter: MonoBehaviour
{
    public Text scoreText;
    public static int destroyedEnemyCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = destroyedEnemyCount.ToString();
    }
}
