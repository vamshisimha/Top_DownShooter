using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateY : MonoBehaviour{

    public float rotatePerSec = 360;

    private float cDeg = 0;

    private void Update() {
        cDeg = (cDeg + rotatePerSec * Time.deltaTime) % 360;
        float shouldRotate = cDeg - transform.eulerAngles.y;
        Transform t = this.transform;
        while(t.parent != null){
            shouldRotate -= t.parent.eulerAngles.y;
            t = t.parent;
        }
        this.transform.RotateAround(this.transform.position, Vector3.up, shouldRotate);
    } 
}