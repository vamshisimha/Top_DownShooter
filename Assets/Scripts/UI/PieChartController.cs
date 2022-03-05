using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PieChartController : MonoBehaviour{
    public float radius = 2;
    [Range(0,360)]
    public float startAngleDegree = 0;
    [Range(0,360)]
    public float angleDegree = 100;

    public int angleDegreePrecision = 1000;
    public int radiusPrecision = 1000;

    private MeshFilter meshFilter;

    private SectorMeshCreator creator = new SectorMeshCreator();

    [ExecuteInEditMode]
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        meshFilter.mesh = creator.CreateMesh(radius, startAngleDegree, angleDegree, angleDegreePrecision, radiusPrecision);
    }

    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        DrawMesh();
    }

   
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        DrawMesh();
    }

    private void DrawMesh()
    {
        Mesh mesh = creator.CreateMesh(radius, startAngleDegree, angleDegree, angleDegreePrecision, radiusPrecision);
        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            Gizmos.DrawLine(convert2World(mesh.vertices[tris[i]]), convert2World(mesh.vertices[tris[i + 1]]));
            Gizmos.DrawLine(convert2World(mesh.vertices[tris[i]]), convert2World(mesh.vertices[tris[i + 2]]));
            Gizmos.DrawLine(convert2World(mesh.vertices[tris[i + 1]]), convert2World(mesh.vertices[tris[i + 2]]));
        }
    }

    private Vector3 convert2World(Vector3 src)
    {
        return transform.TransformPoint(src);
    }

    private class SectorMeshCreator
    {
        private float radius;
        private float startAngleDegree;
        private float angleDegree;

        private Mesh cacheMesh;

        public Mesh CreateMesh(float radius, float startAngleDegree, float angleDegree, int angleDegreePrecision, int radiusPrecision)
        {
            if (checkDiff(radius, startAngleDegree, angleDegree, angleDegreePrecision, radiusPrecision))
            {
                Mesh newMesh = Create(radius, startAngleDegree, angleDegree);
                if (newMesh != null)
                {
                    cacheMesh = newMesh;
                    this.radius = radius;
                    this.startAngleDegree = startAngleDegree;
                    this.angleDegree = angleDegree;
                }
            }
            return cacheMesh;
        }

        private Vector3 CalcPoint(float angle)
        {
            angle = angle % 360;
            if (angle == 0) {
                return new Vector3 (1, 0, 0);
            } else if (angle == 180) {
                return new Vector3(-1,0,0);
            }
            
            if (angle <= 45 || angle > 315) {
                return new Vector3(1,Mathf.Tan(Mathf.Deg2Rad*angle), 0);
            } else if (angle <= 135) {
                return new Vector3(1/Mathf.Tan(Mathf.Deg2Rad*angle), 1, 0);
            } else if (angle <= 225) {
                return new Vector3(-1, -Mathf.Tan(Mathf.Deg2Rad*angle), 0);
            } else {
                return new Vector3(-1/Mathf.Tan(Mathf.Deg2Rad*angle), -1, 0);
            }
        }

        private Mesh Create(float radius, float startAngleDegree, float angleDegree)
        {
            if (startAngleDegree == 360) {
                startAngleDegree = 0;
            }
            Mesh mesh = new Mesh();
            List<Vector3> calcVertices = new List<Vector3> ();
            calcVertices.Add (Vector3.zero);
            calcVertices.Add (CalcPoint (startAngleDegree));

            float [] specialAngle = new float[]{45, 135, 225, 315};
            Vector3 [] specialPoint = new Vector3[] {
                new Vector3(1,1,0), 
                new Vector3(-1,1,0), 
                new Vector3(-1,-1,0),
                new Vector3(1,-1,0)
                // new Vector3(1,0,1),
                // new Vector3(-1,0,1),
                // new Vector3(-1,0,-1),
                // new Vector3(1,0,-1),    
            };
            
            for (int i = 0; i < specialAngle.Length; ++i) {
                if(startAngleDegree < specialAngle[i] && specialAngle[i] - startAngleDegree < angleDegree)
                {
                    calcVertices.Add(specialPoint[i]);
                }
            }
           
            for (int i = 0; i < specialAngle.Length; ++i) {
                if(startAngleDegree < specialAngle[i]+360 && specialAngle[i]+360 - startAngleDegree < angleDegree)
                {
                    calcVertices.Add(specialPoint[i]);
                }
            }
            calcVertices.Add (CalcPoint (startAngleDegree + angleDegree));

            Vector3[] vertices = new Vector3[calcVertices.Count];


            Vector2[] uvs = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; ++i) {
               
                vertices[i] = calcVertices[i]*radius;
              
                uvs[i] = new Vector2(calcVertices[i].x*0.5f+0.5f,calcVertices[i].y*0.5f+0.5f);
            }

            int[] triangles = new int[(vertices.Length-2)*3];
            for (int i = 0, vi = 1; i < triangles.Length; i += 3, vi++)
            {
                triangles[i] = 0;
                triangles[i + 2] = vi;
                triangles[i + 1] = vi + 1;
            }           
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            return mesh;
        }

        private bool checkDiff(float radius, float startAngleDegree, float angleDegree, int angleDegreePrecision, int radiusPrecision)
        {
            return (int)(startAngleDegree - this.startAngleDegree) != 0 || (int)((angleDegree - this.angleDegree) * angleDegreePrecision) != 0 ||
                (int)((radius - this.radius) * radiusPrecision) != 0;
        }
    }
}