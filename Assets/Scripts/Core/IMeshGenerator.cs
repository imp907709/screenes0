using UnityEngine;

namespace Core
{
    public interface IMeshGenerator
    {
        Mesh Generate(float size);
    }
}
