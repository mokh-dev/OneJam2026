using System.Collections;
using UnityEngine;

public class SheepSpawn : MonoBehaviour
{
    [SerializeField] GameObject sheep;
    [SerializeField] float spawnTime1 = 2f;
    [SerializeField] float spawnTime2 = 5f;
    [SerializeField] int numOfSheep;
    
    SheepEscapeManager escapeManager;
    GameObject Perimeter;

    float randomSpawnTime;
    Transform pos1;
    Transform pos2;
    
    void Start()
    {
        //takes the positions if the 2 children as a reference point
        pos1 = transform.GetChild(0);
        pos2 = transform.GetChild(1);

        Perimeter = GameObject.FindGameObjectWithTag("Perimeter");
        escapeManager = Perimeter.GetComponent<SheepEscapeManager>();

        SpawnScript(numOfSheep); //starts the spawning loop
    }

    //method to spawn sheep
    void SpawnScript(int numOfSheeps)
    {
        for(int i = 0; i < numOfSheeps; i++)
        {
            //sets the spawnTime of the next sheep
            randomSpawnTime = Random.Range(spawnTime1, spawnTime2);

            //sets the spawn location of the sheep
            float posX = Random.Range(pos1.position.x, pos2.position.x);
            float posY = Random.Range(pos1.position.y, pos2.position.y);
            Vector2 spawnPosition = new Vector2(posX, posY);

            GameObject newSheep = Instantiate(sheep, spawnPosition, Quaternion.Euler(0, 0, 90)); //spawns the sheep
            escapeManager.addSheepToList(newSheep);
        }
    }
}
