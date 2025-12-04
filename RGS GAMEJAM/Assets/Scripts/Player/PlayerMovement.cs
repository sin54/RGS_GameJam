using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnWalkChanged))]
    private bool isWalking;

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool flipX;

    [SyncVar(hook = nameof(OnHorizontalChanged))]
    private float horizontal;

    [SyncVar(hook =nameof(OnVerticalChanged))]
    private float vertical;

    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator Anim;
    private PlayerInputHandler inputs;
    private SpriteRenderer SR;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputs = GetComponent<PlayerInputHandler>();
        Anim=GetComponentInChildren<Animator>();
        SR=GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (PlayerInputLock.IsLocked) return;
        Vector2 move = inputs.movementInput.normalized;
        //내꺼이동
        bool localWalking = !(move.x == 0 && move.y == 0);
        Anim.SetBool("isWalk", localWalking);
        Anim.SetFloat("horizontal", move.x);
        Anim.SetFloat("vertical", move.y);

        CmdSendAnim(localWalking, SR.flipX, move.x, move.y);
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        Vector2 move = inputs.movementInput.normalized;

        if (move.x > 0.1f) SR.flipX = true;
        else if (move.x < -0.1f) SR.flipX = false;

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    [Command]
    private void CmdSendAnim(bool walking, bool flip, float horiz, float vert)
    {
        isWalking = walking;
        flipX = flip;
        horizontal = horiz;
        vertical = vert;
    }

    private void OnWalkChanged(bool oldValue, bool newValue)
    {
        if (isLocalPlayer) return; // 자기 화면은 이미 처리했으므로 skip
        Anim.SetBool("isWalk", newValue);
    }

    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        if (isLocalPlayer) return;
        SR.flipX = newValue;
    }
    private void OnHorizontalChanged(float oldValue, float newValue)
    {
        if (isLocalPlayer) return;
        Anim.SetFloat("horizontal", newValue);
    }
    private void OnVerticalChanged(float oldValue, float newValue)
    {
        if (isLocalPlayer) return;
        Anim.SetFloat("vertical", newValue);
    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        UnityEngine.InputSystem.PlayerInput playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        playerInput.enabled = true;
    }
}
