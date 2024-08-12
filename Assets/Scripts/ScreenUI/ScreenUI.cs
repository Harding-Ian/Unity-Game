using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    public GameObject TabOverlay;
    public GameObject Crosshair;
    public GameObject ReloadUI;
    public GameObject ChargeUI;
    public GameObject GameManager;
    public GameObject ESCMenuUI;


    private bool inTabOverlay;
    private bool InTabOverlay
    {
        get { return inTabOverlay; }
        set 
        {
            if(inTabOverlay == value) return;
            inTabOverlay = value;

            if(value)
            {
                TabOverlay.SetActive(true);
                Crosshair.GetComponent<CanvasGroup>().alpha = 0f;
                ReloadUI.GetComponent<CanvasGroup>().alpha = 0f;
                ChargeUI.GetComponent<CanvasGroup>().alpha = 0f;
            }
            else
            {
                TabOverlay.SetActive(false);
                Crosshair.GetComponent<CanvasGroup>().alpha = 1f;
                ReloadUI.GetComponent<CanvasGroup>().alpha = 1f;
                ChargeUI.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Tab) && !ESCMenuUI.GetComponent<ESCMenuScript>().inESCMenu) InTabOverlay = true;
        else InTabOverlay = false;

    }
}
