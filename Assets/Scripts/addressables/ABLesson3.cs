using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ABLesson3 : MonoBehaviour
{
    public AssetReference assetReference;
    public AssetReferenceGameObject gameObject;
    void Start()
    {
        //AsyncOperationHandle<GameObject> handle = assetReference.LoadAssetAsync<GameObject>();

        //handle.Completed += TestFun;

        assetReference.LoadAssetAsync<GameObject>().Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                Instantiate(handle.Result);
            //使用标识类
            if (assetReference.IsDone)
                Instantiate(assetReference.Asset);
        };

        
    }

    void Update()
    {

    }
}
