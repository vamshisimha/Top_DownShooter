using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AoeLauncher{
    
    public AoeModel model;

  
    public Vector3 position;

    
    public GameObject caster;

 
    public float radius;

  
    public float duration;

    public float degree;

  
    public AoeTween tween;
    public object[] tweenParam = new object[0];

   
    public Dictionary<string, object> param = new Dictionary<string, object>();

    public AoeLauncher(
        AoeModel model, GameObject caster, Vector3 position, float radius, float duration, float degree, 
        AoeTween tween = null, object[] tweenParam = null, Dictionary<string, object> aoeParam = null
    ){
        this.model = model;
        this.caster = caster;
        this.position = position;
        this.radius = radius;
        this.duration = duration;
        this.degree = degree;
        this.tween = tween;
        if (aoeParam != null) this.param = aoeParam;
        if (tweenParam != null) this.tweenParam = tweenParam;
    }

    public AoeLauncher Clone(){
        return new AoeLauncher(
            this.model,
            this.caster,
            this.position,
            this.radius,
            this.duration,
            this.degree,
            this.tween,
            this.tweenParam,
            this.param
        );
    }
}


public struct AoeModel{
    public string id;

   
    public string prefab;

    
    public bool removeOnObstacle;

   
    public string[] tags;

   
    public float tickTime;

    public AoeOnCreate onCreate;

    
    public object[] onCreateParams;

  
    public AoeOnTick onTick;
    public object[] onTickParams;

   
    public AoeOnRemoved onRemoved;
    public object[] onRemovedParams;

   
    public AoeOnCharacterEnter onChaEnter;
    public object[] onChaEnterParams;

  
    public AoeOnCharacterLeave onChaLeave;
    public object[] onChaLeaveParams;

    public AoeOnBulletEnter onBulletEnter;
    public object[] onBulletEnterParams;

   
    public AoeOnBulletLeave onBulletLeave;
    public object[] onBulletLeaveParams;

    public AoeModel(
        string id, string prefab, string[] tags, float tickTime, bool removeOnObstacle,
        string onCreate, object[] onCreateParam,
        string onRemoved, object[] onRemovedParam,
        string onTick, object[] onTickParam,
        string onChaEnter, object[] onChaEnterParam,
        string onChaLeave, object[] onChaLeaveParam,
        string onBulletEnter, object[] onBulletEnterParam,
        string onBulletLeave, object[] onBulletLeaveParam
    ){
        this.id = id;
        this.prefab = prefab;
        this.tags = tags;
        this.tickTime = tickTime;
        this.removeOnObstacle = removeOnObstacle;
        this.onCreate = onCreate == "" ? null : DesignerScripts.AoE.onCreateFunc[onCreate];//DesignerScripts.AoE.onCreateFunc[onCreate];
        this.onCreateParams = onCreateParam;
        this.onRemoved = onRemoved == "" ? null : DesignerScripts.AoE.onRemovedFunc[onRemoved];
        this.onRemovedParams = onRemovedParam;
        this.onTick = onTick == "" ? null : DesignerScripts.AoE.onTickFunc[onTick];
        this.onTickParams = onTickParam;
        this.onChaEnter = onChaEnter == "" ? null : DesignerScripts.AoE.onChaEnterFunc[onChaEnter];
        this.onChaEnterParams = onChaEnterParam;
        this.onChaLeave = onChaLeave == "" ? null : DesignerScripts.AoE.onChaLeaveFunc[onChaLeave];
        this.onChaLeaveParams = onChaLeaveParam;
        this.onBulletEnter = onBulletEnter == "" ? null : DesignerScripts.AoE.onBulletEnterFunc[onBulletEnter];
        this.onBulletEnterParams = onBulletEnterParam;
        this.onBulletLeave = onBulletLeave == "" ? null : DesignerScripts.AoE.onBulletLeaveFunc[onBulletLeave];
        this.onBulletLeaveParams = onBulletLeaveParam;
    }
}


public class AoeMoveInfo{
    
    public MoveType moveType;

   
    public Vector3 velocity;

   
    public float rotateToDegree;

    public AoeMoveInfo(MoveType moveType, Vector3 velocity, float rotateToDegree){
        this.moveType = moveType;
        this.velocity = velocity;
        this.rotateToDegree = rotateToDegree;
    }
}


public delegate void AoeOnCreate(GameObject aoe);


public delegate void AoeOnRemoved(GameObject aoe);


public delegate void AoeOnTick(GameObject aoe);


public delegate void AoeOnCharacterEnter(GameObject aoe, List<GameObject> cha);


public delegate void AoeOnCharacterLeave(GameObject aoe, List<GameObject> cha);


public delegate void AoeOnBulletEnter(GameObject aoe, List<GameObject> bullet);


public delegate void AoeOnBulletLeave(GameObject aoe, List<GameObject> bullet);


public delegate AoeMoveInfo AoeTween(GameObject aoe, float t); 