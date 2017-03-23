using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using lvlv_voronoi;
using System.Linq;
using System.IO;
using System;

public class PolyManager : MonoBehaviour {
    voronoi voro = new voronoi();
    
    public GameObject terrain;
    public Color[] biomeColor = new Color[13];
    public Material[] biomeMat = new Material[13];
    
    // Use this for initialization
    void Start () {
	  voro.InitVoroni(500, 500, 400);
        voro.SetWater(0.03f,500,400);
        voro.SetOcean();
        voro.SetElevation();
        voro.SetRiver(UnityEngine.Random.Range(10, 15), 0.5f);
        voro.SetMoisture(0.3f);
        voro.SetBiome(voro.BioDiagram);
        //InitMeshData(terrain, 100);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnPostRender()
    {
        GL.LoadOrtho();

        for (int j = 0; j < voro.voronoiCenterList.Count; j++)
        {
            Dictionary<Edge, Edge> edges = voro.voronoiCenterList[j].borders;
            Site center = voro.voronoiCenterList[j].point;


            foreach (KeyValuePair<Edge, Edge> k in edges)
            {
                Color c = Color.black;
                if (voro.voronoiCenterList[j].water)
                {
                    if (voro.voronoiCenterList[j].ocean)
                        c = Color.blue;
                    else
                    {
                        c = Color.blue;
                        c.g += 0.8f;
                    }

                }
                else if (voro.voronoiCenterList[j].coast)
                {
                    c = new Color((float)193 / 255, (float)165 / 255, (float)124 / 255);
                }
                else
                {
                    c = biomeColor[(int)voro.voronoiCenterList[j].biome];
                    
                }


                GL.Begin(GL.TRIANGLES);
                GL.Color(c);
                GL.Vertex3((float)k.Value.a.x / Screen.width, (float)k.Value.a.y / Screen.height, 0);
                GL.Vertex3((float)k.Value.b.x / Screen.width, (float)k.Value.b.y / Screen.height, 0);
                GL.Vertex3((float)center.x / Screen.width, (float)center.y / Screen.height, 0);
                GL.End();
            }

        }
        for (int i = 0; i < voro.voronoiEdgeList.Count; i++)
        {
            Color c = Color.black;
            GL.Begin(GL.LINES);

            Edge e = voro.voronoiEdgeList[i];
            if (e.cora.water && e.corb.water)
            {
                c = Color.blue;
            }
            if (e.cora.coast && e.corb.coast)
            {
                c = Color.red;
            }
            if (e.cora.river > 0)
            {
                c = (Color.blue + new Color(0, 0.8f, 0));
            }
            GL.Color(c);
            GL.Vertex3((float)e.a.x / Screen.width, (float)e.a.y / Screen.height, 0);
            GL.Vertex3((float)e.b.x / Screen.width, (float)e.b.y / Screen.height, 0);
            GL.End();
        }
    }

    void InitMeshData(GameObject gameObject,float maxHeight)
    {
        List<int> triangles = new List<int>();
        Dictionary<Site, Vector4> vertexes = new Dictionary<Site, Vector4>();
        Dictionary<Vector2, float> biomeValue = new Dictionary<Vector2, float>();
        Dictionary<Vector2, int> biomeDis = new Dictionary<Vector2, int>();
        Texture2D biomeTexture = new Texture2D(500, 400);
        Color[,] textureColor = new Color[600, 500];
        Queue<KeyValuePair<Vector2, float>> queue = new Queue<KeyValuePair<Vector2, float>>();

        for (int i=0;i<voro.voronoiCenterList.Count;i++)
        {
            Site s = voro.voronoiCenterList[i].point;
     
            Vector3 v = new Vector3((float)s.x, voro.voronoiCenterList[i].elevation * maxHeight, (float)s.y);
            v += gameObject.transform.position;
            Vector4 v4 = v;
            v4.w = vertexes.Count;
            vertexes.Add(s,v4);
            float c = (float)(int)voro.voronoiCenterList[i].biome / (float)13;
            int x = (int)s.x;
            int y = (int)s.y;

            
        }
        
        foreach(KeyValuePair<Site,Corner> k in voro.voronoiCornerList)
        {
            Site s = k.Key;
         
            Vector3 v = new Vector3((float)s.x, k.Value.elevation * maxHeight, (float)s.y);
            v += gameObject.transform.position;
            Vector4 v4 = v;
            v4.w = vertexes.Count;
            vertexes.Add(s,v4);
            int sumBio = 0;
            foreach(KeyValuePair<int,Center> kc in k.Value.touches)
            {
                sumBio += (int)kc.Value.biome;
            }

            float c = (float)sumBio / (float)13;
            int x = (int)s.x;
            int y = (int)s.y;

            
            
        }
        Dictionary<int,int> biomeDic = new Dictionary<int, int>();
        for (int i = 0; i < voro.voronoiCenterList.Count; i++)
        {
            Center c = voro.voronoiCenterList[i];
           
            if(!c.water)
            {
                foreach(KeyValuePair<Edge,Edge> k in c.borders)
                {
                    Vector4 v4;
                    vertexes.TryGetValue(c.point, out v4);
                    triangles.Add((int)v4.w);
                    biomeDic.Add(triangles.Count - 1, (int)c.biome);
                    double x1, x2, y1, y2;
                    x1 = k.Value.a.x - c.point.x;
                    x2 = k.Value.b.x - c.point.x;
                    y1 = k.Value.a.y - c.point.y;
                    y2 = k.Value.b.y - c.point.y;
                    float j = (float)(x1 * y2 - x2 * y1);
                    if (j < 0)
                    {
                        vertexes.TryGetValue(k.Value.a, out v4);
                        triangles.Add((int)v4.w);
                        biomeDic.Add(triangles.Count - 1, (int)c.biome);

                        vertexes.TryGetValue(k.Value.b, out v4);
                        triangles.Add((int)v4.w);
                        biomeDic.Add(triangles.Count - 1, (int)c.biome);
                    }
                    else
                    {
                        vertexes.TryGetValue(k.Value.b, out v4);
                        triangles.Add((int)v4.w);
                        biomeDic.Add(triangles.Count - 1, (int)c.biome);

                        vertexes.TryGetValue(k.Value.a, out v4);
                        triangles.Add((int)v4.w);
                        biomeDic.Add(triangles.Count - 1, (int)c.biome);
                    }
                }
            }
        }

        Vector3[] vertexesArray = new Vector3[vertexes.Count];
        int[] trianglesArray = new int[triangles.Count];
        Vector2[] uvArray = new Vector2[vertexes.Count];
        List<int>[] trianglesList = new List<int>[13];
        for(int i=0;i<13;i++)
        {
            trianglesList[i] = new List<int>();
        }
        

        foreach(KeyValuePair<Site,Vector4> k in vertexes)
        {
            Vector3 v3 = k.Value;
            vertexesArray[(int)k.Value.w] = v3;
            uvArray[(int)k.Value.w] = new Vector2(v3.x / 500, v3.z / 400);
        }
        for(int i=0;i<triangles.Count;i++)
        {
            trianglesArray[i] = triangles[i];
            trianglesList[biomeDic[i]].Add(triangles[i]);
        }

        Mesh mesh = gameObject.AddComponent<MeshFilter>().mesh;
        
        
        gameObject.AddComponent<MeshRenderer>();

        
        gameObject.GetComponent<Renderer>().materials = biomeMat;
        for(int i=0;i<13;i++)
        {
            gameObject.GetComponent<Renderer>().materials[i].color = biomeColor[i];
        }

        /*设置mesh*/
        mesh.Clear();//更新  
        mesh.vertices = vertexesArray;
        mesh.uv = uvArray;
        mesh.triangles = trianglesArray;
        mesh.subMeshCount = 13;
        for(int i=0;i<13;i++)
        {
            mesh.SetTriangles(trianglesList[i], i);
        }
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
