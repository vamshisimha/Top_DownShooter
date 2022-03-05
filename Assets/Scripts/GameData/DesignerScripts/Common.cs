using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignerScripts
{
    
    public class CommonScripts{
      
        public static int DamageValue(DamageInfo damageInfo, bool asHeal = false){
            bool isCrit = Random.Range(0.00f, 1.00f) <= damageInfo.criticalRate;
            return Mathf.CeilToInt(damageInfo.damage.Overall(asHeal) * (isCrit == true ? 1.80f:1.00f));  
        }
    }
}