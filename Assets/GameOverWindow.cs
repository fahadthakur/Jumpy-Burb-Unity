using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake(){
        scoreText = transform.Find("Text_curScore").GetComponent<Text>();
        
    }

    private void Start() {
        FlappyBird.getInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Bird_OnDied(object sneder, System.EventArgs e){
        Show();
        scoreText.text = Level.getInstance().getPipesPassed().ToString();
        
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    private void Show(){
        gameObject.SetActive(true);
    }
}