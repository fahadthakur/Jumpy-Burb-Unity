using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown() {
        RestartLevel();
    }

    private void OnMouseEnter() {
        GetComponent<RectTransform>().sizeDelta = new Vector2(325f, 325f);
    }
    
    private void OnMouseExit() {
        GetComponent<RectTransform>().sizeDelta = new Vector2(263f, 263f);
    }

    public void RestartLevel(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }
}
