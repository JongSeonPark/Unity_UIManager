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

    void Start()
    {
        loggerText.text = string.Empty;
        UIManager.InstantiateUI("UI_TestDelay").Forget();
    }

    void Update()
    {
        
    }
}
