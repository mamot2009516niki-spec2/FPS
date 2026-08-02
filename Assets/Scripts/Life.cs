using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    int life = 10;
    public Text Lifetext;
    // Start is called before the first frame update
    void Start()
    {
        Lifetext.text = life.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
