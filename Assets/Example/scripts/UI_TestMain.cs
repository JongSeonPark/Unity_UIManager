using ChickenGames;
using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenGames.Test
{
    public class UI_TestMain : MonoBehaviour
    {
        [SerializeField]
        Transform uiRoot;
        [SerializeField]
        Button mobileBackspaceButton;

        void Start()
        {
            UIManager.CreateInstance(new UnityResourceLoader(), uiRoot);
            StartAsync().Forget();
            mobileBackspaceButton?.onClick.AddListener(() => UIManager.Instance.CloseCurrentPage());
        }


        async UniTaskVoid StartAsync()
        {
            UIInstantiateRequest request = UIManager.Instance.OpenPage("TestLoadingPage", delay: 1000);
            await request;
            var popup = request.Result;
            popup.GetComponent<Transform>().localPosition = new Vector3(0, 0);

            UIManager.Instance.OnLastPageClose += () =>
            {
                Debug.Log("LastPage");
            };
        }
    }
}