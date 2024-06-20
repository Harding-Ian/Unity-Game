using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playersCountText;

    [SerializeField] private Button host;

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private NetworkRelay networkRelay;
    private UISceneManager uISceneManager;
    void Start()
    {
        networkRelay = NetworkManager.Singleton.GetComponent<NetworkRelay>();
        uISceneManager = GameObject.Find("UISceneManager").GetComponent<UISceneManager>();
    }

    public void ReadStringInput(string str)
    {
    Debug.Log("input --> " + str);
    networkRelay.JoinRelay(str);
    }

    public void BackButtonPressed()
    {
        uISceneManager.UnloadNetworkMenu();
    }
}
