using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace ChickenGames.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public List<Func<IProgress<float>, CancellationToken, UniTask>> LoadFuncAsyncs { get; protected set; } = new List<Func<IProgress<float>, CancellationToken, UniTask>>();

        public virtual void Init() { }

        public virtual async UniTask LoadingAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (LoadFuncAsyncs.Count == 0) return;


            float[] progressValues = new float[LoadFuncAsyncs.Count];

            int pIdx = 0;
            var tasks = LoadFuncAsyncs.Select(func =>
            {
                var p = new Progress<float>();

                int progressIdx = pIdx++;
                p.ProgressChanged += (v, v2) =>
                {
                    progressValues[progressIdx] = v2;
                    progress?.Report(progressValues.Sum() / progressValues.Length);
                };
                
                return UniTask.Create(async () =>
                {
                    await func.Invoke(p, cancellationToken);
                    (p as IProgress<float>).Report(1);
                });
            }).ToList();

            await UniTask.WhenAll(tasks);


            LoadFuncAsyncs.Clear();
        }

        public virtual void Close() 
        {
        }
    }
}