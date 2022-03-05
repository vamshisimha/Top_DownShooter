using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ChaControlState{
  
    public bool canMove;

    
    public bool canRotate;

    public bool canUseSkill;

    public ChaControlState(bool canMove = true, bool canRotate = true, bool canUseSkill = true){
        this.canMove = canMove;
        this.canRotate = canRotate;
        this.canUseSkill = canUseSkill;
    }

    public void Origin(){
        this.canMove = true;
        this.canRotate = true;
        this.canUseSkill = true;
    }

    public static ChaControlState origin = new ChaControlState(true, true, true);

    

     
    public static ChaControlState stun = new ChaControlState(false, false, false);

    public static ChaControlState operator +(ChaControlState cs1, ChaControlState cs2){
        return new ChaControlState(
            cs1.canMove & cs2.canMove,
            cs1.canRotate & cs2.canRotate,
            cs1.canUseSkill & cs2.canUseSkill
        );
    }
}