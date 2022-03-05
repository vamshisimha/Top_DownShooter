using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitBindManager : MonoBehaviour{
   
    public UnitBindPoint GetBindPointByKey(string key){
        UnitBindPoint[] bindPoints = this.gameObject.GetComponentsInChildren<UnitBindPoint>();
        for (int i = 0; i < bindPoints.Length; i++){
            if (bindPoints[i].key == key){
                return bindPoints[i];
            }
        }
        return null;
    }

   
    public void AddBindGameObject(string bindPointKey, string go, string key, bool loop){
        UnitBindPoint bp = GetBindPointByKey(bindPointKey);
        if (bp == null) return;
        bp.AddBindGameObject(go, key, loop);
    }

  
    public void RemoveBindGameObject(string bindPointKey, string key){
        UnitBindPoint bp = GetBindPointByKey(bindPointKey);
        if (bp == null) return;
        bp.RemoveBindGameObject(key);
    }

   
    public void RemoveAllBindGameObject(string key){
        UnitBindPoint[] bindPoints = this.gameObject.GetComponentsInChildren<UnitBindPoint>();
        for (int i = 0; i < bindPoints.Length; i++){
            bindPoints[i].RemoveBindGameObject(key);
        }
    }
}