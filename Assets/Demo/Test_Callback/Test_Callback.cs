﻿using UnityEngine;
using AssetBundleFramework;
using System.IO;

public class Test_Callback : MonoBehaviour
{
    private string PrefixPath { get; set; }
    private string Platform { get; set; }

    private void Start()
    {
        Platform = GetPlatform();
        PrefixPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../AssetBundle")).Replace("\\", "/");
        PrefixPath += $"/{Platform}";
        ResourceManager.instance.Initialize(GetPlatform(), GetFileUrl, false, 0);

        Initialize();
    }

    private void Initialize()
    {
        ResourceManager.instance.LoadWithCallback("Assets/AssetBundle/UI/UIRoot.prefab", true, uiRootResource =>
        {
            uiRootResource.Instantiate();

            Transform uiParent = GameObject.Find("Canvas").transform;

            ResourceManager.instance.LoadWithCallback("Assets/AssetBundle/UI/TestUI.prefab", true, testUIResource =>
            {
                testUIResource.Instantiate(uiParent, false);
            });
        });
    }

    private string GetPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            default:
                throw new System.Exception($"未支持的平台:{Application.platform}");
        }
    }

    private string GetFileUrl(string assetUrl)
    {
        return $"{PrefixPath}/{assetUrl}";
    }

    private void Update()
    {
        ResourceManager.instance.Update();
    }

    private void LateUpdate()
    {
        ResourceManager.instance.LateUpdate();
    }
}