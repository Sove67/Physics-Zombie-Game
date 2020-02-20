using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLocation : MonoBehaviour {
    public float rayLength;
    public GameObject endPoint;
    public GameObject playerRotX;
    public string rayHitID;
    void Update()
    {
        RaycastHit hit;
        Ray aimRay = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward * rayLength);
        
        Vector3 temp1 = endPoint.transform.eulerAngles;
        temp1.z = playerRotX.transform.eulerAngles.z;
        endPoint.transform.rotation = Quaternion.Euler(temp1);

        if (Physics.Raycast(aimRay, out hit, rayLength))
        {
            rayHitID = hit.collider.tag;
            if (hit.collider.tag == "Enviroment")
            {
                endPoint.transform.position = hit.point;
                endPoint.transform.rotation = Quaternion.LookRotation(hit.normal);
                if (-91 < endPoint.transform.rotation.x && endPoint.transform.rotation.x < -89)
                {
                    if (-1 < endPoint.transform.rotation.z && endPoint.transform.rotation.z < 1)
                    {
                        Vector3 temp2 = endPoint.transform.eulerAngles;
                        temp2.z = playerRotX.transform.eulerAngles.y;
                        endPoint.transform.rotation = Quaternion.Euler(temp2);
                    }
                }
            }
        }

        else
        {
            rayHitID = null;
            endPoint.transform.position = transform.position + transform.forward * rayLength;
        }
    }
}