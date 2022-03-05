using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChaState:MonoBehaviour{
   
    private ChaControlState _controlState = new ChaControlState(true, true, true);

  
    public ChaControlState timelineControlState = new ChaControlState(true,true,true);

    public ChaControlState controlState{
        get{
            return this._controlState + this.timelineControlState;
        }
    }

   
    public float immuneTime{
        get{
            return _immuneTime;
        }
        set{
            _immuneTime = Mathf.Max(_immuneTime, value);
        }
    }
    private float _immuneTime = 0.00f;

   
    public bool charging = false;

    
    public float moveDegree{
        get{
            return _wishToMoveDegree;
        }
    }
    private float _wishToMoveDegree = 0.00f;

   
    public float faceDegree{
        get{
            return _wishToFaceDegree;
        }
    }
    private float _wishToFaceDegree = 0.00f;

 
    public bool dead = false;

    
    private Vector3 moveOrder = new Vector3();

    private List<MovePreorder> forceMove = new List<MovePreorder>();

    
    private List<string> animOrder = new List<string>();

    
    private float rotateToOrder;

    
    private List<float> forceRotate = new List<float>();

   
    public ChaResource resource = new ChaResource(1);

    [Tooltip("The faction the character is in, if the faction is different, there will be fights")]
   
    public int side = 0;

    public string[] tags = new string[0];

   
    public ChaProperty property{get{
        return _prop;
    }}
    private ChaProperty _prop = ChaProperty.zero;

    
    public float moveSpeed{get{
        
        return this._prop.moveSpeed * 5.600f / (this._prop.moveSpeed + 100.000f) + 0.200f;
    }}

  
    public float actionSpeed{
        get{
            return this._prop.actionSpeed * 4.90f / (_prop.actionSpeed + 390.00f) + 0.100f;
        }
    }

 
    public ChaProperty baseProp = new ChaProperty(
        100, 100, 0, 20, 100
    );

   
    public ChaProperty[] buffProp = new ChaProperty[2]{ChaProperty.zero, ChaProperty.zero};

   
    public ChaProperty equipmentProp = ChaProperty.zero;

    
    public List<SkillObj> skills = new List<SkillObj>();

   
    public List<BuffObj> buffs = new List<BuffObj>();


    private UnitMove unitMove;
    private UnitAnim unitAnim;
    private UnitRotate unitRotate;
    private Animator animator;
    private UnitBindManager bindPoints;
    private GameObject viewContainer;

    void Start() {
        rotateToOrder = transform.rotation.eulerAngles.y;

        synchronizedUnits();

        AttrRecheck();
    }

    void FixedUpdate() {
        float timePassed = Time.fixedDeltaTime;
        if (dead == false){
           
            if (_immuneTime > 0) _immuneTime -= timePassed;

           
            for (int i = 0; i < this.skills.Count; i++){
                if (this.skills[i].cooldown > 0){
                    this.skills[i].cooldown -= timePassed;
                }
            }

          
            List<BuffObj> toRemove = new List<BuffObj>();
            for (int i = 0; i < this.buffs.Count; i++){
                if (buffs[i].permanent == false) buffs[i].duration -= timePassed;
                buffs[i].timeElapsed += timePassed;

                if (buffs[i].model.tickTime > 0 && buffs[i].model.onTick != null){
                    
                    if (Mathf.RoundToInt(buffs[i].timeElapsed * 1000) % Mathf.RoundToInt(buffs[i].model.tickTime * 1000) == 0){
                        buffs[i].model.onTick(buffs[i]);
                        buffs[i].ticked += 1;
                    }
                }

              
                if (buffs[i].duration <= 0 || buffs[i].stack <= 0){
                    if (buffs[i].model.onRemoved != null){
                        buffs[i].model.onRemoved(buffs[i]);
                    }
                    toRemove.Add(buffs[i]);
                }
            }
            if (toRemove.Count > 0){
                for (int i = 0; i < toRemove.Count; i++){
                    this.buffs.Remove(toRemove[i]);
                }
                AttrRecheck();
            }
            
            toRemove = null;

           
            bool wishToMove = moveOrder != Vector3.zero;
            if (wishToMove == true) 
            _wishToMoveDegree = Mathf.Atan2(moveOrder.x, moveOrder.z) * 180 / Mathf.PI;
            
            ChaControlState curCS = this.controlState;// _controlState + timelineControlState;

           
            bool tryRun = curCS.canMove == true && moveOrder != Vector3.zero;
            float tryMoveDegree = Mathf.Atan2(moveOrder.x, moveOrder.z) * 180 / Mathf.PI;
            if (tryMoveDegree > 180) tryMoveDegree -= 360;
            if (unitMove){
                if (curCS.canMove == false) moveOrder = Vector3.zero;
                int fmIndex = 0;
                while (fmIndex < forceMove.Count){
                    moveOrder += forceMove[fmIndex].VeloInTime(timePassed);
                    if (forceMove[fmIndex].duration <= 0){
                        forceMove.RemoveAt(fmIndex);
                    }else{
                        fmIndex++;
                    }
                }
                unitMove.MoveBy(moveOrder);
                moveOrder = Vector3.zero;
                //forceMove.Clear();
            }
            
            _wishToFaceDegree = rotateToOrder;
            if (wishToMove == false) _wishToMoveDegree = _wishToFaceDegree;
         
            if (unitRotate){
                if (curCS.canRotate == false) rotateToOrder = transform.rotation.eulerAngles.y;
                for (int i = 0; i < forceRotate.Count; i++){
                   
                    rotateToOrder += forceRotate[i];
                }
                unitRotate.RotateTo(rotateToOrder);
                forceRotate.Clear();
            }
           
            if (unitAnim){
                unitAnim.timeScale = this.actionSpeed;
               
                if (tryRun == false) {
                    animOrder.Add("Stand");  
                }else{
                    string tt = Utils.GetTailStringByDegree(transform.rotation.eulerAngles.y, tryMoveDegree);
                    animOrder.Add("Move" + tt);
                }
             
                for (int i = 0; i < animOrder.Count; i++){
                    unitAnim.Play(animOrder[i]);
                }
                animOrder.Clear();
            }
            if (animator){
                animator.speed = this.actionSpeed;
            }
        }else{
            _wishToFaceDegree = transform.rotation.eulerAngles.y * 180.00f / Mathf.PI;
            _wishToMoveDegree = _wishToFaceDegree;
        }
    }

    private void synchronizedUnits(){
        if (!unitMove) unitMove = this.gameObject.GetComponent<UnitMove>();
        if (!unitAnim) unitAnim = this.gameObject.GetComponent<UnitAnim>();
        if (!unitRotate) unitRotate = this.gameObject.GetComponent<UnitRotate>();  
        if (!animator) animator = this.gameObject.GetComponent<Animator>();  
        if (!bindPoints) bindPoints = this.gameObject.GetComponent<UnitBindManager>();
        if (!viewContainer) viewContainer = this.gameObject.GetComponentInChildren<ViewContainer>().gameObject;
    }

   
    public void OrderMove(Vector3 move){
        this.moveOrder.x = move.x;
        this.moveOrder.z = move.z;
    }

   
    public void AddForceMove(MovePreorder move){
        this.forceMove.Add(move);
    }

   
    public void OrderRotateTo(float degree){
        this.rotateToOrder = degree;
    }

    
    
    public void AddForceRotate(float degree){
        this.forceRotate.Add(degree);
    }

  
    public void Play(string animName){
        animOrder.Add(animName);
    }

 
    public void Kill(){
        this.dead = true;
        if (unitAnim){
            unitAnim.Play("Dead");
        }
       
        if (this.gameObject != SceneVariants.MainActor())
            this.gameObject.AddComponent<UnitRemover>().duration = 5.0f;
    }

    
    private void AttrRecheck(){
        _controlState.Origin();
        this._prop.Zero();

        for (var i = 0; i < buffProp.Length; i++) buffProp[i].Zero();
        for (int i = 0; i < this.buffs.Count; i++){
            for (int j = 0; j < Mathf.Min(buffProp.Length, buffs[i].model.propMod.Length); j++){
                buffProp[j] += buffs[i].model.propMod[j] * buffs[i].stack;
            }
            _controlState += buffs[i].model.stateMod;
        }
        
        this._prop = (this.baseProp + this.equipmentProp + this.buffProp[0]) * this.buffProp[1];

        if (unitMove){
            unitMove.bodyRadius = this._prop.bodyRadius;
        }
    }

   
    public void ModResource(ChaResource value){
        this.resource += value;
        this.resource.hp = Mathf.Clamp(this.resource.hp, 0, this._prop.hp);
        this.resource.ammo = Mathf.Clamp(this.resource.ammo, 0, this._prop.ammo);
        this.resource.stamina = Mathf.Clamp(this.resource.stamina, 0, 100);
        if (this.resource.hp <= 0){
            this.Kill();
        }
    }


   
    public void PlaySightEffect(string bindPointKey, string effect, string effectKey = "", bool loop = false){
        bindPoints.AddBindGameObject(bindPointKey, "Prefabs/" + effect, effectKey, loop);
    }

   
    public void StopSightEffect(string bindPointKey, string effectKey){
        bindPoints.RemoveBindGameObject(bindPointKey, effectKey);
    }

    
    public bool CanBeKilledByDamageInfo(DamageInfo damageInfo){
          if (this.immuneTime > 0 || damageInfo.isHeal() == true) return false;
        int dValue = damageInfo.DamageValue(false);
        return dValue >= this.resource.hp;
    }

    
    public void AddBuff(AddBuffInfo buff){
        List<GameObject> bCaster = new List<GameObject>();
        if (buff.caster) bCaster.Add(buff.caster);
        List<BuffObj> hasOnes = GetBuffById(buff.buffModel.id, bCaster);
        int modStack = Mathf.Min(buff.addStack, buff.buffModel.maxStack);
        bool toRemove = false;
        BuffObj toAddBuff = null;
        if (hasOnes.Count > 0){
           
            hasOnes[0].buffParam = new Dictionary<string, object>();
            if (buff.buffParam != null){
                foreach (KeyValuePair<string, object> kv in buff.buffParam){hasOnes[0].buffParam[kv.Key] = kv.Value;};
            }
            
            hasOnes[0].duration = (buff.durationSetTo == true) ? buff.duration : (buff.duration + hasOnes[0].duration);
            int afterAdd = hasOnes[0].stack + modStack;
            modStack = afterAdd >= hasOnes[0].model.maxStack ? 
                (hasOnes[0].model.maxStack - hasOnes[0].stack) : 
                (afterAdd <= 0 ? (0 - hasOnes[0].stack) : modStack);
            hasOnes[0].stack += modStack;
            hasOnes[0].permanent = buff.permanent;
            toAddBuff = hasOnes[0];
            toRemove = hasOnes[0].stack <= 0;
        }else{
       
            toAddBuff = new BuffObj(
                buff.buffModel,
                buff.caster,
                this.gameObject,
                buff.duration,
                buff.addStack,
                buff.permanent,
                buff.buffParam
            );
            buffs.Add(toAddBuff);
            buffs.Sort((a, b)=>{
                return a.model.priority.CompareTo(b.model.priority);
            });
        }
        if (toRemove == false && buff.buffModel.onOccur != null){
            buff.buffModel.onOccur(toAddBuff, modStack);
        }
        AttrRecheck();
    }

   
    public List<BuffObj> GetBuffById(string id, List<GameObject> caster = null){
        List<BuffObj> res = new List<BuffObj>();
        for (int i = 0; i < this.buffs.Count;  i++){
            if (buffs[i].model.id == id && (caster == null || caster.Count <= 0 || caster.Contains(buffs[i].caster) == true)){
                res.Add(buffs[i]);
            }
        }
        return res;
    }

  
    public SkillObj GetSkillById(string id){
        for (int i = 0; i < skills.Count; i++ ){
            if (skills[i].model.id == id){
                return skills[i];
            }
        }
        return null;
    }
    
   
    public bool CastSkill(string id){
        if (this.controlState.canUseSkill == false) return false; 
        SkillObj skillObj = GetSkillById(id);
        if (skillObj == null || skillObj.cooldown > 0) return false;
        bool castSuccess = false;
        if (this.resource.Enough(skillObj.model.condition) == true){
            TimelineObj timeline = new TimelineObj(
                skillObj.model.effect, this.gameObject, skillObj
            );
            for (int i = 0; i < buffs.Count; i++){
                if (buffs[i].model.onCast != null){
                    timeline = buffs[i].model.onCast(buffs[i], skillObj, timeline);
                }
            }
            if (timeline != null){
                this.ModResource(-1 * skillObj.model.cost);
                SceneVariants.CreateTimeline(timeline);
                castSuccess = true;
            }
            
        }
        skillObj.cooldown = 0.1f;  
        return castSuccess;
    }

    

    public void InitBaseProp(ChaProperty cProp){
        this.baseProp = cProp;
        this.AttrRecheck();
        this.resource.hp = this._prop.hp;
        this.resource.ammo = this._prop.ammo;
        this.resource.stamina = 100;
    }

  
    public void LearnSkill(SkillModel skillModel, int level = 1){
        this.skills.Add(new SkillObj(skillModel, level));
        if (skillModel.buff != null){
            for (int i = 0; i < skillModel.buff.Length; i++){
                AddBuffInfo abi = skillModel.buff[i];
                abi.permanent = true;
                abi.duration = 10;
                abi.durationSetTo = true;
                this.AddBuff(abi);
            }
        }
    }

  
    public void SetView(GameObject view, Dictionary<string, AnimInfo> animInfo){
        if (view == null) return;
        synchronizedUnits();
        view.transform.SetParent(viewContainer.transform);
        view.transform.position = new Vector3(0, this.gameObject.transform.position.y, 0);
        this.gameObject.transform.position = new Vector3(
            this.gameObject.transform.position.x,
            0,
            this.gameObject.transform.position.z
        );
        this.gameObject.GetComponent<UnitAnim>().animInfo = animInfo;
    }

   
    public void SetImmuneTime(float time){
        this._immuneTime = Mathf.Max(this._immuneTime, time);
    }

  
    public bool HasTag(string tag){
        if (this.tags == null || this.tags.Length <= 0) return false;
        for (int i = 0; i < this.tags.Length; i++){
            if(tags[i] == tag){
                return true;
            }
        }
        return false;
    }
}