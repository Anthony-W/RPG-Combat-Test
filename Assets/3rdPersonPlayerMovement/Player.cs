using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    private Character target;

    CameraRaycaster cr;
    CameraHolder ch;

    bool directMovement = true;
    bool autoRun = false;
    bool characterRotating = false;
    bool LMBPressed = false;
    bool RMBPressed = false;
    bool inCombat = false;

    float clickCounterLMB = 0;
    float clickCounterRMB = 0;

    float autoAttackRange = 10;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        ch = GetComponentInChildren<CameraHolder>();
        cr = FindObjectOfType<CameraRaycaster>();
        cr.onMouseOverEnemy += OnMouseOverEnemy;
    }

    // Update is called once per frame
    void Update ()
    {
        MouseInput();
        if (autoRun) AutoRun();
        if (directMovement) DirectMovement();
    }

    private void MouseInput()
    {
        MovementMouseInput();
    }

    private void OnMouseOverEnemy(Enemy enemy)
    {
        if (Input.GetMouseButtonDown(0)) SelectNewTarget(enemy);
        if (Input.GetMouseButtonDown(1)) InteractNewTarget(enemy);
    }

    private void SelectNewTarget(Enemy enemy)
    {
        target = enemy;
        print("Selected " + enemy);
    }

    private void InteractNewTarget(Enemy enemy)
    {
        target = enemy;
        StartCoroutine("StartCombat");
        print("Interacted " + enemy);
    }

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

    public void ClearTarget()
    {
        target = null;
    }

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

    private void DirectMovement()
    {
        float strafe = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");
        Move(strafe, forward);
        float yaw = Input.GetAxis("Rotational") * 5;
        RotateCharacter(yaw);
    }

    //TODO: partial rotation animation
    private void CharacterRotation()
    {
        CameraRotationYaw();

        float cameraYaw = ch.transform.eulerAngles.y;
        float characterYaw = this.transform.eulerAngles.y;

        float a = Math.Abs(cameraYaw - characterYaw);

        if (a >= 90 && a <= 270)
        {
            float b = ch.transform.localEulerAngles.y;
            if(b >= 0 && b <= 180)
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

    private void CameraRotationYaw()
    {
        float yaw = Input.GetAxis("Mouse X") * 5;
        Vector3 newVector = ch.transform.eulerAngles;
        newVector.y += yaw;
        ch.transform.eulerAngles = newVector;
    }

    private void CameraRotationPitch()
    {
        float pitch = Input.GetAxis("Mouse Y") * -5;
        Vector3 newVector = ch.transform.eulerAngles;
        newVector.x += pitch;
        ch.transform.eulerAngles = newVector;
    }

    private void TwoButtonRun()
    {
        autoRun = false;
        float yaw = Input.GetAxis("Mouse X") * 5;
        RotateCharacter(yaw);
        float strafe = Input.GetAxis("Horizontal");
        Move(strafe, 1);
    }

    private void AutoRun()
    {
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
