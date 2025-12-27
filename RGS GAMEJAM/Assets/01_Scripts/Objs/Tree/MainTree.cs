using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using TMPro;
public class MainTree : NetworkBehaviour
{
    public SO_Tree treeData;
    [SerializeField] private GameObject treeReadyParticle;
    [SerializeField] private Image treeHpImg;
    [SerializeField] private TMP_Text treeHPTxt;

    [SyncVar(hook =nameof(OnTreeLevelChanged))] public int TreeLevel;
    [SyncVar] public int NowXp;
    [SyncVar(hook =nameof(OnTreeHPChanged))] public int NowHealth;

    [SyncVar] private double lastInteractTime = 0;
    [SyncVar] private float interactCooldown;

    private Coroutine leafRoutine;
    private Color originalColor = Color.white;
    private bool isFlashing = false;
    private void Awake()
    {

    }

    private void Start()
    {

    }
    public double GetLastInteractionTime()
    {
        return lastInteractTime;
    }
    public float GetInteractionCoolDown()
    {
        return interactCooldown;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        TreeLevel = 0;
        NowHealth = treeData.maxHps[TreeLevel];
        NowXp = 0;
        interactCooldown = treeData.stickProduceCool[TreeLevel];

        leafRoutine = StartCoroutine(leafCoroutine());
    }
    private void Update()
    {
        if (NetworkTime.time > interactCooldown + lastInteractTime)
        {
            treeReadyParticle.SetActive(true);
        }
        else
        {
            treeReadyParticle.SetActive(false);
        }
    }

    [Server]
    public void ServerTryGetStick(CustomNetworkGamePlayer player)
    {
        if (NetworkTime.time < lastInteractTime + interactCooldown)
            return;

        lastInteractTime = NetworkTime.time;

        player.AddLocalStick(treeData.stickAmount[TreeLevel]);
    }
    [Server]
    public void AddXp(int amount)
    {
        NowXp += amount;
        if (NowXp >= treeData.needXps[TreeLevel])
        {
            TreeLevelUp();
        }
    }
    [Server]
    private IEnumerator leafCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(treeData.leafProduceCool[TreeLevel]);
            GameManager.Instance.resourceManager.ServerAddLeaf(treeData.leafAmount[TreeLevel]);
        }
    }
    private void TreeLevelUp()
    {
        NowXp -= treeData.needXps[TreeLevel];
        TreeLevel++;
        interactCooldown = treeData.stickProduceCool[TreeLevel];
    }

    [Server]
    public void OnTreeAttacked(int damage)
    {
        NowHealth -= damage;
    }
    private void OnTreeLevelChanged(int oldValue, int newValue)
    {
        NowHealth = treeData.maxHps[newValue];
        UpdateHpUI();
    }

    private void OnTreeHPChanged(int oldValue, int newValue)
    {
        UpdateHpUI();
        if (oldValue > newValue)
            StartCoroutine(FlashRed());
    }

    private void UpdateHpUI()
    {
        treeHpImg.fillAmount = (float)NowHealth / treeData.maxHps[TreeLevel];
        treeHPTxt.text = $"{NowHealth}/{treeData.maxHps[TreeLevel]}";
    }
    private IEnumerator FlashRed()
    {
        if (isFlashing) yield break;
        isFlashing = true;
        originalColor = treeHpImg.color;
        treeHpImg.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        treeHpImg.color = originalColor;

        isFlashing = false;
    }

}
