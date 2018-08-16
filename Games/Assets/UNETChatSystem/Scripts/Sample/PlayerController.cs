﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private ChatSystem chatSystem;
    private ChatIdentifier chatIdentifier;
    private TeamIdentifier teamIdentifier;

    private InputField nameSetter;

    //you can play with this in the editor to see the different channels by bringing up "team chat" by pressing "U"
    [SerializeField]
    public uint teamIndex;

    //if you are using canvas's per player - you will have to have these in the prefab & assign them from the canvas
    [SerializeField]
    private InputField chatPanelInputField;
    [SerializeField]
    private ContentSizeFitter chatPanelContentPanel;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Canvas canvas;

    private const uint SPECTATOR_INDEX = 1; // the channel index for spectators. perhaps you want to disallow spectators from all chatting if they can see everyone.
    private const uint COUNTER_TERRORIST_INDEX = 2; // the channel index for counter-terrorists
    private const uint TERRORIST_INDEX = 3; // the channel index for terrorists

    void Start()
    {
        if(GameObject.Find("Canvas") != null)
        {
            this.canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            this.canvasGroup = GameObject.Find("Chat Panel").GetComponent<CanvasGroup>();
            this.chatPanelContentPanel = GameObject.Find("Content").GetComponent<ContentSizeFitter>();
            this.chatPanelInputField = GameObject.Find("InputField").GetComponent<InputField>();    
        }


        if (!isLocalPlayer)
        {
            this.enabled = false;
            //if(canvas != null)
            //{
            //    canvas.gameObject.SetActive(false);
            //}
            return;
        }
        chatSystem = GameObject.FindObjectOfType<ChatSystem>();

        chatIdentifier = GameObject.FindObjectOfType<ChatIdentifier>();
        teamIdentifier = GameObject.FindObjectOfType<TeamIdentifier>();
        if(GameObject.Find("Player Name Setter") != null)
        {
            nameSetter = GameObject.Find("Player Name Setter").GetComponent<InputField>();
            nameSetter.onEndEdit.AddListener(value =>
            {
                name = value;
            });    
        }

        if(chatSystem != null)
        {
            canvasGroup = chatSystem.canvasGroup;    
        }

    }

    void Update()
    {
        // we don't want to do extra stuff like refocus chat if we are typing a message. But it's weird if they have to close chat and reopen it 
        // if they sent a message already and it hasn't re-hidden yet.
        if (Input.GetKeyUp(KeyCode.Y) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, 0);
        }
        else if (Input.GetKeyUp(KeyCode.U) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, teamIndex);
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            chatSystem.ForceCloseChat();
        }

        if (isLocalPlayer)
        {
            switch (teamIndex)
            {
                case ChatSystem.UNSET_TEAM:
                    teamIdentifier.textComponent.text = "No Team";
                    break;
                case COUNTER_TERRORIST_INDEX:
                    teamIdentifier.textComponent.text = "CT";
                    break;
                case TERRORIST_INDEX:
                    teamIdentifier.textComponent.text = "Terrorist";
                    break;
                case SPECTATOR_INDEX:
                    teamIdentifier.textComponent.text = "Spectator";
                    break;
                default:
                    teamIdentifier.textComponent.text = "No Team";
                    break;
            }
        }
    }

    private bool ChatSystemIsOpen()
    {
        if (chatSystem == null)
        {
            chatSystem = GameObject.FindObjectOfType<ChatSystem>();
            canvasGroup = chatSystem.canvasGroup;
        }

        return canvasGroup.alpha > 0.01f;
    }
}
