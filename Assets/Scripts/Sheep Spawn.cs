using System.Collections;
using UnityEngine;

public class SheepSpawn : MonoBehaviour
{
    [SerializeField] GameObject Sheep;
    Transform pos1;
    Transform pos2;
    
    void Start()
    {
        pos1 = transform.GetChild(0);
        pos2 = transform.GetChild(1);

        StartCoroutine(SpawnScript(3f));
    }

    void Update()
    {
        
    }

    IEnumerator SpawnScript(float loopTime)
    {
        while(true)
        {
            float posX = Random.Range(pos1.position.x, pos2.position.x);
            float posY = Random.Range(pos1.position.y, pos2.position.y);
            Vector2 spawnPosition = new Vector2(posX, posY);

            Instantiate(Sheep, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(loopTime);
        }
    }
}
