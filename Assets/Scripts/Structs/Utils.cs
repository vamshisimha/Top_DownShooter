using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils{

    public static bool CircleHitRects(Vector2 circlePivot, float circleRadius, List<Rect> rects){
        if (rects.Count <= 0) return false;
        for (var i = 0; i < rects.Count; i++){
            if (Utils.CircleHitRect(circlePivot, circleRadius, rects[i]) == true){
                return true;
            }
        }
        return false;
    }
    public static bool CircleHitRects(Vector2 circlePivot, float circleRadius, Rect[] rects){
        List<Rect> rl = new List<Rect>();
        for (var i = 0; i < rects.Length; i++){
            rl.Add(rects[i]);
        }
        return CircleHitRects(circlePivot, circleRadius, rl);
    }

    public static bool CircleHitRect(Vector2 circlePivot, float circleRadius, Rect rect){
        int xp = circlePivot.x < rect.x ? 0 : (circlePivot.x > rect.x + rect.width ? 2 : 1);
        int yp = circlePivot.y < rect.y ? 0 : (circlePivot.y > rect.y + rect.height ? 2 : 1);
        
        if (yp == 1 && xp == 1) return true;  //在中间，则一定命中
        
        if (yp != 1 && xp == 1){
            float halfRect = rect.height / 2;
            float toHeart = Mathf.Abs(circlePivot.y - (rect.y + halfRect));
            return (toHeart <= circleRadius + halfRect);
        }else
        if (yp == 1 && xp != 1){
            float halfRect = rect.width / 2;
            float toHeart = Mathf.Abs(circlePivot.x - (rect.x + halfRect));
            return (toHeart <= circleRadius + halfRect);
        }else{
            return InRange(
                circlePivot.x, circlePivot.y, 
                yp == 0 ? rect.x : (rect.x + rect.width), 
                xp == 0 ? rect.y : (rect.y + rect.height), 
                circleRadius
            );
        }
    }

   
    public static bool RectCollide(Rect a, Rect b){
        float ar = a.x + a.width;
        float br = b.x + b.width;
        float ab = a.y + a.height;
        float bb = b.y + b.height;
        return (
            (a.x >= b.x && a.x <= br) ||
            (b.x >= a.x && b.x <= ar)
        ) && (
            (a.y >= b.y && a.y <= bb) ||
            (b.y >= a.y && b.y <= ab)
        );
    }

   
    public static bool InRange(float x1, float y1, float x2, float y2, float range){
        return Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) <= Mathf.Pow(range,  2);
    }


  
    public static string GetTailStringByDegree(float faceDegree, float moveDegree){
        float fd = faceDegree;
        float md = moveDegree;
        while (fd < 180) fd += 360;
        while (md < 180) md += 360;
        fd = fd % 360;
        md = md % 360;
        float dd = md - fd;
        if (dd > 180){
            dd -= 360;
        }else if (dd < -180){
            dd += 360;
        }
        //Debug.Log("degree:"+fd + " / " + md + " / " + dd);
        if (dd >= -45 && dd <= 45){
            return "Forward";
        }else
        if (dd < -45 && dd >= -135){
            return "Left";
        }else
        if (dd > 45 && dd <= 135){
            return "Right";
        }else{
            return "Back";
        }
    }

}