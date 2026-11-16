using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private const int QuestionCount = 3;

    private readonly bool[] answered = new bool[QuestionCount];
    private int firstQuestion;
    private Button[] buttons;
    private Text[] labels;
    private string[] defaultLabels;
    private RectTransform[] bubbles;
    private Vector2[] restingPositions;
    private Vector3[] restingScales;
    private float[] phases;
    private GameObject[] checkmarks;
    private CanvasGroup finishBubble;

    private void Awake()
    {
        int count = choiceButtons.Length;
        buttons = new Button[count];
        labels = new Text[count];
        defaultLabels = new string[count];
        bubbles = new RectTransform[count];
        restingPositions = new Vector2[count];
        restingScales = new Vector3[count];
        phases = new float[count];
        checkmarks = new GameObject[QuestionCount];

        for (int i = 0; i < count; i++)
        {
            buttons[i] = choiceButtons[i].GetComponent<Button>();
            labels[i] = choiceButtons[i].GetComponentInChildren<Text>(true);
            defaultLabels[i] = labels[i].text;
            bubbles[i] = (RectTransform)choiceButtons[i].transform;
            restingPositions[i] = bubbles[i].anchoredPosition;
            restingScales[i] = bubbles[i].localScale;
            phases[i] = i * 0.8f;
        }

        for (int i = 0; i < QuestionCount; i++)
            checkmarks[i] = CreateCheckmark(bubbles[i]);

        finishBubble = choiceButtons[QuestionCount].AddComponent<CanvasGroup>();
    }

    public void ResetChoices()
    {
        System.Array.Clear(answered, 0, answered.Length);
        if (gameObject.activeSelf)
            CloseChoice();
    }

    public void OpenChoice(int completedDialogue, int firstQuestion, string[] optionLabels = null)
    {
        if (gameObject.activeSelf)
            return;

        this.firstQuestion = firstQuestion;
        int completedQuestion = completedDialogue - firstQuestion;
        if (completedQuestion >= 0 && completedQuestion < QuestionCount)
            answered[completedQuestion] = true;

        gameObject.SetActive(true);
        for (int i = 0; i < labels.Length; i++)
            labels[i].text = optionLabels == null ? defaultLabels[i] : optionLabels[i];

        for (int i = 0; i < QuestionCount; i++)
            checkmarks[i].SetActive(answered[i]);

        bool finishUnlocked = answered[0] && answered[1] && answered[2];
        Button first = buttons[0];
        Button lastQuestion = buttons[QuestionCount - 1];
        Button finish = buttons[QuestionCount];
        finish.interactable = finishUnlocked;
        finishBubble.alpha = finishUnlocked ? 1f : 0.55f;

        // Explicit navigation can land on disabled buttons unless its links skip them.
        Navigation firstNavigation = first.navigation;
        firstNavigation.selectOnUp = finishUnlocked ? finish : lastQuestion;
        first.navigation = firstNavigation;
        Navigation lastNavigation = lastQuestion.navigation;
        lastNavigation.selectOnDown = finishUnlocked ? finish : first;
        lastQuestion.navigation = lastNavigation;

        canConfirm = false;
        choiceOpenTime = Time.unscaledTime;

        // 默认选中第一个选项
        EventSystem.current?.SetSelectedGameObject(choiceButtons[0]);
    }

    public void CloseChoice()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null &&
            System.Array.IndexOf(choiceButtons, eventSystem.currentSelectedGameObject) >= 0)
        {
            eventSystem.SetSelectedGameObject(null);
        }
        gameObject.SetActive(false);
    }

    // 四个按钮通过 Inspector 传入各自的位置（0–3）。
    public void SelectChoice(int choiceIndex)
    {
        if (!isActiveAndEnabled || !canConfirm || choiceIndex < 0 ||
            choiceIndex >= buttons.Length || !buttons[choiceIndex].interactable)
            return;

        canConfirm = false;

        CloseChoice();
        dialogSystem.LoadChoice(firstQuestion + choiceIndex);
    }

    private void Update()
    {
        if (!canConfirm &&
            Time.unscaledTime - choiceOpenTime >= confirmCooldown)
        {
            canConfirm = true;
        }

        GameObject selected = EventSystem.current?.currentSelectedGameObject;
        for (int i = 0; i < bubbles.Length; i++)
        {
            float speed = choiceButtons[i] == selected ? 2.7f : 1.8f;
            phases[i] = Mathf.Repeat(phases[i] + speed * Time.unscaledDeltaTime, 2f * Mathf.PI);
            float wave = Mathf.Sin(phases[i]);
            bubbles[i].anchoredPosition = restingPositions[i] + Vector2.up * (4f * wave);
            bubbles[i].localScale = restingScales[i] * (0.985f + 0.015f * wave);
        }
    }

    private static GameObject CreateCheckmark(RectTransform bubble)
    {
        // Build the mark from UI strokes so it does not depend on font coverage.
        var mark = new GameObject("Answered Checkmark", typeof(RectTransform));
        RectTransform rect = (RectTransform)mark.transform;
        rect.SetParent(bubble, false);
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
        rect.anchoredPosition = new Vector2(-20f, 0f);
        rect.sizeDelta = new Vector2(20f, 20f);

        AddCheckmarkStroke(rect, new Vector2(-4f, -2f), 8f, -45f);
        AddCheckmarkStroke(rect, new Vector2(3f, 1f), 14f, 45f);
        return mark;
    }

    private static void AddCheckmarkStroke(RectTransform mark, Vector2 position, float width, float angle)
    {
        var stroke = new GameObject("Stroke", typeof(Image));
        Image image = stroke.GetComponent<Image>();
        image.raycastTarget = false;
        image.color = new Color32(150, 255, 180, 255);
        RectTransform rect = image.rectTransform;
        rect.SetParent(mark, false);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, 3f);
        rect.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
