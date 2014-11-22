using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class Region
    {
        private static List<Region> _regions = new List<Region>();
        private static Region refrence;

        public float SqrDist { get { return (Center - refrence.Center).sqrMagnitude; } }

        public Color Color;

        public Vector3 [] Points = new Vector3[6];
        public Region [] Neigbors = new Region[6];

        public Vector2 Center;
        private int size = 10;

        public Region()
        {
            _regions.Add(this);
        }

        public void UpdateBorders()
        {
            refrence = this;

            for (int i = 0; i < 6; i++)
            {
                Points[i] = new Vector3(size * Mathf.Sin(i * Mathf.PI / 4), 0, size * Mathf.Cos(i * Mathf.PI / 4));
            }

        }


    }

    public class MapMesh : MonoBehaviour {

        private const int WIDTH = 500;
        private const int HEIGHT = 500;

        private Region [] _regions= new Region[100];

        // Use this for initialization
        void Start()
        {
            var rand = new Random();

            /*foreach (var reg in _regions)
            {
                reg.Center = new Vector2(Random.Range(-WIDTH,WIDTH), Random.Range(-HEIGHT,HEIGHT));
            }*/

            for (int i = 0; i < 10;i++ )
                for (int j = 0; j < 10; j++)
                {
                    _regions[i*10 + j].Center = new Vector2(i*10, j + 10);
                }


            foreach (var reg in _regions)
            {
                reg.UpdateBorders();
            }



            var mf = (MeshFilter) GetComponent(typeof (MeshFilter));
            var mesh = new Mesh();
            mf.mesh = mesh;

            var vertices = new List<Vector3>();
            var inicies = new List<int>();
            int index = 0;
            foreach (var region in _regions)
            {
                vertices.AddRange(region.Points);
                inicies.AddRange(new [] {index++,index++,index++,index++,index++,index++});
            }

            mesh.vertices = vertices.ToArray();

            var tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            var normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            var uv = new Vector2[4];

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.uv = uv;

        }

        // Update is called once per frame
        void Update () {
	
        }
    }
}
