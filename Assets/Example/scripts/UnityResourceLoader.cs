using Cysharp.Threading.Tasks;
using System.Security.Cryptography;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ChickenGames
{
    public class UnityResourceLoader : IResourceLoader
    {
        public async UniTask<Object> InstantiateAsync(string path, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var objRes = await Resources.LoadAsync(path);

            if (objRes == null)
            {
                Debug.LogError($"{path} is not exist..");
                return null;
            }

            return Object.Instantiate(objRes, parent, instantiateInWorldSpace);
        }

        public async UniTask<Object> LoadAsync(string path)
        {
            return await Resources.LoadAsync(path);
        }

        public bool ReleaseInstance(GameObject instance)
        {
            Object.Destroy(instance);
            return true;
        }
    }
}