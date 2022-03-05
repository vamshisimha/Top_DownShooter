using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct AddBuffInfo{
   
    public GameObject caster;

  
    public GameObject target;

   
    public BuffModel buffModel;

   
    public int addStack;

  
    public bool durationSetTo;

   
    public bool permanent;

   
    public float duration;

  
    public Dictionary<string, object> buffParam;

    public AddBuffInfo(
        BuffModel model, GameObject caster, GameObject target,
        int stack, float duration, bool durationSetTo = true,
        bool permanent = false,
        Dictionary<string, object> buffParam = null
    ){
        this.buffModel = model;
        this.caster = caster;
        this.target = target;
        this.addStack = stack;
        this.duration = duration;
        this.durationSetTo = durationSetTo;
        this.buffParam = buffParam;
        this.permanent = permanent;
    }
}



public class BuffObj{
   
    public BuffModel model;

   
    public float duration;

   
    public bool permanent;

 
    public int stack;

  
    public GameObject caster;

    
    public GameObject carrier;

   
    public float timeElapsed = 0.00f;

 
    public int ticked = 0;

 
    public Dictionary<string, object> buffParam = new Dictionary<string, object>();

    public BuffObj(
        BuffModel model, GameObject caster, GameObject carrier,  float duration, int stack, bool permanent = false,
        Dictionary<string, object> buffParam = null
    ){
        this.model = model;
        this.caster = caster;
        this.carrier = carrier;
        this.duration = duration;
        this.stack = stack;
        this.permanent = permanent;
        if (buffParam != null) {
            foreach(KeyValuePair<string, object> kv in buffParam){
                this.buffParam.Add(kv.Key, kv.Value);
            }
        }
    }
}


public struct BuffModel{
  
    public string id;

  
    public string name;

   
    public int priority;

  
    public int maxStack;

   
    public string[] tags;

    public float tickTime;

   
    public ChaProperty[] propMod;

  
    public ChaControlState stateMod;

   
    public BuffOnOccur onOccur;
    public object[] onOccurParams;

 
    public BuffOnTick onTick;
    public object[] onTickParams;

   
    public BuffOnRemoved onRemoved;
    public object[] onRemovedParams;

  
    public BuffOnCast onCast;
    public object[] onCastParams;

   
    public BuffOnHit onHit;
    public object[] onHitParams;

  
    public BuffOnBeHurt onBeHurt;
    public object[] onBeHurtParams;

   
    public BuffOnKill onKill;
    public object[] onKillParams;

  
    public BuffOnBeKilled onBeKilled;
    public object[] onBeKilledParams;
    
    public BuffModel(
        string id, string name, string[] tags, int priority, int maxStack, float tickTime,
        string onOccur, object[] occurParam,
        string onRemoved, object[] removedParam,
        string onTick, object[] tickParam,
        string onCast, object[] castParam,
        string onHit, object[] hitParam,
        string beHurt, object[] hurtParam,
        string onKill, object[] killParam,
        string beKilled, object[] beKilledParam,
        ChaControlState stateMod, ChaProperty[] propMod = null
    ){
        this.id = id;
        this.name = name;
        this.tags = tags;
        this.priority = priority;
        this.maxStack = maxStack;
        this.stateMod = stateMod;
        this.tickTime = tickTime;
        
        this.propMod = new ChaProperty[2]{
            ChaProperty.zero,
            ChaProperty.zero
        };
        if (propMod != null){
            for (int i = 0; i < Mathf.Min(2, propMod.Length); i++){
                this.propMod[i] = propMod[i];
            }
        }

        this.onOccur = (onOccur == "") ? null : DesignerScripts.Buff.onOccurFunc[onOccur];
        this.onOccurParams = occurParam;
        this.onRemoved = (onRemoved == "") ? null : DesignerScripts.Buff.onRemovedFunc[onRemoved];
        this.onRemovedParams = removedParam;
        this.onTick = (onTick == "") ? null : DesignerScripts.Buff.onTickFunc[onTick];
        this.onTickParams = tickParam;
        this.onCast = (onCast == "") ? null : DesignerScripts.Buff.onCastFunc[onCast];
        this.onCastParams = castParam;
        this.onHit = (onHit == "") ? null : DesignerScripts.Buff.onHitFunc[onHit];
        this.onHitParams = hitParam;
        this.onBeHurt = (beHurt == "") ? null: DesignerScripts.Buff.beHurtFunc[beHurt];
        this.onBeHurtParams = hurtParam;        
        this.onKill = (onKill == "") ? null : DesignerScripts.Buff.onKillFunc[onKill];
        this.onKillParams = killParam;
        this.onBeKilled = (beKilled == "") ? null : DesignerScripts.Buff.beKilledFunc[beKilled];
        this.onBeKilledParams = beKilledParam;
    }
}

public delegate void BuffOnOccur(BuffObj buff, int modifyStack);
public delegate void BuffOnRemoved(BuffObj buff);
public delegate void BuffOnTick(BuffObj buff);
public delegate void BuffOnHit(BuffObj buff, ref DamageInfo damageInfo, GameObject target);
public delegate void BuffOnBeHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker);
public delegate void BuffOnKill(BuffObj buff, DamageInfo damageInfo, GameObject target);
public delegate void BuffOnBeKilled(BuffObj buff, DamageInfo damageInfo, GameObject attacker);
public delegate TimelineObj BuffOnCast(BuffObj buff, SkillObj skill, TimelineObj timeline);