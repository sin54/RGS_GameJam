using UnityEngine;
using UnityEngine.UI;
using Mirror;
using NUnit.Framework.Constraints;
using System.Collections;
public class PlayerTower : NetworkBehaviour
{
    [SerializeField] private Image towerImg;
    [SyncVar] public bool isMovingTower = false;
    [SyncVar(hook = nameof(OnTowerChanged))] public TowerType nowTowerType;
    [SyncVar] public bool isCellEmpty = false;

    private Vector2Int selectedCell;
    private Vector2Int lastSentCell = new Vector2Int(int.MinValue, int.MinValue);

    [SerializeField] private GameObject playerFollowingPrefab;
    private GameObject towerInteractionTrigger;
    
    private void Start()
    {
        isTowerImgShowing(false);
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            if (isMovingTower)
            {
                selectedCell = GameManager.Instance.gridRenderer.HighlightCell(new Vector2(transform.position.x, transform.position.y - 0.35f));
                if (selectedCell != lastSentCell)
                {
                    lastSentCell = selectedCell;
                    CmdCheckCell(selectedCell);
                }
            }
            else
            {
                GameManager.Instance.gridRenderer.UnHighlightCell();
            }
        }
        else
        {
            if (isMovingTower&&isServer)
            {
                selectedCell= GameManager.Instance.gridRenderer.CalcHighlightCell(new Vector2(transform.position.x, transform.position.y - 0.35f));
            }
        }

    }
    public void SetTowerTrigger(GameObject trigger)
    {
        towerInteractionTrigger = trigger;
        towerInteractionTrigger.SetActive(false);
        towerInteractionTrigger.GetComponent<TowerPlaceInteraction>().SetPlayerTower(this);
    }
    private void isTowerImgShowing(bool isShowing)
    {
        Color color = towerImg.color;
        color.a = isShowing ? 1 : 0;
        towerImg.color = color;
    }
    public void AddTower(SO_BaseTower BT)
    {
        if (isMovingTower) return;
        if (!isEnoughResources(BT)) return;
        CmdUseResources(BT.upgradeStuffs[0].needLeaf, BT.upgradeStuffs[0].needStick, BT.upgradeStuffs[0].needStone);
        towerInteractionTrigger.SetActive(true);
        GameManager.Instance.gridRenderer.ToggleGrid(true);
        towerInteractionTrigger.GetComponent<TowerPlaceInteraction>().SetTowerData(BT);
        CmdAddTower(BT.towerType);
    }
    private bool isEnoughResources(SO_BaseTower BT)
    {
        ResourceManager RM = GameManager.Instance.resourceManager;
        return BT.upgradeStuffs[0].needLeaf <= RM.leaf &&
            BT.upgradeStuffs[0].needStick <= RM.stick &&
            BT.upgradeStuffs[0].needStone <= RM.stone;
    }
    public Vector2Int GetSelectedPos()
    {
        return selectedCell;
    }
    [Command]
    private void CmdUseResources(int needLeaf, int needStick, int needStone)
    {
        ResourceManager RM = GameManager.Instance.resourceManager;
        RM.ServerRemoveLeaf(needLeaf);
        RM.ServerRemoveStick(needStick);
        RM.ServerRemoveStone(needStone);
    }

    [Command]
    private void CmdAddTower(TowerType towerType) 
    {
        nowTowerType = towerType;
        isMovingTower = true;
    }
    public void RemoveTower()
    {
        if (!isMovingTower)
        {
            return;
        }
        towerInteractionTrigger.SetActive(false);
        GameManager.Instance.gridRenderer.ToggleGrid(false);
        GameManager.Instance.gridRenderer.UnHighlightCell();
        CmdRemoveTower();

    }
    [Command]
    private void CmdRemoveTower()
    {
        Vector2Int towerPos = selectedCell;
        GameManager.Instance.towerManager.PlaceTower(towerPos.x, towerPos.y, nowTowerType);
        isMovingTower = false;
        nowTowerType = TowerType.NONE;
    }


    private void OnTowerChanged(TowerType oldValue, TowerType newValue)
    {
        if (newValue!=TowerType.NONE)
        {
            towerImg.sprite = GameManager.Instance.towerManager.towerDataDict[newValue].towerIcon;
        }
        else
        {
            towerImg.sprite = null;
        }
        isTowerImgShowing(newValue!=TowerType.NONE);
    }
    [Command]
    private void CmdCheckCell(Vector2Int cell)
    {
        bool empty = GameManager.Instance.towerManager.CanPlace(cell.x + 24, cell.y + 24);
        isCellEmpty = empty;
    }

}
