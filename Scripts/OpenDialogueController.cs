using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Main class we use to interact with dialogue, you can use it or make your own
/// </summary>
///
public class OpenDialogueController : MonoBehaviour
{
    [SerializeField]
    public static OpenDialogueController instance;

    // the source we use to play the audio
    public AudioSource source;

    //the main text we use to diaplay text
    public TextMeshProUGUI dialogueText;

    //The Textmesh Component used for the name
    public TextMeshProUGUI characterName;

    //A list of Button we use to contain Choices, here I use a prefab to allow for an unlimited number of choices, but
    //if you will only have a set amount of choices you don't need i
    public List<GameObject> choices;

    //the prefab I use for the choice button
    //if you will only have a set number of choices you don't need it
    public GameObject buttonPrefab;

    //the scroll view container that acts as a parent to the choice game objects
    //if you will only have a set number of choices you don't need it
    public GameObject choiceHolder;

    //The Gameobject that holds the single Choice Components
    public GameObject single;

    //The Gameobject that holds the multi Choice Components
    public GameObject multi;

    //the handler that holds the dialogue we need
    public DialogueHandeler DialogueHandeler;

    //the image we use to display the actor image
    public Image image;

    public bool animateText;

    public TMP_InputField field;

    //check for text animation
    private bool animatingText;

    //Used for storing the dialoguetext
    private string dText;

    #region dialogueController

    private delegate void ChoiceDelegate(int num);

    private ChoiceDelegate choiceDelegate;

    private bool multinode;

    public Actor[] actors;

    private void Awake()
    {
        single.SetActive(false);
        multi.SetActive(false);
        instance = this;
    }

    private void Start()
    {
        choiceDelegate = SetPlayerChoice;
    }

    private void OnEnable()
    {
        //Subscribing our methods to the dialogue system events
        DialogueSystem.Dialoguenext += NextNode;
        DialogueSystem.Dialogueend += End;
    }

    private void OnDisable()
    {
        //UnSubscribing our methods to the dialogue system events
        DialogueSystem.Dialoguenext -= NextNode;
        DialogueSystem.Dialogueend -= End;
    }

    private void End()
    {
        DialogueSystem.Save("Dialogue");
        //Ending the dialogue
        single.SetActive(false);
        multi.SetActive(false);
    }

    public void ContinueDialogue()
    {
        if (DialogueSystem.InDialogue && !multinode)
        {
            //check if the text is animating if we are skip the animation and load it completely
            if (!animatingText)
            {
                DialogueSystem.Next();
            }
            else
            {
                animatingText = false;
                StopAllCoroutines();
                dialogueText.maxVisibleCharacters = dText.Length;
            }
        }
    }

    public void StartDialogue()
    {
        nodeData Node = DialogueSystem.DStart(DialogueHandeler.DialogueData, DialogueHandeler.ORSNode);
        if (Node != null)
        {
            UpdatedialogueUi(Node);
        }
    }

    public void StartDialogue(DialogueValues dialogue)
    {
        nodeData Node = DialogueSystem.DStart(dialogue, dialogue.startIndex);
        if (Node != null)
        {
            UpdatedialogueUi(Node);
        }
    }

    public void StartDialogue(string dialogueName)
    {
        nodeData Node = DialogueSystem.DStart(dialogueName);
        if (Node != null)
        {
            UpdatedialogueUi(Node);
        }
    }

    public void UpdatedialogueUi(nodeData Node)
    {
        ClearChoices();
        switch (Node.type)
        {
            case TextType.SingleNode:
                multinode = false;
                multi.SetActive(false);
                single.SetActive(true);
                if (Node.dialogue.locked)
                {
                    nodeData NewNode = DialogueSystem.DNext();
                    UpdatedialogueUi(NewNode);
                }
                else
                {
                    dialogueText.text = string.Empty;
                    if (image != null && actors[Node.character.id].expression[Node.character.expression] != null)
                    {
                        image.sprite = actors[Node.character.id].expression[Node.character.expression];
                    }
                    characterName.text = actors[Node.character.id].name;
                    if (animateText)
                    {
                        dText = Node.dialogue.Text;
                        AnimateText(Node.dialogue.Text);
                    }
                    else
                    {
                        dialogueText.text = Node.dialogue.Text;
                    }
                }
                break;

            case TextType.MultiNode:
                multinode = true;
                single.SetActive(false);
                multi.SetActive(true);
                choiceHolder.GetComponent<RectTransform>().sizeDelta =
                new Vector2(0, Node.Choices.Count * 35 + (Node.Choices.Count - 1) * 8);
                for (int i = 0; i < Node.Choices.Count; i++)
                {
                    if (Node.Choices[i].locked)
                    {
                        continue;
                    }
                    GameObject Button = Instantiate(buttonPrefab, choiceHolder.transform);
                    choices.Add(Button);
                    int x = i;
                    Button.GetComponent<Button>().onClick.AddListener(delegate () { choiceDelegate(x); });
                    if (Node.Choices[i].Text[0] == '*' && Node.Choices[i].Text[1] == '*')
                    {
                        Button.GetComponent<Button>().interactable = false;
                        Node.Choices[i].Text = Node.Choices[i].Text.Remove(0, 2);
                    }
                    Button.GetComponentInChildren<TextMeshProUGUI>().text = Node.Choices[i].Text;
                }
                //if you don't wait for end of frame the button is pressed fully and actually activated i don't know why
                StartCoroutine(Selectbutton(choices[0].GetComponent<Button>()));
                break;

            case TextType.EmptyNode:

                if (Node.clip != null)
                {
                    source.PlayOneShot(Node.clip);
                }
                if (Node.Pause == false)
                {
                    DialogueSystem.Next();
                }
                break;

            case TextType.InputNode:
                multi.SetActive(false);
                single.SetActive(false);

                field.transform.parent.gameObject.SetActive(true);
                break;


        }
    }

    private void ClearChoices()
    {
        if (choices.Count != 0)
        {
            foreach (GameObject gameObject in choices)
            {
                gameObject.transform.SetParent(null);
                Destroy(gameObject);
            }
        }
        choices.Clear();
    }

    public void NextNode()
    {
        nodeData Node = DialogueSystem.DNext();
        if (Node != null)
        {
            UpdatedialogueUi(Node);
        }
    }

    public void SetPlayerChoice(int choice)
    {
        nodeData Node = DialogueSystem.DNext(choice);
        if (Node != null)
        { UpdatedialogueUi(Node); }
    }

    private void AnimateText(string text)
    {
        animatingText = true;
        StartCoroutine(Printext(text));
    }

    public void EnterValue()
    {
      bool success=DialogueSystem.InputValue(field.text);
        if (success)
        {
            field.transform.parent.gameObject.SetActive(false);
            DialogueSystem.Next();
        }
        else
        {
            Debug.Log("WrongInput");
        }
    }


    private IEnumerator Selectbutton(Button b)
    {
        yield return new WaitForEndOfFrame();
        b.Select();
    }

    private IEnumerator Printext(string text = null)
    {
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;
        while (dialogueText.maxVisibleCharacters < text.Length)
        {
            yield return new WaitForSecondsRealtime(0.025f);
            dialogueText.maxVisibleCharacters++;
        }
        animatingText = false;
    }

    #endregion dialogueController
}