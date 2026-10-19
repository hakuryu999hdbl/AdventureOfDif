using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour
{
    [Header("ŒðŒÝ’ñŽ¦")]
    public GameObject signSprite;
    public Transform playerTrans;

    private PlayerInputControl playerInput;
    private Animator anim;
    private SpriteRenderer signRenderer;
    private IInteractable targetItem;
    private Collider2D targetCollider;
    private int deviceType;

    private void Awake()
    {
        playerInput = new PlayerInputControl();
        if (signSprite != null)
        {
            anim = signSprite.GetComponent<Animator>();
            signRenderer = signSprite.GetComponent<SpriteRenderer>();
        }
        deviceType = Keyboard.current != null ? 0 : 2;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Gameplay.Confirm.started += OnConfirm;
        InputSystem.onActionChange += OnActionChange;
        RefreshPrompt();
    }

    private void OnDisable()
    {
        playerInput.Gameplay.Confirm.started -= OnConfirm;
        InputSystem.onActionChange -= OnActionChange;
        playerInput.Disable();
        ClearInteraction();
    }

    private void Update()
    {
        if (targetItem != null && !IsValidTarget())
            ClearInteraction();

        if (signRenderer != null)
            signRenderer.enabled = targetItem != null;

        if (signSprite != null && playerTrans != null)
            signSprite.transform.localScale = playerTrans.localScale / 2f;
    }

    private bool IsValidTarget()
    {
        MonoBehaviour behaviour = targetItem as MonoBehaviour;
        return behaviour != null && behaviour.isActiveAndEnabled &&
               behaviour.CompareTag("Interactable") &&
               targetCollider != null && targetCollider.enabled &&
               targetCollider.gameObject.activeInHierarchy;
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (targetItem == null || !IsValidTarget()) return;
        IInteractable current = targetItem;
        current.TriggerAction();
        if (targetItem != null && !IsValidTarget())
            ClearInteraction();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable")) return;
        IInteractable item = other.GetComponent<IInteractable>();
        if (item == null) return;
        if (targetItem == null || !IsValidTarget() || targetCollider == other)
        {
            targetItem = item;
            targetCollider = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == targetCollider)
            ClearInteraction();
    }

    public void ClearInteraction()
    {
        targetItem = null;
        targetCollider = null;
        if (signRenderer != null) signRenderer.enabled = false;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionStarted) return;
        InputAction action = obj as InputAction;
        if (action == null || action.activeControl == null) return;
        var device = action.activeControl.device;
        int next = device is Keyboard ? 0 :
                   device is DualShockGamepad ? 1 :
                   device is XInputController || device is Gamepad ? 2 : -1;
        if (next < 0 || next == deviceType) return;
        deviceType = next;
        RefreshPrompt();
    }

    private void RefreshPrompt()
    {
        if (anim == null) return;
        anim.Play(deviceType == 0 ? "keyboard" :
                  deviceType == 1 ? "ps" : "xbox");
    }
}
