using System.Collections.Generic;
using UnityEngine;

namespace Meshes.Voronoi
{
    public class VoronoiDataGenerator
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
    }
}