using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ChickenGames
{
    public interface IResourceLoader
    {
        UniTask<Object> LoadAsync(string path);
    }
}