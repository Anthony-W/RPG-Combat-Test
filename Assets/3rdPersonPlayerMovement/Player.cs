using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    // TODO: take deltaTime into account for movement

    private Character target; // enemy or npc selected

    CameraRaycaster cr; // raycaster used for detecting clicked objects/characters
    CameraHolder ch; // parent of camera, used to more easily rotate camera around character

    bool directMovement = true; // whether or not WASD movement is being used
    bool autoRun = false; // whether or not auto run is active
    bool characterRotating = false; // whether or not character is currently rotating
    bool LMBPressed = false; // whether or not the left mouse button is pressed
    bool RMBPressed = false; // whether or not the right mouse button is pressed
    bool inCombat = false; // whether or not player is in combat
    bool standingStill = true; // whether or not player is standing still

    float autoAttackRange = 2; // distance at which player starts auto attacking target in combat

    // initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        ch = GetComponentInChildren<CameraHolder>();
        cr = FindObjectOfType<CameraRaycaster>();
        cr.onMouseOverEnemy += OnMouseOverEnemy;
    }

    // once per frame
    void Update ()
    {
        MouseInput();
        if (autoRun) AutoRun();
        if (directMovement) DirectMovement(); // use mouse keys for movement
        if (Input.GetKeyDown(KeyCode.Space)) animator.SetTrigger("jump"); // jump
    }



/////////////////////////////////////////////////////////////////////
//                              INPUT                              //
/////////////////////////////////////////////////////////////////////


    /*
     * handles all input from the mouse
     */
    private void MouseInput()
    {
        MovementMouseInput();
    }


    /*
     * handles all mouse input involving movement
     */
    private void MovementMouseInput()
    {
        if (autoRun) directMovement = false;
        else directMovement = true;

        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButton(1))
            {
                TwoButtonRun();
                CameraRotationPitch();
                directMovement = false;
            }
            else
            {
                CameraRotationYaw();
                CameraRotationPitch();
            }
        }
        else if (Input.GetMouseButton(1))
        {
            characterRotating = true;
            CharacterRotation();
            CameraRotationPitch();
        }
        else
        {
            if (autoRun)
            {
                ch.SlowResetRotation();
            }

            if (characterRotating)
            {
                characterRotating = false;
                Vector3 newVector = new Vector3(0, ch.transform.eulerAngles.y, 0);
                this.transform.eulerAngles = newVector;
                ch.ResetRotationYaw();
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (!autoRun)
            {
                autoRun = true;
                directMovement = false;
            }
            else
            {
                autoRun = false;
                directMovement = true;
            }

        }
    }


    /*
     * called whenever mouse passes over an enemy
     * clicks will target that enemy
     */
    private void OnMouseOverEnemy(Enemy enemy)
    {
        if (Input.GetMouseButtonDown(0)) SelectNewTarget(enemy);
        if (Input.GetMouseButtonDown(1)) InteractNewTarget(enemy);
    }



/////////////////////////////////////////////////////////////////////
//                              COMBAT                             //
/////////////////////////////////////////////////////////////////////


    /*
     * select the clicked enemy as a target, but do not attack
     */
    private void SelectNewTarget(Enemy enemy)
    {
        target = enemy;
        print("Selected " + enemy);
    }


    /*
     * select the clicked enemy as a target, and attack
     */
    private void InteractNewTarget(Enemy enemy)
    {
        target = enemy;
        StartCoroutine("StartCombat");
        print("Interacted " + enemy);
    }


    /*
     * combat loop
     */
    IEnumerator StartCombat()
    {
        inCombat = true;
        while (inCombat)
        {
            print("in combat");
            if (target)
            {
                if (DistanceTo(target) < autoAttackRange)
                {
                    animator.SetTrigger("attack");
                    target.TakeDamage(10, this);
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
            
        }
    }


    /*
     * deselect the current target
     */
    public void ClearTarget()
    {
        target = null;
    }



/////////////////////////////////////////////////////////////////////
//                             MOVEMENT                            //
/////////////////////////////////////////////////////////////////////


    /*
     * handles WASD movement
     */
    private void DirectMovement()
    {
        float strafe = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");
        if (strafe + forward == 0) standingStill = true;
        else standingStill = false;
        Move(strafe, forward);
        float yaw = Input.GetAxis("Rotational") * 5;
        RotateCharacter(yaw);
    }


    /*
     * handles rotation of the entire character (also rotates camera in sync)
     * 
     * TODO: partial rotation animation
     */
    private void CharacterRotation()
    {
        if (!standingStill)
        {
            Vector3 newVector = this.transform.eulerAngles;
            newVector.y = ch.transform.eulerAngles.y;
            this.transform.eulerAngles = newVector;
            ch.ResetRotationYaw();

            float yaw = Input.GetAxis("Mouse X") * 5;
            RotateCharacter(yaw);
        }
        else
        {
            CameraRotationYaw();

            float cameraYaw = ch.transform.eulerAngles.y;
            float characterYaw = this.transform.eulerAngles.y;

            float a = Math.Abs(cameraYaw - characterYaw);

            if (a >= 90 && a <= 270)
            {
                float b = ch.transform.localEulerAngles.y;
                if (b >= 0 && b <= 180)
                {
                    ch.RotateCamera(-20);
                    RotateCharacter(20);
                }
                else
                {
                    ch.RotateCamera(20);
                    RotateCharacter(-20);
                }
            }
        }
        
    }


    /*
     * handles camera rotation around y axis (independent of character rotation)
     */
    private void CameraRotationYaw()
    {
        float yaw = Input.GetAxis("Mouse X") * 5;
        Vector3 newVector = ch.transform.eulerAngles;
        newVector.y += yaw;
        ch.transform.eulerAngles = newVector;
    }


    /*
     * handles camera rotation around x axis (independent of character rotation)
     */
    private void CameraRotationPitch()
    {
        float pitch = Input.GetAxis("Mouse Y") * -5;
        Vector3 newVector = ch.transform.eulerAngles;
        newVector.x += pitch;
        ch.transform.eulerAngles = newVector;
    }


    /*
     * controls movement while both mouse buttons are pressed
     */
    private void TwoButtonRun()
    {
        standingStill = false;
        autoRun = false;
        CharacterRotation();
        float strafe = Input.GetAxis("Horizontal");
        Move(strafe, 1);
    }


    /*
     * automatically moves the character forward while autoRun is active
     */
    private void AutoRun()
    {
        standingStill = false;
        if (Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.UpArrow)
            || Input.GetKey(KeyCode.DownArrow))
        {
            autoRun = false;
            directMovement = true;
            return;
        }


        float strafe = Input.GetAxis("Horizontal");
        Move(strafe, 1);

        float yaw = Input.GetAxis("Rotational") * 5;
        RotateCharacter(yaw);
    }
}