using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
* Classe que utilizada para controlar a HUD de uma dos membros da equipe
* do jogador durante as cenas de batalha.
*/
public class PlayerBattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Text hpMpText;
    public Image healthBar;
    public Image manaBar;
    public GameObject Selection_Indicator;
    public GameObject Target_Indicator;
    private Unit unit;
    public bool isSelected;
    public bool isTarget;
    public TextMeshProUGUI statusMod;
    public TextMeshProUGUI statusCondition;

    public void Awake(){
        if(statusMod == null){
            statusMod = transform.GetChild(transform.childCount-1).GetComponent<TextMeshProUGUI>();
        }
    }

    

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
            nameText.text = unit.unitName + " L." + unit.unitLevel;
        SetStatusConditions();
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

    /**
    * Liga ou desliga o Highlight de alvo da HUD do personagem dependendo se é o personagem selecionado ou não.
    *
    * @param selected Variável que diz se o Highlight deve estar ativo ou não.
    */
    public void is_Target(bool selected){
        Target_Indicator.SetActive(selected);
        isTarget = selected;
    }

    /**
    * Reseta o HUD
    */
    public void Reset(){
        unit = null;
        hpMpText.text = "";
        healthBar.fillAmount = 0f;
        nameText.text = "";
        manaBar.fillAmount = 0f;
        Target_Indicator.SetActive(false);
        isTarget = false;
        Selection_Indicator.SetActive(false);
        isSelected = false;
        statusCondition.text = "";
        statusMod.text = "";
    }

    public void SetStatusModText(int[] modStages){
        string text = "";
        if (modStages[0] != 0){//atk
            text += "<sprite=52>";
            if(modStages[0] > 0){
                text += "<sprite=" + (283 + modStages[0]) + ">";
            }
            else{
                text += "<sprite=" + (289 - modStages[0]) + ">";
            }
        }
        if (modStages[1] != 0){//def
            text += "<sprite=65>";
            if(modStages[1] > 0){
                text += "<sprite=" + (283 + modStages[1]) + ">";
            }
            else{
                text += "<sprite=" + (289 - modStages[1]) + ">";
            }
        }
        if (modStages[2] != 0){//spatk
            text += "<sprite=71>";
            if(modStages[2] > 0){
                text += "<sprite=" + (283 + modStages[2]) + ">";
            }
            else{
                text += "<sprite=" + (289 - modStages[2]) + ">";
            }
        }
        if (modStages[3] != 0){//spdef
            text += "<sprite=90>";
            if(modStages[3] > 0){
                text += "<sprite=" + (283 + modStages[3]) + ">";
            }
            else{
                text += "<sprite=" + (289 - modStages[3]) + ">";
            }
        }
        if (modStages[4] != 0){//speed
            text += "<sprite=95>";
            if(modStages[4] > 0){
                text += "<sprite=" + (283 + modStages[4]) + ">";
            }
            else{
                text += "<sprite=" + (289 - modStages[4]) + ">";
            }
        }
        if (modStages[5] != 0){//acc
            text += "<sprite=126>";
            if(modStages[5] > 0){
                text += "<sprite=" + (283 + modStages[5]) + ">";
            }
            else{
                text += "<sprite=" + (289 - modStages[5]) + ">";
            }
        }
        if (modStages[6] != 0){//evasion
            text += "<sprite=35>";
            if(modStages[6] > 0){
                text += "<sprite=" + (283 + modStages[6]) + ">";
            }
            else{
                text += "<sprite=" + (289 - modStages[6]) + ">";
            }
        }
        statusMod.SetText(text);
    }

    /**
    * Modifica o HUD para que o jogador tenha feedback visual da condição do estado da unidade
    */
    public void SetStatusConditions(){
        statusCondition.gameObject.SetActive(true);
        if(unit.statusCondition == Unit.STATUS_CONDITION.NULL){
            if(unit.isPlayerUnit){
                statusCondition.text = "" + unit.unitLevel;
            }
            else{
                statusCondition.gameObject.SetActive(false);
                statusCondition.text = "";
            }
            return;
        }
        switch (unit.statusCondition)
        {
            case(Unit.STATUS_CONDITION.POISON):
                statusCondition.text = "<sprite=0>";
            break;
            case(Unit.STATUS_CONDITION.BURN):
                statusCondition.text = "<sprite=9>";
            break;
            case(Unit.STATUS_CONDITION.RAGE):
                statusCondition.text = "<sprite=4>";
            break;
            case(Unit.STATUS_CONDITION.FREEZE):
                statusCondition.text = "<sprite=281>";
            break;
            case(Unit.STATUS_CONDITION.PARALYSIS):
                statusCondition.text = "<sprite=8>";
            break;
            default:
            break;
        }
    }
}
