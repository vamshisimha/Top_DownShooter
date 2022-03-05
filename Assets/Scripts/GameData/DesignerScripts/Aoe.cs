using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignerScripts
{
   
    public class AoE{
        public static Dictionary<string, AoeOnCreate> onCreateFunc = new Dictionary<string, AoeOnCreate>(){
            {"CreateSightEffect", CreateSightEffect}
        };
        public static Dictionary<string, AoeOnRemoved> onRemovedFunc = new Dictionary<string, AoeOnRemoved>(){
            {"DoDamageOnRemoved", DoDamageOnRemoved},
            {"CreateAoeOnRemoved", CreateAoeOnRemoved},
            {"BarrelExplosed", BarrelExplosed}
        };
        public static Dictionary<string, AoeOnTick> onTickFunc = new Dictionary<string, AoeOnTick>(){
            {"BlackHole", BlackHole}
        };
        public static Dictionary<string, AoeOnCharacterEnter> onChaEnterFunc = new Dictionary<string, AoeOnCharacterEnter>(){
            {"DoDamageToEnterCha", DoDamageToEnterCha}
        };
        public static Dictionary<string, AoeOnCharacterLeave> onChaLeaveFunc = new Dictionary<string, AoeOnCharacterLeave>(){
            
        };
        public static Dictionary<string, AoeOnBulletEnter> onBulletEnterFunc = new Dictionary<string, AoeOnBulletEnter>(){
            {"BlockBullets", BlockBullets},
            {"SpaceMonkeyBallHit", SpaceMonkeyBallHit}
        };
        public static Dictionary<string, AoeOnBulletLeave> onBulletLeaveFunc = new Dictionary<string, AoeOnBulletLeave>(){
            
        };
        public static Dictionary<string, AoeTween> aoeTweenFunc = new Dictionary<string, AoeTween>(){
            {"AroundCaster", AroundCaster},
            {"SpaceMonkeyBallRolling", SpaceMonkeyBallRolling}
        };


        private static AoeMoveInfo AroundCaster(GameObject aoe, float t){
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (aoeState == null || aoeState.caster == null) return null;
            Vector3 b = aoeState.caster.transform.position;
            
            float dis = aoeState.tweenParam.Length > 0 ? (float)aoeState.tweenParam[0] : 0;
            float degPlus = aoeState.tweenParam.Length > 1 ? (float)aoeState.tweenParam[1] : 0;
            float cDeg = degPlus * t;
            float dr = cDeg * Mathf.PI / 180; 
            
            Vector3 targetP = new Vector3(
                b.x + Mathf.Sin(dr) * dis - aoe.transform.position.x, 
                0,
                b.z + Mathf.Cos(dr) * dis - aoe.transform.position.z
            );

            //Debug.Log("Around Caster " + aoeState.GetHashCode() + " // " + b);

            return new AoeMoveInfo(MoveType.fly, targetP, cDeg % 360);
        }

       
        private static void BlockBullets(GameObject aoe, List<GameObject> bullets){
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;
            AoeModel am = aoeState.model;
            bool countLimited = am.onBulletEnterParams.Length > 0 ? (bool)am.onBulletEnterParams[0] : false;
            int times = aoeState.param.ContainsKey("times") ? (int)aoeState.param["times"] : 1;

            int side = -1;
            if (aoeState.caster){
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                side = ccs.side;
            }

            for (int i = 0; i < bullets.Count; i++){
                BulletState bs = bullets[i].GetComponent<BulletState>();
                int bSide = -1;
                if (bs && bs.caster){
                    ChaState bcs = bs.caster.GetComponent<ChaState>();
                    if (bcs) bSide = bcs.side;
                }
                if (side != bSide){
                    SceneVariants.RemoveBullet(bullets[i], false);
                    SceneVariants.CreateSightEffect("Effect/HitEffect_B", aoe.transform.position, aoe.transform.eulerAngles.y);
                }
            }

            times -= 1;
        }

       
        private static AoeMoveInfo SpaceMonkeyBallRolling(GameObject aoe, float t){
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return null;

            Vector3 velocity = aoeState.tweenParam.Length > 0 ? (Vector3)aoeState.tweenParam[0] : Vector3.zero;
            velocity *= Time.fixedDeltaTime;
            List<Vector3> forces = aoeState.param.ContainsKey("forces") ? (List<Vector3>)aoeState.param["forces"] : null;
            if (forces != null){
                for (int i = 0; i < forces.Count; i++){
                    velocity += forces[i] * Time.fixedDeltaTime;
                }
            }

            float dis = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.z, 2));
            float rr = Mathf.Atan2(velocity.x, velocity.z);
            float rotateTo = rr * 180 / Mathf.PI;

            return new AoeMoveInfo(MoveType.fly, new Vector3(Mathf.Sin(rr) * dis, 0, Mathf.Cos(rr) * dis), rotateTo);
        }

        
        private static void SpaceMonkeyBallHit(GameObject aoe, List<GameObject> bullets){
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            float baseForce = aoeState.model.onBulletEnterParams.Length > 0 ? (float)aoeState.model.onBulletEnterParams[0] : 0;
            if (baseForce == 0) return;

            int side = -1;
            if (aoeState.caster){
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                side = ccs.side;
            }

            if (aoeState.param.ContainsKey("forces") == false){
                aoeState.param["forces"] = new List<Vector3>();   
            }
            for (int i = 0; i < bullets.Count; i++){
                BulletState bs = bullets[i].GetComponent<BulletState>();
                int bSide = -1;
                
                if (bs){
                    if (bs.caster){
                        ChaState bcs = bs.caster.GetComponent<ChaState>();
                        if (bcs) bSide = bcs.side;
                    }
                    if (bSide == side){
                        Vector3 bMove = bs.velocity * baseForce;    //算了，就直接乘把，凑合凑合
                        ((List<Vector3>)aoeState.param["forces"]).Add(bMove);
                        SceneVariants.RemoveBullet(bullets[i]);
                    }
                }
            }

            float scaleTo = 1 + ((List<Vector3>)aoeState.param["forces"]).Count * 0.05f;
            aoeState.radius = 0.25f * scaleTo;
            aoeState.SetViewScale(scaleTo);
            aoeState.ModViewY(aoeState.radius);
        }

       
        private static void DoDamageToEnterCha(GameObject aoe, List<GameObject> characters){
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            object[] p = aoeState.model.onChaEnterParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(0);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length > 2 ? (bool)p[2] : true;
            bool toAlly = p.Length > 3 ? (bool)p[3] : false;
            bool hurtAction = p.Length > 4 ? (bool)p[4] : false;
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";

            Damage damage = baseDamage * (aoeState.propWhileCreate.attack * damageTimes);

            int side = -1;
            if (aoeState.caster){
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < characters.Count; i++){
                ChaState cs = characters[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side))){
                    Vector3 chaToAoe = characters[i].transform.position - aoe.transform.position;
                    SceneVariants.CreateDamage(
                        aoeState.caster, characters[i], 
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[]{DamageInfoTag.directDamage}
                    );
                    if (hurtAction == true) cs.Play("Hurt");
                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }

       
        private static void DoDamageOnRemoved(GameObject aoe){
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            object[] p = aoeState.model.onRemovedParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(0);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length > 2 ? (bool)p[2] : true;
            bool toAlly = p.Length > 3 ? (bool)p[3] : false;
            bool hurtAction = p.Length > 4 ? (bool)p[4] : false;
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";

            Damage damage = baseDamage * (aoeState.propWhileCreate.attack * damageTimes);

            int side = -1;
            if (aoeState.caster){
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < aoeState.characterInRange.Count; i++){
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side))){
                    Vector3 chaToAoe = aoeState.characterInRange[i].transform.position - aoe.transform.position;
                    SceneVariants.CreateDamage(
                        aoeState.caster, aoeState.characterInRange[i], 
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[]{DamageInfoTag.directDamage}
                    );
                    if (hurtAction == true) cs.Play("Hurt");
                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }


       
        private static void BlackHole(GameObject aoe){
            AoeState ast = aoe.GetComponent<AoeState>();
            if (!ast) return;
            for (int i = 0; i < ast.characterInRange.Count; i++){
                ChaState cs = ast.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false){
                    Vector3 disV = aoe.transform.position - ast.characterInRange[i].transform.position;
                    float distance = Mathf.Sqrt(Mathf.Pow(disV.x, 2) + Mathf.Pow(disV.z, 2));
                    float inTime = distance / (distance + 1.00f);   //1米是0.5秒，之后越来越大，但增幅是变小的
                    cs.AddForceMove(new MovePreorder(
                        disV * inTime, 1.00f
                    ));
                }
            }
        }

       
        private static void CreateSightEffect(GameObject aoe){
            AoeState ast = aoe.GetComponent<AoeState>();
            if (!ast) return;
            object[] p = ast.model.onCreateParams;
            string prefab = p.Length > 0 ? (string)p[0] : "";
            SceneVariants.CreateSightEffect(
                prefab, aoe.transform.position, aoe.transform.eulerAngles.y
            );
        }

      
        private static void CreateAoeOnRemoved(GameObject aoe){
            AoeState ast = aoe.GetComponent<AoeState>();
            if (!ast) return;
            object[] p = ast.model.onRemovedParams;
            if (p.Length <= 0) return;
            string id = (string)p[0];
            if (id == "" || DesingerTables.AoE.data.ContainsKey(id) == false) return;
            AoeModel model = DesingerTables.AoE.data[id];
            float radius = p.Length > 1 ? (float)p[1] : 0.01f;
            float duration = p.Length > 2 ? (float)p[2] : 0;
            string aoeTweenId = p.Length > 3 ? (string)p[3] : "";
            AoeTween tween = null;
            if (aoeTweenId != "" && DesignerScripts.AoE.aoeTweenFunc.ContainsKey(aoeTweenId)){
                tween = DesignerScripts.AoE.aoeTweenFunc[aoeTweenId];
            }
            object[] tp = new object[0];
            if (p.Length > 4) tp = (object[])p[4];
            Dictionary<string,object> ap = null;
            if (p.Length > 5) ap = (Dictionary<string, object>)p[5];
            AoeLauncher al = new AoeLauncher(
                model, ast.caster, aoe.transform.position, radius, 
                duration, aoe.transform.eulerAngles.y, tween, tp, ap
            );
            SceneVariants.CreateAoE(al);
        }

        
        private static void BarrelExplosed(GameObject aoe){
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            //new Damage(0, 50), 0.15f, true, false, true, "Effect/HitEffect_A", "Body"
            Damage baseDamage = new Damage(0, 50);
            float damageTimes = 0.15f;
            string effect = "Effect/HitEffect_A";
            string bp = "Body";

            Damage damage = baseDamage * (aoeState.propWhileCreate.attack * damageTimes);

            int side = -1;
            if (aoeState.caster){
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < aoeState.characterInRange.Count; i++){
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && side != cs.side){
                    if (cs.HasTag("Barrel") == true){
                        SceneVariants.CreateDamage(
                            (GameObject)aoeState.param["Barrel"], aoeState.characterInRange[i],
                            new Damage(0, 9999), 0f, 0f, new DamageInfoTag[]{DamageInfoTag.directDamage}
                        );
                    }else{
                        Vector3 chaToAoe = aoeState.characterInRange[i].transform.position - aoe.transform.position;
                        SceneVariants.CreateDamage(
                            aoeState.caster, aoeState.characterInRange[i], 
                            damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                            0.05f, new DamageInfoTag[]{DamageInfoTag.directDamage}
                        );
                        cs.Play("Hurt");
                        cs.PlaySightEffect(bp, effect);
                    }
                    
                }
            }
        }
    }

    
}