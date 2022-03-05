using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AnimInfo{
   
    public string key;

   
    public int priority;

  
    public KeyValuePair<SingleAnimInfo, int>[] animations;


    public static AnimInfo Null = new AnimInfo("", null, 0);

    public AnimInfo(string key, KeyValuePair<SingleAnimInfo, int>[] animations, int priority = 0){
        this.animations = animations;
        this.priority = priority;
        this.key = key;
    }


   
    public SingleAnimInfo RandomKey(){
        if (animations.Length <= 0) return SingleAnimInfo.Null;

        if (animations.Length == 1) return animations[0].Key;
        
        int totalV = 0;
        for (int i = 0; i < animations.Length; i++){
            totalV += animations[i].Value;
        }
        if (totalV <= 0) return SingleAnimInfo.Null;


        int rv = Random.Range(0, totalV);
        int rIndex = 0;
        while (rv > 0){
            rv -= animations[rIndex].Value;
            rIndex += 1;
        }
        rIndex = Mathf.Min(rIndex, animations.Length - 1);
        return animations[rIndex].Key;
    }

  
    public bool ContainsAnim(string animName){
        for (int i = 0; i < animations.Length; i++){
            if (animations[i].Key.animName == animName) return true;
        }
        return false;
    }
}


public struct SingleAnimInfo{
  
    public string animName;

    public float duration;

    public SingleAnimInfo(string animName, float duration = 0){
        this.animName = animName;
        this.duration = duration;
    }

    public static SingleAnimInfo Null = new SingleAnimInfo("", 0);

}