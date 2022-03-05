using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitMove : MonoBehaviour
{
  
    private bool canMove = true;

    [Tooltip("The type of movement of the unit, depending on the game design, this value can also be different")]
    public MoveType moveType = MoveType.ground;

    [Tooltip("The radius of the collision circle of the moving body of the unit, unit: meter")]
    public float bodyRadius = 0.25f;

    [Tooltip(
        "When the unit's movement is blocked by the map, is it to choose a better foothold (true) or stop moving (false)? If it stops moving directly, it will be true when accessing hitObstacle when it stops, otherwise hitObstacle will always be false"
    )]
    public bool smoothMove = true;

    [Tooltip("The radius of the collision circle of the moving body of the unit, unit: meter")]
    public bool ignoreBorder = true;

    public bool hitObstacle{get{
        return _hitObstacle;
    }}
    private bool _hitObstacle = false;
    
   
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate() {
        if (canMove == false || velocity == Vector3.zero) return;   
        
        Vector3 targetPos = new Vector3(
            velocity.x * Time.fixedDeltaTime + transform.position.x,
            velocity.y * Time.fixedDeltaTime + transform.position.y, 
            velocity.z * Time.fixedDeltaTime + transform.position.z
        );

        MapTargetPosInfo mapTargetPosInfo = SceneVariants.map.FixTargetPosition(
            transform.position, bodyRadius, targetPos, moveType, (this.ignoreBorder == true && moveType == MoveType.fly)
        );
        if (smoothMove == false && mapTargetPosInfo.obstacle == true){
            _hitObstacle = true;
            canMove = false;
        }
        transform.position = mapTargetPosInfo.suggestPos;

        velocity = Vector3.zero;
    }

    

    private void StopMoving(){
        velocity = Vector3.zero;
    }


   
    public Vector3 GetMoveDirection(){
        return velocity;
    }

   
    public void MoveBy(Vector3 moveForce){
        if (canMove == false) return;

        this.velocity = moveForce;
    }

    public void DisableMove(){
        StopMoving();
        canMove = false;
    }

    
    public void EnableRotate(){
        canMove = true;
    }
}   


