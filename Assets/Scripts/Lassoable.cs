using UnityEngine;

public class Lassoable : MonoBehaviour
{
    private bool isCaught;
    private Lasso connectedLasso;
    
    public void LassoConnected(Lasso lasso)
    {
        isCaught = true;
        connectedLasso = lasso;
    }

    public void LassoLetGo()
    {
        isCaught = false;
    }


    void Update()
    {
        if (isCaught)
        {
            transform.position = connectedLasso.gameObject.transform.position;
        }
    }
}
