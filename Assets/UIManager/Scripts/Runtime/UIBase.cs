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
    public abstract class UIBase : MonoBehaviour
    {
        List<Func<UniTask>> initFuncAsyncs = new List<Func<UniTask>>();

        public IReadOnlyList<Func<UniTask>> InitFuncAsyncs => initFuncAsyncs;

        int millisecondsDelayTimer = 3f;

        public virtual async UniTask InitializeAsync(Action onDelay = null, Action onDelayDone = null, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tasks = initFuncAsyncs.Select(func => func.Invoke());
            
            int doneCount = 0;
            while (await UniTask.WhenAny(tasks) == initFuncAsyncs.Count)
            {
                progress?.Report((float)++doneCount / initFuncAsyncs.Count);
            }

            initFuncAsyncs.Clear();
        }
    }
}