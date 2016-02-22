using UnityEngine;
using System.Collections;
using System;
[RequireComponent(typeof(Motion))]
public class PlayerInput : MonoBehaviour {
    private const string Forward = "Forward";
    private const string RightMove = "RightMove";
    private const string RightTurn = "RightTurn";
    private const string Jump = "Jump";
    private const string Attack = "Attack";
    private const string Skill1 = "Skill1";
    private const string Skill2 = "Skill2";
    private const string Skill3 = "Skill3";
    private const string Skill4 = "Skill4";

    enum MouseButton { Left = 0, Right = 1, Middle = 2, None = 3 }

    private Player player;
    Motion motion;
    // Use this for initialization
    void Start () {
        isAlive = true;
        player = GetComponent<Player>();
        motion = GetComponent<Motion>();
    }
    private bool isAlive;
    void Dead() {
        isAlive = false;
    }
	// Update is called once per frame
	void Update () {
        if (isAlive)
        {
            float forward = Input.GetAxis(Forward);
            if (forward != 0)
            {
                motion.MoveForward(forward);
            }
            float rightMove = Input.GetAxis(RightMove);
            if (rightMove != 0)
            {
                motion.MoveRight(rightMove);
            }
            if (!Input.GetMouseButton((int)MouseButton.Right))
            {
                float righturn = Input.GetAxis(RightTurn);
                if (righturn != 0)
                {
                    motion.TurnRight( righturn);
                }
            }
            else
            {
                rightMove = Input.GetAxis(RightTurn);
                if (rightMove != 0)
                {
                    motion.MoveRight(rightMove);
                }
                float righturn = Input.GetAxis("Mouse X");
                if (righturn != 0)
                {
                    motion.TurnRight(righturn);
                }
            }
            if (rightMove == 0 && forward == 0)
                motion.Idle();
            if (Input.GetButtonDown(Jump))
            {
                motion.JumpUp();
                
            }
            if (Input.GetButtonUp(Attack))
            {
                player.IsAttacking = false;
                player.IsAttacking = true;
            }
            if (Input.GetButtonUp(Skill1))
            {
                player.Skill(1);
                
            }
            if (Input.GetButtonUp(Skill2))
            {
                player.Skill(2);
            }
            if (Input.GetButtonUp(Skill3))
            {
                player.Skill(3);
            }
            if (Input.GetButtonUp(Skill4))
            {
                player.Skill(4);
            }
        }
    }
}
