using System.Collections;
using UnityEngine;

public class BanditSpawner : MonoBehaviour
{
    [SerializeField] GameObject bandit;
    [SerializeField] float spawnTime1 = 2f;
    [SerializeField] float spawnTime2 = 5f;
    
    float randomSpawnTime;
    Transform pos1;
    Transform pos2;
    
    void Start()
    {
        //takes the positions if the 2 children as a reference point
        pos1 = transform.GetChild(0);
        pos2 = transform.GetChild(1);

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

            //sets the spawn location of the bandit
            float posX = Random.Range(pos1.position.x, pos2.position.x);
            float posY = Random.Range(pos1.position.y, pos2.position.y);
            Vector2 spawnPosition = new Vector2(posX, posY);

            Instantiate(bandit, spawnPosition, Quaternion.Euler(0, 0, 90)); //spawns the bandit
        }
    }
}
