using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageManager : MonoBehaviour{
    private List<DamageInfo> damageInfos = new List<DamageInfo>();

    private void FixedUpdate() {
        int i = 0;
        while( i < damageInfos.Count ){
            DealWithDamage(damageInfos[i]);
            damageInfos.RemoveAt(0);
        }
    }

   
    private void DealWithDamage(DamageInfo dInfo){
     
        if (!dInfo.defender) return;
        ChaState defenderChaState = dInfo.defender.GetComponent<ChaState>();
        if (!defenderChaState) return;
        ChaState attackerChaState = null;
        if (defenderChaState.dead == true) 
            return;
    
        if (dInfo.attacker){
            attackerChaState = dInfo.attacker.GetComponent<ChaState>();
            for (int i = 0; i < attackerChaState.buffs.Count; i++){
                if (attackerChaState.buffs[i].model.onHit != null){
                    attackerChaState.buffs[i].model.onHit(attackerChaState.buffs[i], ref dInfo, dInfo.defender);
                }
            }
        }
       
        for (int i = 0; i < defenderChaState.buffs.Count; i++){
            if (defenderChaState.buffs[i].model.onBeHurt != null){
               defenderChaState.buffs[i].model.onBeHurt(defenderChaState.buffs[i], ref dInfo, dInfo.attacker);
            }
        }
        if (defenderChaState.CanBeKilledByDamageInfo(dInfo) == true){
            
            if (attackerChaState != null){
                for (int i = 0; i < attackerChaState.buffs.Count; i++){
                    if (attackerChaState.buffs[i].model.onKill != null){
                        attackerChaState.buffs[i].model.onKill(attackerChaState.buffs[i], dInfo, dInfo.defender);
                    }
                }
            }
            for (int i = 0; i < defenderChaState.buffs.Count; i++){
                if (defenderChaState.buffs[i].model.onBeKilled != null){
                    defenderChaState.buffs[i].model.onBeKilled(defenderChaState.buffs[i], dInfo, dInfo.attacker);
                }
            }
        }
     
        bool isHeal = dInfo.isHeal();
        int dVal = dInfo.DamageValue(isHeal);
        if (isHeal == true || defenderChaState.immuneTime <= 0){
            if (dInfo.requireDoHurt() == true && defenderChaState.CanBeKilledByDamageInfo(dInfo) == false){
                UnitAnim ua = defenderChaState.GetComponent<UnitAnim>();
                if (ua) ua.Play("Hurt");
            }
            defenderChaState.ModResource(new ChaResource(
                -dVal
            ));
            
            SceneVariants.PopUpNumberOnCharacter(dInfo.defender, Mathf.Abs(dVal), isHeal);
        }

     
        for (int i = 0; i < dInfo.addBuffs.Count; i++){
            GameObject toCha = dInfo.addBuffs[i].target;
            ChaState toChaState = toCha.Equals(dInfo.attacker) ? attackerChaState : defenderChaState;

            if (toChaState != null && toChaState.dead == false){
                toChaState.AddBuff(dInfo.addBuffs[i]);
            }
        }
        
    }

    
    public void DoDamage(GameObject attacker, GameObject target, Damage damage, float damageDegree, float criticalRate, DamageInfoTag[] tags){
        this.damageInfos.Add(new DamageInfo(
            attacker, target, damage, damageDegree, criticalRate, tags
        ));
    }
}
