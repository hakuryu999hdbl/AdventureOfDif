using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoSelectInvoke : MonoBehaviour, ISelectHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        button.onClick.Invoke();
    }
}