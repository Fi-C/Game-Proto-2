using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public int nextNodeIndex;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea] public string text;
    public DialogueChoice[] choices;
    public int nextNodeIndex = -1;
    [Header("Item")]
    public ItemData correctItem;
    public int CorrectItemNode = -1;
    public int WrongItemNode = -1;
}

public class Dialogue : MonoBehaviour
{
    private bool player_detection = false;

    [SerializeField] private PlayerInput playerInput;
    private InputAction m_talkAction;
    private InputAction m_moveAction;
    private InputAction m_exitAction;
    private InputAction m_presentAction;
    private bool inputHeld = false;

    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;

    [Header("Dialogue")]
    [SerializeField] private DialogueNode[] nodes;
    [SerializeField] private Inventory inventory;

    private int currentNode = 0;
    private int currentChoice = 0;
    private bool isTalking =  false;
    private bool choosing = false;


    private void Awake()
    {
        m_talkAction = playerInput.actions.FindAction("Interact");
        m_moveAction = playerInput.actions.FindAction("Move");
        m_exitAction = playerInput.actions.FindAction("Exit");
        m_presentAction = playerInput.actions.FindAction("Present");
        dialogueBox.SetActive(false);
        HideChoices();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_detection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_detection = false;
            EndDialogue();
        }
    }




    private void Update()
    {
        if (!player_detection) return;

        if (!isTalking && m_talkAction.WasPressedThisFrame())
        {
            StartDialogue();
            return;
        }

        if (isTalking && m_exitAction.WasPressedThisFrame())
        {
            EndDialogue();
            return;
        }

        if (isTalking && m_presentAction.WasPressedThisFrame())
        {
            PresentItem();
        }

        if (!isTalking) return;

        Vector2 input = m_moveAction.ReadValue<Vector2>();

        if (choosing)
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

            if (m_talkAction.WasPressedThisFrame())
                SelectChoice();
        }
        else
                if (m_talkAction.WasPressedThisFrame())
            NextNode();
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

        if (node.choices != null && node.choices.Length > 0)
        {
            choosing = true;
            currentChoice = 0;
            ShowChoices(node);
        }
        else
        {
            choosing = false;
            HideChoices();
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

    void ShowChoices(DialogueNode node)
    {
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < node.choices.Length)
            {
                choiceTexts[i].gameObject.SetActive(true);
                choiceTexts[i].text = (i == currentChoice ? "> " : "") + node.choices[i].text;
            }
            else
            {
                choiceTexts[i].gameObject.SetActive(false);
            }
        }
    }

    void HideChoices()
    {
        foreach (var t in choiceTexts)
            t.gameObject.SetActive(false);
    }

    void ChangeChoice(int direction)
    {
        int count = nodes[currentNode].choices.Length;
        currentChoice = (currentChoice + direction + count) % count;
        ShowChoices(nodes[currentNode]);
    }

    void SelectChoice()
    {
        int next = nodes[currentNode].choices[currentChoice].nextNodeIndex;

        if (next < 0)
        {
            EndDialogue();
            return;
        }

        currentNode = next;
        ShowNode();
    }

    void PresentItem()
    {
        DialogueNode node = nodes[currentNode];

        if (inventory.selectedItem == node.correctItem)
        {
            currentNode = node.CorrectItemNode;
        }
        else
        {
            currentNode = node.WrongItemNode;
        }

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
