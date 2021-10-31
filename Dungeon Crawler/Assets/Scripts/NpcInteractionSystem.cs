﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInteractionSystem : MonoBehaviour
{
    public DialogueTrigger start;
    private DialogueManager dialogueManager;
    public GameObject Menu;
    public bool canOpenMenu = true;
    public void Awake(){
        dialogueManager = FindObjectOfType<DialogueManager>();
    }
    public void Start(){
        start.TriggerDialogue();
    }

    void Update(){
        if(dialogueManager.dialogueFinished && canOpenMenu){
			if(!Menu.gameObject.activeSelf){
				Menu.gameObject.SetActive(true);
			}
		}
    }

}
