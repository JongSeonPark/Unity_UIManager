using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMainPage : PageBase
{

    public override void Init()
    {
        base.Init();
        
        LoadFuncAsyncs.Add(async (p, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            await UniTask.Delay(500);
            Debug.Log("Done1");
        });
        LoadFuncAsyncs.Add(async (p, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            await UniTask.Delay(1000);
            Debug.Log("Done2");
        });
        LoadFuncAsyncs.Add(async (p, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            await UniTask.Delay(2500);
            Debug.Log("Done3");
        });
        LoadFuncAsyncs.Add(async (p, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            await UniTask.Delay(5300);
            Debug.Log("Done4");
        });
    }
}
