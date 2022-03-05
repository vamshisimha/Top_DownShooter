using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimelineObj{
   
    public TimelineModel model;
    

  
    public GameObject caster;

  
    public float timeScale{
        get{
            return _timeScale;
        } 
        set{
            _timeScale = Mathf.Max(0.100f, value);
        }
    }
    private float _timeScale = 1.00f;

    
    public object param;

 
    public float timeElapsed = 0;

    

    
    public Dictionary<string, object> values;

    public TimelineObj(TimelineModel model, GameObject caster, object param){
        this.model = model;
        this.caster = caster;
        this.values = new Dictionary<string, object>(); 
        this._timeScale = 1.00f;
        if (caster){
            ChaState cs = caster.GetComponent<ChaState>();
            if (cs){
                this.values.Add("faceDegree", cs.faceDegree);
                this.values.Add("moveDegree", cs.moveDegree);
            }
            this._timeScale = cs.actionSpeed;
        }
        this.param = param;
    }

   
    public object GetValue(string key){
        if (values.ContainsKey(key) == false) return null;
        return values[key];
    }
}


public struct TimelineModel{
    public string id;

  
    public TimelineNode[] nodes;

    public float duration;

   
    public TimelineGoTo chargeGoBack;

    public TimelineModel(string id, TimelineNode[] nodes, float duration, TimelineGoTo chargeGoBack){
        this.id = id;
        this.nodes = nodes;
        this.duration = duration;
        this.chargeGoBack = chargeGoBack;
    }
}


public struct TimelineNode{
   
    public float timeElapsed;

   
    public TimelineEvent doEvent;

   
    public object[] eveParams{get;}

    public TimelineNode(float time, string doEve, params object[] eveArgs){
        this.timeElapsed = time;
        this.doEvent = DesignerScripts.Timeline.functions[doEve];
        this.eveParams = eveArgs;
    }
}


public struct TimelineGoTo{
   
    public float atDuration;

    
    public float gotoDuration;

    public TimelineGoTo(float atDuration, float gotoDuration){
        this.atDuration = atDuration;
        this.gotoDuration = gotoDuration;
    }

    public static TimelineGoTo Null = new TimelineGoTo(float.MaxValue, float.MaxValue);
}

public delegate void TimelineEvent(TimelineObj timeline, params object[] args);