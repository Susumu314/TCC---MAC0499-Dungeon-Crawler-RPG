using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text hpMpText;
    public Image healthBar;
    public Image manaBar;
    public GameObject Selection_Indicator;
    private Unit unit;

    public void SetHUD(Unit u)
    {
        unit = u;
        if (unit.unitName ==""){
            return;
        }
        nameText.text = unit.unitName;
        if(hpMpText)
            hpMpText.text = unit.currentHP + "/" + unit.maxHP + "\n"
                      + unit.currentMana + "/" + unit.maxMana;
        healthBar.fillAmount = (float) unit.currentHP/unit.maxHP;
        if(manaBar)
            manaBar.fillAmount = (float) unit.currentMana/unit.maxMana;
    }

    //fazer isso ser uma animação algum dia
    public void SetHP(int hp){ //set both the HP on the HUD and in the UNIT
        unit.currentHP = hp;
        healthBar.fillAmount = (float)hp/unit.maxHP;
        if(hpMpText)
            hpMpText.text = unit.currentHP + "/" + unit.maxHP + "\n"
                      + unit.currentMana + "/" + unit.maxMana;
    }

    public void SetMP(int mp){//set both the MP on the HUD and in the UNIT
        unit.currentMana = mp;
        manaBar.fillAmount = (float)mp/unit.maxMana;
        if(hpMpText)
            hpMpText.text = unit.currentHP + "/" + unit.maxHP + "\n"
                      + unit.currentMana + "/" + unit.maxMana;
    }

    public void is_Selected(bool selected){
        Selection_Indicator.SetActive(selected);
    }
}
