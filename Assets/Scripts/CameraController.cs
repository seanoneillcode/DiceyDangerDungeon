using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int boundary = 48;
    public int speed = 4;
    public bool mouseMoveEnabled = true;

    void Update()
    {
        Vector3 moveAmount = new Vector3();
        if (mouseMoveEnabled)
        {
            if (Input.mousePosition.x > Screen.width - boundary)
            {
                moveAmount.x += speed * Time.deltaTime;
            }
            if (Input.mousePosition.x < boundary)
            {
                moveAmount.x -= speed * Time.deltaTime;
            }
            if (Input.mousePosition.y > Screen.height - boundary)
            {
                moveAmount.z += speed * Time.deltaTime;
            }
            if (Input.mousePosition.y < boundary)
            {
                moveAmount.z -= speed * Time.deltaTime;
            }

        }

        moveAmount += new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed) * Time.deltaTime;
        transform.position += (transform.right * moveAmount.x);
        transform.position += (transform.forward * moveAmount.z);
    }

}
