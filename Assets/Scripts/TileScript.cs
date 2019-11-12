using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    bool isNode = false;
    int sameDirCount = 0;

    GameObject nextNode;
    GameBoardScript gameBoard;

    Vector2 myDirection;
    Vector2 prevDirection;
    Vector2 myPos;

    void Awake()
    {

        gameBoard = GameObject.Find("Game Board").GetComponent<GameBoardScript>();

    }
    void Update(){}

    void OnMouseUp()
    {

        if (!isNode)
        {

            //turn on UI for building towers
            gameBoard.TileSelected(gameObject);

        }

    }
    
    void ChooseNextDirection()
    {
        
        //prev diretion determines which way the path is being built
        if (prevDirection == Vector2.up || prevDirection == Vector2.zero)//first tile in the path has to be included
        {
            //build two tile up if it doesn't exceed board limits, same for left and right as well but 3 tiles
            if (sameDirCount < 2)
                myDirection = Vector2.up;
            else//choose new random direction if condition above is not met
                myDirection = PickRandomDirection();//this function should check if tile is last also

        }
        else if(prevDirection == Vector2.left)
        {

            if (sameDirCount < 3 && myPos.x - (3 - sameDirCount) > 0)
                myDirection = Vector2.left;
            else
                myDirection = PickRandomDirection();

        }
        else if (prevDirection == Vector2.right)
        {

            if (sameDirCount < 3 && myPos.x + (3 - sameDirCount) < gameBoard.GetBoardWidth() - 1)
                myDirection = Vector2.right;
            else
                myDirection = PickRandomDirection();

        }
        
        
        sameDirCount++;
        Color myColor = transform.GetComponent<SpriteRenderer>().color;
        Vector2 nextNodePos = Vector2.zero;
        nextNodePos = myPos + myDirection;
        nextNode = gameBoard.GetNodeByPos(nextNodePos);
        nextNode.GetComponent<TileScript>().TurnMeIntoNode(myColor, myDirection, sameDirCount);
        
    }

    Vector2 PickRandomDirection()
    {
        
        int choice = 0;
        Vector2 newDirection = Vector2.zero;
        bool directionChosen = false;

        while (!directionChosen)
        {
            if(prevDirection == Vector2.up)// limit the time Up can be randomly selected
            {

                choice = Random.Range(0, 2);

                if (choice == 0 && !gameBoard.leftLimitReached)
                    newDirection = Vector2.left;
                else if (choice == 1 && !gameBoard.rightLimitReached)
                    newDirection = Vector2.right;
                
                if(newDirection != Vector2.zero)
                    directionChosen = true;

            }
            else
            {

                choice = Random.Range(0, 3);

                if (choice == 0 && !gameBoard.leftLimitReached)
                    newDirection = Vector2.left;
                else if (choice == 1)
                    newDirection = Vector2.up;
                else if (choice == 2 && !gameBoard.rightLimitReached)
                    newDirection = Vector2.right;

                if (newDirection != -prevDirection && newDirection != Vector2.zero)
                    directionChosen = true;

            }
            
            
        }
        
        sameDirCount = 0;
        return newDirection;

    }

    public void TurnMeIntoNode(Color pathColor, Vector2 direction, int count)
    {

        isNode = true;
        sameDirCount = count;
        transform.GetComponent<SpriteRenderer>().color = pathColor;
        myPos = transform.position;
        prevDirection = direction;
        gameBoard.CheckLimit(myPos);
        gameBoard.AddMeToPathTileList(gameObject);
        //stop building path once you reach the highest row possible
        if (!gameBoard.topLimitReached)
            ChooseNextDirection();
        else
            myDirection = Vector2.zero;

    }

    public bool GetNodeCheck()
    {

        return isNode;

    }

    public Vector2 GetMyDirection()
    {

        return myDirection;

    }

}
