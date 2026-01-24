using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepEscapeManager : MonoBehaviour
{
    [SerializeField] int maxEscapedSheep = 3;
    [SerializeField] float minTimeToEscape = 2f;
    [SerializeField] float maxTimeToEscape = 5f;
    

    public List<GameObject> sheepList = new List<GameObject>();
    public static SheepEscapeManager Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(escapeSheep());
    }

    public void addSheepToList(GameObject sheep)
    {
        sheepList.Add(sheep);
        SheepBehaviour behaviour = sheep.GetComponent<SheepBehaviour>();
        behaviour.setIsEscaped(false);
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
            int randomSheep = Random.Range(0, sheepList.Count);
            return sheepList[randomSheep];
        }
        else
        {
            return null;
        }
    }

IEnumerator escapeSheep()
{
    while (true)
    {
        yield return new WaitUntil(() => SheepBehaviour.numOfEscaped < maxEscapedSheep);
        yield return new WaitForSeconds(Random.Range(minTimeToEscape, maxTimeToEscape));
        
        if (sheepList.Count > 0) 
        {
            GameObject candidateSheep = GetRandomSheep();
            if (candidateSheep != null)
            {
                SheepBehaviour behaviour = candidateSheep.GetComponent<SheepBehaviour>();
                if (behaviour != null)
                {
                    if (behaviour != null)
                    {
                        behaviour.EscapePen();
                    }
                }
            }
        }
        else
        {
            yield return null;
        }
    }
}

    bool isSheepEscaped(GameObject sheep)
    {
        SheepBehaviour behaviour = sheep.GetComponent<SheepBehaviour>();
        return behaviour.getIsEscape();
    }
}
