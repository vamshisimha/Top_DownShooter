using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageInfo{
    
    public GameObject attacker;

  
    public GameObject defender;

    
    public DamageInfoTag[] tags;

    
    public Damage damage;

   
    public float criticalRate;

   
    public float hitRate = 2.00f;

    
    public float degree;

    
    public List<AddBuffInfo> addBuffs = new List<AddBuffInfo>();

    public DamageInfo(GameObject attacker, GameObject defender, Damage damage, float damageDegree, float baseCriticalRate, DamageInfoTag[] tags){
        this.attacker = attacker;
        this.defender = defender;
        this.damage = damage;
        this.criticalRate = baseCriticalRate;
        this.degree = damageDegree;
        this.tags = new DamageInfoTag[tags.Length];
        for (int i = 0; i < tags.Length; i++){
            this.tags[i] = tags[i];
        }
    }

   
    public int DamageValue(bool asHeal){
        return DesignerScripts.CommonScripts.DamageValue(this, asHeal);
    }

  
    public bool isHeal(){
        for (int i = 0; i < this.tags.Length; i++){
            if (tags[i] == DamageInfoTag.directHeal || tags[i] == DamageInfoTag.periodHeal){
                return true;
            }
        }
        return false;
    }

    
    public bool requireDoHurt(){
        for (int i = 0; i < this.tags.Length; i++){
            if (tags[i] == DamageInfoTag.directDamage){
                return true;
            }
        }
        return false;
    }

  
    public void AddBuffToCha(AddBuffInfo buffInfo){
        this.addBuffs.Add(buffInfo);
    }
}


public struct Damage{
    public int bullet;
    public int explosion;
    public int mental;

    public Damage(int bullet, int explosion = 0, int mental = 0){
        this.bullet = bullet;
        this.explosion = explosion;
        this.mental = mental;
    }

   
    public int Overall(bool asHeal = false){
        return (asHeal == false) ? 
            (Mathf.Max(0, bullet) + Mathf.Max(0, explosion) + Mathf.Max(0, mental)):
            (Mathf.Min(0, bullet) + Mathf.Min(0, explosion) + Mathf.Min(0, mental));
    }

    public static Damage operator +(Damage a, Damage b){
        return new Damage(
            a.bullet + b.bullet,
            a.explosion + b.explosion,
            a.mental + b.mental
        );
    }
    public static Damage operator *(Damage a, float b){
        return new Damage(
            Mathf.RoundToInt(a.bullet * b),
            Mathf.RoundToInt(a.explosion * b),
            Mathf.RoundToInt(a.mental * b)
        );
    }
}


public enum DamageInfoTag{
    directDamage = 0,   
    periodDamage = 1,  
    reflectDamage = 2,  
    directHeal = 10,    
    periodHeal = 11,   
    monkeyDamage = 9999    
}