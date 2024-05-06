using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableMgr
{
    private static AddressableMgr instance = new AddressableMgr();
    public static AddressableMgr Instance => instance;

    public Dictionary<string, IEnumerator> resDic = new Dictionary<string, IEnumerator>();

    private AddressableMgr() { }

    //�첽������Դ
    private void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callBack)
    {
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<T>)resDic[keyName];

            if (handle.IsDone)
            {
                callBack(handle);
            }
            else
            {
                handle.Completed += (obj) =>
                {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        callBack(obj);
                    }
                    else
                    {
                        Debug.LogWarning(keyName + "��Դ����ʧ��");
                        resDic.Remove(keyName);
                    }
                };
            }

        }
        else
        {
            //û�м��ع� ֱ���첽���ز���¼
            handle = Addressables.LoadAssetAsync<T>(name);
            handle.Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    callBack(obj);
                }
                else
                {
                    Debug.LogWarning(keyName + "��Դ����ʧ��");
                    if (resDic.ContainsKey(keyName))
                        resDic.Remove(keyName);
                }
            };
            resDic.Add(keyName, handle);
        }
    }
    //�ͷ���Դ
    public void Release<T>(string name)
    {
        string keyName = name + "_" + typeof(T).Name;
        if (resDic.ContainsKey(keyName))
        {
            AsyncOperationHandle<T> handle = (AsyncOperationHandle<T>)resDic[keyName];
            Addressables.Release(handle);
            resDic.Remove(keyName);
        }
    }
    //�����Դ
    public void Clear()
    {
        resDic.Clear();
        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}
