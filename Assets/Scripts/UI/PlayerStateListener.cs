using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStateListener : MonoBehaviour{
  
    public GameObject playerGameObject;

    Text text;
    private ChaState playerState;

    private void Start() {
        text = this.gameObject.GetComponent<Text>();
    }
    
   
    private void Update() {
        if (playerGameObject == null || text == null) return;
        if (playerState == null) playerState = playerGameObject.GetComponent<ChaState>();
        if (playerState == null) return;   
        int curHp = playerState.resource.hp;
        int maxHp = playerState.property.hp;
        string c = (curHp * 1.000f / (maxHp * 1.000f)) > 0.300f ? "green" : "red";
        text.text = "<color=" + c + ">" + curHp.ToString() + " / " + maxHp.ToString() + "</color>";
    }
}