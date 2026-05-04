using System.Collections.Generic;
using Meshes.Voronoi.VoronatorSharp;
using UnityEngine;

namespace Meshes.Voronoi.VoronatorUsage
{
    /// <summary>
    /// One call: random sites in a size×size square, clipped Voronoi. Does not touch meshes or scene objects.
    /// </summary>
    public static class VoronatorFromParams
    {
        /// <param name="cells">Site count (must be ≥ 3 for Delaunay).</param>
        /// <param name="size">Sites and clip rect are [0, size] on X and Z (Vector2.y = world Z).</param>
        /// <param name="seed">Deterministic site placement.</param>
        public static Voronator Build(int cells, float size, int seed)
        {
            if (cells < 3)
                throw new System.ArgumentException("cells must be at least 3.", nameof(cells));
            if (size <= 0f)
                throw new System.ArgumentException("size must be positive.", nameof(size));

            var sites = new List<Vector2>(cells);
            var old = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < cells; i++)
            {
                sites.Add(new Vector2(
                    Random.Range(0f, size),
                    Random.Range(0f, size)));
            }

            Random.state = old;

            var clipMin = Vector2.zero;
            var clipMax = new Vector2(size, size);
            return new Voronator(sites, clipMin, clipMax);
        }
    }
}
