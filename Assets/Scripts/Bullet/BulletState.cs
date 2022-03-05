using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletState:MonoBehaviour{
    
    public BulletModel model;
    
   
    public GameObject caster;

    
    public ChaProperty propWhileCast = ChaProperty.zero;

    public float fireDegree;

  
    public float speed;

  
    public float duration;

    
    public float timeElapsed = 0;

   
    public BulletTween tween = null;

  
    private Vector3 moveForce = new Vector3();

    
    public Vector3 velocity{
        get{return moveForce;}
    }

   
    public bool useFireDegreeForever = false;

    public List<BulletHitRecord> hitRecords = new List<BulletHitRecord>();

 
    public float canHitAfterCreated = 0;

   
    public GameObject followingTarget = null;

  
    public Dictionary<string, object> param = new Dictionary<string, object>();
    


    
    public int hp = 1;

    private MoveType moveType;
    private bool smoothMove;

    private UnitRotate unitRotate;
    private UnitMove unitMove;
    private GameObject viewContainer;
    
    private void Start() {
        // unitRotate = gameObject.GetComponent<UnitRotate>();
        // unitMove = gameObject.GetComponent<UnitMove>();
        synchronizedUnits();
    }

 
    public bool HitObstacle(){
        return unitMove == null ? false : unitMove.hitObstacle;
    }

   
    public void SetMoveForce(Vector3 mf){
        this.moveForce = mf;

        float moveDeg = (
            useFireDegreeForever == true ||
            timeElapsed <= 0     
            ) ? fireDegree : transform.rotation.eulerAngles.y; 

        moveForce.y = 0;
        moveForce *= speed;
        moveDeg += Mathf.Atan2(moveForce.x, moveForce.z) * 180 / Mathf.PI;
        
        float mR = moveDeg * Mathf.PI / 180;
        float mLen = Mathf.Sqrt(Mathf.Pow(moveForce.x, 2) + Mathf.Pow(moveForce.z, 2));

        moveForce.x = Mathf.Sin(mR) * mLen;
        moveForce.z = Mathf.Cos(mR) * mLen;

        unitMove.MoveBy(moveForce);
        unitRotate.RotateTo(moveDeg);
    }

  
    public void InitByBulletLauncher(BulletLauncher bullet, GameObject[] targets){
        this.model = bullet.model;
        this.caster = bullet.caster;
        if (this.caster && caster.GetComponent<ChaState>()){
            this.propWhileCast = caster.GetComponent<ChaState>().property;
        }
        this.fireDegree = bullet.fireDegree;
        this.speed = bullet.speed;
        this.duration = bullet.duration;
        this.timeElapsed = 0;
        this.tween = bullet.tween;
        this.useFireDegreeForever = bullet.useFireDegreeForever;
        this.canHitAfterCreated = bullet.canHitAfterCreated;
        this.smoothMove = !bullet.model.removeOnObstacle;
        this.moveType = bullet.model.moveType;
        this.hp = bullet.model.hitTimes;

        this.param = new Dictionary<string, object>();
        if (bullet.param != null){
            foreach(KeyValuePair<string, object> kv in bullet.param){
                this.param.Add(kv.Key, kv.Value);
            }
        }
        
        synchronizedUnits();

    
        if (bullet.model.prefab != ""){
            GameObject bulletEffect = Instantiate<GameObject>(
                Resources.Load<GameObject>("Prefabs/Bullet/" + bullet.model.prefab),
                new Vector3(),
                Quaternion.identity,
                viewContainer.transform
            );
            bulletEffect.transform.localPosition = new Vector3(0, this.gameObject.transform.position.y, 0);
            bulletEffect.transform.localRotation = Quaternion.identity;
        }

        this.gameObject.transform.position = new Vector3(
            this.gameObject.transform.position.x,
            0,
            this.gameObject.transform.position.z
        );

        
        this.followingTarget = bullet.targetFunc == null ? null :
            bullet.targetFunc(this.gameObject, targets);
    }

   
    private void synchronizedUnits(){
        if (!unitRotate) unitRotate = gameObject.GetComponent<UnitRotate>();
        if (!unitMove)  unitMove = gameObject.GetComponent<UnitMove>();
        if (!viewContainer) viewContainer = gameObject.GetComponentInChildren<ViewContainer>().gameObject;
        
        unitMove.smoothMove = this.smoothMove;
        unitMove.moveType = this.moveType;
        unitMove.bodyRadius = this.model.radius;
    }

    public void SetMoveType(MoveType toType){
        this.moveType = toType;
        synchronizedUnits();
    }

  
    public bool CanHit(GameObject target){
        if (canHitAfterCreated > 0) return false;
        for (int i = 0; i < this.hitRecords.Count; i++){
            if (hitRecords[i].target == target){
                return false;
            }
        }
        
        ChaState cs = target.GetComponent<ChaState>();
        if (cs && cs.immuneTime > 0) return false;

        return true;
    }

 
    public void AddHitRecord(GameObject target){
        hitRecords.Add(new BulletHitRecord(
            target,
            this.model.sameTargetDelay
        ));
    }

    public void SetRadius(float radius){
        this.model.radius = radius;
        synchronizedUnits();
    }
}