using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TestDelay : UIBase
{
    public override void Init()
    {
        Debug.Log("Initialize");
        onLoadingDelay.AddListener(() => Debug.Log("loadDelay"));
        onLoadingDelayDone.AddListener(() => Debug.Log("loadDelayDone"));
        LoadFuncAsyncs.Add(async () =>
        {
            await UniTask.Delay(4000);
            Debug.Log("Delay Done");
        });
    }
}
