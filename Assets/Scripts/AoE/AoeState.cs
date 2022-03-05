using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AoeState : MonoBehaviour{
    
    public AoeModel model;

   
    public bool justCreated = true;

   
    public float radius;

   
    public GameObject caster;

    
    public float duration;

    
    public float timeElapsed = 0;

  
    public AoeTween tween;

   
    public float tweenRunnedTime = 0;

  
    public ChaProperty propWhileCreate;

    
    public Dictionary<string, object> param = new Dictionary<string, object>();

    
    public List<GameObject> characterInRange = new List<GameObject>();

   
    public List<GameObject> bulletInRange = new List<GameObject>();

   
    public object[] tweenParam;

   
    public Vector3 velocity{
        get{ return this._velo;}
    }
    private Vector3 _velo = new Vector3();

    private UnitMove unitMove;
    private UnitRotate unitRotate;
    private GameObject viewContainer;

    private void Start() {
       
        synchronizedUnits();
    }

   
    public void SetMoveAndRotate(AoeMoveInfo aoeMoveInfo){
        if (aoeMoveInfo != null){
            if (unitMove){
                unitMove.moveType = aoeMoveInfo.moveType;
                unitMove.bodyRadius = this.radius;
                _velo = aoeMoveInfo.velocity / Time.fixedDeltaTime;
                unitMove.MoveBy(_velo);
            }
            if (unitRotate){
                unitRotate.RotateTo(aoeMoveInfo.rotateToDegree);
            }
        }
    }

    public bool HitObstacle(){
        return unitMove == null ? false : unitMove.hitObstacle;
    }

    private void synchronizedUnits(){
        if (!unitMove) unitMove = this.gameObject.GetComponent<UnitMove>();
        if (!unitRotate) unitRotate = this.gameObject.GetComponent<UnitRotate>();
        if (!viewContainer) viewContainer = this.gameObject.GetComponentInChildren<ViewContainer>().gameObject;
        unitMove.bodyRadius = this.radius;
        unitMove.smoothMove = !model.removeOnObstacle;
    }

    public void InitByAoeLauncher(AoeLauncher aoe){
        this.model = aoe.model;
        this.radius = aoe.radius;
        this.duration = aoe.duration;
        this.timeElapsed = 0;
        this.tween = aoe.tween;
        this.tweenParam = aoe.tweenParam;
        this.tweenRunnedTime = 0;
        this.param = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> kv in aoe.param){
            this.param[kv.Key] = kv.Value;
        }//aoe.param;
        this.caster = aoe.caster;
        this.propWhileCreate = aoe.caster ? aoe.caster.GetComponent<ChaState>().property : ChaProperty.zero;
        
        this.transform.position = aoe.position;
        this.transform.eulerAngles.Set(0, aoe.degree, 0);

        synchronizedUnits();

       
        if (aoe.model.prefab != ""){
            GameObject aoeEffect = Instantiate<GameObject>(
                Resources.Load<GameObject>("Prefabs/" + aoe.model.prefab),
                new Vector3(),
                Quaternion.identity,
                viewContainer.transform
            );
            
            aoeEffect.transform.localPosition = new Vector3(0, this.gameObject.transform.position.y, 0);
            aoeEffect.transform.localRotation = Quaternion.identity;
        }
        this.gameObject.transform.position = new Vector3(
            this.gameObject.transform.position.x,
            0,
            this.gameObject.transform.position.z
        );
    }

   
    public void SetViewScale(float scaleX = 1, float scaleY = 1, float scaleZ = 1){
        synchronizedUnits();
        viewContainer.transform.localScale.Set(scaleX, scaleY, scaleZ);
    }

  
    public void ModViewY(float toY){
        this.viewContainer.transform.position = new Vector3(
            viewContainer.transform.position.x,
            toY,
            viewContainer.transform.position.z
        );
    }
}