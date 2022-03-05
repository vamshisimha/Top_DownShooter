using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitAnim : MonoBehaviour{
    private Animator animator;

   
    public float timeScale = 1;

   
    public Dictionary<string, AnimInfo> animInfo;

  
    private AnimInfo playingAnim = null;

    private float priorityDuration = 0;

    private int currentAnimPriority{
        get{
            return playingAnim == null ? 0 : 
                (priorityDuration <= 0 ? 0 : playingAnim.priority);
        }
    }

   

    void Start() {
        animator = this.gameObject.GetComponent<Animator>();
    }

    void FixedUpdate() {
        if (!animator) animator = this.gameObject.GetComponentInChildren<Animator>();   

        if (!animator || animInfo == null || animInfo.Count <= 0) return;   

        if (priorityDuration > 0) priorityDuration -= Time.fixedDeltaTime * timeScale;
    }

    
    public void Play(string animName){
        if (animInfo.ContainsKey(animName) == false || animator == null) return;
        if (playingAnim != null && playingAnim.key == animName) return;  
        AnimInfo toPlay = animInfo[animName];
        if (currentAnimPriority > toPlay.priority) return;   
        SingleAnimInfo playOne = toPlay.RandomKey();
        animator.Play(playOne.animName);
        playingAnim = toPlay;
        priorityDuration = playOne.duration;
    }

  
    public void SetAnimator(Animator animator){
        this.animator = animator;
    }
}