using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace ChickenGames.UI
{
    public class UIManager
    {
        public static UIManager Instance { get; private set; } = null;

        public static void CreateInstance(IResourceLoader resourceLoader, Transform uiRoot)
        {
            if (Instance != null)
            {
                Debug.LogError("Already Instance exist");
                return;
            }
            Instance = new UIManager();
            Instance.resourceLoader = resourceLoader;
            Instance.root = uiRoot;
        }
        

        List<PopupBase> popups = new List<PopupBase>();
        IResourceLoader resourceLoader = new ResourcesLoader();
        Transform root;
        public Transform Root { 
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

        
        public UIInstantiateRequest InstantiateUI(string path, IProgress<float> progress = null, CancellationToken cancellationToken = default, int delay = 1000)
        {
            return new UIInstantiateRequest(resourceLoader, path, progress, cancellationToken, delay, root);
        }        

        public UIInstantiateRequest OpenPopup(string path, IProgress<float> progress = null, CancellationToken cancellationToken = default, int delay = 1000)
        {
            var req = new UIInstantiateRequest(resourceLoader, path, progress, cancellationToken, delay, root);
            req.Complete += (obj) => {
                var popupComp = obj.GetComponent<PopupBase>();
                var trans = obj.GetComponent<Transform>();
                popupComp.OnClick.AddListener(_ =>
                {
                    Debug.Log("OnClick popup");
                    popups.Remove(popupComp);
                    popups.Add(popupComp);
                    trans.SetAsLastSibling();
                });

                popupComp.OnClose.AddListener(() =>
                {
                    popups.Remove(popupComp);
                    Object.Destroy(obj);
                });

                popups.Add(popupComp);
            };
            return req;
        }
    }

    public class UIInstantiateRequest : CustomYieldInstruction
    {
        public UnityEvent onLoadingDelay = new UnityEvent(), onLoadingDelayDone = new UnityEvent();
        public Action<GameObject> Complete { get; set; }

        public bool IsDone { get; protected set; } = false;
        public override bool keepWaiting => !IsDone;

        public int MillisecondsDelayTimer { get; protected set; } = 1000;

        public GameObject Result { get; protected set; }



        public UIInstantiateRequest(IResourceLoader loader, string path, IProgress<float> progress, CancellationToken cancellationToken, int millisecondsDelayTimer = 3000, Transform parent = null)
        {
            MillisecondsDelayTimer = millisecondsDelayTimer;

            UniTask.Void(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                CancellationTokenSource delayCts = new CancellationTokenSource();
                bool delayRun = false;
                UniTask.Void(async () =>
                {
                    await UniTask.Delay(MillisecondsDelayTimer);
                    if (!delayCts.IsCancellationRequested)
                    {
                        onLoadingDelay?.Invoke();
                        delayRun = true;
                    }
                    onLoadingDelay?.RemoveAllListeners();
                });

                var res = await loader.LoadAsync(path) as GameObject;

                var uiObject = Object.Instantiate(res);
                var uiComp = uiObject.GetComponent<UIBase>();
                uiComp.Init();
                await uiComp.LoadingAsync(progress, cancellationToken);

                IsDone = true;

                delayCts.Cancel();
                if (delayRun)
                    onLoadingDelayDone?.Invoke();

                onLoadingDelayDone?.RemoveAllListeners();


                if (parent !=  null)
                    uiObject.GetComponent<Transform>().SetParent(parent);

                Result = uiObject;

                Complete?.Invoke(Result);
            });
        }


        Object uiObject;

        protected virtual Object GetResult()
        {
            return uiObject;
        }

    }

    public interface IResourceLoader
    {
        UniTask<Object> LoadAsync(string path);
    }

    public class ResourcesLoader : IResourceLoader
    {
        public async UniTask<Object> LoadAsync(string path)
        {
            return await Resources.LoadAsync(path);
        }
    }
}