using System.Collections.Generic;
using UnityEngine;

namespace Meshes.SquareDrops
{
    public class SquareDropsGenerator
    {
        /// <summary>Sites in XZ within the given rectangle (e.g. mesh bounds), reproducible with seed.</summary>
        public List<Vector2> GenerateSites(int count, Vector2 min, Vector2 max, int seed)
        {
            var sites = new List<Vector2>();
            var old = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < count; i++)
            {
                sites.Add(new Vector2(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y)
                ));
            }
            Random.state = old;
            return sites;
        }

        public SquareDropsMeshData Build(List<Vector2> sites, float yPlane, float halfExtent)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            foreach (var site in sites)
            {
                int startIndex = vertices.Count;

                float s = halfExtent;

                vertices.Add(new Vector3(site.x - s, yPlane, site.y - s));
                vertices.Add(new Vector3(site.x + s, yPlane, site.y - s));
                vertices.Add(new Vector3(site.x + s, yPlane, site.y + s));
                vertices.Add(new Vector3(site.x - s, yPlane, site.y + s));

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