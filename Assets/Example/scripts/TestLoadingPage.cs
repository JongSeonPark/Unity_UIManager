using ChickenGames.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenGames.Test
{
    public class TestLoadingPage : PageBase
    {
        [SerializeField]
        Image progressImg;
        [SerializeField]
        Text progressText;

        public override void Init()
        {
            base.Init();
            Progress<float> progress = new Progress<float>();
            var req = UIManager.Instance.OpenPage("TestMainPage", progress: progress);

            progress.ProgressChanged += (v, v2) =>
            {
                progressImg.fillAmount = v2;
                progressText.text = $"{v2 * 100}%";
            };
        }
    }
}