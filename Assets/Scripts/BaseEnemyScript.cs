using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyScript : MonoBehaviour
{

    int health = 0;

    float moveSpeed = 0;
    //keeps them from bunching up when walking
    float myOffSetX = 0;
    float myOffSetY = 0;
    float winTimer = 0;//need to make them cost "tries" when they win

    bool isFrozen = true;
    bool endReached = false;

    Vector2 curDirection = Vector2.zero;


    GameObject curTile = null;

    GameBoardScript board;
    EnemySpawnerScript myPool;

    List<GameObject> towersFiringOnMe = new List<GameObject>();

    void Update()
    {
        
        if (!isFrozen)
        {

            if (health <= 0)
            {

                ResetMe();
                board.GainMorale(5);
                foreach (GameObject tower in towersFiringOnMe)
                    tower.GetComponent<TowerScript>().RemoveEnemyFromList(gameObject);

            }
            if (winTimer <= Time.time && endReached)
            {
                
                board.lives--;
                ResetMe();

            }

            if (curDirection == Vector2.zero)
            {//Reached the end of the path

                endReached = true;
                curDirection = Vector2.up;
                winTimer = Time.time + 2;

            }

            transform.localPosition += (Vector3)(curDirection * moveSpeed) * Time.deltaTime;


            if (!endReached)
            {

                if (transform.position.y - myOffSetY >= curTile.transform.position.y && curDirection == Vector2.up)
                {

                    curDirection = curTile.GetComponent<TileScript>().GetMyDirection();
                    GetNextTile();

                }

                else if (transform.position.x + myOffSetX <= curTile.transform.position.x && curDirection == Vector2.left)
                {

                    curDirection = curTile.GetComponent<TileScript>().GetMyDirection();
                    GetNextTile();

                }
                else if (transform.position.x + myOffSetX >= curTile.transform.position.x && curDirection == Vector2.right)
                {

                    curDirection = curTile.GetComponent<TileScript>().GetMyDirection();
                    GetNextTile();

                }

            }
            
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if(other != null)
        {

            if(other.transform.tag == "Bullet")
            {

                health -= other.GetComponent<BulletScript>().GetDamage();
                other.GetComponent<BulletScript>().SendMeBackToPool();

            }

        }
        
    }

    void GetNextTile()
    {

        Vector2 index = Vector2.zero;
        index = (Vector2)curTile.transform.position;
        index += curDirection;
        curTile = board.GetNodeByPos(index);

    }
    
    void ResetMe()
    {

        myPool.PutMeBackInPool(gameObject, "Basic Bug");
        transform.parent = myPool.transform;
        transform.position = myPool.transform.position;
        curDirection = Vector2.zero;
        endReached = false;
        FreezeMe();

    }

    public void WakeUp()
    {

        Vector3 pos = curTile.transform.position;
        pos.y += myOffSetY - 1;
        pos.x += myOffSetX;
        transform.position = pos;

        isFrozen = false;

    }

    public void InitializeMe(string name, GameObject tile, GameBoardScript gameBoard, EnemySpawnerScript spawner)
    {

        myPool = spawner;
        myOffSetY = Random.Range(-0.4f, 0.4f);
        myOffSetX = Random.Range(-0.4f, 0.4f);
        board = gameBoard;
        health = 15;
        moveSpeed = 1;
        curTile = tile;
        curDirection = curTile.GetComponent<TileScript>().GetMyDirection();

    }
    
    public void FreezeMe()
    {

        isFrozen = true;

    }

    public Vector2 GetMyDirection()
    {

        return curDirection;

    }

    public void AddMeToTowersList(GameObject newTower)
    {

        towersFiringOnMe.Add(newTower);

    }

    public void RemoveMeFromTowersList(GameObject tower)
    {

        towersFiringOnMe.Remove(tower);

    }

}
