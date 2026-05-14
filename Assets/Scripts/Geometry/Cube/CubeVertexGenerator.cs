using System.Collections.Generic;
using UnityEngine;

namespace Geometry.Cube
{
    // cube vertices generator
    public class CubeVertexGenerator 
    {
        public static List<Vector3> Generate(float size)
        {
            float h = size * 0.5f;

            return new ()   
            {
                new Vector3(-h,-h,-h),
                new Vector3(h,-h,-h),
                new Vector3(h,h,-h),
                new Vector3(-h,h,-h),

                new Vector3(-h,-h,h),
                new Vector3(h,-h,h),
                new Vector3(h,h,h),
                new Vector3(-h,h,h),
            };
        }
    }
    
    // cube triangles generator
    public class CubeTriangleGenerator 
    {
        public static List<int> Generate()
        {
            return new List<int>()
            {   
                0,2,1, 0,3,2,
                1,2,6, 6,5,1,
                5,6,7, 7,4,5,
                4,7,3, 3,0,4,
                3,7,6, 6,2,3,
                4,0,1, 1,5,4
            };
        }
    }
    
    
}