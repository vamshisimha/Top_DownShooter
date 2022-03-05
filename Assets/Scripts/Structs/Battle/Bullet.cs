using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletLauncher{
    
    public BulletModel model;

   
    public GameObject caster;

    
    public Vector3 firePosition;

   
    public float fireDegree;

   
    public float speed;

   
    public float duration;


    public BulletTargettingFunction targetFunc;

   
    public BulletTween tween = null;

  
    public bool useFireDegreeForever = false;

   
    public float canHitAfterCreated = 0;

    public Dictionary<string, object> param;

    public BulletLauncher(
        BulletModel model, GameObject caster, Vector3 firePos, float degree, float speed, float duration,
        float canHitAfterCreated = 0,
        BulletTween tween = null, BulletTargettingFunction targetFunc = null, bool useFireDegree = false,
        Dictionary<string, object> param = null
    ){
        this.model = model;
        this.caster = caster;
        this.firePosition = firePos;
        this.fireDegree = degree;
        this.speed = speed;
        this.duration = duration;
        this.tween = tween;
        this.useFireDegreeForever = useFireDegree;
        this.targetFunc = targetFunc;
        this.param = param;
    }
}


public struct BulletModel{
    public string id;

   
    public string prefab;

   
    public float radius;

   
    public int hitTimes;

  
    public float sameTargetDelay;

    
    public BulletOnCreate onCreate;

    public object[] onCreateParam;

   
    public BulletOnHit onHit;

  
    public object[] onHitParams;

   
    public BulletOnRemoved onRemoved;

   
    public object[] onRemovedParams;

   
    public MoveType moveType;

    
    public bool removeOnObstacle;

  
    public bool hitFoe;

   
    public bool hitAlly;

    public BulletModel(
        string id, string prefab, 
        string onCreate = "",
        object[] createParams = null,
        string onHit = "",
        object[] onHitParams = null,
        string onRemoved = "",
        object[] onRemovedParams = null,
        MoveType moveType = MoveType.fly, bool removeOnObstacle = true,
        float radius = 0.1f, int hitTimes = 1, float sameTargetDelay = 0.1f,
        bool hitFoe = true, bool hitAlly = false
    ){
        this.id = id;
        this.prefab = prefab;
        this.onHit = onHit == "" ? null : DesignerScripts.Bullet.onHitFunc[onHit];
        this.onRemoved = onRemoved == "" ? null : DesignerScripts.Bullet.onRemovedFunc[onRemoved];
        this.onCreate = onCreate == "" ? null : DesignerScripts.Bullet.onCreateFunc[onCreate];
        this.onCreateParam = createParams != null ? createParams : new object[0];
        this.onHitParams = onHitParams != null ? onHitParams : new object[0];
        this.onRemovedParams = onRemovedParams != null ? onRemovedParams : new object[0];
        this.radius = radius;
        this.hitTimes = hitTimes;
        this.sameTargetDelay = sameTargetDelay;
        this.moveType = moveType;
        this.removeOnObstacle = removeOnObstacle;
        this.hitAlly = hitAlly;
        this.hitFoe = hitFoe;
    }
}


public class BulletHitRecord{
   
    public GameObject target;

    
    public float timeToCanHit;

    public BulletHitRecord(GameObject character, float timeToCanHit){
        this.target = character;
        this.timeToCanHit = timeToCanHit;
    }
}



public delegate void BulletOnCreate(GameObject bullet);


public delegate void BulletOnHit(GameObject bullet, GameObject target);


public delegate void BulletOnRemoved(GameObject bullet);


public delegate Vector3 BulletTween(float t, GameObject bullet, GameObject target);


public delegate GameObject BulletTargettingFunction(GameObject bullet, GameObject[] targets);

