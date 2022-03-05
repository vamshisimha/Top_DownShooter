using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignerScripts
{
    public class Timeline{
        public static Dictionary<string, TimelineEvent> functions = new Dictionary<string, TimelineEvent>(){
            {"CasterPlayAnim", CasterPlayAnim},
            {"CasterForceMove", CasterForceMove},
            {"SetCasterControlState", SetCasterControlState},
            {"PlaySightEffectOnCaster", PlaySightEffectOnCaster},
            {"StopSightEffectOnCaster", StopSightEffectOnCaster},
            {"FireBullet", FireBullet},
            {"CasterImmune", CasterImmune},
            {"CreateAoE", CreateAoE},
            {"AddBuffToCaster", AddBuffToCaster},
            {"CasterAddAmmo", CasterAddAmmo},
            {"SummonCharacter", SummonCharacter}
        };

       
        private static void FireBullet(TimelineObj tlo, params object[] args){
            if (args.Length <= 0) return;
            
            if (tlo.caster){
                UnitBindManager ubm = tlo.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;

                BulletLauncher bLauncher = (BulletLauncher)args[0];
                UnitBindPoint ubp = ubm.GetBindPointByKey(args.Length > 1 ? (string)args[1] : "Muzzle");
                if (!ubp) return;

                bLauncher.caster = tlo.caster;
                bLauncher.fireDegree = tlo.caster.transform.rotation.eulerAngles.y;
                bLauncher.firePosition = ubp.gameObject.transform.position;

                SceneVariants.CreateBullet(bLauncher);
            }
        }

       
        private static void CreateAoE(TimelineObj tlo, params object[] args){
            if (args.Length <= 0) return;
            
            if (tlo.caster){
                UnitBindManager ubm = tlo.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;

                AoeLauncher aLauncher = ((AoeLauncher)args[0]).Clone();
                bool inFront = args.Length > 1 ? (bool)args[1] : true;
                
                aLauncher.caster = tlo.caster;
                aLauncher.degree += tlo.caster.transform.rotation.eulerAngles.y;

                float rr = aLauncher.degree * Mathf.PI / 180;
                Vector3 pos = aLauncher.position;
                
                float dis = Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.z, 2));
                if (inFront == true){
                    dis += tlo.caster.GetComponent<ChaState>().property.bodyRadius + aLauncher.radius;
                } 

                aLauncher.position.x = dis * Mathf.Sin(rr) + tlo.caster.transform.position.x;
                aLauncher.position.z = dis * Mathf.Cos(rr) + tlo.caster.transform.position.z;

                aLauncher.tweenParam = new object[]{
                    new Vector3(
                        dis * Mathf.Sin(rr),
                        0,
                        dis * Mathf.Cos(rr)
                    )
                };

                SceneVariants.CreateAoE(aLauncher);
            }
        }

       
        private static void CasterPlayAnim(TimelineObj tlo, params object[] args){
            if (tlo.caster){
                string animName = args.Length >= 1 ? (string)(args[0]) : "";

                if (animName == "") return;

                bool getTail = args.Length >= 2 ? (bool)(args[1]) : false;
                bool useCurrentDeg = args.Length >= 3 ? (bool)(args[2]) : false;
                
                ChaState cs = tlo.caster.GetComponent<ChaState>();
                if (cs){
                    float faceDeg = useCurrentDeg == true ? cs.faceDegree : (float)tlo.GetValue("faceDegree");
                    float moveDeg = useCurrentDeg == true ? cs.moveDegree : (float)tlo.GetValue("moveDegree");
                    if (getTail == true) animName += Utils.GetTailStringByDegree(faceDeg, moveDeg);
                    cs.Play(animName); 
                }
            }
        }

        
        private static void CasterForceMove(TimelineObj tlo, params object[] args){
            if (tlo.caster){
                ChaState cs = tlo.caster.GetComponent<ChaState>();
                float dis = args.Length >= 1 ? (float)args[0] : 0.00f;
                float inSec = (args.Length >= 2 ? (float)args[1] : 0.00f) / tlo.timeScale;  //移动速度可得手动设置倍速
                float degOffset = args.Length >= 3 ? (float)args[2] : 0.00f;
                bool basedOnMoveDir = args.Length >= 4 ? (bool)args[3] : true;
                bool useCurrentDeg = args.Length >= 5 ? (bool)args[4] : false;
                
                if (cs){
                    float mr = (
                        (
                            basedOnMoveDir == true ? 
                                (useCurrentDeg == true ? cs.moveDegree : (float)tlo.GetValue("moveDegree")) : 
                                (useCurrentDeg == true ? cs.faceDegree : (float)tlo.GetValue("faceDegree"))
                        ) + degOffset
                    ) * Mathf.PI / 180.00f;

                    Vector3 mdir = new Vector3(
                        Mathf.Sin(mr) * dis,
                        0,
                        Mathf.Cos(mr) * dis
                    );
                    cs.AddForceMove(new MovePreorder(mdir, inSec));
                }
            }
        }

        
        private static void SetCasterControlState(TimelineObj tlo, params object[] args){
            if (tlo.caster){
                ChaState cs = tlo.caster.GetComponent<ChaState>();
                if (cs){
                    if (args.Length >= 1) cs.timelineControlState.canMove = (bool)args[0];
                    if (args.Length >= 2) cs.timelineControlState.canRotate = (bool)args[1];
                    if (args.Length >= 3) cs.timelineControlState.canUseSkill = (bool)args[2];
                }
            }
        }

      
        private static void PlaySightEffectOnCaster(TimelineObj tlo, params object[] args){
            if (tlo.caster){
                ChaState cs = tlo.caster.GetComponent<ChaState>();
                if (cs){
                    string bindPointKey = args.Length >= 1 ? (string)args[0] : "Body";
                    string effectName = args.Length >= 2 ? (string)args[1] : "";
                    string effectKey = args.Length >= 3 ? (string)args[2] : Random.value.ToString();
                    bool loop = args.Length >= 4 ? (bool)args[3] : false;
                    cs.PlaySightEffect(bindPointKey, effectName, effectKey, loop);
                }
            }
        }

       
        private static void StopSightEffectOnCaster(TimelineObj tlo, params object[] args){
            if (tlo.caster){
                ChaState cs = tlo.caster.GetComponent<ChaState>();
                if (cs){
                    string bindPointKey = args.Length >= 1 ? (string)args[0] : "Body";
                    string effectKey = args.Length >= 2 ? (string)args[1] : "";
                    if (effectKey == "") return;
                    cs.StopSightEffect(bindPointKey, effectKey);
                }
            }
        }

      
        private static void CasterImmune(TimelineObj timelineObj, params object[] args){
            if (timelineObj.caster){
                ChaState cs = timelineObj.caster.GetComponent<ChaState>();
                if (cs && args.Length > 0){
                    float immT = (float)args[0];
                    cs.SetImmuneTime(immT);
                }
            }
        }

      
        private static void CasterAddAmmo(TimelineObj timelineObj, params object[] args){
            if (timelineObj.caster){
                ChaState cs = timelineObj.caster.GetComponent<ChaState>();
                if (cs && args.Length > 0){
                    int modCount = (int)args[0];
                    cs.ModResource(new ChaResource(cs.resource.hp, modCount + cs.resource.ammo, cs.resource.stamina));
                }
            }
        }

       
        private static void AddBuffToCaster(TimelineObj timelineObj, params object[] args){
            if (timelineObj.caster && args.Length > 0){
                AddBuffInfo abi = (AddBuffInfo)args[0];
                abi.caster = timelineObj.caster;
                abi.target = timelineObj.caster;
                ChaState cs = timelineObj.caster.GetComponent<ChaState>();
                if (cs){
                    cs.AddBuff(abi);
                }
            }
        }

        private static void SummonCharacter(TimelineObj timelineObj, params object[] args){
            if (!timelineObj.caster) return;

            string prefab = args.Length > 0 ? (string)args[0] : "";
            int side = -1;
            Vector3 pos = timelineObj.caster.transform.position;
            ChaProperty cp = args.Length >  1? (ChaProperty)args[1] : new ChaProperty(100, 1); 
            float degree = args.Length > 2 ? (float)args[2] : 0;
            string uaInfo = args.Length > 3 ? (string)args[3] : "";
            string[] tags = args.Length > 4 ? (string[])args[4] : null;
            AddBuffInfo[] addBuffs = args.Length > 5 ? (AddBuffInfo[])args[5] : new AddBuffInfo[0];

            GameObject sumGuy = SceneVariants.CreateCharacter(prefab, side, pos, cp, degree, uaInfo, tags);
            ChaState sgs = sumGuy.GetComponent<ChaState>();
            for (int i = 0; i < addBuffs.Length; i++){
                addBuffs[i].caster = timelineObj.caster;
                addBuffs[i].target = sumGuy;
                sgs.AddBuff(addBuffs[i]);
            }
        }

    }
}