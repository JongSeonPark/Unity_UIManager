using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
namespace ChickenGames.UI
{
    public abstract class PopupBase : UIBase
    {
        List<Func<UniTask>> initFuncAsyncs = new List<Func<UniTask>>();
    }
}