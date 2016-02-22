using UnityEngine;
using System.Collections;
using System;

public class CharacterAnimator : MonoBehaviour,ICharacterAnimation {
    private Animator animator;

    public void PlayCast()
    {
        animator.SetTrigger("Casting");
    }

    public void PlayIdle()
    {
        animator.ResetTrigger("Casting");
        //animator.ResetTrigger("Attacking");
    }

    public void PlayJump()
    {
        
    }

    public void PlayRun()
    {
        animator.SetBool("Runing",true);

        animator.SetBool("Walking", false);
    }

    public void PlayScan()
    {
        
    }

    public void PlayWalk()
    {
        animator.SetBool("Walking", true);
        animator.SetBool("Runing", false);
    }

    public void Skill(int number)
    {
       
    }

    void Awake() {
        animator = GetComponentInChildren<Animator>();
        GetComponent<BaseNPCMovementAI>().SetAnimationController(this);
        GetComponent<BaseCharacterBehavior>().onAttackStart += CharacterAnimator_onAttackStart;
    }

    private void CharacterAnimator_onAttackStart()
    {
        animator.SetBool("Attacking",true);
    }

    void Start() { }
}
