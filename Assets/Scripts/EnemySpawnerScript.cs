using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    //Basic Bug variables
    [SerializeField]
    GameObject bBugPrefab = null;
    GameObject[] bBugPool = new GameObject[50];

    GameObject startingTile = null;
    GameBoardScript board;
    
    void Start()
    {

        board = GameObject.Find("Game Board").GetComponent<GameBoardScript>();
        startingTile = board.GetPathStart();

        for (int i = 0; i < bBugPool.Length; i++)
        {

            bBugPool[i] = Instantiate(bBugPrefab, transform);
            bBugPool[i].GetComponent<BaseEnemyScript>().InitializeMe("Basic Bug", startingTile, board, this);
            bBugPool[i].transform.name = "Basic Bug";

        }

    }

    public GameObject GiveMeAnEnemy(string type)
    {

        GameObject chosenEnemy = null;

        for (int i = 0; i < bBugPool.Length; i++)
        {

            if (bBugPool[i] != null)
            {

                chosenEnemy = bBugPool[i];
                bBugPool[i] = null;
                break;

            }

        }

        return chosenEnemy;

    }

    public void PutMeBackInPool(GameObject enemy, string type)
    {

        for (int i = 0; i < bBugPool.Length; i++)
        {

            if (bBugPool[i] == null)
            {

                enemy.transform.position = transform.position;
                enemy.transform.parent = transform;
                bBugPool[i] = enemy;
                break;

            }

        }

    }

}
