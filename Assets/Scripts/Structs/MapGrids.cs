using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct GridInfo{
  
    public string prefabPath;

  
    public bool groundCanPass;

    
    public bool flyCanPass;

    public GridInfo(string prefabPath, bool characterCanPass = true, bool bulletCanPass = true){
        this.prefabPath = prefabPath;
        this.groundCanPass = characterCanPass;
        this.flyCanPass = bulletCanPass;
    }

    public static GridInfo VoidGrid{get;} = new GridInfo("", false, false);
}

public class MapInfo{
  
    public GridInfo[,] grid;

  
    public Vector2 gridSize{get;}

  
    public Rect border{get;}


    public MapInfo(GridInfo[,] map, Vector2 gridSize){
        this.grid = map;
        this.gridSize = new Vector2(
            Mathf.Max(0.1f, gridSize.x),  
            Mathf.Max(0.1f, gridSize.y)
        );
        this.border = new Rect(
            -gridSize.x / 2.00f,
            -gridSize.y / 2.00f,
            gridSize.x * MapWidth(),
            gridSize.y * MapHeight()
        );
    }

    

    
    public int MapWidth(){
        return grid.GetLength(0);
    }

 
    public int MapHeight(){
        return grid.GetLength(1);
    }

  
    public GridInfo GetGridInPosition(Vector3 pos){
        Vector2Int gPos = GetGridPosByMeter(pos.x, pos.z);
        if (gPos.x < 0 || gPos.x >= MapWidth() || gPos.y < 0 || gPos.y >= MapHeight()) 
            return GridInfo.VoidGrid;
        return grid[gPos.x, gPos.y];
    }

  
    public Vector2Int GetGridPosByMeter(float x, float z){
        return new Vector2Int(
           
            Mathf.RoundToInt(x / gridSize.x),
            Mathf.RoundToInt(z / gridSize.y)
        );
    }

   
    public bool CanGridPasses(int gridX, int gridY, MoveType moveType, bool ignoreBorder){
        if (gridX < 0 || gridX >= MapWidth() || gridY < 0 || gridY >= MapHeight()) return ignoreBorder;
        switch (moveType){
            case MoveType.ground: return grid[gridX, gridY].groundCanPass;
            case MoveType.fly: return grid[gridX, gridY].flyCanPass;
        }
        return false;
    }

   
    public bool CanUnitPlacedHere(Vector3 pos, float radius, MoveType moveType){
        Vector2Int lt = GetGridPosByMeter(pos.x - radius, pos.z - radius);
        Vector2Int rb = GetGridPosByMeter(pos.x + radius, pos.z + radius);
        int aw = rb.x - lt.x + 1;
        int ah = rb.y - lt.y + 1;
        List<Rect> collisionRects = new List<Rect>();
        for (int i = lt.x; i <= rb.x; i++){
            for (int j = lt.y; j <= rb.y; j++){
                if (CanGridPasses(i, j, moveType, false) == false){
                    collisionRects.Add(new Rect(
                        (i - 0.5f) * gridSize.x,
                        (j - 0.5f) * gridSize.y,
                        gridSize.x,
                        gridSize.y
                    ));
                }
                
            }
        }
        return !Utils.CircleHitRects(new Vector2(pos.x, pos.z), radius, collisionRects);
        // Vector2Int gPos = GetGridPosByMeter(pos.x, pos.z);
        // if (gPos.x < 0 || gPos.y < 0 || gPos.x >= MapWidth() || gPos.y >= MapHeight()) return false;
        // return grid[gPos.x, gPos.y].groundCanPass;
    }

        
  
    public Vector3 GetRandomPosForCharacter(RectInt range, float chaRadius = 0.00f, MoveType moveType = MoveType.ground){
        List<Vector3> mayRes = new List<Vector3>();
        for (var i = range.x; i < range.x + range.width; i ++){
            for (var j = range.y; j < range.y + range.height; j++){
                //if (i >= 0 && i < MapWidth() && j >= 0 && j < MapHeight() && gridInfo[i, j].characterCanPass == true){
                Vector3 ranPos = new Vector3(
                    i * gridSize.x, 
                    0, 
                    j * gridSize.y
                );
                if (CanUnitPlacedHere(ranPos, chaRadius, moveType) == true) {  
                    mayRes.Add(ranPos);
                }
            }
        }
        return mayRes[Mathf.FloorToInt(Random.Range(0, mayRes.Count))];
    }


    
 
    public float GetNearestVerticalBlock(Vector3 pivot, float dir, float radius, MoveType moveType, bool ignoreBorder){
        if (dir == 0) return pivot.x;
        int dv = dir > 0 ? 1 : -1;
        float bestX = pivot.x + dir;
        int seekWidth = Mathf.CeilToInt((Mathf.Abs(dir) + radius) / gridSize.x + 2);
        Vector2Int gPos = GetGridPosByMeter(pivot.x, pivot.z);
        for (var i = 0; i < seekWidth; i++){
            int cgX = gPos.x + dv * i;
            if (this.CanGridPasses(cgX, gPos.y, moveType, ignoreBorder) == false){
                float wallX = (cgX - dv * 0.5f) * gridSize.x - dv * radius;
                if (dv > 0){
                    return Mathf.Min(wallX, bestX);
                }else{
                    return Mathf.Max(wallX, bestX);
                }
            }
        }
        return bestX;
    }

    public float GetNearestHorizontalBlock(Vector3 pivot, float dir, float radius, MoveType moveType, bool ignoreBorder){
        if (dir == 0) return pivot.z;
        int dv = dir > 0 ? 1 : -1;
        float bestZ = pivot.z + dir;
        int seekHeight = Mathf.CeilToInt((Mathf.Abs(dir) + radius) / gridSize.y + 2);
        Vector2Int gPos = GetGridPosByMeter(pivot.x, pivot.z);
        for (var i = 0; i < seekHeight; i++){
            int cgY = gPos.y + dv * i;
            if (this.CanGridPasses(gPos.x, cgY, moveType, ignoreBorder) == false){
                float wallZ = (cgY - dv * 0.5f) * gridSize.y - dv * radius;
                if (dv > 0){
                    return Mathf.Min(wallZ, bestZ);
                }else{
                    return Mathf.Max(wallZ, bestZ);
                }
            }
        }
        return bestZ;
    }

    
    public MapTargetPosInfo FixTargetPosition(Vector3 pivot, float radius, Vector3 targetPos, MoveType moveType, bool ignoreBorder){
        float xDir = targetPos.x - pivot.x;
        float zDir = targetPos.z - pivot.z;
        float bestX = GetNearestVerticalBlock(pivot, xDir, radius, moveType, ignoreBorder);
        float bestZ = GetNearestHorizontalBlock(pivot, zDir, radius, moveType, ignoreBorder);

        bool obstacled =  (
            Mathf.RoundToInt(bestX * 1000) != Mathf.RoundToInt(targetPos.x * 1000) || 
            Mathf.RoundToInt(bestZ * 1000) != Mathf.RoundToInt(targetPos.z * 1000)
        );
        return new MapTargetPosInfo(obstacled, new Vector3(bestX, targetPos.y, bestZ));
    }
}


public struct MapTargetPosInfo{
   
    public bool obstacle;

   
    public Vector3 suggestPos;

    public MapTargetPosInfo(bool obstacle, Vector3 suggestPos){
        this.obstacle = obstacle;
        this.suggestPos = suggestPos;
    }
}
