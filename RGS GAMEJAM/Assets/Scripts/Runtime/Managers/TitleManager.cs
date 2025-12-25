using Mirror;
using Mirror.Discovery;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public void StartHost()
    {
        Loader.Load(Loader.Scene.HostConnect);
    }

    public void StartClient()
    {
        Loader.Load(Loader.Scene.ClientConnect);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
