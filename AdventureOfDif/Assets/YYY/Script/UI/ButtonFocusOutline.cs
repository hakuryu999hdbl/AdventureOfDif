using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Outline))]
public class ButtonFocusOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    private Button button;
    private Outline outline;
    private bool pointerOver;
    private bool selected;

    private void Awake()
    {
        button = GetComponent<Button>();
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        selected = EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == gameObject;
        Refresh();
    }

    private void OnDisable()
    {
        pointerOver = false;
        outline.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
        Refresh();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        Refresh();
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
        Refresh();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        Refresh();
    }

    private void Refresh()
    {
        outline.enabled = isActiveAndEnabled && button.IsInteractable() && (pointerOver || selected);
    }
}
