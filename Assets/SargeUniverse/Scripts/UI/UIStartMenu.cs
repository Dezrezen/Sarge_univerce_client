using SargeUniverse.Scripts.Model.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SargeUniverse.Scripts.UI
{
    public class UIStartMenu : MonoBehaviour
    {
        [SerializeField] private Button _playButton = null;
        
        [Header("Message Box")]
        [SerializeField] private UIMessageBox _messageBox = null;

        [Header("Load Bar")]
        [SerializeField] private GameObject _loadBar = null;
        [SerializeField] private Image _loadBarFill = null;
        [SerializeField] private TMP_Text _loadBarText = null;
        
        private AsyncOperation _operation;
        private INetworkClient _networkClient;
    
        [Inject]
        private void Construct(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }

        private void Start()
        {
            _playButton.onClick.AddListener(LoadScene);
            
            _networkClient.GetWsClient().OnConnectToServer += ConnectionOkResponse;
            _networkClient.GetWsClient().OnErrorMessage += ConnectionErrorResponse;
            
            _loadBar.SetActive(false);
            _loadBarFill.fillAmount = 0;
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(LoadScene);
            
            _networkClient.GetWsClient().OnConnectToServer -= ConnectionOkResponse;
            _networkClient.GetWsClient().OnErrorMessage -= ConnectionErrorResponse;
        }

        private void Update()
        {
            if (_operation == null)
            {
                return;
            }
            
            _loadBarText.text = Mathf.Ceil((_operation.progress * 100)) + " %";
            _loadBarFill.fillAmount = _operation.progress;
            
            if (_operation.isDone)
            {
                _loadBar.SetActive(false);
            }
        }
        
        private void LoadScene()
        {
            _playButton.interactable = false;
            _networkClient.GetWsClient().ConnectToServer();
        }

        private void ConnectionOkResponse()
        {
            ConnectionResponse(true);
        }

        private void ConnectionErrorResponse(string message = "")
        {
            ConnectionResponse(false, message);
        }

        private void ConnectionResponse(bool successful, string message = "")
        {
            if (successful)
            {
                StartLoadScene();
            }
            else
            {
                _messageBox.Show(
                    "Failed to connect to server", 
                    new MessageBoxButtonData("Connect", MessageResponded)
                );
            }
        }
        
        private void MessageResponded()
        {
            _playButton.interactable = true;
        }
    
        private void StartLoadScene()
        {
            _playButton.gameObject.SetActive(false);
            _loadBar.SetActive(true);
            
            _operation = SceneManager.LoadSceneAsync(Constants.GameScene);
        }
    }
}