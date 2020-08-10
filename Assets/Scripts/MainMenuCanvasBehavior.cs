
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvasBehavior : MonoBehaviour
{
    [SerializeField] private GameObject StartJoinGamePanel = null;

    [SerializeField] private GameObject StartGamePanel = null;
    [SerializeField] private Text numPlayersJoinInputText = null;
    [SerializeField] private Text hostUserNameInputText = null;
    [SerializeField] private int numPlayersAbleJoin = 0;

    [SerializeField] private GameObject JoinGamePanel = null;
    [SerializeField] private Text serverIPInputText = null;
    [SerializeField] private Text userNameInputText = null;
    [SerializeField] private string serverIP = "";

    public void OpenStartGameSetupMenu()
    {
        //Go to new menu where player inputs,
        //Num other players that can join. (0 = single player.)
        //Button to go to new scene for starting server, letting player in, and starting game.

        StartJoinGamePanel.SetActive(false);

        StartGamePanel.SetActive(true);
 
    }

    public void StartGameScene()
    {
        numPlayersAbleJoin = int.Parse(numPlayersJoinInputText.text);

        if (numPlayersAbleJoin >= 0)
        {
            SetupData.instance.maxNumPlayersJoin = numPlayersAbleJoin;
            SetupData.instance.username = hostUserNameInputText.text;
            //Go to network host scene.
            SceneManager.LoadScene("HostServerScene");
        }
    }

    public void OpenJoinGameSetupMenu()
    {
        //Go to menu where player inputs,
        //IP to join.
        //Button to go to new scene for joining server, letting player in, and starting game.

        StartJoinGamePanel.SetActive(false);
        
        JoinGamePanel.SetActive(true);
    }

    public void JoinGameScene()
    {
        serverIP = serverIPInputText.text;

        if (serverIP.Trim().Length > 0)
        {
            SetupData.instance.serverIP = serverIP;
            SetupData.instance.username = userNameInputText.text;
            //Go to client join scene.
            SceneManager.LoadScene("ClientJoinScene");
        }
    }

    public void GoBackToMainMenu()
    {
        StartGamePanel.SetActive(false);
        JoinGamePanel.SetActive(false);

        StartJoinGamePanel.SetActive(true);
    }
}
