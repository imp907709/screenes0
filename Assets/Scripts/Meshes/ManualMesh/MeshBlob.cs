using System;
using System.Collections.Generic;
using System.Linq;
using Meshes.GeneralMesh;
using Unity.Mathematics;
using UnityEngine;

namespace Meshes.ManualMesh
{
    public class MeshBlob
    {
        public static List<Vector3> AddRand(List<Vector3> verts)
        {
            var r = new System.Random();
            
            for (int i = 0; i < verts.Count; i++)
            {
                var cordX = verts[i].x + r.Next(1, 1000) * 0.001f;
                var cordY = verts[i].y + r.Next(1, 1000) * 0.001f;
                var cordZ = verts[i].z + r.Next(1, 1000) * 0.001f;
                
                verts[i] = new Vector3(cordX, cordY, cordZ);
            }
            
            return verts;
        }

        public static List<Vector3> DrawPoints3d(int xSize = 10, int ySize = 10, int zSize = 10)
        {
            Debug.Log("DrawPoints3d");
            var res = new List<Vector3>();
            
            for (int i = 0; i < xSize; i++)
            for (int i2 = 0; i2 < zSize; i2++)
            for (int i3 = 0; i3 < zSize; i3++)
                res.Add(new Vector3(i, i3, i2));
            
            return res;
        }
        
        public static List<Vector3> DrawPoints2d(int xSize = 10, int ySize = 10, int zSize = 10)
        {
            var res = new List<Vector3>();
            
            for (int i = 0; i < xSize; i++)
            {
                for (int i2 = 0; i2 < zSize; i2++)
                {
                    res.Add(new Vector3(i, 0, i2));
                }
            }
            
            return res;
        }

        public static List<Vector3> DrawLines(int xSize = 10, int ySize = 10, int zSize = 10)
        {
            var res = new List<Vector3>();

            for (int i = 0; i < xSize; i++)
            {
                res.Add(new Vector3(i,0,0));
            }

            for (int i = 0; i < zSize; i++)
            {
                res.Add(new Vector3(0, 0, i));
            }
            
    
            return res;
        }
        
        public static List<Vector3> AddLowNoise(List<Vector3> verts)
            => AddNoise(verts, 2f, 40f);
        public static List<Vector3> AddHighNoise(List<Vector3> verts)
            => AddNoise(verts, 0.5f, 80f);
        

        public static List<Vector3> AddNoise(List<Vector3> centers, float amplitude, float frequency)
        {
            for(int i =0; i<centers.Count; i++)
            {
                var c = centers[i];
                
                c.y += Mathf.PerlinNoise(
                    c.x * frequency,
                    c.z * frequency
                ) * amplitude;
                
                centers[i]= c;
            }

            return centers;
        }
        
        public static UnityEngine.Mesh CreateTrianlge()
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();
            
            verts.Add(new Vector3(0.0f, 0.0f, 0.0f));
            verts.Add(new Vector3(0.0f, 1.0f, 1.0f));
            verts.Add(new Vector3(1.0f, 1.0f, 0.0f));
      
            tris.Add(0);
            tris.Add(1);
            tris.Add(2);
            
            tris.Add(2);
            tris.Add(1);
            tris.Add(0);
            
            return MeshGeneral.Apply(verts, tris, "CustomTriangle");
        }
        
        public static UnityEngine.Mesh CreateOctahedron(float radius = 1f)
        {
            List<Vector3> verts = new List<Vector3>();

            // 0–7 ring
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2f / 8f;

                verts.Add(new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                ));
            }

            // ADD CENTER (this was missing)
            verts.Add(Vector3.zero); // index 8

            List<int> tris = new List<int>();

            int centerIndex = 8;

            for (int i = 0; i < 8; i++)
            {
                int next = (i + 1) % 8;

                tris.Add(centerIndex);
                tris.Add(i);
                tris.Add(next);
            }

            return MeshGeneral.Apply(verts, tris, "CustomOctahedron");
        }

        public static UnityEngine.Mesh CreateHexagonMesh(float radius = 1f)
        {
            var vecs =  new List<Vector3>();

            for (int i = 0; i < 6; i++)
            {
                // 2p / edges
                float angle = i * Mathf.PI * 2f / 6f;
                var v = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                vecs.Add(v);
            }
            
            // zero center
            vecs.Add(Vector3.zero);
            var centerIndex = 6;
            
            var tris = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                
                tris.Add(centerIndex);
                tris.Add(i);
                tris.Add(next);
            }
            return MeshGeneral.Apply(vecs, tris, "Hexagon");
        }
        
        public static UnityEngine.Mesh CreatePlane(int width = 10, int depth = 10, float resolution = 10)
        {
            var vecs = new List<Vector3>();
            
            float stepX = (float)width / (resolution - 1);
            float stepZ = (float)depth / (resolution - 1);

            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float px = x * stepX;
                    float pz = z * stepZ;
                    
                    vecs.Add(new Vector3(px,0,pz));
                }
            }

            var tris = new List<int>();

            if (tris?.Any() != true)
            {
                debugVecs = vecs;
                return CreateVertexDebugMeshCube(vecs);
            }
            
            return MeshGeneral.Apply(vecs, tris, "Plane");
        }

        
        public static List<Vector3> CreateHexGrid(int width = 10, int height =10, float radius = 1)
        {
            var result = new List<Vector3>();

            float widthStep = radius * 1.5f;
            float heightStep = radius * Mathf.Sqrt(3f);

            for (int q = 0; q < width; q++)
            {
                for (int r = 0; r < height; r++)
                {
                    float x = q * widthStep;
                    float z = (r + q * 0.5f) * heightStep;

                    result.Add(new Vector3(x, 0f, z));
                }
            }

            return result;
        }
        
        private static List<Vector3> debugVecs = new ();
        
        

        public static UnityEngine.Mesh CreatePlaneAdjusted(int width = 10, int depth = 10, float resolution = 50)
        {
            Debug.Log($"CreatePlaneAdjusted {width} {depth} {resolution}");
            var verts = CreatePlaneVertexes(width, depth, resolution);
            
            verts = AdjustVertical(verts, -50,+50,0.01f);
            
            // var mesh = CreateSquareDots(verts);
            var mesh = CreateVertexDebugMeshCube(verts);

            return mesh;
        }

        // Sample vertice array
        public static List<Vector3> CreatePlaneVertexes(int width = 10, int depth = 10, float resolution = 10)
        {
            Debug.Log($"CreatePlaneVertexes {width} {depth} {resolution}");
            var vecs = new List<Vector3>();
            
            float stepX = (float)width / (resolution - 1);
            float stepZ = (float)depth / (resolution - 1);

             Debug.Log($"CreatePlaneVertexes {stepX} {stepZ} ");
            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float px = x * stepX;
                    float pz = z * stepZ;
                    
                    vecs.Add(new Vector3(px,0,pz));
                }
            }

            return vecs;
        }

        public static List<Vector3> AdjustVertical(List<Vector3> verts, int min = -100, int max=100, float range = 0.1f)
        {
            Debug.Log($"AdjustVertical {verts.Count} {min} {max} {range}");
            var r = new System.Random();
            for (int i=0; i<verts.Count; i++)
            {
                var v = verts[i];
                var n = r.Next(min,max) * range;
                v.y = n;
                verts[i] = v;
            }
            return verts;
        }

        // sample debug square a long dots mesh
        public static UnityEngine.Mesh CreateSquareDots(List<Vector3> vecs)
        {
            var tris = new List<int>();

            if (tris?.Any() != true)
            {
                debugVecs = vecs;
                return CreateVertexDebugMeshPlane(vecs);
            }
            
            return MeshGeneral.Apply(vecs, tris, "Plane");
        }
        
        void OnDrawGizmos()
        {
            if (debugVecs == null) return;

            Gizmos.color = Color.red;

            foreach (var v in debugVecs)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }
        }
        
        public static UnityEngine.Mesh CreateVertexDebugMeshCube(List<Vector3> points, float size = 0.05f)
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();

            foreach (var p in points)
            {
                AddCube(p, size, verts, tris);
            }

            UnityEngine.Mesh m = new UnityEngine.Mesh();
            m.SetVertices(verts);
            m.SetTriangles(tris, 0);
            m.RecalculateNormals();
            m.RecalculateBounds();

            return m;
        }
        public static void AddCube(Vector3 center, float size, List<Vector3> verts, List<int> tris)
        {
            int start = verts.Count;
            float h = size * 0.5f;

            verts.Add(center + new Vector3(-h, -h, -h));
            verts.Add(center + new Vector3( h, -h, -h));
            verts.Add(center + new Vector3( h,  h, -h));
            verts.Add(center + new Vector3(-h,  h, -h));

            verts.Add(center + new Vector3(-h, -h,  h));
            verts.Add(center + new Vector3( h, -h,  h));
            verts.Add(center + new Vector3( h,  h,  h));
            verts.Add(center + new Vector3(-h,  h,  h));

            AddQuad(tris, start + 0, start + 1, start + 2, start + 3);
            AddQuad(tris, start + 5, start + 4, start + 7, start + 6);
            AddQuad(tris, start + 4, start + 0, start + 3, start + 7);
            AddQuad(tris, start + 1, start + 5, start + 6, start + 2);
            AddQuad(tris, start + 4, start + 5, start + 1, start + 0);
            AddQuad(tris, start + 3, start + 2, start + 6, start + 7);
        }
        
        
        // Creates sample debug squares on vertices
        public static UnityEngine.Mesh CreateVertexDebugMeshPlane(List<Vector3> verts, float size = 0.05f)
        {
            Debug.Log($"CreateVertexDebugMeshPlane {size}");
            var v = new List<Vector3>();
            var tris = new List<int>();

            foreach (var p in verts)
            {
                Debug.DrawLine(p, p +  new Vector3(size, size, size), Color.green, 10f);
                
                int startIndex = v.Count;

                // simple "cross quad" (cheap visible marker)

                v.Add(p + new Vector3(-size, 0, -size));
                v.Add(p + new Vector3(size, 0, -size));
                v.Add(p + new Vector3(size,0, size));
                v.Add(p + new Vector3(-size, 0, size));

                tris.Add(startIndex + 0);
                tris.Add(startIndex + 1);
                tris.Add(startIndex + 2);

                tris.Add(startIndex + 2);
                tris.Add(startIndex + 3);
                tris.Add(startIndex + 0);
            }

            UnityEngine.Mesh m = new UnityEngine.Mesh();
            m.SetVertices(v);
            m.SetTriangles(tris, 0);
            m.RecalculateNormals();
            m.RecalculateBounds();

            return m;
        }
        
        public static UnityEngine.Mesh CreateDebugCube(Vector3 center, float size)
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();

            float h = size * 0.5f;

            int start = 0;

            // 8 vertices
            verts.Add(center + new Vector3(-h, -h, -h));
            verts.Add(center + new Vector3( h, -h, -h));
            verts.Add(center + new Vector3( h,  h, -h));
            verts.Add(center + new Vector3(-h,  h, -h));

            verts.Add(center + new Vector3(-h, -h,  h));
            verts.Add(center + new Vector3( h, -h,  h));
            verts.Add(center + new Vector3( h,  h,  h));
            verts.Add(center + new Vector3(-h,  h,  h));

            // faces

            AddQuad(tris, start + 0, start + 1, start + 2, start + 3); // back
            AddQuad(tris, start + 5, start + 4, start + 7, start + 6); // front
            AddQuad(tris, start + 4, start + 0, start + 3, start + 7); // left
            AddQuad(tris, start + 1, start + 5, start + 6, start + 2); // right
            AddQuad(tris, start + 4, start + 5, start + 1, start + 0); // bottom
            AddQuad(tris, start + 3, start + 2, start + 6, start + 7); // top

            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        private static void AddQuad(List<int> tris, int a, int b, int c, int d)
        {
            tris.Add(a); tris.Add(b); tris.Add(c);
            tris.Add(a); tris.Add(c); tris.Add(d);
        }
        
        // generalization for angled patch
        // min 3 edges
        // qube 4, hex 6, oct 8, circles ~> 25
        public static (List<Vector3> vecs, List<int> triag, string name) CreateAngledMesh(int vertices = 3, float radius = 1f )
        {
            var vecs =  new List<Vector3>();

            for (int i = 0; i < vertices; i++)
            {
                // 2p / edges
                float angle = i * Mathf.PI * 2f / vertices;
                var v = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                vecs.Add(v);
            }
            
            // zero center
            vecs.Add(Vector3.zero);
            var centerIndex = vertices;
            
            var tris = new List<int>();

            for (int i = 0; i < vertices; i++)
            {
                int next = (i + 1) % vertices;
                
                tris.Add(centerIndex);
                tris.Add(i);
                tris.Add(next);
            }
            return (vecs, tris, "Angled");
        }
        
        /// <summary>Per-hex vertex colors via <see cref="GenerateHexGridColors"/>; pair with a vertex-color material.</summary>
        public static UnityEngine.Mesh CreateHexGridMesh(int width =30, int height =30, float radius =1)
        {
            // 1. build structured grid (your method)
            var grid = CreateHexGridFromShape(width, height, radius);

            ApplyHeightMap(grid, 1f, 0.1f, 7f);
            SmoothHeight(grid, 2);
            
            
            
            // 2. flatten vertices (your method)
            var verts = FlattenGrid(grid);

            // 3. build triangles (your method)
            var tris = CreateHexGridTris(grid);

            // 4. create mesh
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);

            var colors = GenerateHexGridColors(grid);
            mesh.SetColors(colors);
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        public static List<int> CreateHexGridTris(List<List<Vector3>> grid)
        {
            var tris = new List<int>();

            int offset = 0;

            foreach (var hex in grid)
            {
                // YOUR layout:
                // 0–5 = ring
                // 6 = center

                int center = offset + 6;

                for (int i = 0; i < 6; i++)
                {
                    int next = (i + 1) % 6;

                    tris.Add(center);
                    tris.Add(offset + i);
                    tris.Add(offset + next);
                }

                offset += hex.Count; // = 7
            }

            return tris;
        }
        public static List<Vector3> FlattenGrid(List<List<Vector3>> grid)
        {
            var verts = new List<Vector3>();

            foreach (var hex in grid)
            {
                verts.AddRange(hex);
            }

            return verts;
        }
        
        public static List<Color> GenerateHexGridColors(List<List<Vector3>> grid)
        {
            var colors = new List<Color>();
            var rnd = new System.Random();

            foreach (var hex in grid)
            {
                Color c = new Color(
                    (float)rnd.NextDouble(),
                    (float)rnd.NextDouble(),
                    (float)rnd.NextDouble()
                );

                for (int i = 0; i < hex.Count; i++)
                    colors.Add(c);
            }

            return colors;
        }
        
        /// <summary>
        /// <see cref="CreateHexGridMesh"/> plus a renderer material that shows <c>mesh.colors</c> (vertex colors are not applied by <see cref="CreateHexGridMesh"/> alone — Unity needs a matching shader).
        /// </summary>
        public static GameObject CreateHexGridMeshObject(
            int width = 30,
            int height = 30,
            float radius = 0.1f,
            string objectName = "HexGrid")
        {
            Debug.Log($"{nameof(CreateHexGridMeshObject)} {width} {height} {radius}");
            
            // Mesh mesh = CreateHexGridMesh(width, height, radius);
            var meshs = CreateHexGridMeshesNew(width, height, radius);
            var mesh = MergeHexMeshes(meshs);
            Material material = MaterialFactory.GetBiomeVertexColorMaterial() ?? MaterialFactory.GetDefaultMaterial();
            return MeshObjectFactory.Create(mesh, material, objectName);
        }
        public static UnityEngine.Mesh MergeHexMeshes(List<UnityEngine.Mesh> meshes)
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();
            var colors = new List<Color>();

            int offset = 0;

            foreach (var m in meshes)
            {
                var mVerts = m.vertices;
                var mTris = m.triangles;
                var mColors = m.colors;

                verts.AddRange(mVerts);

                for (int i = 0; i < mTris.Length; i++)
                    tris.Add(mTris[i] + offset);

                colors.AddRange(mColors);

                offset += mVerts.Length;
            }

            var mesh = new UnityEngine.Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetColors(colors);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        public static List<UnityEngine.Mesh> CreateHexGridMeshesNew(
            int width = 30,
            int height = 30,
            float radius = 1)
        {
            // 1
            var grid = CreateHexGridFromShape(width, height, radius);

            // 2
            ApplyHeightMap(grid, 1f, 0.1f, 7f);

            // 3
            SmoothHeight(grid, 2);

            // 4
            return BuildHexCellMeshes(grid);
        }
        
        /// <summary>
        /// Flat-top regular hex on the XZ plane (Y up), circumradius <paramref name="radius"/>.
        /// Matches <see cref="CreateHexGridFromShape"/> center spacing (axial flat-top layout).
        /// </summary>
        public static List<Vector3> CreateHexagonVecs(float radius = 1f)
        {
            var vecs = new List<Vector3>();

            float offset = Mathf.PI / 6f; // optional rotation (flat-top)

            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI * 2f / 6f + offset;

                vecs.Add(new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius
                ));
            }

            vecs.Add(Vector3.zero);
            return vecs;
        }
        public static List<List<Vector3>> CreateHexGridFromShape(
            int width,
            int height,
            float radius)
        {
            Debug.Log($"{nameof(CreateHexGridFromShape)} {width} {height} {radius}");
            
            var grid = new List<List<Vector3>>();

            var hexShape = CreateHexagonVecs(radius);

            float xStep = Mathf.Sqrt(3f) * radius;
            float zStep = 1.5f * radius;

            for (int q = 0; q < width; q++)
            {
                for (int r = 0; r < height; r++)
                {
                    float x = q * xStep + (r % 2 == 0 ? 0 : xStep * 0.5f);
                    float z = r * zStep;

                    Vector3 offset = new Vector3(x, 0f, z);

                    var hexInstance = new List<Vector3>();

                    foreach (var v in hexShape)
                        hexInstance.Add(v + offset);

                    grid.Add(hexInstance);
                }
            }

            return grid;
        }
        
        public static void ApplyHeightMap(
            List<List<Vector3>> grid,
            float scale,
            float frequency,
            float maxHeight)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                var hex = grid[i];

                for (int v = 0; v < hex.Count; v++)
                {
                    Vector3 p = hex[v];

                    float noise = Mathf.PerlinNoise(
                        (p.x + 1000f) * frequency,
                        (p.z + 1000f) * frequency
                    );

                    float height = noise * maxHeight;

                    p.y += height * scale;

                    hex[v] = p;
                }
            }
        }
        public static void SmoothHeight(List<List<Vector3>> grid, int iterations = 1)
        {
            for (int it = 0; it < iterations; it++)
            {
                var newGrid = new List<List<Vector3>>();

                foreach (var hex in grid)
                {
                    var newHex = new List<Vector3>();

                    for (int i = 0; i < hex.Count; i++)
                    {
                        Vector3 current = hex[i];

                        Vector3 sum = current;
                        int count = 1;

                        foreach (var otherHex in grid)
                        {
                            foreach (var v in otherHex)
                            {
                                float dist = Vector3.Distance(current, v);

                                if (dist > 0f && dist < 1.5f)
                                {
                                    sum += v;
                                    count++;
                                }
                            }
                        }

                        Vector3 avg = sum / count;

                        newHex.Add(new Vector3(
                            current.x,
                            avg.y,
                            current.z
                        ));
                    }

                    newGrid.Add(newHex);
                }

                grid = newGrid;
            }
        }
        public static List<UnityEngine.Mesh> BuildHexCellMeshes(List<List<Vector3>> grid)
        {
            var meshes = new List<UnityEngine.Mesh>();

            var rnd = new System.Random();

            foreach (var hex in grid)
            {
                // 1
                var verts = CreateHexCellVerts(hex);

                // 2
                var tris = CreateHexCellTris();

                // 3
                var colors = CreateHexCellColors(
                    verts.Count,
                    new Color(
                        (float)rnd.NextDouble(),
                        (float)rnd.NextDouble(),
                        (float)rnd.NextDouble()
                    )
                );

                // 4
                var mesh = AssembleHexCellMesh(
                    verts,
                    tris,
                    colors
                );

                meshes.Add(mesh);
            }

            return meshes;
        }
        public static List<Vector3> CreateHexCellVerts(List<Vector3> hex)
        {
            return new List<Vector3>(hex);
        }
        public static List<int> CreateHexCellTris()
        {
            var tris = new List<int>();

            int center = 6;

            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;

                tris.Add(center);
                tris.Add(i);
                tris.Add(next);
            }

            return tris;
        }
        public static List<Color> CreateHexCellColors(
            int count,
            Color color)
        {
            var colors = new List<Color>();

            for (int i = 0; i < count; i++)
                colors.Add(color);

            return colors;
        }
        
        public static UnityEngine.Mesh AssembleHexCellMesh(
            List<Vector3> verts,
            List<int> tris,
            List<Color> colors)
        {
            var mesh = new UnityEngine.Mesh();

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetColors(colors);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}