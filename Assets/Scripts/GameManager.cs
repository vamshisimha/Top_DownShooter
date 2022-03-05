using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
   
    public GameObject mainActor{get{
        return mainCharacter;
    }}
    private GameObject mainCharacter;  

 
    private GameObject root;

    
    private Dictionary<string, GameObject> sightEffect = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("GameObjectLayer");

      
        SceneVariants.RandomMap(Random.Range(10, 15), Random.Range(10, 15));
        CreateMapGameObjects();

      
        Vector3 playerPos = SceneVariants.map.GetRandomPosForCharacter(new RectInt(0, 0, SceneVariants.map.MapWidth(), SceneVariants.map.MapHeight()));
        mainCharacter = this.CreateCharacter(
            "FemaleGunner", 1, new Vector3(), new ChaProperty(100, Random.Range(5000,7000), 600, Random.Range(50,70)), 0
        );  
        mainCharacter.AddComponent<PlayerController>().mainCamera = Camera.main;
        
       
        GameObject.Find("Main Camera").GetComponent<CamFollow>().SetFollowCharacter(mainCharacter);   
      
        GameObject.Find("PlayerHP").GetComponent<PlayerStateListener>().playerGameObject = mainCharacter;

       
        mainCharacter.transform.position = playerPos;  
        ChaState mcs = mainCharacter.GetComponent<ChaState>();
        mcs.LearnSkill(DesingerTables.Skill.data["fire"]);
        mcs.LearnSkill(DesingerTables.Skill.data["roll"]);
        mcs.LearnSkill(DesingerTables.Skill.data["spaceMonkeyBall"]);  
        mcs.LearnSkill(DesingerTables.Skill.data["homingMissle"]);   
        mcs.LearnSkill(DesingerTables.Skill.data["cloakBoomerang"]);
        mcs.LearnSkill(DesingerTables.Skill.data["teleportBullet"]);
        mcs.LearnSkill(DesingerTables.Skill.data["grenade"]);
        mcs.LearnSkill(DesingerTables.Skill.data["explosiveBarrel"]);

       
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate() {
        
        List<string> toRemoveKey = new List<string>();
        foreach(KeyValuePair<string, GameObject> se in sightEffect){
            if (se.Value == null) toRemoveKey.Add(se.Key);
        }
        for (int i = 0; i < toRemoveKey.Count; i++) sightEffect.Remove(toRemoveKey[i]);
        toRemoveKey = null;

        
    }


   
    private GameObject CreateFromPrefab(string prefabPath, Vector3 position = new Vector3(), float rotation = 0.00f){
        GameObject go = Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/" + prefabPath),
            position,
            Quaternion.identity
        );
        if (rotation != 0){
            go.transform.Rotate(new Vector3(0, rotation, 0));
        }
        go.transform.SetParent(root.transform);
        return go;
    }

  
    private void CreateMapGameObjects(){
        GameObject[] mt = GameObject.FindGameObjectsWithTag("MapTile");
        for (var i = 0; i < mt.Length; i++){
            Destroy(mt[i]);
        }
        mt = null;

        for (var i = 0; i < SceneVariants.map.MapWidth(); i++){
            for (var j = 0; j < SceneVariants.map.MapHeight(); j++){
                CreateFromPrefab(SceneVariants.map.grid[i,j].prefabPath, new Vector3(i, 0, j));
            }
        }
    }

 
    public void CreateBullet(BulletLauncher bulletLauncher){
       
        GameObject bulletObj = Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/Bullet/BulletObj"),
            bulletLauncher.firePosition,
            Quaternion.identity,
            root.transform
        );
        
      
        bulletObj.transform.RotateAround(bulletObj.transform.position, Vector3.up, bulletLauncher.fireDegree);
        
        bulletObj.GetComponent<BulletState>().InitByBulletLauncher(
            bulletLauncher, 
            GameObject.FindGameObjectsWithTag("Character") 
        );
    }

  
    public void RemoveBullet(GameObject bullet, bool immediately = false){
        if (!bullet) return;
        BulletState bulletState = bullet.GetComponent<BulletState>();
        if (!bulletState) return;
        bulletState.duration = 0;
        if (immediately == true){
            if (bulletState.model.onRemoved != null){
                bulletState.model.onRemoved(bullet);
            }
            Destroy(bullet);
        }
    }

   
    public void CreateAoE(AoeLauncher aoeLauncher){
       
        GameObject aoeObj = Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/Effect/AoeObj"),
            aoeLauncher.position,
            Quaternion.identity,
            root.transform
        );
        
        aoeObj.GetComponent<AoeState>().InitByAoeLauncher(aoeLauncher);
    }

   
    public void RemoveAoE(GameObject aoe, bool immediately = false){
        if (!aoe) return;
        AoeState aoeState = aoe.GetComponent<AoeState>();
        if (!aoeState) return;
        aoeState.duration = 0;
        if (immediately == true){    
            if (aoeState.model.onRemoved != null){
                aoeState.model.onRemoved(aoe);
            }
            Destroy(aoe);
        }
    }

    
    public void CreateSightEffect(string prefab, Vector3 pos, float degree, string key = "", bool loop = false){
        if (sightEffect.ContainsKey(key) == true) return;    

        GameObject effectGO = Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/"+prefab),
            pos,
            Quaternion.identity,
            this.gameObject.transform
        );
        effectGO.transform.RotateAround(effectGO.transform.position, Vector3.up, degree);
        if (!effectGO) return;
        SightEffect se = effectGO.GetComponent<SightEffect>();
        if (!se){
            Destroy(effectGO);
            return;
        } 
        if (loop == false){
            effectGO.AddComponent<UnitRemover>().duration = se.duration;
        }

        if (key != "")  sightEffect.Add(key, effectGO);
    }

    
    public void RemoveSightEffect(string key){
        if (sightEffect.ContainsKey(key) == false) return;
        Destroy(sightEffect[key]);
        sightEffect.Remove(key);
    }

   
    public GameObject CreateCharacter(string prefab, int side, Vector3 pos, ChaProperty baseProp, float degree, string unitAnimInfo = "Default_Gunner", string[] tags = null){
        GameObject chaObj = CreateFromPrefab("Character/CharacterObj");
     
        ChaState cs = chaObj.GetComponent<ChaState>();
        if (cs){
            cs.InitBaseProp(baseProp);
            cs.side = side;
            Dictionary<string, AnimInfo> aInfo = new Dictionary<string, AnimInfo>();
            if (unitAnimInfo != "" && DesingerTables.UnitAnimInfo.data.ContainsKey(unitAnimInfo)){
                aInfo = DesingerTables.UnitAnimInfo.data[unitAnimInfo];
            }
            cs.SetView(CreateFromPrefab("Character/" + prefab), aInfo);
            if (tags != null) cs.tags = tags;
        }
        
        chaObj.transform.position = pos;
        chaObj.transform.RotateAround(chaObj.transform.position, Vector3.up, degree);
        return chaObj;
    }
}
