using UnityEngine;
using System.Collections;
using System;
[AddComponentMenu("Game/SDRPG Animation Control")]
public class PlayerAnimation : MonoBehaviour, ICharacterAnimation
{
    public AnimationClip[] AttackAmin;
    public AnimationClip[] DamagedAmin;
    public AnimationClip[] DieAmin;
    public AnimationClip[] IdleAmin;
    public AnimationClip[] JumpAmin;
    public AnimationClip[] RunAmin;
    public AnimationClip[] WalkAmin;

    private enum AnimationState {
        Attack,
        Damaged,
        Die,
        Idle,
        Jump,
        Run,
        Walk
    }

    private AnimationState state;
    private Animation _animation;
    private float _idleStateBufferTime = 0.2f;
    private float _idleStateBufferTimer = 0f;
    private bool isLockCombatLayer=false;
    private BaseCharacterBehavior character;
    // Use this for initialization
    void Start () {
        isAlive = true;
        state = AnimationState.Idle;
        _animation = GetComponentInChildren<Animation>();
        foreach (AnimationClip anim in JumpAmin)
        {

            _animation[anim.name].layer = 5;
        }
        foreach (AnimationClip anim in AttackAmin)
        {
            _animation[anim.name].layer = 6;
            _animation[anim.name].AddMixingTransform(transform);
            _animation[anim.name].speed = 1f;
        }

        foreach (AnimationClip anim in DamagedAmin)
        {
            _animation[anim.name].layer = 7;
        }

        if(transform.GetComponent<BaseNPCMovementAI>() != null){
            transform.GetComponent<BaseNPCMovementAI>().SetAnimationController(this);
        }
        if(transform.GetComponent<Motion>() != null){
            transform.GetComponent<Motion>().SetAnimationController(this);
        }
        character = GetComponent<BaseCharacterBehavior>();
        character.onAttackStart += Attack;
        character.OnDead += Dead;
        character.OnDamaged += GetDamage;

        StartCoroutine("FSM");
	}

    private void Dead(BaseCharacterBehavior npc)
    {
        isAlive = false;
        _animation.Play(DieAmin[0].name, PlayMode.StopAll);
    }

    IEnumerator FSM() {
        while (isAlive)
        {           
            switch (state)
            {
                case AnimationState.Idle:
                    Idle();
                    break;               
                case AnimationState.Run:
                    Run();
                    break;
                case AnimationState.Walk:
                    Walk();
                    break;
                default:
                    break;
            }            
            yield return 0;
        }
    }
    // Update is called once per frame
    float attackRestTimer = 0f;
    float comboAttckInterval = .2f;
	void Update () {
        if (character.IsAttacking)
        {            
            if (!_animation.IsPlaying(AttackAmin[curAttackActIndex].name)) {
                attackRestTimer = Time.time;               
                character.IsAttacking = false;
            }
        }

    }
    int curAttackActIndex;
    private bool isAlive;
    public void Attack() {
        if (!character.IsSkilling)
        {
            if (attackRestTimer == 0 ||Time.time - attackRestTimer < comboAttckInterval)
            {
                curAttackActIndex = curAttackActIndex >= AttackAmin.Length - 1 ? 0 : curAttackActIndex + 1;
            }
            else
            {
                attackRestTimer = 0;
                curAttackActIndex = 0;
            }                   
            if (JumpAmin.Length>0 && _animation.IsPlaying(JumpAmin[0].name))
                curAttackActIndex = 2;
            _animation.Play(AttackAmin[curAttackActIndex].name, PlayMode.StopAll);
            
        }
    }
    public void GetDamage(float originalDamage, DamageType type, float damageCause, BaseCharacterBehavior attackTo, BaseCharacterBehavior attackFrom)
    {
        if(!character.IsSkilling && !character.IsAttacking)
            _animation.Play(DamagedAmin[0].name);
        
    }
    public void Idle() {
        if(Time.time - _idleStateBufferTimer >_idleStateBufferTime)
             _animation.Play(IdleAmin[0].name);
    }
    void Jump  (){
        _animation.Play(JumpAmin[0].name, PlayMode.StopAll);        
    }
    void Run(){
        _animation.Play(RunAmin[0].name);
        _idleStateBufferTimer = Time.time;
        state = AnimationState.Run;
    }
    void Walk() {
        _animation.Play(WalkAmin[0].name);
        _idleStateBufferTimer = Time.time;
        state = AnimationState.Walk;
    }

    public void PlayIdle()
    {
        state = AnimationState.Idle;
    }
   
    void PlayAnimation(string name) {
        _animation.Play(name);
    }   
    
    public void PlayScan()
    {
        //throw new NotImplementedException();
    }
    public void Skill(int number)
    {
        //throw new NotImplementedException();
    }

    public void PlayWalk()
    {
        state = AnimationState.Walk;
    }

    public void PlayRun()
    {
        state = AnimationState.Run;
    }

    public void PlayCast()
    {
        
    }

    public void PlayJump()
    {
        if (!character.IsSkilling)
            Jump();
    }
}
