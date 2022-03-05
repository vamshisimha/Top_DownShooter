using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UnitRotate : MonoBehaviour
{
  
    private bool canRotate = true;

    
    public float rotateSpeed;

    private float targetDegree = 0.00f;  

    void FixedUpdate() {
        if (this.canRotate == false || DoneRotate() == true) return;

        float sDeg = transform.rotation.eulerAngles.y;
        if (sDeg > 180.00f) sDeg -= 360.00f;
        float degDis = targetDegree - sDeg;
        float nagDis = targetDegree > sDeg ? (targetDegree - 360.00f - sDeg) : (targetDegree + 360.00f - sDeg);
        bool nagDegree = Mathf.Abs(degDis) < Mathf.Abs(nagDis) ? (degDis < 0) : (nagDis < 0);  
        float rotSpeed = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, Mathf.Abs(degDis), Mathf.Abs(nagDis)); 
        if (nagDegree) rotSpeed *= -1;
        
        transform.Rotate(new Vector3(0, rotSpeed, 0));
    }

   
    private bool DoneRotate(){
        float rotSpeed = this.rotateSpeed * Time.fixedDeltaTime;
        return Mathf.Abs(transform.rotation.eulerAngles.y - targetDegree) < Mathf.Min(0.01f, rotSpeed); 
    }

  
    public void RotateTo(float degree){
        targetDegree = degree;
    }

   
    public void RotateTo(float x, float z){
        targetDegree = Mathf.Atan2(x, z) * 180.00f / Mathf.PI;   
    }

  
    public void RotateBy(float degree){
        targetDegree = transform.rotation.eulerAngles.y + degree;
    }

  
    public void DisableRotate(){
        canRotate = false;
        targetDegree = transform.rotation.eulerAngles.y;
    }

    public void EnableRotate(){
        canRotate = true;
    }
}
