using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using UnityEngine.Events;

namespace ChickenGames.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        List<Func<UniTask>> loadFuncAsyncs = new List<Func<UniTask>>();

        public List<Func<UniTask>> LoadFuncAsyncs => loadFuncAsyncs;

        int millisecondsDelayTimer = 3000;

        protected UnityEvent onLoadingDelay = new UnityEvent(), onLoadingDelayDone = new UnityEvent();

        public abstract void Init();

        public virtual async UniTask LoadingAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (loadFuncAsyncs.Count == 0) return;

            var tasks = loadFuncAsyncs.Select(func => func.Invoke());

            int doneCount = 0;
            while (await UniTask.WhenAny(tasks) == loadFuncAsyncs.Count)
            {
                progress?.Report((float)++doneCount / loadFuncAsyncs.Count);
            }

            loadFuncAsyncs.Clear();
        }
    }
}