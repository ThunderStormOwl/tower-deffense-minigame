using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{

    float shotCooldown = 0;
    float angle = 0;

    string myType = "";

    Vector2 myTargetRot = Vector2.zero;

    Vector3 myCurRot = Vector3.zero;

    BulletPoolScript bulletPool;

    List <GameObject> myTargets = new List<GameObject>();
    
    public void BuildMe(string type)
    {

        bulletPool = GameObject.Find("Bullet Pool").GetComponent<BulletPoolScript>();
        myType = type;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other != null)
        {

            if(other.transform.tag == "Enemy")
            {

                myTargets.Add(other.gameObject);
                other.GetComponent<BaseEnemyScript>().AddMeToTowersList(gameObject);

            }

        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other != null)
        {

            if (other.transform.tag == "Enemy")
            {
                
                if(myTargets.Contains(other.gameObject))
                    RemoveEnemyFromList(other.gameObject);

                other.GetComponent<BaseEnemyScript>().RemoveMeFromTowersList(gameObject);

            }

        }

    }

    void Update()
    {
       
        if (myTargets.Count > 0)
        {
            
            UpdateRotation();
            
            switch (myType)
            {

                case "Shotgun":
                    ShotgunTowerBehavior();
                    break;

                case "Rifle":
                    RifleTowerBehavior();
                    break;

                case "Assault":
                    AssaultTowerBahavior();
                    break;

            }

        }
            
    }

    void UpdateRotation()
    {

        if(myTargets[0] != null)
        {

            myTargetRot = (Vector2)myTargets[0].transform.position - (Vector2)transform.position;
            angle = Mathf.Atan2(myTargetRot.y, myTargetRot.x) * Mathf.Rad2Deg;
            myCurRot.z = angle - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }
        
    }

    void ShotgunTowerBehavior()
    {

        if(shotCooldown <= Time.time)
        {

            GameObject[] bullets = new GameObject[7];
            for(int i = 0; i < bullets.Length; i++)
            {

                bullets[i] = bulletPool.GiveMeABullet();


            }

            float bulletSpreadX = -0.3f;
            float bulletSpreadY = -0.3f;

            foreach (GameObject bullet in bullets)
            {
                
                bullet.transform.position = transform.position;
                //Get direction between me and first still alive target that entered my collider
                Vector3 targetPlusSpread = myTargets[0].transform.position;
                //need to check if the enemy is going up or down to get the spread in the right direction
                if(myTargets[0].GetComponent<BaseEnemyScript>().GetMyDirection() == Vector2.up)
                {
                    targetPlusSpread.y += bulletSpreadY;
                    bulletSpreadY += 0.1f;
                }
                    
                else
                {
                    targetPlusSpread.x += bulletSpreadX;
                    bulletSpreadX += 0.1f;
                }
                   
                Vector3 dir = targetPlusSpread - transform.position;
                
                bullet.GetComponent<BulletScript>().ShootMe(dir.normalized, 1);
                

            }
                
            shotCooldown = Time.time + 2;

        }

    }

    void RifleTowerBehavior()
    {

        if (shotCooldown <= Time.time)
        {

            GameObject bullet = bulletPool.GiveMeABullet();
            bullet.transform.position = transform.position;
            Vector3 dir = myTargets[0].transform.position - transform.position;
            bullet.GetComponent<BulletScript>().ShootMe(dir.normalized, 7);
            
            shotCooldown = Time.time + 3;

        }

    }

    void AssaultTowerBahavior()
    {

        if (shotCooldown <= Time.time)
        {

            GameObject bullet = bulletPool.GiveMeABullet();
            bullet.transform.position = transform.position;
            Vector3 dir = myTargets[0].transform.position - transform.position;
            bullet.GetComponent<BulletScript>().ShootMe(dir.normalized, 2);//2

            shotCooldown = Time.time + 0.25f;

        }

    }

    public void RemoveEnemyFromList(GameObject enemy)
    {

        myTargets.Remove(enemy);

    }

}
