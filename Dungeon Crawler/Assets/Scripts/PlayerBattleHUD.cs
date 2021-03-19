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

    public void SetHUD(Unit unit)
    {
        if (unit.unitName ==""){
            return;
        }
        nameText.text = unit.unitName;
        hpMpText.text = unit.currentHP + "/" + unit.maxHP + "\n"
                      + unit.currentMana + "/" + unit.maxMana;
        healthBar.fillAmount = (float) unit.currentHP/unit.maxHP;
        manaBar.fillAmount = (float) unit.currentMana/unit.maxMana;
    }

    public void SetHP(int hp, Unit unit){
        unit.currentHP = hp;
        healthBar.fillAmount = hp/unit.maxHP;
    }

    public void SetMP(int mp, Unit unit){
        unit.currentMana = mp;
        manaBar.fillAmount = mp/unit.maxMana;
    }
}
