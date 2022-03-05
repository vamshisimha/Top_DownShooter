using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopTextManager : MonoBehaviour{
    private void Start() {
        
    }

   
    public void PopUpNumberOnCharacter(GameObject cha, int value, bool asHeal = false, bool asCritical = false){
        if (!cha) return;
        Vector2 mScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, cha.transform.position);

        GameObject text = Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/UI/PopText"),
            mScreenPos,
            Quaternion.identity,
            this.gameObject.transform
        );

        text.GetComponent<UnitPopText>().target = cha;

        Text txt = text.GetComponent<Text>();
        txt.text = "<color="+(asHeal == true ? "green" : "red")+">" + (asHeal == true ? "+" : "-") + value.ToString() + (asCritical == true ? "!" : "") + "</color>";
        txt.fontSize = asCritical == false ? 30 : 40;
    }

 
    public void PopUpStringOnCharacter(GameObject cha, string text, int size = 30){
        if (!cha) return;
        Vector2 mScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, cha.transform.position);

        GameObject textObj = Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/UI/PopText"),
            mScreenPos,
            Quaternion.identity,
            this.gameObject.transform
        );

        textObj.GetComponent<UnitPopText>().target = cha;

        Text txt = textObj.GetComponent<Text>();
        txt.text = text;
        txt.fontSize = size;
    }   
}