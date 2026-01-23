using Unity.Netcode;
using UnityEngine;

public class AutoSetupHostTest : MonoBehaviour
{
    // This script is only to make the player a host to test 
    void Start()
    {
        NetworkManager.Singleton.StartHost();
    }


}
