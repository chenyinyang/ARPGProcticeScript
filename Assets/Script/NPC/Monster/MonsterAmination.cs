using UnityEngine;
using System.Collections;
using System;

public class MonsterAmination : MonoBehaviour, ICharacterAnimation
{
    private Animator _animator;
    private const string WALKING = "Walking";
    private const string RUNNING = "Running";
    private const string DEAD = "Dead";
    private const string ATTACKING = "Attacking";
    private const string SKILLING = "Skilling";
    private const string TAKE_DAMAGE = "TakeDamage";
    private const string PUSHED = "Pushed";
    private const string SEARCH = "Search";
    private bool startAttack = false;
    private BaseCharacterBehavior character;
    void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }
	// Use this for initialization
	void Start () {
        if (transform.GetComponent<BaseNPCMovementAI>() != null)
        {
            transform.GetComponent<BaseNPCMovementAI>().SetAnimationController(this);
        }
        character = GetComponent<BaseCharacterBehavior>();
        
        character.onAttackStart += Attack;
        character.OnDead += Dead;
        character.OnDamaged += GetDamage;
        _animator.Play("Walk");
	}
	
	// Update is called once per frame
	void Update () {
        if (startAttack && !_animator.GetBool(ATTACKING))
        {
            //Debug.Log("MONSTER NOT IN ATTACK");
            if (character != null)
                character.IsAttacking = false;
            startAttack = false;
        }
        if (startSkill && !_animator.GetBool(SKILLING))
        {
            //Debug.Log("MONSTER NOT IN SKILL");
            SendMessage("SkillCasting", false);
            startSkill = false;
        }
	}
    public void PlayIdle() {
        _animator.SetBool(SEARCH, false);
        _animator.SetBool(WALKING, false);
        _animator.SetBool(RUNNING, false);
    }
    public void PlayScan() {
        _animator.SetBool(SEARCH, true);
        _animator.SetBool(WALKING, false);
        _animator.SetBool(RUNNING, false);
    }
   
    public void GetDamage(float originalDamage, DamageType type, float damageCause, BaseCharacterBehavior attackTo, BaseCharacterBehavior attackFrom)
    {
        PlayIdle();
        _animator.Play("Damage");
    }
    
    public void Attack() {
        if (!_animator.GetBool(ATTACKING))
        {
            //Debug.Log("Monster Attack");
            PlayIdle();
            _animator.SetBool(ATTACKING, true);
            //_animator.Play("Attack");
            if (character != null)
            {
                character.IsAttacking = false;
                character.IsAttacking = true;
            }
            startAttack = true;
        }
    }
    private bool startSkill = false;
    public void Skill(int number) {

        if (!_animator.GetBool(SKILLING))
        {         
            PlayIdle();
            _animator.SetBool(SKILLING, true);        
            SendMessage("SkillCasting", true);
            startSkill = true;
        }
        
    }
    public void Dead(BaseCharacterBehavior npc) {
        _animator.SetBool(DEAD, true);
    }

    public void PlayWalk()
    {
        _animator.SetBool(SEARCH, false);
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(WALKING, true);
    }

    public void PlayRun()
    {
        _animator.SetBool(SEARCH, false);
        _animator.SetBool(RUNNING, true);
        _animator.SetBool(WALKING, false);
    }

    public void PlayCast()
    {
        _animator.SetBool(SEARCH, false);
        _animator.SetBool(RUNNING, false);
        _animator.SetBool(WALKING, false);
    }

    public void PlayJump()
    {
        
    }
}
