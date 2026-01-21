using System.Collections;
using UnityEngine;

public class BanditSpawner : MonoBehaviour
{
    [SerializeField] GameObject bandit;
    [SerializeField] float spawnTime1 = 2f;
    [SerializeField] float spawnTime2 = 5f;
    
    float randomSpawnTime;
    float posX;
    float posY;
    Transform pos1;
    Transform pos2;
    Transform upperPos1;
    Transform upperPos2;
    Transform lowerPos1;
    Transform lowerPos2;
    
    void Start()
    {
        //takes the positions if the 2 children as a reference point
        pos1 = transform.GetChild(0);
        pos2 = transform.GetChild(1);
        upperPos1 = transform.GetChild(2);
        upperPos2 = transform.GetChild(3);
        lowerPos1 = transform.GetChild(4);
        lowerPos2 = transform.GetChild(5);

        StartCoroutine(SpawnScript()); //starts the spawning loop
    }

    //method to spawn Bandits
    IEnumerator SpawnScript()
    {
        while(true)
        {
            //sets the spawnTime of the next bandit
            randomSpawnTime = Random.Range(spawnTime1, spawnTime2);
            yield return new WaitForSeconds(randomSpawnTime);

            chooseSpawnPosition();

            Vector2 spawnPosition = new Vector2(posX, posY); //spawns the bandit

            Instantiate(bandit, spawnPosition, Quaternion.Euler(0, 0, 90)); //spawns the bandit
        }
    }

    void chooseSpawnPosition()
    {
        int spawnRange = Random.Range(1, 4);

        if (spawnRange == 1)
        {
            posX = Random.Range(pos1.position.x, pos2.position.x);
            posY = Random.Range(pos1.position.y, pos2.position.y);
        }
        else if(spawnRange == 2)
        {
            posX = Random.Range(upperPos1.position.x, upperPos2.position.x);
            posY = Random.Range(upperPos1.position.y, upperPos2.position.y);
        }
        else
        {
            posX = Random.Range(lowerPos1.position.x, lowerPos2.position.x);
            posY = Random.Range(lowerPos1.position.y, lowerPos2.position.y);
        }
    }
}
