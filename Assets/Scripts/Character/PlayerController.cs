using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;

    private ChaState chaState;

    void Start() {
        chaState = this.gameObject.GetComponent<ChaState>();   
    }

    void FixedUpdate() {
        if (!chaState || chaState.dead == true) return;

        float ix = Input.GetAxis("Horizontal");
        float iz = Input.GetAxis("Vertical");
        bool[] sBtn = new bool[]{
            Input.GetButton("Fire5"),
            Input.GetButton("Fire4"),
            Input.GetButton("Fire3"),
            Input.GetButton("Fire2"),
            Input.GetButton("Fire1"),
            Input.GetButton("Jump")
        };
        
        Vector2 cursorPos = Input.mousePosition;

        float rotateTo = transform.rotation.eulerAngles.y;
       
        if (mainCamera){
           
            Vector2 mScreenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, transform.position);
            rotateTo = Mathf.Atan2(cursorPos.x - mScreenPos.x, cursorPos.y - mScreenPos.y) * 180.00f / Mathf.PI;
            chaState.OrderRotateTo(rotateTo);
        }

        if (ix != 0 || iz != 0){
            float mSpd = chaState.moveSpeed;
            Vector3 mInfo = new Vector3(ix*mSpd, 0, iz*mSpd);
            chaState.OrderMove(mInfo);
        }

        string[] skillId = new string[]{
             "explosiveBarrel", "teleportBullet","grenade","cloakBoomerang","fire","roll"
        };

        bool btnHolding = false;
        for (int i = 0; i < sBtn.Length; i++){
            if (sBtn[i] == true){
                chaState.CastSkill(skillId[i]);
                btnHolding = true;
            }
        }
        chaState.charging = btnHolding;
    }
}