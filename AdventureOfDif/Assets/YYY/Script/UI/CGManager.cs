using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CGManager : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GridLayoutGroup gallery;
    [SerializeField] private Button exitButton;
    [SerializeField] private Outline exitFocus;

    private readonly List<Button> cgButtons = new List<Button>();
    private readonly List<GameObject> selectionFrames = new List<GameObject>();
    private InputAction cancelAction;
    private GameObject lastSelection;

    private void OnEnable()
    {
        cancelAction = eventSystem.GetComponent<InputSystemUIInputModule>().cancel.action;
        cancelAction.performed += OnCancel;
    }

    private void OnDisable()
    {
        cancelAction.performed -= OnCancel;
    }

    private void Start()
    {
        foreach (Transform card in gallery.transform)
        {
            if (card.TryGetComponent(out Button button))
            {
                cgButtons.Add(button);
                selectionFrames.Add(card.Find("Selection Frame")?.gameObject);
            }
        }

        ConfigureNavigation();
        eventSystem.SetSelectedGameObject(exitButton.gameObject);
        RefreshSelection(eventSystem.currentSelectedGameObject);
    }

    private void LateUpdate()
    {
        GameObject selection = eventSystem.currentSelectedGameObject;
        if (selection != lastSelection)
        {
            RefreshSelection(selection);
        }
    }

    private void ConfigureNavigation()
    {
        int columns = gallery.constraintCount;
        Button topRight = GetButton(Mathf.Min(columns, cgButtons.Count) - 1);
        exitButton.navigation = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = topRight,
            selectOnLeft = topRight
        };

        for (int index = 0; index < cgButtons.Count; index++)
        {
            int column = index % columns;
            int nextRowStart = index - column + columns;
            Button below = nextRowStart < cgButtons.Count
                ? GetButton(Mathf.Min(index + columns, cgButtons.Count - 1))
                : null;
            cgButtons[index].navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = index < columns ? exitButton : GetButton(index - columns),
                selectOnDown = below,
                selectOnLeft = column > 0 ? GetButton(index - 1) : null,
                selectOnRight = cgButtons[index] == topRight
                    ? exitButton
                    : column < columns - 1 ? GetButton(index + 1) : null
            };
        }
    }

    private Button GetButton(int index)
    {
        return index >= 0 && index < cgButtons.Count ? cgButtons[index] : null;
    }

    private void RefreshSelection(GameObject selection)
    {
        lastSelection = selection;
        exitFocus.enabled = selection == exitButton.gameObject;
        for (int index = 0; index < cgButtons.Count; index++)
        {
            GameObject frame = selectionFrames[index];
            if (frame != null)
            {
                frame.SetActive(selection == cgButtons[index].gameObject);
            }
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        var audio = AudioManager.Instance;
        audio.PlayFX(audio.UI_Click);
        ExitToMenu();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
