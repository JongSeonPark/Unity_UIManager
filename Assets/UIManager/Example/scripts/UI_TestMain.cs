using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TestMain : MonoBehaviour
{
    [SerializeField]
    Text loggerText;

    [SerializeField]
    Transform uiRoot;

    private void Awake()
    {
    }

    void Start()
    {
        UIManager.CreateInstance(new ResourcesLoader(), uiRoot);

        loggerText.text = string.Empty;
        StartAsync().Forget();
    }


    async UniTaskVoid StartAsync()
    {
        UIInstantiateRequest request = UIManager.Instance.InstantiateUI("UI_TestDelay", delay: 1000);
        await request;
        var popup = request.Result;
        popup.GetComponent<Transform>().localPosition = new Vector3(0, 0);
    }

    void Update()
    {
        
    }
}
