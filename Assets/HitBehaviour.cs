using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBehaviour : StateMachineBehaviour
{
    MarioController m_MarioController;
    public MarioController.TPunchType m_PunchType;
    public float m_EnablePunchStartAnimationPct;
    public float m_EnablePunchEndAnimationPct;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioController = animator.GetComponent<MarioController>();
        m_MarioController.EnableHitCollider(m_PunchType, false);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool l_Enabled = stateInfo.normalizedTime >= m_EnablePunchStartAnimationPct && stateInfo.normalizedTime >= m_EnablePunchEndAnimationPct;
        m_MarioController.EnableHitCollider(m_PunchType, l_Enabled);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioController.EnableHitCollider(m_PunchType, false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
