using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TestMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.InstantiateUI("UI_TestDelay").Forget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
