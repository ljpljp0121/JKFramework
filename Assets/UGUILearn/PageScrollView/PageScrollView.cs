using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageScrollView : MonoBehaviour
{
    #region ×Ö¶Î
    ScrollRect rect;
    private int pageCount;
    private float[] pages;

    public float moveTime = 0.3f;
    private float timer = 0;
    #endregion



    void Start()
    {
        rect = transform.GetComponent<ScrollRect>();
        pageCount = this.transform.GetChild(0).GetChild(0).childCount;
        pages = new float[pageCount];
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i] = i * (1 / (float)(pageCount - 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
