using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookControl : MonoBehaviour
{
    public float mouseSensX = 100f;
    public float mouseSensY = 100f;

    public GameObject target;
    public GameObject xGim;
    public GameObject yGim;
    private Rigidbody rb;

    private bool blazed = false;
    private bool cursorLock = true;

    private Vector2 rot = Vector2.zero;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (cursorLock)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (!cursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        if (blazed)
        {
            rot.y += Input.GetAxisRaw("IMouse X") * mouseSensX;
            rot.x += -Input.GetAxisRaw("IMouse Y") * mouseSensY;
        }
        else 
        {
            rot.y += Input.GetAxisRaw("Mouse X") * mouseSensX;
            rot.x += -Input.GetAxisRaw("Mouse Y") * mouseSensY;
        }
        rot.x = Mathf.Clamp(rot.x, -40, 70);
        xGim.transform.localRotation = Quaternion.Euler(rot.x, 0, 0);
        yGim.transform.eulerAngles = new Vector2(0, rot.y);

        xGim.transform.position = target.transform.position;
    }

    public void UnBlaze()
    {
        blazed = false;
    }
    public void Blaze()
    {
        blazed = true;
    }
}
