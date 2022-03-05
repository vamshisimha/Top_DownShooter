using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitRemover : MonoBehaviour {
    
    public float duration = 1.0f;

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0){
            Destroy(this.gameObject);
        }
    }
}