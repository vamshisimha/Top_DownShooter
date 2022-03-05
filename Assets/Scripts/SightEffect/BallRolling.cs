using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallRolling : MonoBehaviour{
    private Vector3 pWasPos = new Vector3();

    private Renderer render;
    private void Start() {
        pWasPos = this.transform.parent.position;
        render =  this.gameObject.GetComponent<Renderer>();
        if (!render) render = this.gameObject.GetComponentInChildren<Renderer>();
    }
    
    private void Update() {
        if (!render) return;
        Vector3 mDis = this.transform.parent.position - pWasPos;
        float r = render.bounds.size.x / 2; 
        float degreeX = mDis.x * 180.00f / (Mathf.PI * r) - this.transform.parent.eulerAngles.x; 
        float degreeZ = mDis.z * 180.00f / (Mathf.PI * r) - this.transform.parent.eulerAngles.z;
        transform.RotateAround(transform.position,Vector3.right,degreeX);
        transform.RotateAround(transform.position,Vector3.back,degreeZ);
        pWasPos = this.transform.parent.position;
    }
}