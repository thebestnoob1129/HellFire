using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool = "";
    public string isUsingRootMotionBool = "";
    public bool isInteractingStatus = false;
    public bool isUsingRootMotionStatus = false;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isInteractingBool, isInteractingStatus);
        animator.SetBool(isUsingRootMotionBool, isInteractingStatus);
    }

}
