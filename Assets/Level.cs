using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform pipeBody;
    public Transform pipeHead;
    

    private const float PIPE_WIDTH = 1.75f;
    private const float PIPE_HEAD_HEIGHT = 0.98f;
    private const float CAMERA_ORTHO_SIZE = 5;
    private const float PIPE_MOVE_SPEED = 3f;
    private const float PIPE_DESTROY_POS = -13f;
    private const float PIPE_SPAWN_POS = +13f;
    private const float BIRD_POS = -1f;

    private static Level instance;

    public static Level getInstance(){
        return instance;
    }

    private List<Pipe> pipeList;
    private int pipePassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;
    private AudioSource audio;

    public enum Difficulty{
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State{
        WaitingToStart,
        Playing,
        BirdDead,
    }

    private void Awake() {
        instance = this;
        pipeList = new List<Pipe>();
        setDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
        audio = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //createGapPipe(5f, 4f, 5f);
        FlappyBird.getInstance().OnDied += Bird_OnDied;
        FlappyBird.getInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e){
        //print("Dead");
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e){
        //print("Dead");
        state = State.BirdDead;
    }

    

    private void Update(){
        if(state == State.Playing){
            HandlePipeMovement();
            HandleSpawning();
        } 
    }

    private void HandleSpawning(){
        pipeSpawnTimer -= Time.deltaTime;
        if(pipeSpawnTimer < 0){
            //Time to spawn another pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 1f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            
            createGapPipe(height, gapSize, PIPE_SPAWN_POS);
        }    
    }

    private void HandlePipeMovement(){
        for(int i=0; i<pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightOfBird = pipe.GetXPos() > BIRD_POS;
            pipe.Move();
            if(isToTheRightOfBird && pipe.GetXPos() < BIRD_POS){
                //pipe passed the bird
                pipePassedCount++;
                audio.Play();
            }
            if(pipe.GetXPos() < PIPE_DESTROY_POS){
                pipe.destroyPipe();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void setDifficulty(Difficulty difficulty){
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 5.2f;
                pipeSpawnTimerMax = 2.5f;
                break;
            case Difficulty.Medium:
                gapSize = 4f;
                pipeSpawnTimerMax = 2f; 
                break;
            case Difficulty.Hard:
                gapSize = 3f;
                pipeSpawnTimerMax = 1.5f;
                break;
            case Difficulty.Impossible:
                gapSize = 2f;
                pipeSpawnTimerMax = 1f;
                break;
        }
    }

    private Difficulty getDifficulty(){
        if(pipesSpawned >= 20) return Difficulty.Impossible;
        if(pipesSpawned >= 13) return Difficulty.Hard;
        if(pipesSpawned >= 7) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    private void createGapPipe(float gapY, float gapSize, float xPos){
        createPipe(gapY - gapSize * 0.5f, xPos, true);
        createPipe(CAMERA_ORTHO_SIZE*2f - gapY - gapSize * 0.5f, xPos, false);
        pipesSpawned ++;
        setDifficulty(getDifficulty());
    }

    private void createPipe(float height, float xPos, bool createBottom){
        //Setting up pipe head
        Transform pipeHeadPos = Instantiate(pipeHead);
        float pipeHeadYPosition;
        if(createBottom){
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * 0.5f;
        }else
        {
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * 0.5f;
        }
        pipeHeadPos.position = new Vector3(xPos, pipeHeadYPosition);

        //setting up pipe body
        Transform pipeBodPos = Instantiate(pipeBody);
        float pipeBodyYPosition;
        if(createBottom){
            pipeBodyYPosition =  -CAMERA_ORTHO_SIZE;
        }else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBodPos.localScale = new Vector3(1, -1, 1); 
        }
        pipeBodPos.position = new Vector3(xPos, pipeBodyYPosition);
  

        SpriteRenderer pipeBodyRenderer = pipeBodPos.GetComponent<SpriteRenderer>();
        pipeBodyRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBodPos.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe pipe = new Pipe(pipeHeadPos, pipeBodPos);
        pipeList.Add(pipe);
    }

    public int getPipesSpawned(){
        return pipesSpawned;
    }

    public int getPipesPassed(){
        return pipePassedCount / 2;
    }

    //Represents a single Pipe
    private class Pipe{

        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform){
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
        }

        public void Move(){
            pipeHeadTransform.position += new Vector3(-1,0,0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1,0,0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPos(){
            return pipeHeadTransform.position.x;
        }

        public void destroyPipe(){
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}