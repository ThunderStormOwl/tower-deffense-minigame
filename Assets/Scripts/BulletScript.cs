using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    float lifeTimer = 0;
    readonly float moveSpeed = 10;
    int myDmg = 0;
    
    Vector2 myDirection = Vector2.zero;

    public GameObject myPool = null;

    void Start()
    {
        
    }
    
    void Update()
    {

        if(myDirection != Vector2.zero)
        {

            if (lifeTimer <= Time.time)
                SendMeBackToPool();

            transform.localPosition += (Vector3)(myDirection * moveSpeed) * Time.deltaTime;

        }
        
    }

    public void ShootMe(Vector2 direction, int dmg)
    {

        myDirection = direction;
        myDmg = dmg;
        lifeTimer = Time.time + 2;

    }

    public void SendMeBackToPool()
    {

        myDmg = 0;
        lifeTimer = 0;
        myDirection = Vector2.zero;
        transform.position = myPool.transform.position;
        myPool.GetComponent<BulletPoolScript>().PutMeBackInPool(gameObject);

    }

    public int GetDamage()
    {

        return myDmg;

    }
    
}
