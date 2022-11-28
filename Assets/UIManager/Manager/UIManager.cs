using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace ChickenGames.UI
{
    public sealed class UIManager
    {
        public static UIManager Instance { get; private set; } = null;

        private UIManager() { }

        public static void CreateInstance(IResourceLoader resourceLoader, Transform uiRoot)
        {
            if (Instance != null)
            {
                Debug.LogError("Already Instance exist");
                return;
            }
            Instance = new UIManager();
            Instance.resourceLoader = resourceLoader;
            Instance.Root = uiRoot;
        }


        List<PopupBase> popups = new List<PopupBase>();
        Stack<PageBase> pages = new Stack<PageBase>();
        IResourceLoader resourceLoader = new UnityResourceLoader();
        Transform root;

        public RectTransform PageRoot { get; private set; }
        public RectTransform PopupRoot { get; private set; }
        public Transform Root
        {
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
            private set
            {
                root = value;
                var pagesObj = new GameObject("Pages");
                var popupsObj = new GameObject("Popups");
                PageRoot = pagesObj.AddComponent<RectTransform>();
                PopupRoot = popupsObj.AddComponent<RectTransform>();

                SetFullScreenRectTransform(PageRoot);
                SetFullScreenRectTransform(PopupRoot);

                PageRoot.SetParent(Root, false);
                PopupRoot.SetParent(Root, false);
            }
        }


        public UIInstantiateRequest InstantiateUI(string path, IProgress<float> progress = null, CancellationToken cancellationToken = default, int delay = 1000)
        {
            return new UIInstantiateRequest(resourceLoader, path, progress, cancellationToken, delay);
        }

        public UIInstantiateRequest OpenPopup(string path, IProgress<float> progress = null, CancellationToken cancellationToken = default, int delay = 1000)
        {
            var request = new UIInstantiateRequest(resourceLoader, path, progress, cancellationToken, delay, PopupRoot);
            request.Complete += (obj) =>
            {
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
                });

                popups.Add(popupComp);
            };
            return request;
        }

        public UIInstantiateRequest OpenPage(string path, IProgress<float> progress = null, CancellationToken cancellationToken = default, int delay = 1000)
        {
            var request = new UIInstantiateRequest(resourceLoader, path, progress, cancellationToken, delay, PageRoot);
            request.Complete += (obj) =>
            {
                var pageComp = obj.GetComponent<PageBase>();
                var trans = obj.GetComponent<Transform>();

                pages.Push(pageComp);
            };
            return request;
        }


        public Action OnLastPageClose { get; set; }
        public void CloseCurrentPage()
        {
            if (pages.Count > 1 && pages.TryPop(out var page))
            {
                page.Close();
            }
            else
            {
                OnLastPageClose?.Invoke();
            }
        }

        void SetFullScreenRectTransform(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.position = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(0, 0);
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
            UniTask.Create(async () =>
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

                if (res == null)
                {
                    throw new Exception($"{path} is not exist..");
                }

                var uiObject = Object.Instantiate(res);
                var uiComp = uiObject.GetComponent<UIBase>();
                uiComp.Init();


                cancellationToken.Register(() =>
                {
                    Object.Destroy(uiObject);
                });

                await uiComp.LoadingAsync(progress, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                IsDone = true;

                delayCts.Cancel();
                delayCts.Dispose();
                if (delayRun)
                    onLoadingDelayDone?.Invoke();

                onLoadingDelayDone?.RemoveAllListeners();


                if (parent != null)
                    uiObject.GetComponent<Transform>().SetParent(parent, false);

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
}