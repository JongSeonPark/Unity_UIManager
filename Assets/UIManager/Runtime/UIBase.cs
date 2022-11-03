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
        public List<Func<UniTask>> LoadFuncAsyncs { get; protected set; } = new List<Func<UniTask>>();

        public abstract void Init();

        public virtual async UniTask LoadingAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (LoadFuncAsyncs.Count == 0) return;

            var tasks = LoadFuncAsyncs.Select(func => func.Invoke());
            
            int doneCount = 0;
            while (await UniTask.WhenAny(tasks) == LoadFuncAsyncs.Count)
            {
                progress?.Report((float)++doneCount / LoadFuncAsyncs.Count);
            }

            LoadFuncAsyncs.Clear();
        }
    }
}