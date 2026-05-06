using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler,
    ISelectHandler,
    ISubmitHandler
{

    [Header("防止重复播放")]
    public bool playHoverOnSelect = true;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private bool CanPlay()
    {
        return button == null || button.interactable;
    }

    // 鼠标移上去
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanPlay()) return;

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
    }

    // 手柄 / 键盘选中按钮
    public void OnSelect(BaseEventData eventData)
    {
        if (!CanPlay()) return;
        if (!playHoverOnSelect) return;

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
    }

    // 鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanPlay()) return;

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Click);
    }

    // 手柄 / 键盘确认
    public void OnSubmit(BaseEventData eventData)
    {
        if (!CanPlay()) return;

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Click);
    }
}