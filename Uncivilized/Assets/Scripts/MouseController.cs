using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {

    bool isDraggingCamera = false;
    Vector3 LastMousePosition;
    

    private void Start()
    {
        UpdateCurrentFunc = NullFunc;
    }

    public delegate void UpdateFunc();

    UpdateFunc UpdateCurrentFunc;

    public void UpdateDetectMode()
    {
        Vector3 hitPos = MouseToGroundPlane();

        if (Input.GetMouseButtonDown(0))
        {
            LastMousePosition = hitPos;
        }
        else if((Input.GetMouseButton(0)) && LastMousePosition != hitPos)
        {
            UpdateCurrentFunc = CameraMapDrag;
        }else if (!Input.GetMouseButtonUp(0))
        {
            UpdateCurrentFunc = NullFunc;
        }
    
        
    }

    public void NullFunc()
    {

    }

    private void Update()
    {
        if (StateManager.IGMenuOpen) return;

        UpdateMouseWheelScroll();
        UpdateDetectMode();
        UpdateCurrentFunc();
        CameraWASDControls();


    }

    Vector3 MouseToGroundPlane()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayLength = mouseRay.origin.y / mouseRay.direction.y;

        return mouseRay.origin - (mouseRay.direction * rayLength);
    }

    public void UpdateMouseWheelScroll()
    {
        Vector3 hitPos = MouseToGroundPlane();

        float minCameraHeight = 3f;
        float maxCameraHeight = 20f;


        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > 0.01f)
        {

            Vector3 p = Camera.main.transform.position;
            Vector3 dir = hitPos - p;


            if (p.y < minCameraHeight)
            {
                p.y = minCameraHeight;

            }
            else if (p.y > maxCameraHeight)
            {
                p.y = maxCameraHeight;
            }
            Camera.main.transform.position = p;


            if ((scrollAmount > 0 && p.y > minCameraHeight) || (scrollAmount < 0 && p.y < maxCameraHeight))
            {

                Camera.main.transform.Translate(dir * scrollAmount * Time.deltaTime * 60f, Space.World);
            }
            p = Camera.main.transform.position;

        }
        float velocity = 0f;
        float targetRotation = Mathf.Lerp(20, 80, Camera.main.transform.position.y / ((maxCameraHeight - minCameraHeight) / 1.2f));
        Camera.main.transform.rotation = Quaternion.Euler(
                Mathf.SmoothDampAngle(Camera.main.transform.rotation.eulerAngles.x,
                                             targetRotation,
                                             ref velocity,
                                             0.08f
                                              ),
                Camera.main.transform.rotation.eulerAngles.y,
                Camera.main.transform.rotation.eulerAngles.z
            );
    }


    public void CameraMapDrag()
    {


        Vector3 hitPos = MouseToGroundPlane();

        Vector3 diff = LastMousePosition - hitPos;
        Camera.main.transform.Translate(diff, Space.World);

        LastMousePosition = MouseToGroundPlane();

    }

    void CameraWASDControls()
    {
        Vector3 translate = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
            );

        
        Camera.main.transform.Translate(translate * 30f * Time.deltaTime, Space.World);
    }
}
