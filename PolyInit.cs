using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace lvlv_voronoi
{

    public class voronoi
    {
        public BiomeType[,] BioDiagram = new BiomeType[4, 6]
        {
            {BiomeType.SUBTROPICAL_DESERT,BiomeType.GRASSLAND,BiomeType.TROPICAL_MONSOON,BiomeType.TROPICAL_MONSOON,BiomeType.RAINFOREST,BiomeType.RAINFOREST },
            {BiomeType.TEMPERATE_DESERT,BiomeType.GRASSLAND,BiomeType.GRASSLAND,BiomeType.TEMPERATE_DECIDUOUS,BiomeType.TEMPERATE_DECIDUOUS,BiomeType.TEMPERATE_RAINFOREST },
            {BiomeType.TEMPERATE_DESERT,BiomeType.TEMPERATE_DESERT,BiomeType.SHRUB,BiomeType.SHRUB,BiomeType.CONIFEROUS,BiomeType.CONIFEROUS },
            {BiomeType.SCORCH,BiomeType.ROCK,BiomeType.TUNDRA,BiomeType.SNOW,BiomeType.SNOW,BiomeType.SNOW }
        };
        System.Random seeder;
        public List<Center> voronoiCenterList = new List<Center>();//voroni图所有的中心点
        public Dictionary<Site, Corner> voronoiCornerList = new Dictionary<lvlv_voronoi.Site, Corner>();//voroni图所有的拐角点
        public List<Edge> voronoiEdgeList = new List<Edge>();//vironoi图所有边

        Voronoi voroObject = new Voronoi();


        public voronoi()
        {
            
            //初始化随机数对象
            seeder = new System.Random();
            
        }

       
        public void InitVoroni(int siteCount,int width,int height)
        {
            List<DelaunayTriangle> allTriangle = new List<DelaunayTriangle>();//delaunay三角形集合
            List<Site> sitesP = new List<Site>();
            int seed = seeder.Next();
            System.Random rand = new System.Random(seed);
            List<Edge> trianglesEdgeList = new List<Edge>();//Delaunay三角形网所有边
            
            List<Edge> voronoiRayEdgeList = new List<Edge>();//voroni图外围射线边
            //初始设定点数为20
            //初始设定画布大小是500*400
            //超级三角形顶点坐标为（250,0），（0,400），（500,400）
            //点集区域为（125,200），（125,400），（375,200），（375,400），随便设置，只要满足点落在三角形区域中
            for (int i = 0; i < siteCount; i++)
            {

                Vector2 pf = new Vector2((float)(rand.NextDouble() * width), (float)(rand.NextDouble() * height));
                //PointF pf=new PointF((float)(rand.NextDouble() * 250 + 125), (float)(rand.NextDouble() * 200 + 200));
                Site site = new Site(pf.x, pf.y);
                sitesP.Add(site);
                Center c = new Center();
                c.point = site;
                voronoiCenterList.Add(c);
                
            }

            //按点集坐标X值排序
            sitesP.Sort(new SiteSorterXY());
            voronoiCenterList.Sort(new CenterSorterXY());
            for (int i = 0; i < voronoiCenterList.Count; i++)
            {
                voronoiCenterList[i].index = i;
            }


            int relaxNum = 2;
            for (int r = 0; r < relaxNum; r++)
            {
                if (r > 0)
                {
                    allTriangle.Clear();
                    trianglesEdgeList.Clear();
                    voronoiEdgeList.Clear();
                    voronoiRayEdgeList.Clear();
                    voronoiCornerList.Clear();

                    for (int t = 0; t < voronoiCenterList.Count; t++)
                    {
                        bool isBorder = false;
                        double sumx = 0, sumy = 0;
                        double num = voronoiCenterList[t].corners.Count;
                        foreach (KeyValuePair<Site, Corner> k in voronoiCenterList[t].corners)
                        {
                            if (k.Value.point.x <= 20 || k.Value.point.x >= width - 20 || k.Value.point.y <= 20 || k.Value.point.y >= height - 20)
                            {
                                isBorder = true;
                                break;
                            }
                            sumx += k.Value.point.x;
                            sumy += k.Value.point.y;
                        }
                        Center c = new Center();
                        if (!isBorder)
                        {
                            c.point = new Site(sumx / num, sumy / num);
                            voronoiCenterList[t] = c;
                            sitesP[t] = new Site(sumx / num, sumy / num);
                        }
                        else
                        {
                            c.point = voronoiCenterList[t].point;
                            c.border = true;
                            voronoiCenterList[t] = c;
                            sitesP[t] = c.point;
                        }
                    }


                }

                //将超级三角形的三点添加到三角形网中
                Site A = new Site(0, 0);
                Site B = new Site(width, 0);
                Site C = new Site(0, height);
                Site D = new Site(width, height);
                Center CA = new Center();
                CA.point = A;
                CA.index = -1;
                Center CB = new Center();
                CB.point = B;
                CB.index = -1;
                Center CC = new Center();
                CC.point = C;
                CC.index = -1;
                Center CD = new Center();
                CD.point = D;
                CD.index = -1;
                sitesP.Add(A);
                sitesP.Add(B);
                sitesP.Add(C);
                sitesP.Add(D);

                voronoiCenterList.Add(CA);
                voronoiCenterList.Add(CB);
                voronoiCenterList.Add(CC);
                voronoiCenterList.Add(CD);

                DelaunayTriangle dt = new DelaunayTriangle(A, B, C, CA, CB, CC);
                DelaunayTriangle dt2 = new DelaunayTriangle(D, B, C, CD, CB, CC);
                allTriangle.Add(dt);
                allTriangle.Add(dt2);
                //构造Delaunay三角形网
                voroObject.setDelaunayTriangle(allTriangle, sitesP, voronoiCenterList);

                sitesP.Sort(new SiteSorterXY());
                voronoiCenterList.Sort(new CenterSorterXY());
                for (int i = 0; i < voronoiCenterList.Count; i++)
                {
                    voronoiCenterList[i].index = i;
                }
                //
                //不要移除，这样就不用画Delaunay三角形网外围边的射线
                //移除超级三角形
                //voroObject.remmoveTrianglesByOnePoint(allTriangle, A);
                //voroObject.remmoveTrianglesByOnePoint(allTriangle, B);
                //voroObject.remmoveTrianglesByOnePoint(allTriangle, C);

                //返回Delaunay三角形网所有边
                trianglesEdgeList = voroObject.returnEdgesofTriangleList(allTriangle);



                //填充neighbor
                for (int i = 0; i < allTriangle.Count; i++)
                {
                    DelaunayTriangle t = allTriangle[i];
                    try
                    {
                        t.center1.neighbors.Add(t.center2.index, t.center2);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center1.neighbors.Add(t.center3.index, t.center2);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center2.neighbors.Add(t.center1.index, t.center1);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center2.neighbors.Add(t.center3.index, t.center3);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center3.neighbors.Add(t.center1.index, t.center1);

                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center3.neighbors.Add(t.center2.index, t.center2);
                    }
                    catch (ArgumentException)
                    {

                    }


                }
                //获取所有Voronoi边
                voronoiEdgeList = voroObject.returnVoronoiEdgesFromDelaunayTriangles(allTriangle, voronoiRayEdgeList, voronoiCenterList, voronoiCornerList);

                foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
                {
                    if (k.Value.point.x <= 20 || k.Value.point.x >= width - 20 || k.Value.point.y <= 20 || k.Value.point.y >= height - 20)
                    {
                        k.Value.border = true;

                    }
                }

            }
            

           
        }
        Texture2D spreadPoints()
        {

            Texture2D texture = new Texture2D(1000, 800);
            

            

            return texture;
        }

       
       public void SetWater(float waterRate,int width,int height)
        {
            foreach(KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                if (k.Value.border || Mathf.PerlinNoise((float)k.Key.x, (float)k.Key.y) < waterRate + 0.0008f * Vector2.Distance(new Vector2((float)k.Key.x, (float)k.Key.y), new Vector2(width / 2, height / 2)))
                {
                    k.Value.water = true;
                    foreach(KeyValuePair<int,Center> kc in k.Value.touches)
                    {
                        kc.Value.water = true;
                    }
                }
            }
        }

        public void SetOcean()
        {
            Queue<Center> queue = new Queue<Center>();
            for(int i=0;i<voronoiCenterList.Count;i++)
            {
                if(voronoiCenterList[i].border)
                {
                    
                    voronoiCenterList[i].ocean = true;
                    foreach (KeyValuePair<Site, Corner> ck in voronoiCenterList[i].corners)
                    {
                        ck.Value.ocean = true;
                        ck.Value.water = true;
                    }
                    queue.Enqueue(voronoiCenterList[i]);
                }
            }
            while(queue.Count > 0)
            {
                Center c = queue.Dequeue();
                foreach(KeyValuePair<int,Center> k in c.neighbors)
                {
                    if(!k.Value.ocean)
                    {
                        if(k.Value.water)
                        {
                            k.Value.ocean = true;
                            foreach(KeyValuePair<Site,Corner> ck in k.Value.corners)
                            {
                                ck.Value.ocean = true;
                                ck.Value.water = true;
                            }
                            queue.Enqueue(k.Value);
                        }
                        else
                        {
                            foreach (KeyValuePair<Site, Corner> ck in k.Value.corners)
                            {
                                ck.Value.coast = true;
                            }
                            k.Value.coast = true;
                        }
                    }
                }
            }
            
        }

        public void SetElevation()
        {
            Queue<Corner> queue = new Queue<Corner>();
            foreach(KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                if(k.Value.coast)
                {
                    k.Value.elevation = 0.0f;
                    queue.Enqueue(k.Value);
                }
                else
                {
                    k.Value.elevation = -1;
                }
                foreach(KeyValuePair<int,Center> kcen in k.Value.touches)
                {
                    if(kcen.Value.water)
                    {
                        k.Value.water = true;
                        break;
                    }
                }
            }

            while(queue.Count > 0)
            {
                Corner c = queue.Dequeue();
                float newElevation = c.elevation + 0.01f;
                foreach(KeyValuePair<int,Corner> k in c.adjacent)
                {

                    if (!c.water && !k.Value.water)
                    {
                        newElevation += 1;
                        newElevation += UnityEngine.Random.Range(0, 10);
                    }
                    if(newElevation < k.Value.elevation || k.Value.elevation < 0 )
                    {
                        k.Value.elevation = newElevation;
                        
                        queue.Enqueue(k.Value);
                    }
                }
                
            }

            float maxElevation = 0;

            foreach(KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                if(k.Value.elevation > maxElevation)
                {
                    maxElevation = k.Value.elevation;
                }
            }
            foreach (KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                k.Value.elevation /= maxElevation;
                float maxEle = 0;
                Corner temp = null;
                foreach (KeyValuePair<int, Corner> kc in k.Value.adjacent)
                {
                    if((k.Value.elevation - kc.Value.elevation)>maxEle)
                    {
                        maxEle = k.Value.elevation - kc.Value.elevation;
                        temp = kc.Value;
                    }
                }
                k.Value.downslope = temp;
            }
            for(int i=0;i<voronoiCenterList.Count;i++)
            {
                float sumEle = 0;
                foreach (KeyValuePair<Site, Corner> k in voronoiCenterList[i].corners)
                {
                    sumEle += k.Value.elevation;
                }
                voronoiCenterList[i].elevation = sumEle / voronoiCenterList[i].corners.Count;
            }

        }

        public void SetRiver(int RiverCount,float minElevation)
        {
            for (int i = 0; i < RiverCount; i++)
            {
                Corner startCorner = null;
                while (startCorner == null || startCorner.elevation <= minElevation)
                {
                    int pos = UnityEngine.Random.Range(0, voronoiCornerList.Count);
                    startCorner = voronoiCornerList.Values.ElementAt(pos);
                }
                Corner nextCorner = startCorner;
                while(nextCorner!=null && !nextCorner.water)
                {
                    nextCorner.river += 1;
                    nextCorner = nextCorner.downslope;
                }
            }
        }

        public void SetMoisture(float wet)
        {
            Queue<Corner> queue = new Queue<Corner>();
            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                if ((k.Value.water && !k.Value.ocean) || k.Value.coast || k.Value.river > 0)
                {
                    k.Value.water_distance = 0;
                    queue.Enqueue(k.Value);
                }
                else
                {
                    k.Value.water_distance = -1;
                }
            }

            while (queue.Count > 0)
            {
                Corner c = queue.Dequeue();
                int newWaterdistance = c.water_distance + 1;
                foreach (KeyValuePair<int, Corner> k in c.adjacent)
                {

                    if (!c.water && !k.Value.water)
                    {
                        newWaterdistance += 1;
                    }
                    if (newWaterdistance < k.Value.elevation || k.Value.elevation < 0)
                    {
                        k.Value.water_distance = newWaterdistance;
                        queue.Enqueue(k.Value);
                    }
                }

            }

            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                k.Value.moisture = Mathf.Pow(UnityEngine.Random.Range(0.95f, 1) * wet, k.Value.water_distance);
            }
            float maxMoisture = 0;
            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                if(k.Value.moisture > maxMoisture)
                {
                    maxMoisture = k.Value.moisture;
                }
            }
            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                
                    k.Value.moisture /= maxMoisture;
                
            }

            for (int i = 0; i < voronoiCenterList.Count; i++)
            {
                float sumMoi = 0;
                foreach (KeyValuePair<Site, Corner> k in voronoiCenterList[i].corners)
                {
                    sumMoi += k.Value.moisture;
                }
                voronoiCenterList[i].moisture = sumMoi / voronoiCenterList[i].corners.Count;
            }
        }

        public void SetBiome(BiomeType[,] BiomeDiagram)
        {
            for(int i=0;i<voronoiCenterList.Count;i++)
            {
                Center c = voronoiCenterList[i];
                if(!c.water && !c.coast)
                {
                    int x = (int)(c.elevation * 4);
                    int y = (int)(c.moisture * 6);
                    x = Mathf.Clamp(x, 0, 3);
                    y = Mathf.Clamp(y, 0, 5);
                    c.biome = BiomeDiagram[x, y];
                }
            }
        }
    }
}
