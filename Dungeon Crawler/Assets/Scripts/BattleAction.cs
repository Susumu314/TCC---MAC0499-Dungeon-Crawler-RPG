using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Classe que representa o sistema de ações que podem ser realizadas em uma batalha
*/

public class BattleAction : MonoBehaviour
{
    static private float BASEPOWER = 50;
    public enum Act
    {
        ATTACK, GUARD, ESCAPE, CAPTURE, 
        SKILL, ITEM, NULL
    }
    private Act act;
    private List<Unit> TargetList = new List<Unit>(); //todos os alvos de uma ação serão colocados dentro deste array

    private Unit unitRef;
    private int skillID;
    private int itemID;
    Skill.SkillData attackAction = new Skill.SkillData (/*name*/        "Tackle", 
                                                        /*type*/         Skill.TYPE.NORMAL, 
                                                        /*target_type*/  Skill.TARGET_TYPE.SINGLE, 
                                                        /*priority*/     Skill.PRIORITY.NORMAL, 
                                                        /*power*/        (int)BASEPOWER, 
                                                        /*accuracy*/     100, 
                                                        /*cost*/         0,
                                                        /*status_effect*/Skill.STATUS_EFFECT.NULL, 
                                                        /*isSpecial*/    false, 
                                                        /*isRanged*/     false,
                                                        /*ID*/           -1,
                                                        /*VFX*/          "Punch",
                                                        /*VFX_COLOR*/    Color.white,
                                                        /*DESC*/         "");
    /**
    * Seta as ações a serem tomadas por uma unidade durante a batalha
    */
    public void SetAction(Act acao = Act.NULL, List<Unit> Targets = null){
        if (acao != Act.NULL)//a acao só vai ser nula quando o SetAction for chamado para atribuição apenas dos Targets
                             //ou seja, a função assume que a acao ja foi escolhida numa chamada anterior do SetAction
            act = acao;
        TargetList = Targets;
    }

    public int[] SetSkill(int i){
        act = Act.SKILL;
        skillID = unitRef.skillList[i];
        //olha informação da skill para ver que tipo de alvo que deve ser utilizado
        int[] r = new int[2];
        Skill.SkillData s = Skill.SkillList[skillID];
        r[0] = (int)s.Priority;
        r[1] = (int)s.Target_type;
        return(r);
    }

    public int[] SetItem(int index){
        act = Act.ITEM;
        itemID = index;
        //olha informação da skill para ver que tipo de alvo que deve ser utilizado
        int[] r = new int[2];
        Item.ItemData i = Item.ItemList[itemID];
        r[0] = (int)i.Priority;
        r[1] = (int)i.Target_type;
        return(r);
    }
    public void Start(){
        unitRef = gameObject.GetComponent<Unit>();
    }

    /**
    * Realiza a ação de batalhas setadas pelo jogador durante o turno de seleção de ações
    */
    public IEnumerator PerformAction(){
        float wait = 0.75f;
        /*por enquanto, ações em alvos mortos, resultam em pular a ação*/
        switch (act)
        {
            case Act.ATTACK://deals normal physical damage to the target
            {
                if(!TargetList[0].isDead){
                    //criar uma skill pra ataque basico e sumir com esse act.ATTACK
                    

                    int damage = DamageCalculation(attackAction, TargetList[0]);
                    wait += MoveAnimation("Punch", TargetList[0].HUD.transform, Color.white);
                    //play soundfx
                    //display mensage
                    //wait for animations and mensage
                    Debug.Log(damage);
                    TargetList[0].TakeDamage(damage);
                }
                break;
            }
            case Act.GUARD://Guard Action
            {
                TargetList[0].SetGuard(true);//aqui esta assumindo que o target está sendo corretamente associado ao usuário
                wait += MoveAnimation("Barrier", TargetList[0].HUD.transform, Color.white);
                break;
            }
            case Act.ESCAPE://Tenta escapar
            {
                TargetList[0].Escape();
                break;
            }
            case Act.SKILL://tenta capturar o demonio
            {
                int damage;
                bool payed = false;
                Skill.SkillData s = Skill.SkillList[skillID];
                for (int i = 0; i < TargetList.Count; i++)
                {
                    if(TargetList[i].isDead){
                        continue;
                    }
                    if(!payed){//paga o custo para usar a skill
                        payed = unitRef.PaySkillCost(s.Cost, s.IsSpecial);
                    }
                    if(!payed){//caso nao tenha conseguido pagar o custo da skill, não executa a skill
                        Debug.Log("Não foi possível pagar a habilidade");
                        i = TargetList.Count;
                        continue;
                    }
                    //calcula o dano e registra o dano (se a habilidade causar dano)
                    if(s.Power > 0){
                        damage = DamageCalculation(s, TargetList[i]);
                        Debug.Log(damage);
                        TargetList[i].TakeDamage(damage);
                    }
                    else if(s.Power < 0){
                        damage = HealCalculation(-(float)s.Power, TargetList[i]);
                        TargetList[i].HealDamage(damage);
                    }
                    wait += MoveAnimation(s.VFX, TargetList[i].HUD.transform, s.COLOR);
                }
                break;
            }
            case Act.ITEM://tenta capturar o demonio
            {
                int damage;
                bool payed = false;
                Item.ItemData item = Item.ItemList[itemID];
                for (int i = 0; i < TargetList.Count; i++)
                {
                    if(TargetList[i].isDead){
                        continue;
                    }
                    if(!payed){//paga o custo para usar a skill
                        payed = unitRef.PayItemCost(itemID);
                    }
                    if(!payed){//caso nao tenha conseguido pagar o custo da skill, não executa a skill
                        Debug.Log("Não foi possível pagar a habilidade");
                        i = TargetList.Count;
                        continue;
                    }
                    if(item.Status_effect == Item.STATUS_EFFECT.CAPTURE){
                        StartCoroutine(TargetList[0].Capture(item.Power, item.COLOR));
                        continue;
                    }
                    //calcula o dano e registra o dano (se a habilidade causar dano)
                    if(item.Power > 0){
                        damage = ItemDamageCalculation(item, TargetList[i]);
                        Debug.Log(damage);
                        TargetList[i].TakeDamage(damage);
                    }
                    else if(item.Power < 0){
                        damage = HealCalculation(-(float)item.Power, TargetList[i]);
                        TargetList[i].HealDamage(damage);
                    }
                    wait += MoveAnimation(item.VFX, TargetList[i].HUD.transform, item.COLOR);
                }
                break;
            }
            default: 
            break;
        }
        yield return new WaitForSeconds(wait);
    }        
    float MoveAnimation(string animation, Transform target_transform, Color color){
        GameObject Anim = Instantiate(Resources.Load("VFX/Skill_Animation"), target_transform.position, Quaternion.identity) as GameObject;
        Animator animator = Anim.GetComponent<Animator>();
        if(color != Color.white){
            Anim.GetComponent<SpriteRenderer>().color =  color;
        }
        animator.Play(animation);
        //Fetch the current Animation clip information for the base layer
        AnimatorClipInfo[] m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        return m_CurrentClipInfo[0].clip.length;
    }
    /**
    * Equação que calcula o dano
    */
    int DamageCalculation(Skill.SkillData s, Unit Target){
        float POWERRATIO = (s.Power/BASEPOWER);
        float ATKDEFRATIO;
        if(!s.IsSpecial){
            ATKDEFRATIO = ((float)unitRef.attack/(float)Target.defence);
        }
        else{
            ATKDEFRATIO = ((float)unitRef.special_attack/(float)Target.special_defence);
        }

        float TARGETLANEMODIFIER;
        float ATTACKERLANEMODIFIER;
        if(!s.IsRanged){
            TARGETLANEMODIFIER = (1 - Target.isBackLine*0.25f);//Atacar alvos na backline causa 25% a menos de dano
            ATTACKERLANEMODIFIER = (1 - unitRef.isBackLine*0.25f);//Atacantes da backline causam 25% a menos de dano fisico
        }
        else{
            TARGETLANEMODIFIER = 1;
            ATTACKERLANEMODIFIER = 1;
        }
        int damage = Mathf.CeilToInt(((5*unitRef.unitLevel)/5 + 2) * POWERRATIO * ATKDEFRATIO * TARGETLANEMODIFIER * ATTACKERLANEMODIFIER);
        return damage;
    }

    /**
    * Equação que calcula o dano de um item
    */
    int ItemDamageCalculation(Item.ItemData s, Unit Target){
        float POWERRATIO = (s.Power/BASEPOWER);
        float ATKDEFRATIO;
        if(!s.IsSpecial){
            ATKDEFRATIO = ((float)unitRef.attack/(float)Target.defence);
        }
        else{
            ATKDEFRATIO = ((float)unitRef.special_attack/(float)Target.special_defence);
        }

        float TARGETLANEMODIFIER;
        float ATTACKERLANEMODIFIER;
        if(!s.IsRanged){
            TARGETLANEMODIFIER = (1 - Target.isBackLine*0.25f);//Atacar alvos na backline causa 25% a menos de dano
            ATTACKERLANEMODIFIER = (1 - unitRef.isBackLine*0.25f);//Atacantes da backline causam 25% a menos de dano fisico
        }
        else{
            TARGETLANEMODIFIER = 1;
            ATTACKERLANEMODIFIER = 1;
        }
        int damage = Mathf.CeilToInt(((5*unitRef.unitLevel)/5 + 2) * POWERRATIO * ATKDEFRATIO * TARGETLANEMODIFIER * ATTACKERLANEMODIFIER);
        return damage;
    }

    /**
    * Equação que calcula o dano
    */
    int HealCalculation(float POWER, Unit Target){
        float POWERRATIO = (POWER/BASEPOWER);
        int damage = Mathf.CeilToInt(((5*unitRef.unitLevel)/5 + 2) * POWERRATIO);
        return damage;
    }
}
