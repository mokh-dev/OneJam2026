using System.Collections.Generic;
using UnityEngine;

public class SheepEscapeManager : MonoBehaviour
{
    [SerializeField] int maxEscapedSheep = 3;
    [SerializeField] float minTimeToEscape = 2f;
    [SerializeField] float maxTimeToEscape = 5f;

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

    // Update is called once per frame
    void Update()
    {
        if(maxEscapedSheep < 3)
        {
            
        }
    }

    public void addSheepToList(GameObject sheep)
    {
        sheepList.Add(sheep);
        SheepBehaviour behaviour = sheep.GetComponent<SheepBehaviour>();
        behaviour.setIsEscaped(false);
        behaviour.flagForEscape(false);
    }

    public void removeSheepFromList(GameObject sheep)
    {
        if (sheepList.Contains(sheep))
        {
            sheepList.Remove(sheep);
        }
    }

    public void killSheep(GameObject sheep)
    {
        if (sheepList.Contains(sheep))
        {
            sheepList.Remove(sheep);
            Destroy(sheep);
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

    void escapeSheep(GameObject sheep, bool escaped)
    {
        SheepBehaviour behaviour = sheep.GetComponent<SheepBehaviour>();
        behaviour.setIsEscaped(escaped);
        behaviour.EscapePen();
    }

    void escapeSheep(GameObject sheep)
    {
        this.escapeSheep (sheep, true);
    }

    bool isSheepEscaped(GameObject sheep)
    {
        SheepBehaviour behaviour = sheep.GetComponent<SheepBehaviour>();
        return behaviour.getIsEscape();
    }
}
