using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
* Classe que utilizada para controlar a HUD de uma dos membros da equipe
* do jogador durante as cenas de batalha.
*/
public class PlayerBattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text hpMpText;
    public Image healthBar;
    public Image manaBar;
    public GameObject Selection_Indicator;
    private Unit unit;
    public bool isSelected;

    /**
    * Seta a HUD.
    *
    * @param u Contém as informações da unidade a qual a HUD representa.
    */

    public void SetHUD(Unit u)
    {
        unit = u;
        if (unit.unitName ==""){
            return;
        }
        if(hpMpText)
            hpMpText.text = unit.currentHP + "/" + unit.maxHP + "\n"
                      + unit.currentMana + "/" + unit.maxMana;
        healthBar.fillAmount = (float) unit.currentHP/unit.maxHP;
        if(manaBar){ //apenas personagens do jogador apresentam barra de mana
            nameText.text = unit.unitName;
            manaBar.fillAmount = (float) unit.currentMana/unit.maxMana;
            
        }
        else
            nameText.text = unit.unitName + " LvL" + unit.unitLevel;
    }

    /**
    * Atualiza a vida da unidade na HUD.
    *
    * @param hp Vida atual da unidade.
    */
    public IEnumerator SetHP(int hp){ //set both the HP on the HUD and in the UNIT
        unit.currentHP = hp;
        healthBar.fillAmount = (float)hp/unit.maxHP;
        if(hpMpText)
            hpMpText.text = unit.currentHP + "/" + unit.maxHP + "\n"
                      + unit.currentMana + "/" + unit.maxMana;
        //Se for inimigo, mostra a barra de vida durante um tempo quando houver uma mudança em sua hp
        if(unit.isPlayerUnit){
            yield return null;
        }
        else{
            Selection_Indicator.SetActive(true);
            Selection_Indicator.transform.GetChild(1).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Selection_Indicator.transform.GetChild(1).gameObject.SetActive(true);
            Selection_Indicator.SetActive(false);
        }
    }


    /**
    * Atualiza a mana da unidade na HUD.
    *
    * @param mp Mana atual da unidade.
    */

    public void SetMP(int mp){//set both the MP on the HUD and in the UNIT
        unit.currentMana = mp;
        manaBar.fillAmount = (float)mp/unit.maxMana;
        if(hpMpText)
            hpMpText.text = unit.currentHP + "/" + unit.maxHP + "\n"
                      + unit.currentMana + "/" + unit.maxMana;
    }

    /**
    * Liga ou desliga o Highlight da HUD do personagem dependendo se é o personagem selecionado ou não.
    *
    * @param selected Variável que diz se o Highlight deve estar ativo ou não.
    */
    public void is_Selected(bool selected){
        Selection_Indicator.SetActive(selected);
        isSelected = selected;
    }
}
