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
    }

    public void LassoLetGo()
    {
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
