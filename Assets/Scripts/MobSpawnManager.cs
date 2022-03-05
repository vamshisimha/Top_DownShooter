using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MobSpawnManager : MonoBehaviour{
    [Tooltip("Keep the maximum number of monsters, once it exceeds, it will not be spawned\nIf the number is too large, I will not be responsible for all consequences...")]
    public int maxMob;

    [Tooltip("The cycle of spawning monsters must be uniform now, I'm not happy to take random numbers, that's it")]
    public float spawnPeriod = 10.0f;  

    private float timePassed = 0;
    private bool justCreated = true;
    private int spawned = 0;    

    private static int mobSide = 2;

    private void FixedUpdate() {
        if (justCreated == true && maxMob > 0){
            Spawn();
        }
        timePassed += Time.fixedDeltaTime;
        if (timePassed >= spawnPeriod){
            timePassed = 0;
            Spawn();
        }
    }

    private void Spawn(){
        GameObject[] cha = GameObject.FindGameObjectsWithTag("Character");
        int toSpawn = maxMob;
        for (int i = 0; i < cha.Length; i++){
            ChaState cs = cha[i].GetComponent<ChaState>();
            if (cs != null && cs.dead == false && cs.side == mobSide){
                toSpawn -= 1;
            }
        }
        for (int i = 0; i < toSpawn; i++){
            GameObject enemy = SceneVariants.CreateCharacter(
                "MaleGunner", mobSide, 
                SceneVariants.map.GetRandomPosForCharacter(new RectInt(0, 0, SceneVariants.map.MapWidth(), SceneVariants.map.MapHeight())),
                new ChaProperty(Random.Range(50,70), 50 + spawned * 2, 0, Random.Range(15,30) + spawned, 100, 0.25f, 0.4f), Random.Range(0.00f, 359.99f)
            );
            enemy.AddComponent<SimpleAI>();
            spawned += 1;
        }
    }
}