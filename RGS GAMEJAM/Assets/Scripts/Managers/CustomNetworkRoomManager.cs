using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;
public class CustomNetworkRoomManager : NetworkRoomManager
{
    [SerializeField] private SO_PoolableDB database;
    private RoomSync roomSync;
    private RoomData tempRoomData;
    private Character tempCharacter;
    private int tempIndex;
    public override void OnStartClient()
    {
        base.OnStartClient();

        foreach (var entry in database.poolables)
        {
            var prefab = entry.prefab;

            NetworkClient.RegisterPrefab(
                prefab,
                (SpawnMessage msg) =>
                {
                    GameObject obj = GameManager.Instance.poolManager.Get(prefab);
                    obj.transform.SetPositionAndRotation(msg.position, msg.rotation);

                    var pooledObj = obj.GetComponent<BasePooledObject>();
                    pooledObj.originalPrefab = prefab;
                    pooledObj.OnSpawnFromPool();

                    return obj;
                },
                (GameObject obj) =>
                {
                    var pooledObj = obj.GetComponent<BasePooledObject>();
                    GameManager.Instance.poolManager.Return(pooledObj.originalPrefab ?? prefab, obj);
                }
            );
        }
    }

    public void SetTempRoomData(RoomData roomData) {
        tempRoomData = roomData;
    }
    public void SetRoomData(RoomData roomData)
    {
        maxConnections = roomData.maxPlayers;
        roomSync.SetRoomData(roomData);
    }
    public RoomData GetRoomData()
    {
        return roomSync.GetRoomData();
    }

    public void SetCharacter(Character character)
    {
        tempCharacter = character;
    }
    public Character GetCharacter()
    {
        return tempCharacter;
    }
    public void SetIndex(int name) {
        tempIndex = name;
    }
    public int GetIndex()
    {
        return tempIndex;
    }

    //개같은코드
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);

        if (sceneName == GameplayScene)
        {
            foreach (var player in roomSlots.ToList()) 
            {
                if (player != null)
                {
                    player.gameObject.SetActive(false);
                }
            }
        }

        else if (sceneName == RoomScene)
        {
            GameObject roomManagerObj = GameObject.Find("RoomManager_Network");
            if (roomManagerObj != null)
            {
                roomSync = roomManagerObj.GetComponent<RoomSync>();
                SetRoomData(tempRoomData);
            }

            foreach (var player in roomSlots.ToList())
            {
                if (player != null)
                {
                    player.gameObject.SetActive(true);
                }
            }
        }
    }
    public override void OnRoomClientSceneChanged()
    {
        base.OnRoomClientSceneChanged();

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "GameScene")
        {
            foreach (var player in roomSlots.ToList())
            {
                if (player != null)
                    player.gameObject.SetActive(false);
            }
        }
        else if (sceneName == "Lobby")
        {
            foreach (var player in roomSlots.ToList())
            {
                if (player != null)
                    player.gameObject.SetActive(true);
            }
        }
    }
    //개같은코드 끝
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        roomSync.AddRoomPlayer();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        roomSync.RemoveRoomPlayer();
    }
    public void StartGame()
    {
        if (allPlayersReady||roomSync.GetRoomData().maxPlayers==1)
        {
            ServerChangeScene(GameplayScene);
        }
    }
    
    public override void OnRoomServerPlayersReady()
    {
    }
}
