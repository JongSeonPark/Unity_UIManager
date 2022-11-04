using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UI_TestDelay : PopupBase
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

            StartAsync().Forget();
        });
    }

    private void Start()
    {
    }

    async UniTask StartAsync()
    {
        UIInstantiateRequest request = UIManager.Instance.OpenPopup("UI_TestDelay");

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

        await request;
        var popup = request.Result;


        popup.GetComponent<Transform>().localPosition = transform.localPosition + new Vector3(10, 0);
    }
}
