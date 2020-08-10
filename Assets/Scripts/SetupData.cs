using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupData : MonoBehaviour
{
    public static SetupData instance;

    public int maxNumPlayersJoin { get; set; }

    public string serverIP { get;  set; }
    public string username { get; set; }


    public void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
}
