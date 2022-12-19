using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ChickenGames.UI
{
    public sealed class UIManager
    {
        #region Singleton

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

        #endregion

        List<PopupBase> popups = new List<PopupBase>();
        Stack<PageBase> pages = new Stack<PageBase>();
        IResourceLoader resourceLoader;
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
                    resourceLoader.ReleaseInstance(obj);
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
                resourceLoader.ReleaseInstance(page.gameObject);
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
}