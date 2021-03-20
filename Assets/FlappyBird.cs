using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlappyBird : MonoBehaviour
{
    public float jumpSpeed = 10f;

    private float timeToGravity = 0.5f; 

    private AudioSource audio;

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    public static FlappyBird instance;

    private State state;

    public static FlappyBird getInstance(){
        return instance;
    }

    private enum State{
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Awake() {
        instance = this;
        GetComponent<Rigidbody2D>().gravityScale = 1f;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
        audio = GetComponent<AudioSource>();
    }

    void Update() {
        switch(state){
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
                {
                    state = State.Playing;
                    GetComponent<Rigidbody2D>().bodyType =  RigidbodyType2D.Dynamic;
                    jump();
                    if(OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
                {
                    jump();
                }
                break;
            case State.Dead:
                break;
        }
        
        timeToGravity -= Time.deltaTime;

        if(timeToGravity <= 0 )
            GetComponent<Rigidbody2D>().gravityScale = 5;
        
    }

    private void jump(){
        GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        audio.Play();
        if(OnDied != null) OnDied (this, EventArgs.Empty);
    }   
}
