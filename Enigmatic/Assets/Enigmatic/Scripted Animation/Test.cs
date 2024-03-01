using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private ScriptedAnimationController ScriptedAnimationController;
    

    private void Start()
    {
        ScriptedAnimationController.FlowMove(transform, new Vector3(0, 0.5f, 0), 5)
            .AddFinishedLisener(Finish).Safely(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ScriptedAnimationController.FlowMove(transform, new Vector3(0, 0.5f, 0), 20).Safely(this);
            ScriptedAnimationController.GetAnimationBySender(this).AddFinishedLisener(Finish);
        }
    }

    private void Finish()
    {
        Debug.LogWarning("Is Finished!");
    } 
}
