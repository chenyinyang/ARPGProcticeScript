using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BaseCharacterBehavior))]
public class Motion : MonoBehaviour {
    Transform myTransform;
    public Transform model;
    private CharacterController characterController;
    private BaseCharacterBehavior character;
    const float speed = .8f;
    const float rotationSpeed = 2.5f;
    const float jumpTime = .5f;
    const float gravity = 3f;
    float jumpSpeed = 3f;
    float dropDownDis;
    void FallDownCheck() {
        if (!characterController.isGrounded)
        {
            if (vSpeed < 0)
            {
                //空中,速度負der 下墜中,累計距離
                dropDownDis += vSpeed * UPDATE_INTERVAL * -1;
            }
            //空中,速度被拉下來
            vSpeed -= gravity * UPDATE_INTERVAL;
            
        }
        else
        {
            //地面
            //如果有累積下墜距離
            if ((int)dropDownDis > 0)
            {
                //_character.Damage(dropDownDis, DamageType.Nature, false, 2000,false,_character);
                dropDownDis = 0;
            }
            //有往上移動量
            if (vSpeed > 0)
            {
                vSpeed += gravity * UPDATE_INTERVAL * -1;
            }
            else
            {
                //重力常態影響
                vSpeed = gravity * UPDATE_INTERVAL * -1;
            }
        }   
        moveDirectionInTime += myTransform.up * vSpeed * UPDATE_INTERVAL;
        //characterController.Move(myTransform.up * vSpeed * UPDATE_INTERVAL);
    }
    private float vSpeed = 0;
    private bool lockModel;

    const float UPDATE_INTERVAL = .02f;
    float updateTimer = 0;
    Vector3 moveDirectionInTime;
    float rotationAngleInTime;
    void onUpdate() {
        if (character.IsDead)
            CancelInvoke("onUpdate");
        FallDownCheck();

        if (moveDirectionInTime != Vector3.zero)
        {
            if (model && !lockModel && moveDirectionInTime.normalized != Vector3.down && moveDirectionInTime.normalized != Vector3.up)
            {
                model.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(moveDirectionInTime, Vector3.up));
            }
            //有非掉落的移動 , 或者是在不在地上
            if (moveDirectionInTime.normalized != Vector3.down || !characterController.isGrounded)
            {
                if (characterAnimation != null)
                    characterAnimation.PlayRun();
                character.CastingCancel();
            }                      
            characterController.Move(moveDirectionInTime);
            moveDirectionInTime = Vector3.zero;
        }
        if (rotationAngleInTime!=0) {
            myTransform.Rotate(Vector3.up, rotationAngleInTime);
            rotationAngleInTime = 0;
        }
    }

    //AnimationBinding
    ICharacterAnimation characterAnimation;
    public void SetAnimationController(ICharacterAnimation animationController)
    {
        this.characterAnimation = animationController;
    }
    void Awake() {
        characterController = GetComponent<CharacterController>(); 
        character = GetComponent<BaseCharacterBehavior>();
        myTransform = transform;
        jumpSpeed = gravity * jumpTime / 2;
        lockModel = false;
        moveDirectionInTime = Vector3.zero;
    }
    void Start() {
        InvokeRepeating("onUpdate", 0, UPDATE_INTERVAL);
    }
    // Update is called once per frame
    
    public void MoveForward(float direction)
    {              
        if (direction != 0)
        {
            if (characterController.isGrounded)
                moveDirectionInTime += myTransform.forward * direction * speed * Time.deltaTime;
            //characterController.SimpleMove(myTransform.forward * direction  * speed);
            else
                moveDirectionInTime += myTransform.forward * direction * speed * Time.deltaTime;
            //characterController.Move(myTransform.forward * direction * speed * Time.deltaTime);
        }

    }
    public void MoveRight(float direction)
    {   
        if (direction != 0)
        {
            if (characterController.isGrounded)
                moveDirectionInTime += transform.right * (direction > 0 ? 1 : -1) * speed * Time.deltaTime;
                //characterController.SimpleMove(transform.right * (direction > 0 ? 1 : -1) * speed);
            else
                moveDirectionInTime += transform.right * (direction > 0 ? 1 : -1) * speed * Time.deltaTime;
            //characterController.Move(transform.right * (direction > 0 ? 1 : -1) * speed * Time.deltaTime);                        
        }
    }
    public void TurnRight(float direction)
    {
        rotationAngleInTime += direction * rotationSpeed;
    }
    public void JumpUp()
    {
        //Debug.Log(jumpSpeed + " " + jumpTime);
        //character.CastingCancel();
        if (characterController.isGrounded)
        {
            if (characterAnimation != null)
                characterAnimation.PlayJump();
            vSpeed = jumpSpeed;
        }
    }
    public void SkillCasting(bool isCasting)
    {
        lockModel = isCasting;
        //SendMessage("Skilling", isCasting);
    }
    public void Idle() {
        if (characterAnimation != null)
            characterAnimation.PlayIdle();
    }
}
