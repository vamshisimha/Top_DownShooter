using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletManager : MonoBehaviour{
    private void FixedUpdate() {
        
        GameObject[] bullet = GameObject.FindGameObjectsWithTag("Bullet");
        if (bullet.Length <= 0) return;
        GameObject[] character = GameObject.FindGameObjectsWithTag("Character");
        if (bullet.Length <= 0 || character.Length <= 0) return;

        float timePassed = Time.fixedDeltaTime;

        for (int i = 0; i < bullet.Length; i++){
            BulletState bs = bullet[i].GetComponent<BulletState>();
            if (!bs || bs.hp <= 0) continue;

            if (bs.timeElapsed <= 0 && bs.model.onCreate != null){
                bs.model.onCreate(bullet[i]);
            }

           
            int hIndex = 0;
            while (hIndex < bs.hitRecords.Count){
                bs.hitRecords[hIndex].timeToCanHit -= timePassed;
                if (bs.hitRecords[hIndex].timeToCanHit <= 0 || bs.hitRecords[hIndex].target == null){
                   
                    bs.hitRecords.RemoveAt(hIndex);
                }else{
                    hIndex += 1;
                }
            }

           
            bs.SetMoveForce(
                bs.tween == null ? Vector3.forward : bs.tween(bs.timeElapsed, bullet[i], bs.followingTarget)
            );

         
            if (bs.canHitAfterCreated > 0) {
                bs.canHitAfterCreated -= timePassed;  
            }else{
                float bRadius = bs.model.radius;
                int bSide = -1;
                if (bs.caster){
                    ChaState bcs = bs.caster.GetComponent<ChaState>();
                    if (bcs){
                        bSide = bcs.side;
                    }
                }

                for (int j = 0; j < character.Length; j++){
                    if (bs.CanHit(character[j]) == false) continue;

                    ChaState cs = character[j].GetComponent<ChaState>();
                    if (!cs || cs.dead == true || cs.immuneTime > 0) continue;

                    if (
                        (bs.model.hitAlly == false && bSide == cs.side) ||
                        (bs.model.hitFoe == false && bSide != cs.side)
                    ) continue;
                    
                    float cRadius = cs.property.hitRadius;
                    Vector3 dis = bullet[i].transform.position - character[j].transform.position;
                    
                    if (Mathf.Pow(dis.x, 2) + Mathf.Pow(dis.z, 2) <= Mathf.Pow(bRadius + cRadius, 2)){
                     
                        bs.hp -= 1;

                        if (bs.model.onHit != null){
                            bs.model.onHit(bullet[i],character[j]);
                        }
                        
                        if (bs.hp > 0){
                            bs.AddHitRecord(character[j]);
                        }else{
                            Destroy(bullet[i]);
                            continue;
                        }
                    }
                }
            }

           
            bs.duration -= timePassed;
            bs.timeElapsed += timePassed;
            if (bs.duration <= 0 || bs.HitObstacle() == true){
                if (bs.model.onRemoved != null){
                    bs.model.onRemoved(bullet[i]);
                }
                Destroy(bullet[i]);
                continue;
            }
        }
    }
}