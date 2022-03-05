using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePreorder{
   
    public Vector3 velocity;

  
    private float inTime;

   
    public float duration;
    public MovePreorder(Vector3 velocity, float duration){
        this.velocity = velocity;
        this.duration = duration;
        this.inTime = duration;
    }

 
    public Vector3 VeloInTime(float time){
        if (time >= duration){
            this.duration = 0;
        }else{
            this.duration -= time;
        }
        return inTime <= 0 ? velocity : (velocity / inTime);
    }
}

public enum MoveType{
    ground = 0,
    fly = 1
}