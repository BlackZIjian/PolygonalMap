
using System;
using System.Collections.Generic;
namespace lvlv_voronoi
{

    //点
	public class Site
	{
		public double x, y;
 
        public Site()
        { }
        public Site(double x, double y)
		{
            this.x = x;
            this.y = y;
		}
	}
	

	//三角形的边
	public class CenterEdge
	{
        
        public Center a, b;
        public CenterEdge(Center a, Center b)
        {
            this.a = a;
            this.b = b;
            
        }
	}

    public class CornerEdge
    {

        public Corner a, b;
        public CornerEdge(Corner a, Corner b)
        {
            this.a = a;
            this.b = b;

        }
    }
    public class Edge
    {

        public Site a, b;
        public Center ca, cb;
        public Corner cora, corb;
        public int river;
        public Edge(Site a, Site b,Center ca,Center cb)
        {
            this.a = a;
            this.b = b;
            this.ca = ca;
            this.cb = cb;

        }

        public Edge(Site a, Site b, Corner ca, Corner cb)
        {
            this.a = a;
            this.b = b;
            this.cora = ca;
            this.corb = cb;

        }
    }

    //自定义排序规则
    public class SiteSorterXY : IComparer<Site>
	{
        public int Compare(Site p1, Site p2)
		{
			if ( p1.x > p2.x ) return 1;
            if (p1.x < p2.x) return -1;
			return 0;
		}
	}
    public class CenterSorterXY : IComparer<Center>
    {
        public int Compare(Center p1, Center p2)
        {
            if (p1.point.x > p2.point.x) return 1;
            if (p1.point.x < p2.point.x) return -1;
            return 0;
        }
    }

    public class DelaunayTriangle
    {
        Voronoi voronoi = new Voronoi();
        public Site site1, site2, site3;//三角形三点
        public Center center1, center2, center3;
        public Site centerPoint;//外界圆圆心
        public double radius;//外接圆半径
        public List<DelaunayTriangle> adjoinTriangle;//邻接三角形 

        

        public DelaunayTriangle(Site site1, Site site2, Site site3,Center center1, Center center2, Center center3)
        {
            centerPoint = new Site();
            this.site1 = site1;
            this.site2 = site2;
            this.site3 = site3;
            this.center1 = center1;

            this.center2 = center2;

            this.center3 = center3;

            //构造外接圆圆心以及半径
            voronoi.circle_center(centerPoint, site1, site2, site3, ref radius);
        }
    }
    public enum BiomeType
    {
        SNOW,
        TUNDRA,
        ROCK,
        SCORCH,
        CONIFEROUS,
        SHRUB,
        TEMPERATE_DESERT,
        TEMPERATE_RAINFOREST,
        TEMPERATE_DECIDUOUS,
        GRASSLAND,
        RAINFOREST,
        TROPICAL_MONSOON,
        SUBTROPICAL_DESERT
    }
    public class Center
    {
        public int index;

        public Site point;  // location
        public bool water;  // lake or ocean
        public bool ocean;  // ocean
        public bool coast;  // land polygon touching an ocean
        public bool border;  // at the edge of the map
        public BiomeType biome;  // biome type (see article)
        public float elevation = 0;  // 0.0-1.0
        public float moisture = 0;  // 0.0-1.0

        public Dictionary<int,Center> neighbors;
        public Dictionary<Edge,Edge> borders;
        public Dictionary<Site,Corner> corners;

        public Center()
        {
            neighbors = new Dictionary<int, Center>();
            borders = new Dictionary<Edge, Edge>();
            corners = new Dictionary<Site, Corner>();
        }
    }

    public class Corner
    {
        public int index;

        public Site point;  // location
        public bool water;  // lake or ocean
        public bool ocean;  // ocean
        public bool coast;  // land polygon touching an ocean
        public bool border;  // at the edge of the map
        public float elevation = 0;  // 0.0-1.0
        public float moisture = 0;  // 0.0-1.0

        public Dictionary<int,Center> touches;
        public Dictionary<Edge,Edge> protrudes;
        public Dictionary<int,Corner> adjacent;

        public int river;  // 0 if no river, or volume of water in river
        public Corner downslope;  // pointer to adjacent corner most downhill
        public Corner watershed;  // pointer to coastal corner, or null
        public int watershed_size;
        public int water_distance;

        public Corner()
        {
            touches = new Dictionary<int, Center>();
            protrudes = new Dictionary<Edge, Edge>();
            adjacent = new Dictionary<int, Corner>();
        }
    }

}
