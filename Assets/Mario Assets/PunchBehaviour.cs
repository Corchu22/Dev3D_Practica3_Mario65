using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    MarioController m_MarioController;
    public MarioController.TPunchType m_PunchType;
    public float m_EnablePunchStartAnimationPct;
    public float m_EnablePunchAnimationPct;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioController = animator.GetComponent<MarioController>();
        m_MarioController.EnableHitCollider(m_PunchType, false);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool l_Enabled = stateInfo.normalizedTime >= m_EnablePunchStartAnimationPct && stateInfo.normalizedTime >= m_EnablePunchAnimationPct;

    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
