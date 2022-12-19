using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace ChickenGames.UI
{
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

                
                var uiObject = await loader.InstantiateAsync(path) as GameObject;
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