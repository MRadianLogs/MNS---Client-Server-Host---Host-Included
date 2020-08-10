using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSideManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Start up server.
        NetworkManager.instance.StartServer(SetupData.instance.maxNumPlayersJoin);

        //Connect host player to server. (Possible to do this first?? Then open to other players?)
        Client.instance.ConnectToServer();
        
    }

}
