using UnityEngine;
using UnityEngine.EventSystems;

public class AVGChoice : MonoBehaviour
{
    [Header("选项确认冷却")]
    [SerializeField] private float confirmCooldown = 0.2f;

    private float choiceOpenTime;
    private bool canConfirm;

    [Header("四个选项按钮")]
    public GameObject[] choiceButtons;

    [Header("AVG")]
    public DialogSystem dialogSystem;

    public void OpenChoice()
    {
        gameObject.SetActive(true);

        canConfirm = false;
        choiceOpenTime = Time.unscaledTime;

        // 默认选中第一个选项
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(choiceButtons[0]);
    }

    public void CloseChoice()
    {
        gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    // 四个按钮通过 Inspector 传入不同编号
    public void SelectChoice(int number)
    {
        if (!canConfirm)
            return;

        canConfirm = false;

        CloseChoice();
        dialogSystem.LoadChoice(number);
    }

    private void Update()
    {
        if (!canConfirm &&
            Time.unscaledTime - choiceOpenTime >= confirmCooldown)
        {
            canConfirm = true;
        }
    }
}