using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneVariants{
    public static MapInfo map;

  
    public static void RandomMap(int mapWidth, int mapHeight, float waterline = 6.00f){
        GridInfo grass = new GridInfo("Terrain/Grass");
        GridInfo water = new GridInfo("Terrain/Water", false);
        GridInfo[,] mGrids = new GridInfo[mapWidth, mapHeight];
        for (var i = 0; i < mapWidth; i++){
            for (var j = 0; j < mapHeight; j++){
                float pValue = Mathf.PerlinNoise(i / mapWidth, j / mapHeight) * Random.Range(10.00f, 20.00f);
                mGrids[i, j] = (pValue <= waterline) ? water : grass;
            }
        }
        map = new MapInfo(mGrids, Vector2.one);
    }


    
    public static GameObject MainActor(){
        return GameObject.Find("GameManager").GetComponent<GameManager>().mainActor;
    }

   
    public static void CreateBullet(BulletLauncher bulletLauncher){
        GameObject.Find("GameManager").GetComponent<GameManager>().CreateBullet(bulletLauncher);
    }

  
    public static void RemoveBullet(GameObject bullet, bool immediately = false){
        GameObject.Find("GameManager").GetComponent<GameManager>().RemoveBullet(bullet, immediately);
    }

  
    public static void CreateAoE(AoeLauncher aoeLauncher){
        GameObject.Find("GameManager").GetComponent<GameManager>().CreateAoE(aoeLauncher);
    }

   
    public static void RemoveAoE(GameObject aoe, bool immediately = false){
        GameObject.Find("GameManager").GetComponent<GameManager>().RemoveAoE(aoe, immediately);
    }

  
    public static void CreateTimeline(TimelineModel timelineModel, GameObject caster, object source){
        GameObject.Find("GameManager").GetComponent<TimelineManager>().AddTimeline(timelineModel, caster, source);
    }

 
    public static void CreateTimeline(TimelineObj timeline){
        GameObject.Find("GameManager").GetComponent<TimelineManager>().AddTimeline(timeline);
    }

  
    public static void CreateSightEffect(string prefab, Vector3 pos, float degree, string key = "", bool loop = false){
        GameObject.Find("GameManager").GetComponent<GameManager>().CreateSightEffect(prefab, pos, degree, key, loop);
    }

   
    public static void RemoveSightEffect(string key){
        GameObject.Find("GameManager").GetComponent<GameManager>().RemoveSightEffect(key);
    }

   
    public static void CreateDamage(GameObject attacker, GameObject target, Damage damage, float damageDegree, float criticalRate, DamageInfoTag[] tags){
        GameObject.Find("GameManager").GetComponent<DamageManager>().DoDamage(attacker, target, damage, damageDegree, criticalRate, tags);
    }

  
    public static GameObject CreateCharacter(string prefab, int side, Vector3 pos, ChaProperty baseProp, float degree, string unitAnimInfo = "Default_Gunner", string[] tags = null){
        return GameObject.Find("GameManager").GetComponent<GameManager>().CreateCharacter(prefab, side, pos, baseProp, degree, unitAnimInfo, tags);
    }

    
    public static void PopUpNumberOnCharacter(GameObject cha, int value, bool asHeal = false, bool asCritical = false){
        GameObject.Find("Canvas").GetComponent<PopTextManager>().PopUpNumberOnCharacter(cha, value, asHeal, asCritical);
    }

   
    public static void PopUpStringOnCharacter(GameObject cha, string text, int size = 30){
        GameObject.Find("Canvas").GetComponent<PopTextManager>().PopUpStringOnCharacter(cha, text, size);
    }
}