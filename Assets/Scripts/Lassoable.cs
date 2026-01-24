using UnityEngine;

public class Lassoable : MonoBehaviour
{
    private bool isCaught;
    private Lasso connectedLasso;
    
    public void LassoConnected(Lasso lasso)
    {
        isCaught = true;
        connectedLasso = lasso;
        if(gameObject.TryGetComponent<BanditBehaviour>(out BanditBehaviour banditBehaviour) == true)
        {
            banditBehaviour.setIsRecovering(true);
        }

        if(gameObject.TryGetComponent<SheepBehaviour>(out SheepBehaviour sheepBehaviour) == true)
        {
            if (sheepBehaviour.GetGrabbedBy() != null)
            {
                sheepBehaviour.GetGrabbedBy().GetComponent<BanditBehaviour>().DropSheep();
            }
            sheepBehaviour.setIsRecovering(true);
        }
    }

    public void LassoLetGo()
    {
        if(gameObject.TryGetComponent<SheepBehaviour>(out SheepBehaviour sheepBehaviour) == true)
        {
            if (sheepBehaviour.GetGrabbedBy() != null)
            {
                sheepBehaviour.GetGrabbedBy().GetComponent<BanditBehaviour>().DropSheep();
            }
            StartCoroutine(sheepBehaviour.StartRecovery());
        }
        isCaught = false;
        if(gameObject.TryGetComponent<BanditBehaviour>(out BanditBehaviour banditBehaviour) == true)
        {
            StartCoroutine(banditBehaviour.StartRecovery());
        }
    }


    void Update()
    {
        if (isCaught)
        {
            transform.position = connectedLasso.gameObject.transform.position;
        }
    }
}
