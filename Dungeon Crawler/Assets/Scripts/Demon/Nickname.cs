using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nickname : MonoBehaviour
{
    public InputField inputField;
    public GameObject box;
    public OW_MenuSystem owMenuSystem;
    public void OnOpenField(){
        box.SetActive(true);
        owMenuSystem.state = OW_MenuSystem.OW_State.NICKNAME;
        inputField.Select();
    }

    public void OnConfirm(){
        owMenuSystem.ChangeNickname(inputField.text);
        box.SetActive(false);
        this.GetComponent<Button>().Select();
        owMenuSystem.state = OW_MenuSystem.OW_State.DEMONMENU;
    }
}
