using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform camRotPointX;
    public Transform camRotPointY;
    public float sensitivity = 1;
    float xAxisClamp = 0.0f;

	void Update ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        RotateCamera();
    }

    void RotateCamera()
    {
        float xInput = Input.GetAxis("Mouse X");
        float yInput = Input.GetAxis("Mouse Y");

        float rotAmountX = xInput * sensitivity;
        float rotAmountY = yInput * sensitivity;

        xAxisClamp -= rotAmountY;

        Vector3 targetRotY = camRotPointY.rotation.eulerAngles;
        Vector3 targetRotX = camRotPointX.rotation.eulerAngles;

        targetRotY.x -= rotAmountY;
        targetRotY.z = 0;
        targetRotX.y += rotAmountX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90;
            targetRotY.x = 90;
        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90;
            targetRotY.x = 270;
        }

        camRotPointY.rotation = Quaternion.Euler(targetRotY);
        camRotPointX.rotation = Quaternion.Euler(targetRotX);

    }
}
