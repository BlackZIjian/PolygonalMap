using System;
using System.Collections.Generic;


namespace lvlv_voronoi
{
    public partial class Voronoi
    {
        //根据Delaunay三角形网构造Voronoi图的边
        public List<Edge> returnVoronoiEdgesFromDelaunayTriangles(List<DelaunayTriangle> allTriangle, List<Edge> voronoiRayEdgeList,List<Center> voronoiCenterList,Dictionary<Site,Corner> voronoiCornerList)
        {
            List<Edge> voronoiEdgeList = new List<Edge>();
            //List<Edge> voronoiRayEdgeList = new List<Edge>();
            for (int i = 0; i < allTriangle.Count; i++)
            {
                List<Edge> neighborEdgeList=new List<Edge>();//三角形邻接边集合
                for (int j = 0; j < allTriangle.Count; j++)//为了找出邻接三角形数为2的三角形，即最外边的三角形，循环只能从0开始
                {
                    if (j != i)//不与自身比较
                    {
                        Edge neighborEdge = findCommonEdge(allTriangle[i], allTriangle[j]);
                        if (neighborEdge != null)
                        {
                            neighborEdgeList.Add(neighborEdge);
                            //构造Voronoi边
                            Corner c1, c2;
                            if (!voronoiCornerList.TryGetValue(allTriangle[i].centerPoint, out c1))
                            {
                                c1 = new Corner();
                                c1.point = allTriangle[i].centerPoint;
                                c1.index = voronoiCornerList.Count;
                                voronoiCornerList.Add(allTriangle[i].centerPoint, c1);
                            }
                            if (!voronoiCornerList.TryGetValue(allTriangle[j].centerPoint, out c2))
                            {
                                c2 = new Corner();
                                c2.point = allTriangle[j].centerPoint;
                                c2.index = voronoiCornerList.Count;
                                voronoiCornerList.Add(allTriangle[j].centerPoint, c2);
                            }
                            Edge voronoiEdge = new Edge(allTriangle[i].centerPoint, allTriangle[j].centerPoint,c1,c2);
                            if (!voronoiEdgeList.Contains(voronoiEdge))
                            {
                                voronoiEdgeList.Add(voronoiEdge);
                                
                                //c1的touches添加
                                try
                                {
                                    c1.touches.Add(neighborEdge.ca.index, neighborEdge.ca);
                                }
                                catch(ArgumentException)
                                {

                                }
                                try
                                {
                                    c1.touches.Add(neighborEdge.cb.index, neighborEdge.cb);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //c2的touches添加
                                try
                                {
                                    c2.touches.Add(neighborEdge.ca.index, neighborEdge.ca);
                                }
                                catch (ArgumentException)
                                {

                                }
                                try
                                {
                                    c2.touches.Add(neighborEdge.cb.index, neighborEdge.cb);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //c1的protrudes添加
                                try
                                {
                                    c1.protrudes.Add(voronoiEdge, voronoiEdge);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //c2的protrudes添加
                                try
                                {
                                    c2.protrudes.Add(voronoiEdge, voronoiEdge);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //c1的adjacent添加
                                try
                                {
                                    c1.adjacent.Add(c2.index, c2);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //c2的adjacent添加
                                try
                                {
                                    c2.adjacent.Add(c1.index, c1);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //A的corners添加
                                try
                                {
                                    neighborEdge.ca.corners.Add(c1.point, c1);
                                }
                                catch (ArgumentException)
                                {

                                }
                                try
                                {
                                    neighborEdge.ca.corners.Add(c2.point, c2);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //B的corners添加
                                try
                                {
                                    neighborEdge.cb.corners.Add(c1.point, c1);
                                }
                                catch (ArgumentException)
                                {

                                }
                                try
                                {
                                    neighborEdge.cb.corners.Add(c2.point, c2);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //A的borders添加
                                try
                                {
                                    neighborEdge.ca.borders.Add(voronoiEdge, voronoiEdge);
                                }
                                catch (ArgumentException)
                                {

                                }

                                //B的borders添加
                                try
                                {
                                    neighborEdge.cb.borders.Add(voronoiEdge, voronoiEdge);
                                }
                                catch (ArgumentException)
                                {

                                }

                            }
                        }
                    }
                }
                if (neighborEdgeList.Count== 2)//表示此三角形是外围三角形，Voronoi边需要射线
                {
                    Site midpoint;
                    Edge rayEdge = null;
                    Corner c1, c2 = null;
                    Center a=null, b=null;
                    
                    if (!voronoiCornerList.TryGetValue(allTriangle[i].centerPoint, out c1))
                    {
                        c1 = new Corner();
                        c1.point = allTriangle[i].centerPoint;
                        c1.index = voronoiCornerList.Count;
                        voronoiCornerList.Add(allTriangle[i].centerPoint, c1);
                    }
                    //找出最外边并寻找中点构造Voronoi射线边
                    if (isPointOnEdge(neighborEdgeList[0], allTriangle[i].site1) && isPointOnEdge(neighborEdgeList[1], allTriangle[i].site1))
                    {
                        midpoint = findMidPoint(allTriangle[i].site2, allTriangle[i].site3);
                        rayEdge = produceRayEdge(allTriangle[i].centerPoint, midpoint);//产生较长的射线，原理实现还是线段画出的线
                        voronoiRayEdgeList.Add(rayEdge);
                        a = allTriangle[i].center2;
                        b = allTriangle[i].center3;
                        
                    }
                    if (isPointOnEdge(neighborEdgeList[0], allTriangle[i].site2) && isPointOnEdge(neighborEdgeList[1], allTriangle[i].site2))
                    {
                        midpoint = findMidPoint(allTriangle[i].site1, allTriangle[i].site3);
                        rayEdge = produceRayEdge(allTriangle[i].centerPoint, midpoint);
                        voronoiRayEdgeList.Add(rayEdge);
                        a = allTriangle[i].center1;
                        b = allTriangle[i].center3;
                       
                    } 
                    if (isPointOnEdge(neighborEdgeList[0], allTriangle[i].site3) && isPointOnEdge(neighborEdgeList[1], allTriangle[i].site3))
                    {
                        midpoint = findMidPoint(allTriangle[i].site1, allTriangle[i].site2);
                        rayEdge = produceRayEdge(allTriangle[i].centerPoint, midpoint);
                        voronoiRayEdgeList.Add(rayEdge);
                        a = allTriangle[i].center1;
                        b = allTriangle[i].center2;
                        
                    }

                    if (rayEdge != null && !voronoiCornerList.TryGetValue(rayEdge.b, out c2))
                    {
                        c2 = new Corner();
                        c2.point = rayEdge.b;
                        c2.index = voronoiCornerList.Count;
                        
                        voronoiCornerList.Add(rayEdge.b, c2);
                    }
                   
                    
                    
                    //c2的touches添加
                    try
                    {
                        c2.touches.Add(a.index, a);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        c2.touches.Add(b.index, b);
                    }
                    catch (ArgumentException)
                    {

                    }
                    Edge borderEdge = new Edge(a.point, b.point, a, b);
                    //c1的protrudes添加
                    try
                    {
                        c1.protrudes.Add(borderEdge, borderEdge);
                    }
                    catch (ArgumentException)
                    {

                    }

                    //c2的protrudes添加
                    
                    try
                    {
                        
                        c2.protrudes.Add(borderEdge, borderEdge);
                    }
                    catch (ArgumentException)
                    {

                    }

                    //c1的adjacent添加
                    try
                    {
                        c1.adjacent.Add(c2.index, c2);
                    }
                    catch (ArgumentException)
                    {

                    }

                    //c2的adjacent添加
                    try
                    {
                        c2.adjacent.Add(c1.index, c1);
                    }
                    catch (ArgumentException)
                    {

                    }

                    //A的corners添加
                    try
                    {
                        a.corners.Add(c2.point, c2);
                    }
                    catch (ArgumentException)
                    {

                    }
                    

                    //B的corners添加
                    try
                    {
                        b.corners.Add(c2.point, c2);
                    }
                    catch (ArgumentException)
                    {

                    }
                    

                    //A的borders添加
                    try
                    {
                        a.borders.Add(borderEdge, borderEdge);
                    }
                    catch (ArgumentException)
                    {

                    }

                    //B的borders添加
                    try
                    {
                        b.borders.Add(borderEdge, borderEdge);
                    }
                    catch (ArgumentException)
                    {

                    }
                }
            }
            return voronoiEdgeList;
        }

        //根据三角形链表返回三角形所有的边
        public List<Edge> returnEdgesofTriangleList(List<DelaunayTriangle> allTriangle)
        {
            List<Edge> commonEdges = new List<Edge>();
            for (int i = 0; i < allTriangle.Count; i++)
            {
                Edge edge1 = new Edge(allTriangle[i].site1, allTriangle[i].site2,allTriangle[i].center1,allTriangle[i].center2);
                Edge edge2 = new Edge(allTriangle[i].site1, allTriangle[i].site3, allTriangle[i].center1, allTriangle[i].center3);
                Edge edge3 = new Edge(allTriangle[i].site2, allTriangle[i].site3, allTriangle[i].center2, allTriangle[i].center3);
                if (!commonEdges.Contains(edge1))
                    commonEdges.Add(edge1);
                if (!commonEdges.Contains(edge2))
                    commonEdges.Add(edge2);
                if (!commonEdges.Contains(edge3))
                    commonEdges.Add(edge3);
            }
            return commonEdges;
        }

        //根据点集构造Delaunay三角形网
        public void setDelaunayTriangle(List<DelaunayTriangle> allTriangle, List<Site> sites,List<Center> centers)
        {

            for (int i = 0; i < sites.Count; i++)
            {
                List<DelaunayTriangle> tmpTriList = new List<DelaunayTriangle>();
                //拷贝所有三角形
                for (int j = 0; j < allTriangle.Count; j++)
                {
                    tmpTriList.Add(allTriangle[j]);
                }

                //受影响的三角形链表
                List<DelaunayTriangle> influenedTriangles = new List<DelaunayTriangle>();
                //新形成的三角形链表
                List<DelaunayTriangle> newTriangles = new List<DelaunayTriangle>();
                //受影响三角形的公共边
                List<Edge> commonEdges = new List<Edge>();

                for (int j = 0; j < tmpTriList.Count; j++)
                {
                    double lengthToCenter;//该点到圆心距离
                    lengthToCenter = distance2Point(tmpTriList[j].centerPoint, sites[i]);
                    if (lengthToCenter < tmpTriList[j].radius)
                    {
                        influenedTriangles.Add(tmpTriList[j]);//添加到受影响的三角形链表
                        allTriangle.Remove(tmpTriList[j]);//移除当前三角形
                    }
                 }

               //从受影响的三角形链表中，形成新的三角形链表
               for(int k=0;k<influenedTriangles.Count;k++)
               {
                   addNewDelaunayTriangle(newTriangles,influenedTriangles[k],sites[i],centers[i]);
               }

               //查找受影响三角形的公共边
               if (influenedTriangles.Count > 1)
               {
                   commonEdges = findCommonEdges(influenedTriangles);
               }
               //将受影响三角形中的公共边所在的新形成的三角形排除
               if (commonEdges.Count>0)
               {
                   remmoveTrianglesByEdges(newTriangles, commonEdges);
               }
               //对新形成的三角形进行局部优化
               LOP(newTriangles);
               //将优化后的新形成的三角形添加到三角形链表中
               for (int k = 0; k < newTriangles.Count; k++)
               {
                   allTriangle.Add(newTriangles[k]);
               } 
            }
        }

        //移除所有边边所在的三角形
        public void remmoveTrianglesByEdges(List<DelaunayTriangle> allTriangles, List<Edge> edges)
        {
     
            List<DelaunayTriangle> tmpTriList=new List<DelaunayTriangle>();
            //拷贝所有三角形
            for (int i = 0; i < allTriangles.Count; i++)
            {
                tmpTriList.Add(allTriangles[i]);
            }

            for (int i = 0; i < tmpTriList.Count; i++)
            {
                for (int j = 0; j < edges.Count; j++)
                {
                    if (isEdgeOnTriangle(tmpTriList[i], edges[j]))
                    {
                        allTriangles.Remove(tmpTriList[i]);
                    }
                }
            }
        }

        //移除一条边所在的三角形
        public void remmoveTrianglesByOneEdge(List<DelaunayTriangle> allTriangles, Edge edge)
        {
            List<DelaunayTriangle> tmpTriList = new List<DelaunayTriangle>();
            //拷贝所有三角形
            for (int i = 0; i < allTriangles.Count; i++)
            {
                tmpTriList.Add(allTriangles[i]);
            }
            for (int i = 0; i < tmpTriList.Count; i++)
            {
                if (isEdgeOnTriangle(tmpTriList[i], edge))
                    allTriangles.Remove(tmpTriList[i]);
            }
        }

        //移除点所在的三角形
        public void remmoveTrianglesByOnePoint(List<DelaunayTriangle> allTriangles, Site site)
        {
            List<DelaunayTriangle> tmpTriList = new List<DelaunayTriangle>();
            //拷贝所有三角形
            for (int i = 0; i < allTriangles.Count; i++)
            {
                tmpTriList.Add(allTriangles[i]);
            }
            for (int i = 0; i < tmpTriList.Count; i++)
            {
                if (isPointOnTriangle(tmpTriList[i], site))
                    allTriangles.Remove(tmpTriList[i]);
            }
        }

        //判断边是否属于三角形
        public bool isEdgeOnTriangle(DelaunayTriangle triangel,Edge edge)
        {
            int samePointNum = 0;
            if (siteIsEqual(edge.a, triangel.site1) || siteIsEqual(edge.a, triangel.site2) || siteIsEqual(edge.a, triangel.site3))
                samePointNum++;
            if (siteIsEqual(edge.b, triangel.site1) || siteIsEqual(edge.b, triangel.site2) || siteIsEqual(edge.b, triangel.site3))
                samePointNum++;
            if(samePointNum==2)
                return true;
            return false;
        }

        //判断点是否属于三角形
        public bool isPointOnTriangle(DelaunayTriangle triangle, Site site)
        {
            if (siteIsEqual(site, triangle.site1) || siteIsEqual(site, triangle.site2) || siteIsEqual(site, triangle.site3))
                return true;
            return false;
        }

        //判断点是否在边上
        public bool isPointOnEdge(Edge edge, Site site)
        {
            if (siteIsEqual(site, edge.a) || siteIsEqual(site, edge.b))
                return true;
            return false;
        }

        //将点与受影响的三角形三点连接，形成新的三个三角形添加到三角形链中
        public void addNewDelaunayTriangle(List<DelaunayTriangle> allTriangles, DelaunayTriangle influenedTri, Site point, Center centerPoint)
        {
            allTriangles.Add(new DelaunayTriangle(influenedTri.site1, influenedTri.site2, point, influenedTri.center1, influenedTri.center2, centerPoint));
            allTriangles.Add(new DelaunayTriangle(influenedTri.site1, influenedTri.site3, point, influenedTri.center1, influenedTri.center3, centerPoint));
            allTriangles.Add(new DelaunayTriangle(influenedTri.site2, influenedTri.site3, point, influenedTri.center2, influenedTri.center3, centerPoint));
        }


        //对新形成的三角形进行局部优化
        public List<DelaunayTriangle> LOP(List<DelaunayTriangle> newTriList)
        {
            List<DelaunayTriangle> resultTriList=new List<DelaunayTriangle>();
            //拷贝新形成的三角
            for (int i = 0; i < newTriList.Count; i++)
            {
                resultTriList.Add(newTriList[i]);
            }

            for (int i = 0; i < newTriList.Count; i++)
            {
                for (int j = i + 1; j < newTriList.Count; j++)
                {
                    Edge commonEdge;//需要调整对角线的的三角形的公共边
                    Site anotherPoint=new Site();//新对角线的另一点
                    Center anotherCenter = new Center();
                    if (isInCircle(newTriList[j], newTriList[i].site1))//三角形点在外接圆内
                    {
                        //找出两个三角形的公共边
                        commonEdge=findCommonEdge(newTriList[i],newTriList[j]);
                        if(commonEdge!=null)
                        {  
                            //移除需要调整的三角形
                            resultTriList.Remove(newTriList[i]);
                            resultTriList.Remove(newTriList[j]);
                            //找出对角线的另一点
                            if (siteIsEqual(newTriList[j].site1, commonEdge.a) == false && siteIsEqual(newTriList[j].site1, commonEdge.b) == false)
                            {
                                anotherPoint = newTriList[j].site1;
                                anotherCenter = newTriList[j].center1;
                            }
                            if (siteIsEqual(newTriList[j].site2, commonEdge.a) == false && siteIsEqual(newTriList[j].site2, commonEdge.b) == false)
                            {
                                anotherPoint = newTriList[j].site2;
                                anotherCenter = newTriList[j].center2;
                            }
                            if (siteIsEqual(newTriList[j].site3, commonEdge.a) == false && siteIsEqual(newTriList[j].site3, commonEdge.b) == false)
                            {
                                anotherPoint = newTriList[j].site3;
                                anotherCenter = newTriList[j].center3;
                            }
                            //形成两个新的三角形
                            resultTriList.Add(new DelaunayTriangle(newTriList[i].site1,anotherPoint,commonEdge.a,newTriList[i].center1,anotherCenter,commonEdge.ca));
                            resultTriList.Add(new DelaunayTriangle(newTriList[i].site1,anotherPoint,commonEdge.b,newTriList[i].center1,anotherCenter,commonEdge.cb));
                         }
                    }

                    if (isInCircle(newTriList[j], newTriList[i].site2))//三角形点在外接圆内
                    {
                        //找出两个三角形的公共边
                        commonEdge = findCommonEdge(newTriList[i], newTriList[j]);
                        if(commonEdge!=null)
                        {  
                            //移除需要调整的三角形
                            resultTriList.Remove(newTriList[i]);
                            resultTriList.Remove(newTriList[j]);
                            //找出对角线的另一点
                            if (siteIsEqual(newTriList[j].site1, commonEdge.a) == false && siteIsEqual(newTriList[j].site1, commonEdge.b) == false)
                                anotherPoint = newTriList[j].site1;
                            if (siteIsEqual(newTriList[j].site2, commonEdge.a) == false && siteIsEqual(newTriList[j].site2, commonEdge.b) == false)
                                anotherPoint = newTriList[j].site2;
                            if (siteIsEqual(newTriList[j].site3, commonEdge.a) == false && siteIsEqual(newTriList[j].site3, commonEdge.b) == false)
                                anotherPoint = newTriList[j].site3;
                            //形成两个新的三角形
                            resultTriList.Add(new DelaunayTriangle(newTriList[i].site2, anotherPoint, commonEdge.a, newTriList[i].center2, anotherCenter, commonEdge.ca));
                            resultTriList.Add(new DelaunayTriangle(newTriList[i].site2, anotherPoint, commonEdge.a, newTriList[i].center2, anotherCenter, commonEdge.cb));
                        }
                    }

                    if (isInCircle(newTriList[j], newTriList[i].site3))//三角形点在外接圆内
                    {
                        //找出两个三角形的公共边
                       commonEdge = findCommonEdge(newTriList[i], newTriList[j]);
                       if(commonEdge!=null)
                        {  
                            //移除需要调整的三角形
                            resultTriList.Remove(newTriList[i]);
                            resultTriList.Remove(newTriList[j]);
                            //找出对角线的另一点
                            if (siteIsEqual(newTriList[j].site1, commonEdge.a) == false && siteIsEqual(newTriList[j].site1, commonEdge.b) == false)
                                anotherPoint = newTriList[j].site1;
                            if (siteIsEqual(newTriList[j].site2, commonEdge.a) == false && siteIsEqual(newTriList[j].site2, commonEdge.b) == false)
                                anotherPoint = newTriList[j].site2;
                            if (siteIsEqual(newTriList[j].site3, commonEdge.a) == false && siteIsEqual(newTriList[j].site3, commonEdge.b) == false)
                                anotherPoint = newTriList[j].site3;
                            //形成两个新的三角形
                            resultTriList.Add(new DelaunayTriangle(newTriList[i].site3, anotherPoint, commonEdge.a, newTriList[i].center3, anotherCenter, commonEdge.ca));
                            resultTriList.Add(new DelaunayTriangle(newTriList[i].site3, anotherPoint, commonEdge.a, newTriList[i].center3, anotherCenter, commonEdge.cb));
                        }
                    }
                }
            }
            newTriList = resultTriList;
            return resultTriList;//返回优化后的新形成的三角形
        }



        //找出受影响的三角形的公共边
        public List<Edge> findCommonEdges(List<DelaunayTriangle> influenedTriangles)
        {
            List<Edge> coomonEdges=new List<Edge>();
            Edge tmpEdge;
            for (int i = 0; i < influenedTriangles.Count; i++)
            {
                for (int j = i + 1; j < influenedTriangles.Count; j++)
                {
                    tmpEdge = findCommonEdge(influenedTriangles[i], influenedTriangles[j]);
                    if (tmpEdge!= null)
                    {
                        coomonEdges.Add(tmpEdge);
                    }
                }
            }
            return coomonEdges;
        } 

        //找出两个三角形的公共边
        public Edge findCommonEdge(DelaunayTriangle chgTri1, DelaunayTriangle chgTri2)
        {
            Edge edge;
            List<Site> commonSites=new List<Site>();
            List<Center> commonCenters = new List<Center>();
            if (siteIsEqual(chgTri1.site1, chgTri2.site1) || siteIsEqual(chgTri1.site1, chgTri2.site2) || siteIsEqual(chgTri1.site1, chgTri2.site3))
            {
                commonSites.Add(chgTri1.site1);
                commonCenters.Add(chgTri1.center1);
            }
            if (siteIsEqual(chgTri1.site2, chgTri2.site1) || siteIsEqual(chgTri1.site2, chgTri2.site2) || siteIsEqual(chgTri1.site2, chgTri2.site3))
            {
                commonSites.Add(chgTri1.site2);
                commonCenters.Add(chgTri1.center2);
            }
            if (siteIsEqual(chgTri1.site3, chgTri2.site1) || siteIsEqual(chgTri1.site3, chgTri2.site2) || siteIsEqual(chgTri1.site3, chgTri2.site3))
            {
                commonSites.Add(chgTri1.site3);
                commonCenters.Add(chgTri1.center3);
            }
            if (commonSites.Count == 2)
            {
                edge = new Edge(commonSites[0], commonSites[1],commonCenters[0],commonCenters[1]);
                return edge;
            }
            return null;
        }

        //判断两点是否相同
        public bool siteIsEqual(Site a,Site b)
        {
            if (a.x == b.x && a.y == b.y)
                return true;
            return false;
        }

        //找出亮点的中点
        public Site findMidPoint(Site a,Site b)
        {
            Site midpoint=new Site((a.x+b.x)/2.0,(a.y+b.y)/2.0);
            return midpoint;

        }

        //判断插入点是否在三角形边上
        public Site[] isOnEdges(DelaunayTriangle triangle,Site site)
        {
            Site[] edges=new Site[2];
            Site a=triangle.site1;
            Site b=triangle.site2;
            Site c=triangle.site3;
           
            if((site.y-a.y)*(site.x-b.x)==(site.y-b.y)*(site.x-a.x))//点在ab边上
            {
                edges[0]=a;
                edges[1]=b;
            }

            if ((site.y - a.y) * (site.x - c.x) == (site.y - c.y) * (site.x - a.x))//点在ac边上
            {
                edges[0] = a;
                edges[1] = c;
            }

            if ((site.y - b.y) * (site.x - c.x) == (site.y - c.y) * (site.x - b.x))//点在bc边上
            {
                edges[0] = b;
                edges[1] = c;
            }
            return edges;
        }

     //判断点是否在三角形外接圆的内部
     public bool isInCircle(DelaunayTriangle triangle, Site site)
     {
        double lengthToCenter;//该点到圆心距离
        lengthToCenter=distance2Point(triangle.centerPoint,site);
        if (lengthToCenter < triangle.radius)
        {
            return true;
        }
        return false;
     }

     //根据两点求以第一个点为起点的射线边
     public Edge produceRayEdge(Site start, Site direction)
     {
         Site end = new Site();
            Center centerEnd = new Center();
         Edge longEdge;
         end.x = 100 * (direction.x - start.x) + start.x;//找出射线方向的较大的x终点
         end.y = (direction.y - start.y) * (end.x - start.x) / (direction.x - start.x) + start.y;
         longEdge = new Edge(start,end,new Corner(),new Corner());
         return longEdge;
     }


     //求两点之间距离
     public double distance2Point(Site p,Site p2)
      {
          double value=Math.Sqrt(Math.Abs(p.x-p2.x)*Math.Abs(p.x-p2.x)+Math.Abs(p.y-p2.y)*Math.Abs(p.y-p2.y));
          return value;
      }

     //求三角形的外接圆心
     public void circle_center(Site center, Site sites0, Site sites1, Site sites2, ref double radius) 
      { 
           double x1,x2,x3,y1,y2,y3; 
           double x  =  0; 
           double y  =  0;

           x1 = sites0.x;
           x2 = sites1.x;
           x3 = sites2.x;
           y1 = sites0.y;
           y2 = sites1.y;
           y3 = sites2.y; 
           x=((y2-y1)*(y3*y3-y1*y1+x3*x3-x1*x1)-(y3-y1)*(y2*y2-y1*y1+x2*x2-x1*x1))/(2*(x3-x1)*(y2-y1)-2*((x2-x1)*(y3-y1))); 
           y=((x2-x1)*(x3*x3-x1*x1+y3*y3-y1*y1)-(x3-x1)*(x2*x2-x1*x1+y2*y2-y1*y1))/(2*(y3-y1)*(x2-x1)-2*((y2-y1)*(x3-x1))); 
         
           center.x  =  x  ; 
           center.y  =  y  ;
           radius = Math.Sqrt(Math.Abs(sites0.x - x)*Math.Abs(sites0.x - x) + Math.Abs(sites0.y - y) * Math.Abs(sites0.y - y)); 
     }

        

    }
}
