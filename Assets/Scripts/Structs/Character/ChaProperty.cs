using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ChaProperty{
   
    public int hp;

   
    public int attack;

   
    public int moveSpeed;

    public int actionSpeed;

    public int ammo;

  
    public float bodyRadius;

 
    public float hitRadius;

   
    public MoveType moveType;

    public ChaProperty(
        int moveSpeed, int hp = 0, int ammo = 0, int attack = 0, int actionSpeed = 100, 
        float bodyRadius = 0.25f, float hitRadius = 0.25f, MoveType moveType = MoveType.ground
    ){
        this.moveSpeed = moveSpeed;
        this.hp = hp;
        this.ammo = ammo;
        this.attack = attack;
        this.actionSpeed = actionSpeed;
        this.bodyRadius = bodyRadius;
        this.hitRadius = hitRadius;
        this.moveType = moveType;
    }

    
    public static ChaProperty zero = new ChaProperty(0,0,0,0,0,0,0,0);

   
    public void Zero(MoveType moveType = MoveType.ground){
        this.hp = 0;
        this.moveSpeed = 0;
        this.ammo = 0;
        this.attack = 0;
        this.actionSpeed = 0;
        this.bodyRadius = 0;
        this.hitRadius = 0;
        this.moveType = moveType;
    }

   
    public static ChaProperty operator +(ChaProperty a, ChaProperty b){
        return new ChaProperty(
            a.moveSpeed + b.moveSpeed,
            a.hp + b.hp,
            a.ammo + b.ammo,
            a.attack + b.attack,
            a.actionSpeed + b.actionSpeed,
            a.bodyRadius + b.bodyRadius,
            a.hitRadius + b.hitRadius,
            a.moveType == MoveType.fly || b.moveType == MoveType.fly ? MoveType.fly : MoveType.ground
        );
    }
    public static ChaProperty operator *(ChaProperty a, ChaProperty b){
        return new ChaProperty(
            Mathf.RoundToInt(a.moveSpeed * (1.0000f + Mathf.Max(b.moveSpeed, -0.9999f))),
            Mathf.RoundToInt(a.hp * (1.0000f + Mathf.Max(b.hp, -0.9999f))),
            Mathf.RoundToInt(a.ammo * (1.0000f + Mathf.Max(b.ammo, -0.9999f))),
            Mathf.RoundToInt(a.attack * (1.0000f + Mathf.Max(b.attack, -0.9999f))),
            Mathf.RoundToInt(a.actionSpeed * (1.0000f + Mathf.Max(b.actionSpeed, -0.9999f))),
            a.bodyRadius * (1.0000f + Mathf.Max(b.bodyRadius, -0.9999f)),
            a.hitRadius * (1.0000f + Mathf.Max(b.hitRadius, -0.9999f)),
            a.moveType == MoveType.fly || b.moveType == MoveType.fly ? MoveType.fly : MoveType.ground
        );
    }
    public static ChaProperty operator *(ChaProperty a, float b){
        return new ChaProperty(
            Mathf.RoundToInt(a.moveSpeed * b),
            Mathf.RoundToInt(a.hp * b),
            Mathf.RoundToInt(a.ammo * b),
            Mathf.RoundToInt(a.attack * b),
            Mathf.RoundToInt(a.actionSpeed * b),
            a.bodyRadius * b,
            a.hitRadius * b,
            a.moveType
        );
    }
}


public class ChaResource{
   
    public int hp;

 
    public int ammo;

  
    public int stamina;

    public ChaResource(int hp, int ammo = 0, int stamina = 0){
        this.hp = hp;
        this.ammo = ammo;
        this.stamina = stamina;
    }

  
    public bool Enough(ChaResource requirement){
        return (
            this.hp >= requirement.hp &&
            this.ammo >= requirement.ammo &&
            this.stamina >= requirement.stamina
        );
    }

    public static ChaResource operator +(ChaResource a, ChaResource b){
        return new ChaResource(
            a.hp + b.hp,
            a.ammo + b.ammo,
            a.stamina + b.stamina
        );
    }
    public static ChaResource operator *(ChaResource a, float b){
        return new ChaResource(
            Mathf.FloorToInt(a.hp * b),
            Mathf.FloorToInt(a.ammo * b),
            Mathf.FloorToInt(a.stamina * b)
        );
    }
    public static ChaResource operator *(float a, ChaResource b){
        return new ChaResource(
            Mathf.FloorToInt(b.hp * a),
            Mathf.FloorToInt(b.ammo * a),
            Mathf.FloorToInt(b.stamina * a)
        );
    }
    public static ChaResource operator -(ChaResource a, ChaResource b){
        return a + b * (-1);
    }

    public static ChaResource Null = new ChaResource(0);
}