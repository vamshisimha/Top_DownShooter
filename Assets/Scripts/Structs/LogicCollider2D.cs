using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollisionResult{
   
    public bool hit;

 
    public Vector2 pushTo;

    public CollisionResult(bool hit, Vector2 pushTo){
        this.hit = hit;
        this.pushTo = pushTo;
    }
}