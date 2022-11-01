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
        MillisecondsDelayTimer = 5000;
        onLoadingDelay.AddListener(() => Debug.Log("Delay"));
        onLoadingDelayDone.AddListener(() => Debug.Log("Delay Done"));
        LoadFuncAsyncs.Add(async () =>
        {
            await UniTask.Delay(4000);
            Debug.Log("Load Done");
        });
    }
}
