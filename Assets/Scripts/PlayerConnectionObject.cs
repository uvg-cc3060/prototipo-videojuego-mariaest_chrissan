using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer == false)
        {
            return;
        }

        // Solo corre en la compu que lo creo
        CmdSpawnPlayer();
    }

    public GameObject PlayerUnitPrefab;


    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer == false)
        {
            return;
        }
    }


    [Command]
    void CmdSpawnPlayer()
    {
        // Server Side
        GameObject go = Instantiate(PlayerUnitPrefab);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }


}