using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillObj{
  
    public SkillModel model;

  
    public int level;

  
    public float cooldown;

    public SkillObj(SkillModel model, int level = 1){
        this.model = model;
        this.level = level;
        this.cooldown = 0;
    }
}


public struct SkillModel{
   
    public string id;

   
    public ChaResource condition;

    
    public ChaResource cost;

    
    public TimelineModel effect;

   
    public AddBuffInfo[] buff;

    public SkillModel(string id, ChaResource cost, ChaResource condition, string effectTimeline, AddBuffInfo[] buff){
        this.id = id;
        this.cost = cost;
        this.condition = condition;
        this.effect = DesingerTables.Timeline.data[effectTimeline]; 
        this.buff = buff;
    }
}