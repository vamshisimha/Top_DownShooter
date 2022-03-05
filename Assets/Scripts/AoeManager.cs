using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AoeManager : MonoBehaviour {
    private void FixedUpdate() {
        GameObject[] aoe = GameObject.FindGameObjectsWithTag("AoE");
        if (aoe.Length <= 0) return;
        GameObject[] cha = GameObject.FindGameObjectsWithTag("Character");
        GameObject[] bullet = GameObject.FindGameObjectsWithTag("Bullet");

        float timePassed = Time.fixedDeltaTime;

        for (int i = 0; i < aoe.Length; i++){
            AoeState aoeState = aoe[i].GetComponent<AoeState>();
            if (!aoeState) continue;

            if (aoeState.duration > 0 && aoeState.tween != null){
                AoeMoveInfo aoeMoveInfo = aoeState.tween(aoe[i], aoeState.tweenRunnedTime);
                aoeState.tweenRunnedTime += timePassed;
                aoeState.SetMoveAndRotate(aoeMoveInfo);
            }
            
            if (aoeState.justCreated == true){
               
                aoeState.justCreated = false;
            
                for (int m = 0; m < cha.Length; m++){
                    if (
                        cha[m] &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z, 
                            cha[m].transform.position.x, cha[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    ){
                        aoeState.characterInRange.Add(cha[m]);
                    }
                }
               
                for (int m = 0; m < bullet.Length; m++){
                    if (
                        bullet[m] &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z, 
                            bullet[m].transform.position.x, bullet[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    ){
                        aoeState.bulletInRange.Add(bullet[m]);
                    }
                }
               
                if (aoeState.model.onCreate != null){
                    aoeState.model.onCreate(aoe[i]);
                }
                
            }else{
                
                List<GameObject> leaveCha = new List<GameObject>();
                List<GameObject> toRemove = new List<GameObject>();
                for (int m = 0; m < aoeState.characterInRange.Count; m++){
                    if (aoeState.characterInRange[m] != null){
                        if (Utils.InRange(
                                aoe[i].transform.position.x, aoe[i].transform.position.z, 
                                aoeState.characterInRange[m].gameObject.transform.position.x, aoeState.characterInRange[m].gameObject.transform.position.z,
                                aoeState.radius
                            ) == false
                        ){
                            leaveCha.Add(aoeState.characterInRange[m]);
                            toRemove.Add(aoeState.characterInRange[m]);
                        }
                    }else{
                        toRemove.Add(aoeState.characterInRange[m]);
                    }
                        
                }
                for (int m = 0; m < toRemove.Count; m++){
                    aoeState.characterInRange.Remove(toRemove[m]);
                }
                if (aoeState.model.onChaLeave != null){
                    aoeState.model.onChaLeave(aoe[i], leaveCha);
                }

               
                List<GameObject> enterCha = new List<GameObject>();
                for (int m = 0; m < cha.Length; m++){
                    if (
                        cha[m] &&
                        aoeState.characterInRange.IndexOf(cha[m]) < 0 &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z, 
                            cha[m].transform.position.x, cha[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    ){
                        enterCha.Add(cha[m]);
                    }
                }
                if (aoeState.model.onChaEnter != null){
                    aoeState.model.onChaEnter(aoe[i], enterCha);
                }
                for (int m = 0; m < enterCha.Count; m++){
                    if (enterCha[m] != null && enterCha[m].GetComponent<ChaState>() && enterCha[m].GetComponent<ChaState>().dead == false){
                        aoeState.characterInRange.Add(enterCha[m]);
                    }
                }

                
                List<GameObject> leaveBullet = new List<GameObject>();
                toRemove = new List<GameObject>();
                for (int m = 0; m < aoeState.bulletInRange.Count; m++){
                    if (aoeState.bulletInRange[m]){
                        if (Utils.InRange(
                                aoe[i].transform.position.x, aoe[i].transform.position.z, 
                                aoeState.bulletInRange[m].gameObject.transform.position.x, aoeState.bulletInRange[m].gameObject.transform.position.z,
                                aoeState.radius
                            ) == false
                        ){
                            leaveBullet.Add(aoeState.bulletInRange[m]);
                            toRemove.Add(aoeState.bulletInRange[m]);
                        }
                    }else{
                        toRemove.Add(aoeState.bulletInRange[m]);
                    }
                        
                }
                for (int m = 0; m < toRemove.Count; m++){
                    aoeState.bulletInRange.Remove(toRemove[m]);
                }
                if (aoeState.model.onBulletLeave != null){
                    aoeState.model.onBulletLeave(aoe[i], leaveBullet);
                }
                toRemove = null;

               
                List<GameObject> enterBullet = new List<GameObject>();
                for (int m = 0; m < bullet.Length; m++){
                    if (
                        bullet[m] &&
                        aoeState.bulletInRange.IndexOf(bullet[m]) < 0 &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z, 
                            bullet[m].transform.position.x, bullet[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    ){
                        enterBullet.Add(bullet[m]);
                    }
                }
                if (aoeState.model.onBulletEnter != null){
                    aoeState.model.onBulletEnter(aoe[i], enterBullet);
                }
                for (int m = 0; m < enterBullet.Count; m++){
                    if (enterBullet[m] != null){
                        aoeState.bulletInRange.Add(enterBullet[m]);
                    }
                }
            }
         
            aoeState.duration -= timePassed;
            aoeState.timeElapsed += timePassed;
            if (aoeState.duration <= 0 || aoeState.HitObstacle() == true){
                if (aoeState.model.onRemoved != null){
                    aoeState.model.onRemoved(aoe[i]);
                }
                Destroy(aoe[i]);
                continue;
            }else{
               
                if (
                    aoeState.model.tickTime > 0 && aoeState.model.onTick != null &&
                    Mathf.RoundToInt(aoeState.duration * 1000) % Mathf.RoundToInt(aoeState.model.tickTime * 1000) == 0
                ){
                    aoeState.model.onTick(aoe[i]);
                }
            }

           
        }

            
    }
}