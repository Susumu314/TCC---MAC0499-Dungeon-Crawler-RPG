using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class SelectableElement : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
    public Battle_System Battle_System;
    public string text = "Description text goes here";
    public void OnSelect(BaseEventData eventData)
    {
        if(!Battle_System){
            return;
        }
        Battle_System.dialogueText.text = text;
    }
}