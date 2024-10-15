using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using SargeUniverse.Scripts;
using Server;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Basic class for loader
/// </summary>
public class Loader:MonoBehaviour
{
    private AsyncOperation mOperation;

    [SerializeField]
    private GameObject background;
	
    [SerializeField]
    private TextMeshProUGUI percentLabel;

    [SerializeField] private Image progressFill;
    
    public static Loader instance;
    private object _sceneName = null;

    private INetworkClient _networkClient;
    
    [Inject]
    private void Construct(INetworkClient networkClient)
    {
        _networkClient = networkClient;
    }
    
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    
    private void Update()
    {
        if(mOperation != null)
        {
            percentLabel.text = Mathf.Ceil((mOperation.progress * 100)) + " %";
            progressFill.fillAmount = mOperation.progress;
            if(mOperation.isDone)
            {
                background.SetActive(false);
                enabled = false;
            }
        }
    }

    public void loadScene(object name)
    {
        _sceneName = name;
        
        _networkClient.GetWsClient().OnConnectToServer += () => ConnectionResponse(true);
        _networkClient.GetWsClient().OnErrorMessage += (msg) => ConnectionResponse(false, msg);
        _networkClient.GetWsClient().ConnectToServer();
    }

    private void ConnectionResponse(bool successful, string message = "")
    {
        _networkClient.GetWsClient().OnConnectToServer -= () => ConnectionResponse(true);
        _networkClient.GetWsClient().OnErrorMessage -= (msg) => ConnectionResponse(false, msg);
        if (successful)
        {
            StartLoadScene(_sceneName);
        }
        else
        {
            /*MessageBox.Open(0, 
                0.8f, 
                false, 
                MessageResponded, 
                new string[] { "Failed to connect to server." }, 
                new string[] {"Connect"}
                );*/
        }
    }

    private void MessageResponded(int layoutIndex, int buttonIndex)
    {
        if (layoutIndex == 0)
        {
            loadScene(_sceneName);
        }
    }
    
    private void StartLoadScene(object name)
    {
        enabled = true;
        background.SetActive(true);
        mOperation = SceneManager.LoadSceneAsync(name.ToString());
    }
}