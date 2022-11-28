using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenGames.Test
{
    public class TestMainPage : PageBase
    {
        [SerializeField]
        Button openSubPageButton;

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

            openSubPageButton?.onClick.AddListener(() =>
            {
                UIManager.Instance.OpenPage("TestSubPage");
            });
        }
    }
}