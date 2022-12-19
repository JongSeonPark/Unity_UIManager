using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ChickenGames
{
    public interface IResourceLoader
    {
        UniTask<Object> LoadAsync(string path);
        UniTask<Object> InstantiateAsync(string path, Transform parent = null, bool instantiateInWorldSpace = false);
        bool ReleaseInstance(GameObject instance);
    }
}