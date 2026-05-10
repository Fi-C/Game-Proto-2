using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public int nextNodeIndex;
}

public enum DialogueNodeType
{
    Normal,
    Choice,
    ItemChoice
}

[System.Serializable]
public class DialogueNode
{
    [TextArea]
    public string text;

    public DialogueNodeType nodeType;

    [Header("Normal")]
    public int nextNodeIndex = -1;

    [Header("Choices")]
    public DialogueChoice[] choices;

    [Header("Item Choice")]
    public ItemData requiredItem;
    public int successNode = -1;
    public int failNode = -1;
}

public class Dialogue : MonoBehaviour
{
    private bool playerDetection = false;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    private InputAction TalkAction;
    private InputAction MoveAction;
    private InputAction ExitAction;

    private bool inputHeld = false;

    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;

    [Header("Dialogue")]
    [SerializeField] private DialogueNode[] nodes;

    [Header("Inventory")]
    [SerializeField] private Inventory inventory;

    private int currentNode = 0;
    private int currentChoice = 0;

    private bool isTalking = false;
    private bool choosing = false;

    private void Awake()
    {
        TalkAction = playerInput.actions.FindAction("Interact");
        MoveAction = playerInput.actions.FindAction("Move");
        ExitAction = playerInput.actions.FindAction("Exit");

        dialogueBox.SetActive(false);
        HideChoices();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetection = false;
            EndDialogue();
        }
    }

    private void Update()
    {
        if (!playerDetection)
            return;

        if (!isTalking && TalkAction.WasPressedThisFrame())
        {
            StartDialogue();
            return;
        }

        if (isTalking && ExitAction.WasPressedThisFrame())
        {
            EndDialogue();
            return;
        }

        if (!isTalking)
            return;

        Vector2 input = MoveAction.ReadValue<Vector2>();

        if (choosing)
        {
            HandleChoiceInput(input);
        }
        else
        {
            if (TalkAction.WasPressedThisFrame())
            {
                NextNode();
            }
        }
    }

    void HandleChoiceInput(Vector2 input)
    {
        if (input.y > 0.5f && !inputHeld)
        {
            ChangeChoice(-1);
            inputHeld = true;
        }
        else if (input.y < -0.5f && !inputHeld)
        {
            ChangeChoice(1);
            inputHeld = true;
        }
        else if (Mathf.Abs(input.y) < 0.2f)
        {
            inputHeld = false;
        }

        if (TalkAction.WasPressedThisFrame())
        {
            SelectChoice();
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        currentNode = 0;

        dialogueBox.SetActive(true);

        PlayerMovement.dialogue = true;

        ShowNode();
    }

    void ShowNode()
    {
        DialogueNode node = nodes[currentNode];

        dialogueText.text = node.text;

        HideChoices();
        choosing = false;

        switch (node.nodeType)
        {
            case DialogueNodeType.Normal:
                break;

            case DialogueNodeType.Choice:
                dialogueText.text = node.text;
                choosing = true;
                currentChoice = 0;
                ShowChoices(node.choices);
                break;

            case DialogueNodeType.ItemChoice:
                bool hasItem = 
                    inventory.HasItem(node.requiredItem);

                currentNode =
                    (hasItem ? node.successNode : node.failNode);
                if (currentNode < 0)
                {
                    EndDialogue();
                    return;
                }
                ShowNode(); 
                break;
        }
    }

    void NextNode()
    {
        int next = nodes[currentNode].nextNodeIndex;

        if (next < 0)
        {
            EndDialogue();
            return;
        }

        currentNode = next;

        ShowNode();
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceTexts[i].gameObject.SetActive(true);

                choiceTexts[i].text =
                    (i == currentChoice ? "> " : "") +
                    choices[i].text;
            }
            else
            {
                choiceTexts[i].gameObject.SetActive(false);
            }
        }
    }

    void HideChoices()
    {
        foreach (var choice in choiceTexts)
        {
            choice.gameObject.SetActive(false);
        }
    }

    void ChangeChoice(int direction)
    {
        DialogueNode node = nodes[currentNode];

        int count = node.choices.Length;

            currentChoice = (currentChoice + direction + count) % count;
        ShowChoices(node.choices);
    }

    void SelectChoice()
    {
        DialogueNode node = nodes[currentNode];

        int next = node.choices[currentChoice].nextNodeIndex;

        if (next < 0)
        {
            EndDialogue();
            return;
        }

        currentNode = next;
        ShowNode();
    }


    void EndDialogue()
    {
        isTalking = false;
        choosing = false;

        dialogueBox.SetActive(false);

        PlayerMovement.dialogue = false;
    }
}