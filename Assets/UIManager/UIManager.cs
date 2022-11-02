using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ChickenGames.UI
{
    public static class UIManager
    {
        static RectTransform root;
        static Stack<PopupBase> popups;

        public static RectTransform Root { 
            get
            {
                if (root == null)
                {
                    var canvas = GameObject.Find("Canvas");
                    if (canvas == null)
                        Debug.Log("don't find Canvas");
                    else
                    {
                        root = canvas.GetComponent<RectTransform>();
                    }
                }
                return root;
            } 
            set => root = value; 
        }

        public static async UniTask InstantiateUI(string path, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // todo: IResourceController 등, 다른 외부 빌드 경우 변경해야 하므로,,,
            var res = await Resources.LoadAsync(path) as GameObject;
            var uiObject = UnityEngine.Object.Instantiate(res);
            var uiComp = uiObject.GetComponent<UIBase>();
            uiComp.Init();
            await uiComp.LoadingAsync(progress, cancellationToken);

            uiObject.GetComponent<RectTransform>().SetParent(Root);
        }
    }
}