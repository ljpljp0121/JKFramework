using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UIFramework;
using UnityEngine;
using Utils;

public class UIDemoController : MonoBehaviour
{
    [SerializeField] private UISettings defaultUISettings = null;
    //[SerializeField] private FakePlayerData fakePlayerData = null;
    [SerializeField] private Camera cam = null;
    [SerializeField] private Transform transformToFollow = null;

    private UIFrame uiFrame;

    private void Awake()
    {
        uiFrame = defaultUISettings.CreateUIInstance();
        //Signals.Get<StartDemoSignal>().AddListener(OnStartDemo);
        //Signals.Get<NavigateToWindowSignal>().AddListener(OnNavigateToWindow);
        //Signals.Get<ShowConfirmationPopupSignal>().AddListener(OnShowConfirmationPopup);
    }

    private void OnDestroy()
    {
        //Signals.Get<StartDemoSignal>().RemoveListener(OnStartDemo);
        //Signals.Get<NavigateToWindowSignal>().RemoveListener(OnNavigateToWindow);
        //Signals.Get<ShowConfirmationPopupSignal>().RemoveListener(OnShowConfirmationPopup);
    }

    private void Start()
    {
        uiFrame.OpenWindow(ScreenIDs.StartGameWindow);
    }

    private void OnStartDemo()
    {
        uiFrame.ShowPanel(ScreenIDs.NavigationPanel);
        uiFrame.ShowPanel(ScreenIDs.ToastPanel);
    }

    private void OnNavigateToWindow(string windowID)
    {
        uiFrame.CloseCurrentWindow();

        //switch (windowID)
        //{
        //    case ScreenIDs.PlayerWindow:
        //        uiFrame.OpenWindow(windowID, new PlayerWindowProperties(fakePlayerData.LevelProgress));
        //        break;
        //    case ScreenIDs.CameraProjectionWindow:
        //        transformToFollow.parent.gameObject.SetActive(true);
        //        uiFrame.OpenWindow(windowID, new CameraProjectionWindowProperties(cam, transformToFollow));
        //        break;
        //    default:
        //        uiFrame.OpenWindow(windowID);
        //        break;
        //}


        uiFrame.OpenWindow(windowID);
    }

    //private void OnShowConfirmationPopup(ConfirmationPopupProperties popupPayload)
    //{
    //    uiFrame.OpenWindow(ScreenIDs.ConfirmationPopup, popupPayload);
    //}
}