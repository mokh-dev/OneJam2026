using System.Collections.Generic;
using UnityEngine;

public class SheepEscapeManager : MonoBehaviour
{
    public List<GameObject> sheepList = new List<GameObject>();
    public static SheepEscapeManager SheepManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (SheepManager == null)
        {
            SheepManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void addSheepToList(GameObject sheep)
    {
        sheepList.Add(sheep);
    }

    public void removeSheepFromList(GameObject sheep)
    {
        if (sheepList.Contains(sheep))
        {
            sheepList.Remove(sheep);
        }
    }

    public GameObject GetRandomSheep()
    {
        if (sheepList.Count > 0)
        {
            int randomSheep = Random.Range(0, sheepList.Count + 1);
            return sheepList[randomSheep];
        }
        else
        {
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
