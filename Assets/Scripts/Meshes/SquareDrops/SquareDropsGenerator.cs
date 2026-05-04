using System.Collections.Generic;
using UnityEngine;

namespace Meshes.SquareDrops
{
    public class SquareDropsGenerator
    {
        public List<Vector2> GenerateSites(int count, float size)
        {
            var sites = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                sites.Add(new Vector2(
                    Random.Range(0, size),
                    Random.Range(0, size)
                ));
            }

            return sites;
        }
        
        public SquareDropsMeshData Build(List<Vector2> sites)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            foreach (var site in sites)
            {
                int startIndex = vertices.Count;

                float s = 0.5f;

                vertices.Add(new Vector3(site.x - s, 0, site.y - s));
                vertices.Add(new Vector3(site.x + s, 0, site.y - s));
                vertices.Add(new Vector3(site.x + s, 0, site.y + s));
                vertices.Add(new Vector3(site.x - s, 0, site.y + s));

                triangles.Add(startIndex + 0);
                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 2);

                triangles.Add(startIndex + 2);
                triangles.Add(startIndex + 3);
                triangles.Add(startIndex + 0);
            }

            return new SquareDropsMeshData
            {
                Vertices = vertices.ToArray(),
                Triangles = triangles.ToArray()
            };
        }
    }
}