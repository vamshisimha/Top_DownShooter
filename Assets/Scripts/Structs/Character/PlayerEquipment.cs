using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct EquipmentObj{
   
    public EquipmentModel model;

   
    public EquipmentObj(EquipmentModel model){
        this.model = model;
    }
}


public struct EquipmentModel{
   
    public string id;

  
    public string icon;

   
    public string name;

   
    public string[] tags;

   
    public EqupmentSlot slot;

  
    public ChaProperty equipmentProperty;

   
    public SkillModel[] skills;

  
    public AddBuffInfo[] buffs;

    public EquipmentModel(
        string id, string icon, string name, string[] tags, 
        ChaProperty equipment,
        SkillModel[] skills,
        AddBuffInfo[] buffs,
        EqupmentSlot slot = EqupmentSlot.weapon
    ){
        this.id = id;
        this.name = name;
        this.icon = icon;
        this.tags = tags;
        this.slot = slot;
        this.equipmentProperty = equipment;
        this.skills = skills;
        this.buffs = buffs;
    }
}


public enum EqupmentSlot{
    weapon = 1,
    helm = 2, 
    armor = 3, 
    trinket = 4 

    
}