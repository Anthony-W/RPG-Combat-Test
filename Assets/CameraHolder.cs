using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RotateCamera(float yaw)
    {
        Vector3 newVector = this.transform.eulerAngles;
        newVector.y += yaw;
        this.transform.eulerAngles = newVector;
    }

    public void ResetRotationYaw()
    {
        float x = transform.localEulerAngles.x;
        transform.localEulerAngles = new Vector3(x, 0, 0);
    }

    public void SlowResetRotation()
    {
        float x = transform.localEulerAngles.x;
        float y = transform.localEulerAngles.y;      

        //change angles from (0 to 360) to (-180 to 180)
        if (x > 180)
        {
            x -= 180;
            x = -(180 - x);
        }

        if (y > 180)
        {
            y -= 180;
            y = -(180 - y);
        }


        //decide which direction to rotate
        if (x > 1) x -= 1f;
        else if (x < -1) x += 1f;
        else x = 0;

        if (y > 1) y -= 1f;
        else if (y < -1) y += 1f;
        else y = 0;
    

        transform.localEulerAngles = new Vector3(x, y, 0);
    }
}
