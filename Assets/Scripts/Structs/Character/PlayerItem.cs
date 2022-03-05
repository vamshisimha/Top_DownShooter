using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ItemObj{
    
    public ItemModel model;

   
    public int count;

  
    public float cooldown;

    public ItemObj(ItemModel model, int count = 0, float cooldown = 0){
        this.model = model;
        this.cooldown = cooldown;
        this.count = count;
    }
}



public struct ItemModel{
   
    public string id;

    
    public string icon;

   
    public string name;

   
    public string[] tags;

 
    public int maxStack;

   
    public TimelineModel useEffect;

    

    public ItemModel(
        string id, string icon, string name, string[] tags, 
        TimelineModel useEffect,
        int maxStack = 1
    ){
        this.id = id;
        this.name = name;
        this.icon = icon;
        this.tags = tags;
        this.maxStack = maxStack;
        this.useEffect = useEffect;
    }
}

