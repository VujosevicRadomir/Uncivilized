using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveHandler : MonoBehaviour {

    Vector3 OldPosition;

	// Use this for initialization
	void Start () {
        OldPosition = this.transform.position;
	}
	
    bool CameraMovedCheck()
    {
        if(OldPosition != this.transform.position)
        {
            OldPosition = this.transform.position;
            return true;
        }
        return false;
    }

	// Update is called once per frame
	void Update () {
        if (CameraMovedCheck())
        {
            HexComponent[] hexes = GameObject.FindObjectsOfType<HexComponent>();

            foreach(HexComponent hex in hexes)
            {
                hex.UpdatePosition();
            }
        }
	}
}
