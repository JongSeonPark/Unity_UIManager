using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ChickenGames
{
    public class UnityResourceLoader : IResourceLoader
    {
        public async UniTask<Object> LoadAsync(string path)
        {
            return await Resources.LoadAsync(path);
        }
    }
}