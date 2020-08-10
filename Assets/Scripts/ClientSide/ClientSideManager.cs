using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSideManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Set IP from setup.
        Client.instance.ClientJoinSetupIP();
        //Attempt to join server.
        Client.instance.ConnectToServer();
    }

}
