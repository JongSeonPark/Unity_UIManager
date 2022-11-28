using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace ChickenGames.Test
{
    public class TestLoopPopup : PopupBase
    {
        [SerializeField]
        RectTransform panelTransform;

        GameObject loadingObject;
        CancellationTokenSource loadingCTS;

        public override void Init()
        {
            Debug.Log("Initialize");

            LoadFuncAsyncs.Add(async (p, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                await UniTask.Delay(3000);
                Debug.Log("Load Done");

                StartAsync(ct).Forget();
            });
        }

        CancellationTokenSource openUICTS;

        async UniTask StartAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            openUICTS = new CancellationTokenSource();
            UIInstantiateRequest request = UIManager.Instance.OpenPopup("TestLoopPopup", cancellationToken: openUICTS.Token);

            request.onLoadingDelay.AddListener(() =>
            {
                loadingCTS = new CancellationTokenSource();
                UniTask.Void(async () =>
                {
                    var obj = await Resources.LoadAsync<GameObject>("LoadingUI");
                    loadingCTS.Token.ThrowIfCancellationRequested();
                    loadingObject = Instantiate(obj as GameObject);
                    loadingObject.GetComponent<Transform>().SetParent(panelTransform, false);
                });
            });

            request.onLoadingDelayDone.AddListener(() =>
            {
                loadingCTS.Cancel();
                loadingCTS = null;
                Destroy(loadingObject);
                loadingObject = null;
            });

            request.Complete += (obj) =>
            {
                obj.GetComponent<Transform>().localPosition = transform.localPosition + new Vector3(10, 0);
            };
            await request;

            openUICTS?.Dispose();
            openUICTS = null;
        }

        public override void Close()
        {
            base.Close();
            openUICTS?.Cancel();
            openUICTS?.Dispose();
        }
    }
}