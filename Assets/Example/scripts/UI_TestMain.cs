using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TestMain : MonoBehaviour
{
    [SerializeField]
    Transform uiRoot;
    [SerializeField]
    Button mobileBackspaceButton;

    private void Awake()
    {
    }

    void Start()
    {
        UIManager.CreateInstance(new ResourcesLoader(), uiRoot);
        StartAsync().Forget();
        mobileBackspaceButton?.onClick.AddListener(() => UIManager.Instance.CloseCurrentPage());
    }


    async UniTaskVoid StartAsync()
    {
        //UIInstantiateRequest request = UIManager.Instance.OpenPopup("UI_TestDelay", delay: 1000);
        //await request;
        //var popup = request.Result;
        //popup.GetComponent<Transform>().localPosition = new Vector3(0, 0);

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
