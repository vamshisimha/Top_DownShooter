using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitPopText : MonoBehaviour{
  
    private float duration = totalDuration;

    
    public float popHeight = 10.000f;

   
    public GameObject target;

   
    private static float totalDuration = 1.50f;


    private void Update() {
        if (!target) return;

        float timePassed = Time.deltaTime;

        Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.transform.position);
        this.transform.position = pos + Vector2.up * ease((totalDuration - duration) / totalDuration) * popHeight;

        duration -= timePassed;
        if (duration <= 0) Destroy(this.gameObject);
    }

   
    private float ease(float t){
        t = Mathf.Clamp(t, 0.000f, 1.000f);
        return Mathf.Sqrt(t);
    }
}