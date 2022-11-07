using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSubPage : PageBase
{
    [SerializeField]
    Button backButton;

    [SerializeField]
    Button openLoopPopupButton;

    public override void Init()
    {
        base.Init();
        
        LoadFuncAsyncs.Add(async (p, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            await UniTask.Delay(500);
        });
        backButton.onClick.AddListener(() =>
        {
            UIManager.Instance.CloseCurrentPage();
        });

        openLoopPopupButton.onClick.AddListener(async() =>
        {
            UIInstantiateRequest request = UIManager.Instance.OpenPopup("TestLoopPopup", delay: 1000);
            await request;
            var popup = request.Result;
            popup.GetComponent<Transform>().localPosition = new Vector3(0, 0);
        });
    }
}
