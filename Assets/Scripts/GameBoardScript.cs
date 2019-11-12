using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardScript : MonoBehaviour
{
    //private variables
    bool spawning = false;
    bool waveFinished = false;

    int morale = 100;
    int enemiesSpawned = 0;

    float startWaveTimer = 0;
    float spawnTimer = 0;

    Transform buildRoomUI = null;

    List<GameObject> pathTiles = new List<GameObject>();

    public GameObject selectedTile = null;
    GameObject[,] board;

    //public variables
    public bool leftLimitReached = false;
    public bool rightLimitReached = false;
    public bool topLimitReached = false;

    public int lives = 3;
    
    //UI Variables
    Canvas myCanvas;

    Text moraleMeterText;
    Text waveTimerText;

    //serialized variables
    [SerializeField]
    int boardSizeX = 50;
    [SerializeField]
    int boardSizeY = 50;
    [SerializeField]
    int shotgunCost = 0;
    [SerializeField]
    int rifleCost = 0;
    [SerializeField]
    int assaultCost = 0;

    [SerializeField]
    GameObject tilePrefab = null;
    [SerializeField]
    GameObject towerPrefab = null;

    //sprite/amimation variables
    Color regularTileColor = new Color(0, 0, 0.3f);
    Color pathTileColor = new Color(0.4f, 0.4f, 0.4f);
    
    //input variables
    bool DetectedMouseDown { get { return Input.GetMouseButtonUp(0); } }

    Vector2 CurrentClickPosition
    {
        get
        {
            Vector2 inputPos;
            inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return inputPos;
        }
    }

    RaycastHit2D detectedObject;

    void Start()
    {

        startWaveTimer = 10 + Time.time;

        board = new GameObject[boardSizeX, boardSizeY];

        myCanvas = gameObject.GetComponentInChildren<Canvas>();
        buildRoomUI = myCanvas.transform.Find("Build Tower UI");
        moraleMeterText = myCanvas.transform.Find("Morale Meter").GetComponent<Text>();
        waveTimerText = myCanvas.transform.Find("Timer").GetComponent<Text>();

        moraleMeterText.text = "Morale: " + morale;
        
        Vector2 myPos = Vector2.zero;

        for(int y = 0; y < boardSizeY; y++)
        {

            myPos.y = y;

            for (int x = 0; x < boardSizeX; x++)
            {
                
                myPos.x = x;
                board[x, y] = Instantiate(tilePrefab, transform) as GameObject;
                board[x, y].transform.position = myPos;
                board[x, y].transform.name = "Tile " + x + " " + y;
                board[x, y].GetComponent<SpriteRenderer>().color = regularTileColor;

            }

        }

        int X = (int)boardSizeX / 2; //Random.Range(1, boardSizeX -1);
        board[X, 0].GetComponent<TileScript>().TurnMeIntoNode(pathTileColor, Vector2.zero, 0);

    }
    void Update()
    {

        DetectInput();

        //Wave Stuff
        if (startWaveTimer <= Time.time && !waveFinished)
            StartWave();
        else if(startWaveTimer - Time.time > 0)
            waveTimerText.text = (int)(startWaveTimer - Time.time) + " Seconds";

        if (spawning && spawnTimer <= Time.time && enemiesSpawned < 20)
            SpawnEnemy();
        else if (enemiesSpawned > 20)
            spawning = false;


    }

    void DetectInput()
    {

        if (DetectedMouseDown)
        {

            Vector2 inputPos;
            inputPos = CurrentClickPosition;
            detectedObject = Physics2D.Raycast(inputPos, Vector2.zero);

            if (detectedObject.transform != null)
            {//if d=etec a node that isnt "blue" than turn off UI
                
                if (detectedObject.transform.tag == "Tile")
                {

                    bool isNode = detectedObject.transform.GetComponent<TileScript>().GetNodeCheck();
                    if (isNode)
                    {

                        if (selectedTile != null)
                        {

                            selectedTile.GetComponent<SpriteRenderer>().color = regularTileColor;
                            selectedTile = null;

                        }
                        if (buildRoomUI.gameObject.activeInHierarchy == true)
                            buildRoomUI.gameObject.SetActive(false);

                    }


                }

            }
            else
            {

                if (selectedTile != null)
                {

                    selectedTile.GetComponent<SpriteRenderer>().color = regularTileColor;
                    selectedTile = null;

                }
                if (buildRoomUI.gameObject.activeInHierarchy == true)
                    buildRoomUI.gameObject.SetActive(false);

            }

        }

    }

    public void CheckLimit(Vector2 index)
    {

        if (index.y == boardSizeY - 1)
            topLimitReached = true;

        if (index.x == 1)
            leftLimitReached = true;
        else if (index.x > 0 && leftLimitReached)
            leftLimitReached = false;

        if (index.x == boardSizeX - 2)
            rightLimitReached = true;
        else if (index.x < boardSizeX -1 && rightLimitReached)
            rightLimitReached = false;

    }

    public void TileSelected(GameObject tile)
    {//handles when user presses a tile

        if (selectedTile != null)
        {//turn off currently selected tile UI
            
            selectedTile.GetComponent<SpriteRenderer>().color = regularTileColor;

        }
        
        tile.GetComponent<SpriteRenderer>().color = Color.green;
        selectedTile = tile;

        if(buildRoomUI.gameObject.activeInHierarchy == false)
            buildRoomUI.gameObject.SetActive(true);

    }

    public void BuildTower(string type)
    {

        GameObject newTower = null;
        
        switch (type)
        {

            case "Shotgun":

                if (morale >= shotgunCost)
                {

                    newTower = Instantiate(towerPrefab, selectedTile.transform);
                    morale -= shotgunCost;
                    moraleMeterText.text = "Morale: " + morale;


                }   
                else
                {

                    print("not enough morale!");
                    return;

                }

                newTower.GetComponent<SpriteRenderer>().color = Color.red;
                newTower.transform.name = "Shotgun Survivor";
                
                break;

            case "Rifle":

                if (morale >= rifleCost)
                {

                    newTower = Instantiate(towerPrefab, selectedTile.transform);
                    newTower.GetComponent<CircleCollider2D>().radius = 5;
                    morale -= rifleCost;
                    moraleMeterText.text = "Morale: " + morale;

                }
                else
                {

                    print("not enough morale!");
                    return;

                }

                newTower.GetComponent<SpriteRenderer>().color = Color.yellow;
                newTower.transform.name = "Rifle Survivor";

                break;

            case "Assault":

                if (morale >= assaultCost)
                {

                    newTower = Instantiate(towerPrefab, selectedTile.transform);
                    newTower.GetComponent<CircleCollider2D>().radius = 2.5f;
                    morale -= assaultCost;
                    moraleMeterText.text = "Morale: " + morale;

                }
                else
                {

                    print("not enough morale!");
                    return;

                }

                newTower.GetComponent<SpriteRenderer>().color = Color.cyan;
                newTower.transform.name = "Assault Survivor";

                break;

        }

        newTower.GetComponent<TowerScript>().BuildMe(type);

    }

    public void GainMorale(int ammount)
    {

        morale += ammount;
        moraleMeterText.text = "Morale: " + morale;

    }

    public void AddMeToPathTileList(GameObject tile)
    {

        pathTiles.Add(tile);

    }

    public GameObject GetPathStart()
    {

        return pathTiles[0];

    }

    public GameObject GetNodeByPos(Vector2 pos)
    {
        
        return board[(int)pos.x, (int)pos.y];

    }

    public int GetBoardHeight()
    {

        return boardSizeY;

    }

    public int GetBoardWidth()
    {

        return boardSizeY;

    }

    //probably want to make this a separate script "Enemy Wave Manager"

    EnemySpawnerScript enemySpawner;

    void StartWave()
    {

        enemySpawner = GameObject.Find("Enemy Spawner").GetComponent<EnemySpawnerScript>();

        SpawnEnemy();

        waveFinished = true;
        spawning = true;
        
    }

    void SpawnEnemy()
    {
        
        GameObject newEnemy = enemySpawner.GiveMeAnEnemy("Basic Bug");
        newEnemy.GetComponent<BaseEnemyScript>().WakeUp();
        newEnemy.transform.parent = GameObject.Find("Wave Manager").transform;
        enemiesSpawned++;
        spawnTimer = 0.1f + Time.time;

    }

}