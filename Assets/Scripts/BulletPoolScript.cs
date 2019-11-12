using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolScript : MonoBehaviour
{
    
    GameObject[] bullets = new GameObject[200];

    [SerializeField]
    GameObject bulletPrefab = null;

    void Start()
    {

        for (int i = 0; i < bullets.Length; i++)
        {

            bullets[i] = Instantiate(bulletPrefab, transform);
            bullets[i].GetComponent<BulletScript>().myPool = gameObject;
            bullets[i].transform.name = "Bullet";

        }
            

    }
    public GameObject GiveMeABullet()
    {

        GameObject chosenBullet = null;

        for (int i = 0; i < bullets.Length; i++)
        {

            if (bullets[i] != null)
            {

                chosenBullet = bullets[i];
                bullets[i] = null;
                break;

            }

        }

        return chosenBullet;

    }

    public void PutMeBackInPool(GameObject bullet)
    {

        for (int i = 0; i < bullets.Length; i++)
        {

            if (bullets[i] == null)
            {

                bullets[i] = bullet;
                break;

            }

        }

    }
    
}
