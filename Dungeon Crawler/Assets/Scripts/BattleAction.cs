using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
* Classe que representa o sistema de ações que podem ser realizadas em uma batalha
*/

public class BattleAction : MonoBehaviour
{    
    static private float BASEPOWER = 50;
    public enum Act
    {
        ATTACK, GUARD, ESCAPE, CAPTURE, 
        SKILL, ITEM, SWITCH, NULL
    }
    public Act act;
    private List<Unit> TargetList = new List<Unit>(); //todos os alvos de uma ação serão colocados dentro deste array
    private List<Unit> enemyPartyRef = new List<Unit>();
    private Unit unitRef;
    public int skillID;
    private int itemID;
    private float timer = 0.0f;
    private bool startTimer = false;
    private bool skip = false;
    public float skipTime;
    private float resistMod; 
    Skill.SkillData attackAction = new Skill.SkillData (/*name*/        "Tackle", 
                                                        /*type*/         BaseStats.TYPE.NORMAL, 
                                                        /*target_type*/  Skill.TARGET_TYPE.SINGLE, 
                                                        /*priority*/     Skill.PRIORITY.NORMAL, 
                                                        /*power*/        (int)BASEPOWER, 
                                                        /*accuracy*/     100, 
                                                        /*cost*/         0,
                                                        /*status_effect*/new Skill.EFFECT[] {Skill.EFFECT.NULL}, 
                                                        /*isSpecial*/    false, 
                                                        /*isRanged*/     false,
                                                        /*ID*/           -1,
                                                        /*VFX*/          "Punch",
                                                        /*VFX_COLOR*/    Color.white,
                                                        /*DESC*/         "",
                                                        /*OWUse*/        false);
    Battle_System Battle_System;
    OW_MenuSystem Menu_System;
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
        skipTime = 1.0f;
    }

    public void InitBattleSystemRef(Battle_System b){
        Battle_System = b;
    }

    public void InitMenuSystemRef(OW_MenuSystem m){
        Menu_System = m;
    }
    void Update(){
        if(startTimer){
            timer += Time.deltaTime;
            if(Input.anyKeyDown){
                skip = true;
            }
        }
    }

    /**
    * Realiza a ação de batalhas setadas pelo jogador durante o turno de seleção de ações
    */
    public IEnumerator PerformAction(){
        float wait = 0.1f;
        /*Checa status conditions para verificar se a unidade consegue agir neste turno*/
        int rng = Random.Range(0,100);
        bool canAct = true;
        switch (unitRef.statusCondition)
        {
            case Unit.STATUS_CONDITION.FREEZE:
                if(rng < 20){//descongela
                    unitRef.HealVolatileCondition();
                    yield return ShowDialog(unitRef.unitName + " is no longer frozen!", skipTime);
                }
                else{//congelado
                    wait += MoveAnimation("Frozen", unitRef.HUD.transform, new Color(0.23f, 0.84f, 0.99f, 1.0f));
                    yield return ShowDialog(unitRef.unitName + " is frozen solid!", skipTime);
                    canAct = false;
                }
            break;
            case Unit.STATUS_CONDITION.PARALYSIS:
                if(rng < 25){
                    wait += MoveAnimation("Paralysis", unitRef.HUD.transform, Color.white);
                    yield return ShowDialog(unitRef.unitName + " is paralyzed!\nIt can't move!", skipTime);
                    canAct = false;
                }
            break;
            default:
            break;
        }
        if(canAct){
            switch (act)
            {
                case Act.ATTACK://deals normal physical damage to the target
                {
                    if(TargetList[0].isDead){
                        if(TargetList.Count == 1){
                            TargetList[0] = Battle_System.FindNextTarget(TargetList[0]);
                            if(!TargetList[0]){
                                break;
                            }
                        }
                        else{
                            break;
                        }
                    }
                    //criar uma skill pra ataque basico e sumir com esse act.ATTACK
                    int damage = DamageCalculation(attackAction, TargetList[0]);
                    wait += MoveAnimation("Punch", TargetList[0].HUD.transform, Color.white);
                    //play soundfx
                    //display mensage
                    //wait for animations and mensage
                    Debug.Log(damage);
                    TargetList[0].TakeDamage(damage,1);
                    yield return ShowDialog(unitRef.unitName + " attacked " + TargetList[0].unitName + ".", skipTime);
                    break;
                }
                case Act.GUARD://Guard Action
                {
                    TargetList[0].SetGuard(true);//aqui esta assumindo que o target está sendo corretamente associado ao usuário
                    wait += MoveAnimation("Barrier", TargetList[0].HUD.transform, Color.white);
                    yield return ShowDialog(unitRef.unitName + " is on guard.", skipTime);
                    break;
                }
                case Act.SWITCH://Switch Action
                {
                    unitRef.Switch(TargetList[0]);
                    break;
                }
                case Act.ESCAPE://Tenta escapar
                {
                    yield return ShowDialog(unitRef.unitName + " is trying to escape.", skipTime);
                    bool success = TargetList[0].Escape();
                    if(success){
                        yield return Battle_System.EscapeBattle(unitRef.unitName);
                    }
                    else{
                        yield return ShowDialog("The party couldn't escape!", skipTime);
                    }
                    break;
                }
                case Act.SKILL://tenta capturar o demonio
                {
                    int damage = 0;
                    bool payed = false;
                    Skill.SkillData s = Skill.SkillList[skillID];
                    for (int i = 0; i < TargetList.Count; i++)
                    {
                        if(TargetList[i].isDead){
                            if(TargetList.Count == 1){
                                TargetList[i] = Battle_System.FindNextTarget(TargetList[0]);
                                if(!TargetList[i]){
                                    break;
                                }
                            }
                            else{
                                continue;
                            }
                        }
                        if(!payed){//paga o custo para usar a skill
                            payed = unitRef.PaySkillCost(s.Cost, s.IsSpecial);
                        }
                        if(!payed){//caso nao tenha conseguido pagar o custo da skill, não executa a skill
                            yield return ShowDialog(unitRef.unitName + " can't pay the skill cost.", skipTime);
                            i = TargetList.Count;
                            continue;
                        }
                        if(i == 0){
                            yield return ShowDialog(unitRef.unitName + " used " + s.Name, skipTime);
                        }
                        if(!Accuracy_Check(s, TargetList[i])){
                            Debug.Log("Missed " + TargetList[i].unitName);
                            yield return ShowDialog(unitRef.unitName + " missed " + TargetList[i].unitName, skipTime);
                            continue;
                        }
                        wait += MoveAnimation(s.VFX, TargetList[i].HUD.transform, s.COLOR);
                        //calcula o dano e registra o dano (se a habilidade causar dano)
                        if(s.Power > 0){
                            damage = DamageCalculation(s, TargetList[i]);
                            Debug.Log(damage);
                            TargetList[i].TakeDamage(damage, resistMod);
                            if(resistMod > 1){
                                yield return ShowDialog("It's SUPER EFFECTIVE!!!\nDamage:" + damage, skipTime); 
                            }
                            if(resistMod == 0){
                                yield return ShowDialog("It's not effective.\nDamage:" + damage, skipTime); 
                            }
                            else if(resistMod < 1){
                                yield return ShowDialog("It's not very effective.\nDamage:" + damage, skipTime); 
                            }
                        }
                        else if(s.Power < 0){
                            damage = HealCalculation(-(float)s.Power, TargetList[i]);
                            TargetList[i].HealDamage(damage);
                            yield return ShowDialog("Healed: " + damage, skipTime); 
                        }
                        //ativa efeitos especiais da habilidade
                        foreach(Skill.EFFECT effect in s.Effect){
                            if(effect != Skill.EFFECT.NULL){
                                yield return Skill_Effect(effect, TargetList[i]);
                            }
                            if(effect == Skill.EFFECT.LIFESTEAL){
                                unitRef.HealDamage(Mathf.CeilToInt(damage*0.5f));
                            }
                        }
                    }
                    break;
                }
                case Act.ITEM:
                {
                    int damage;
                    bool payed = false;
                    Item.ItemData item = Item.ItemList[itemID];
                    if(item.Status_effect[0] == Skill.EFFECT.RETURNCITY){
                        if(GameManager.Instance.state == GameManager.State.Overworld){
                            SceneManager.LoadScene("City_Template");
                        }
                    }
                    for (int i = 0; i < TargetList.Count; i++)
                    {
                        if(TargetList[i].isDead){
                            yield return ShowDialog("The target is already defeated", skipTime);
                            continue;
                        }
                        if(!payed){//paga o custo para usar a skill
                            payed = unitRef.PayItemCost(itemID);
                        }
                        if(!payed){//caso nao tenha conseguido pagar o custo da skill, não executa a skill
                            yield return ShowDialog("You don't have any " + item.Name + " left.", skipTime);
                            i = TargetList.Count;
                            continue;
                        }
                        wait += MoveAnimation(item.VFX, TargetList[i].HUD.transform, item.COLOR);
                        if(item.Status_effect[0] == Skill.EFFECT.CAPTURE){
                            StartCoroutine(TargetList[0].Capture(item.Power, item.COLOR));
                            continue;
                        }
                        //calcula o dano e registra o dano (se a habilidade causar dano)
                        if(item.Power > 0){
                            damage = ItemDamageCalculation(item, TargetList[i]);
                            Debug.Log(damage);
                            TargetList[i].TakeDamage(damage, resistMod);
                            if(resistMod > 1){
                                yield return ShowDialog("It's SUPER EFFECTIVE!!!\nDamage:" + damage, skipTime); 
                            }
                            if(resistMod == 0){
                                yield return ShowDialog("It's not effective.\nDamage:" + damage, skipTime); 
                            }
                            else if(resistMod < 1){
                                yield return ShowDialog("It's not very effective.\nDamage:" + damage, skipTime); 
                            }
                        }
                        else if(item.Power < 0){
                            damage = HealCalculation(-(float)item.Power, TargetList[i]);
                            TargetList[i].HealDamage(damage);
                            yield return ShowDialog("Healed: " + damage, skipTime); 
                        }
                        foreach(Skill.EFFECT effect in item.Status_effect){
                            if(effect != Skill.EFFECT.NULL){
                                yield return Skill_Effect(effect, TargetList[i]);
                            }
                        }
                    }
                    break;
                }
                default: 
                break;
            }
        }
        //yield return new WaitForSeconds(wait);//esse yield return que esta crashando tudo
        yield return new WaitForSeconds(0.7f);
        print("Oh baby" + unitRef.unitName + wait);
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
        //checa por condições de status que afetam dano
        float CONDITIONMODIFIER = 1;
        if(!s.IsSpecial){
            ATKDEFRATIO = ((float)unitRef.attack*unitRef.attackModifier/((float)Target.defence*Target.defenceModifier));
            if(unitRef.statusCondition == Unit.STATUS_CONDITION.BURN){//se está burn, dê metade do dano com golpes fisicos
                CONDITIONMODIFIER = 0.5f;
            }
            if(unitRef.statusCondition == Unit.STATUS_CONDITION.RAGE){//se está burn, dê metade do dano com golpes fisicos
                CONDITIONMODIFIER = 1.25f;
            }
        }
        else{
            ATKDEFRATIO = ((float)unitRef.special_attack*unitRef.special_attackModifier/((float)Target.special_defence*Target.special_defenceModifier));
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
        float RESISTMODIFIER = BaseStats.Type_Chart[(int)s.Type,(int)Target.type];
        resistMod = RESISTMODIFIER;
        int damage = Mathf.CeilToInt(((5*unitRef.unitLevel)/5 + 2) * POWERRATIO * ATKDEFRATIO * TARGETLANEMODIFIER * ATTACKERLANEMODIFIER * RESISTMODIFIER * CONDITIONMODIFIER);
        return damage;
    }

    /**
    * Equação que calcula o dano de um item
    */
    int ItemDamageCalculation(Item.ItemData s, Unit Target){
        float POWERRATIO = (s.Power/BASEPOWER);
        float ATKDEFRATIO;
        if(!s.IsSpecial){
            ATKDEFRATIO = ((float)unitRef.attack*unitRef.attackModifier/(float)Target.defence*Target.defenceModifier);
        }
        else{
            ATKDEFRATIO = ((float)unitRef.special_attack*unitRef.special_attackModifier/(float)Target.special_defence*Target.special_defenceModifier);
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
        float RESISTMODIFIER = BaseStats.Type_Chart[(int)s.Type,(int)Target.type];
        int damage = Mathf.CeilToInt(((5*unitRef.unitLevel)/5 + 2) * POWERRATIO * ATKDEFRATIO * TARGETLANEMODIFIER * ATTACKERLANEMODIFIER * RESISTMODIFIER);
        return damage;
    }

    /**
    * Equação que calcula o dano
    */
    int HealCalculation(float POWER, Unit Target){
        float POWERRATIO = (POWER/BASEPOWER);
        int damage = Mathf.CeilToInt(((5*unitRef.unitLevel)/5 + 2) * POWERRATIO* 1.5f);//1.5 adicionado porque acho que o healing esta muito fraco
        return damage;
    }

    /**
    * Função que checa se um golpe acertou o alvo ou não
    */
    private bool Accuracy_Check(Skill.SkillData s, Unit Target){
        int rng = UnityEngine.Random.Range(0, 100);
        int accuracy = Mathf.RoundToInt(s.Accuracy*unitRef.accuracyModifier*(1.0f/Target.evasion));
        Debug.Log("Accuracy_Check - RNG = " + rng + " accuracy = " + accuracy);
        return (rng<accuracy);
    }

    /**
    * Função que mostra os dialogos de batalha, esperando um tempo determinado ou input do usuario para passar o dialogo
    * 
    * @param text Texto a ser mostrado pela caixa de dialogos
    * @waitTime Tempo maximo para se esperar antes de passar o dialogo automaticamente
    */
    private IEnumerator ShowDialog(string text, float waitTime){
        if(Battle_System != null){
            Battle_System.dialogueText.text = text;
            startTimer = true;
            timer = 0f;
            yield return new WaitUntil(() => ((timer >= waitTime) || skip));
            startTimer = false;
            skip = false;
        }
        yield return null;
    }

    /**
    * Função que executa os efeitos das habilidades
    */
    private IEnumerator Skill_Effect(Skill.EFFECT e, Unit Target){
        int r = Random.Range(0, 100);
        switch (e)
        {   
            //buffs e debuffs a status incrementam e decrementam os modificadores em multiplos de 50% de cada vez
            case Skill.EFFECT.ATK_UP:{
                Target.modStages[0] = Mathf.Min(6, Target.modStages[0] + 1);
                if (Target.modStages[0] >= 1)
                {
                    Target.attackModifier = (2.0f + Target.modStages[0])/2.0f;
                    Debug.Log("Attack mod" + Target.attackModifier);
                }
                else{
                    Target.attackModifier = 2.0f/(2.0f - Target.modStages[0]);
                    Debug.Log("Attack mod " + Target.attackModifier);
                }
                yield return ShowDialog("ATK UP!", skipTime);
                break;
            }
            case Skill.EFFECT.DEF_UP:{
                Target.modStages[1] = Mathf.Min(6, Target.modStages[1] + 1);
                if (Target.modStages[1] >= 1)
                {
                    Target.defenceModifier = (2.0f + Target.modStages[1])/2.0f;
                }
                else{
                    Target.defenceModifier = 2.0f/(2.0f - Target.modStages[1]);
                }
                yield return ShowDialog("DEF UP!", skipTime);
                break;
            }
            case Skill.EFFECT.SPATK_UP:{
                Target.modStages[2] = Mathf.Min(6, Target.modStages[2] + 1);
                if (Target.modStages[2] >= 1)
                {
                    Target.special_attackModifier = (2.0f + Target.modStages[2])/2.0f;
                }
                else{
                    Target.special_attackModifier = 2.0f/(2.0f - Target.modStages[2]);
                }
                yield return ShowDialog("SPATK UP!", skipTime);
                break;
            }
            case Skill.EFFECT.SPDEF_UP:{
                Target.modStages[3] = Mathf.Min(6, Target.modStages[3] + 1);
                if (Target.modStages[3] >= 1)
                {
                    Target.special_defenceModifier = (2.0f + Target.modStages[3])/2.0f;
                }
                else{
                    Target.special_defenceModifier = 2.0f/(2.0f - Target.modStages[3]);
                }
                yield return ShowDialog("SPDEF UP!", skipTime);
                break;
            }
            case Skill.EFFECT.SPEED_UP:{
                Target.modStages[4] = Mathf.Min(6, Target.modStages[4] + 1);
                if (Target.modStages[4] >= 1)
                {
                    Target.speedModifier = (2.0f + Target.modStages[4])/2.0f;
                }
                else{
                    Target.speedModifier = 2.0f/(2.0f - Target.modStages[4]);
                }
                yield return ShowDialog("SPEED UP!", skipTime);
                break;
            }
            case Skill.EFFECT.ACC_UP:{
                Target.modStages[5] = Mathf.Min(6, Target.modStages[5] + 1);
                if (Target.modStages[5] >= 1)
                {
                    Target.accuracyModifier = (3.0f + Target.modStages[5])/3.0f;
                }
                else{
                    Target.accuracyModifier = 3.0f/(3.0f - Target.modStages[5]);
                }
                yield return ShowDialog("ACCURACY UP!", skipTime);
                break;
            }
            case Skill.EFFECT.EVASION_UP:{
                Target.modStages[6] = Mathf.Min(6, Target.modStages[6] + 1);
                if (Target.modStages[6] >= 1)
                {
                    Target.evasion = (3.0f + Target.modStages[6])/3.0f;
                }
                else{
                    Target.evasion = 3.0f/(3.0f - Target.modStages[6]);
                }
                yield return ShowDialog("EVASION UP!", skipTime);
                break;
            }
            case Skill.EFFECT.ATK_DOWN:{
                Target.modStages[0] = Mathf.Max(-6, Target.modStages[0] - 1);
                if (Target.modStages[0] >= 1)
                {
                    Target.attackModifier = (2.0f + Target.modStages[0])/2.0f;
                }
                else{
                    Target.attackModifier = 2.0f/(2.0f - Target.modStages[0]);
                }
                yield return ShowDialog("ATK DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.DEF_DOWN:{
                Target.modStages[1] = Mathf.Max(-6, Target.modStages[1] - 1);
                if (Target.modStages[1] >= 1)
                {
                    Target.defenceModifier = (2.0f + Target.modStages[1])/2.0f;
                }
                else{
                    Target.defenceModifier = 2.0f/(2.0f - Target.modStages[1]);
                }
                yield return ShowDialog("DEF DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.SPATK_DOWN:{
                Target.modStages[2] = Mathf.Max(-6, Target.modStages[2] - 1);
                if (Target.modStages[2] >= 1)
                {
                    Target.special_attackModifier = (2.0f + Target.modStages[2])/2.0f;
                }
                else{
                    Target.special_attackModifier = 2.0f/(2.0f - Target.modStages[2]);
                }
                yield return ShowDialog("SPATK DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.SPDEF_DOWN:{
                Target.modStages[3] = Mathf.Max(-6, Target.modStages[3] - 1);
                if (Target.modStages[3] >= 1)
                {
                    Target.special_defenceModifier = (2.0f + Target.modStages[3])/2.0f;
                }
                else{
                    Target.special_defenceModifier = 2.0f/(2.0f - Target.modStages[3]);
                }
                yield return ShowDialog("SPDEF DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.SPEED_DOWN:{
                Target.modStages[4] = Mathf.Max(-6, Target.modStages[4] - 1);
                if (Target.modStages[4] >= 1)
                {
                    Target.speedModifier = (2.0f + Target.modStages[4])/2.0f;
                }
                else{
                    Target.speedModifier = 2.0f/(2.0f - Target.modStages[4]);
                }
                yield return ShowDialog("SPEED DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.ACC_DOWN:{
                Target.modStages[5] = Mathf.Max(-6, Target.modStages[5] - 1);
                if (Target.modStages[5] >= 1)
                {
                    Target.accuracyModifier = (3.0f + Target.modStages[5])/3.0f;
                }
                else{
                    Target.accuracyModifier = 3.0f/(3.0f - Target.modStages[5]);
                }
                yield return ShowDialog("ACCURACY DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.EVASION_DOWN:{
                Target.modStages[6] = Mathf.Max(-6, Target.modStages[6] - 1);
                if (Target.modStages[6] >= 1)
                {
                    Target.evasion = (3.0f + Target.modStages[6])/3.0f;
                }
                else{
                    Target.evasion = 3.0f/(3.0f - Target.modStages[6]);
                }
                yield return ShowDialog("EVASION DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_ATK_UP:{
                Target.modStages[0] = Mathf.Min(6, Target.modStages[0] + 2);
                if (Target.modStages[0] >= 1)
                {
                    Target.attackModifier = (2.0f + Target.modStages[0])/2.0f;
                    Debug.Log("Attack mod" + Target.attackModifier);
                }
                else{
                    Target.attackModifier = 2.0f/(2.0f - Target.modStages[0]);
                    Debug.Log("Attack mod " + Target.attackModifier);
                }
                yield return ShowDialog("ATK UP!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_DEF_UP:{
                Target.modStages[1] = Mathf.Min(6, Target.modStages[1] + 2);
                if (Target.modStages[1] >= 1)
                {
                    Target.defenceModifier = (2.0f + Target.modStages[1])/2.0f;
                }
                else{
                    Target.defenceModifier = 2.0f/(2.0f - Target.modStages[1]);
                }
                yield return ShowDialog("DEF UP!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_SPATK_UP:{
                Target.modStages[2] = Mathf.Min(6, Target.modStages[2] + 2);
                if (Target.modStages[2] >= 1)
                {
                    Target.special_attackModifier = (2.0f + Target.modStages[2])/2.0f;
                }
                else{
                    Target.special_attackModifier = 2.0f/(2.0f - Target.modStages[2]);
                }
                yield return ShowDialog("SPATK UP!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_SPDEF_UP:{
                Target.modStages[3] = Mathf.Min(6, Target.modStages[3] + 2);
                if (Target.modStages[3] >= 1)
                {
                    Target.special_defenceModifier = (2.0f + Target.modStages[3])/2.0f;
                }
                else{
                    Target.special_defenceModifier = 2.0f/(2.0f - Target.modStages[3]);
                }
                yield return ShowDialog("SPDEF UP!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_SPEED_UP:{
                Target.modStages[4] = Mathf.Min(6, Target.modStages[4] + 2);
                if (Target.modStages[4] >= 1)
                {
                    Target.speedModifier = (2.0f + Target.modStages[4])/2.0f;
                }
                else{
                    Target.speedModifier = 2.0f/(2.0f - Target.modStages[4]);
                }
                yield return ShowDialog("SPEED UP!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_ACC_UP:{
                Target.modStages[5] = Mathf.Min(6, Target.modStages[5] + 2);
                if (Target.modStages[5] >= 1)
                {
                    Target.accuracyModifier = (3.0f + Target.modStages[5])/3.0f;
                }
                else{
                    Target.accuracyModifier = 3.0f/(3.0f - Target.modStages[5]);
                }
                yield return ShowDialog("ACCURACY UP!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_EVASION_UP:{
                Target.modStages[6] = Mathf.Min(6, Target.modStages[6] + 2);
                if (Target.modStages[6] >= 1)
                {
                    Target.evasion = (3.0f + Target.modStages[6])/3.0f;
                }
                else{
                    Target.evasion = 3.0f/(3.0f - Target.modStages[6]);
                }
                yield return ShowDialog("EVASION UP!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_ATK_DOWN:{
                Target.modStages[0] = Mathf.Max(-6, Target.modStages[0] - 2);
                if (Target.modStages[0] >= 1)
                {
                    Target.attackModifier = (2.0f + Target.modStages[0])/2.0f;
                }
                else{
                    Target.attackModifier = 2.0f/(2.0f - Target.modStages[0]);
                }
                yield return ShowDialog("ATK DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_DEF_DOWN:{
                Target.modStages[1] = Mathf.Max(-6, Target.modStages[1] - 2);
                if (Target.modStages[1] >= 1)
                {
                    Target.defenceModifier = (2.0f + Target.modStages[1])/2.0f;
                }
                else{
                    Target.defenceModifier = 2.0f/(2.0f - Target.modStages[1]);
                }
                yield return ShowDialog("DEF DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_SPATK_DOWN:{
                Target.modStages[2] = Mathf.Max(-6, Target.modStages[2] - 2);
                if (Target.modStages[2] >= 1)
                {
                    Target.special_attackModifier = (2.0f + Target.modStages[2])/2.0f;
                }
                else{
                    Target.special_attackModifier = 2.0f/(2.0f - Target.modStages[2]);
                }
                yield return ShowDialog("SPATK DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_SPDEF_DOWN:{
                Target.modStages[3] = Mathf.Max(-6, Target.modStages[3] - 2);
                if (Target.modStages[3] >= 1)
                {
                    Target.special_defenceModifier = (2.0f + Target.modStages[3])/2.0f;
                }
                else{
                    Target.special_defenceModifier = 2.0f/(2.0f - Target.modStages[3]);
                }
                yield return ShowDialog("SPDEF DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_SPEED_DOWN:{
                Target.modStages[4] = Mathf.Max(-6, Target.modStages[4] - 2);
                if (Target.modStages[4] >= 1)
                {
                    Target.speedModifier = (2.0f + Target.modStages[4])/2.0f;
                }
                else{
                    Target.speedModifier = 2.0f/(2.0f - Target.modStages[4]);
                }
                yield return ShowDialog("SPEED DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_ACC_DOWN:{
                Target.modStages[5] = Mathf.Max(-6, Target.modStages[5] - 2);
                if (Target.modStages[5] >= 1)
                {
                    Target.accuracyModifier = (3.0f + Target.modStages[5])/3.0f;
                }
                else{
                    Target.accuracyModifier = 3.0f/(3.0f - Target.modStages[5]);
                }
                yield return ShowDialog("ACCURACY DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.STRONG_EVASION_DOWN:{
                Target.modStages[6] = Mathf.Max(-6, Target.modStages[6] - 2);
                if (Target.modStages[6] >= 1)
                {
                    Target.evasion = (3.0f + Target.modStages[6])/3.0f;
                }
                else{
                    Target.evasion = 3.0f/(3.0f - Target.modStages[6]);
                }
                yield return ShowDialog("EVASION DOWN!", skipTime);
                break;
            }
            case Skill.EFFECT.POISON:
                if(Target.type != BaseStats.TYPE.POISON){
                    Target.statusCondition = Unit.STATUS_CONDITION.POISON;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("POISONED!", skipTime);
                }
                break;
            
            case Skill.EFFECT.LOWPOISON:
                if(Target.type != BaseStats.TYPE.POISON){
                    if(r < 10){ // 10% de chance de causar o efeito
                        Target.statusCondition = Unit.STATUS_CONDITION.POISON;
                        Target.HUD.SetStatusConditions();
                        yield return ShowDialog("POISONED!", skipTime);
                    }
                }
                break;
            
            case Skill.EFFECT.HIGHPOISON:
                if(Target.type != BaseStats.TYPE.POISON){
                    if(r < 30){ // 30% de chance de causar o efeito
                        Target.statusCondition = Unit.STATUS_CONDITION.POISON;
                        Target.HUD.SetStatusConditions();
                        yield return ShowDialog("POISONED!", skipTime);
                    }
                }
                break;

            case Skill.EFFECT.HEALPOISON:
                if(Target.statusCondition == Unit.STATUS_CONDITION.POISON){
                    Target.statusCondition = Unit.STATUS_CONDITION.NULL;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("Poison cured!", skipTime);
                }
                else
                    yield return ShowDialog("It did nothing.", skipTime);
                break;
            
            case Skill.EFFECT.BURN:
                if(Target.type != BaseStats.TYPE.FIRE){
                    Target.statusCondition = Unit.STATUS_CONDITION.BURN;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("BURNED!", skipTime);
                }
                break;
            
            case Skill.EFFECT.LOWBURN:
                if(Target.type != BaseStats.TYPE.FIRE){
                    if(r < 10){ // 10% de chance de causar o efeito
                        Target.statusCondition = Unit.STATUS_CONDITION.BURN;
                        Target.HUD.SetStatusConditions();
                        yield return ShowDialog("BURNED!", skipTime);
                    }
                }
                break;
            
            case Skill.EFFECT.HIGHBURN:
                if(Target.type != BaseStats.TYPE.FIRE){
                    if(r < 30){ // 30% de chance de causar o efeito
                        Target.statusCondition = Unit.STATUS_CONDITION.BURN;
                        Target.HUD.SetStatusConditions();
                        yield return ShowDialog("BURNED!", skipTime);
                    }
                }
                break;

            case Skill.EFFECT.HEALBURN:
                if(Target.statusCondition == Unit.STATUS_CONDITION.BURN){
                    Target.statusCondition = Unit.STATUS_CONDITION.NULL;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("Burn cured!", skipTime);
                }
                else
                    yield return ShowDialog("It did nothing.", skipTime);
                break;
            
            case Skill.EFFECT.RAGE:
                Target.statusCondition = Unit.STATUS_CONDITION.RAGE;
                Target.HUD.SetStatusConditions();
                yield return ShowDialog("ENRAGED!", skipTime);
                break;
            
            case Skill.EFFECT.LOWRAGE:
                if(r < 10){ // 10% de chance de causar o efeito
                    Target.statusCondition = Unit.STATUS_CONDITION.RAGE;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("ENRAGED!", skipTime);
                }
                break;
            
            case Skill.EFFECT.HIGHRAGE:
                if(r < 30){ // 30% de chance de causar o efeito
                    Target.statusCondition = Unit.STATUS_CONDITION.RAGE;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("ENRAGED!", skipTime);
                }
                break;

            case Skill.EFFECT.HEALRAGE:
                if(Target.statusCondition == Unit.STATUS_CONDITION.RAGE){
                    Target.statusCondition = Unit.STATUS_CONDITION.NULL;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("Rage cured!", skipTime);
                }
                else
                    yield return ShowDialog("It did nothing.", skipTime);
                break;

            case Skill.EFFECT.FREEZE:
                if(Target.type != BaseStats.TYPE.ICE){
                    Target.statusCondition = Unit.STATUS_CONDITION.FREEZE;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("FROZEN!", skipTime);
                }
                break;
            
            case Skill.EFFECT.LOWFREEZE:
                if(Target.type != BaseStats.TYPE.ICE){
                    if(r < 10){ // 10% de chance de causar o efeito
                        Target.statusCondition = Unit.STATUS_CONDITION.FREEZE;
                        Target.HUD.SetStatusConditions();
                        yield return ShowDialog("FROZEN!", skipTime);
                    }
                }
                break;
            
            case Skill.EFFECT.HIGHFREEZE:
                if(Target.type != BaseStats.TYPE.ICE){
                    if(r < 30){ // 30% de chance de causar o efeito
                        Target.statusCondition = Unit.STATUS_CONDITION.FREEZE;
                        Target.HUD.SetStatusConditions();
                        yield return ShowDialog("FROZEN!", skipTime);
                    }
                }
                break;

            case Skill.EFFECT.HEALFREEZE:
                if(Target.statusCondition == Unit.STATUS_CONDITION.FREEZE){
                    Target.statusCondition = Unit.STATUS_CONDITION.NULL;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("Freeze cured!", skipTime);
                }
                else
                    yield return ShowDialog("It did nothing.", skipTime);
                break;

            case Skill.EFFECT.PARALYSIS:
                Target.statusCondition = Unit.STATUS_CONDITION.PARALYSIS;
                Target.HUD.SetStatusConditions();
                yield return ShowDialog("PARALYSED!", skipTime);
                break;
            
            case Skill.EFFECT.LOWPARALYSIS:
                if(r < 10){ // 10% de chance de causar o efeito
                    Target.statusCondition = Unit.STATUS_CONDITION.PARALYSIS;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("PARALYSED!", skipTime);
                }
                break;
            
            case Skill.EFFECT.HIGHPARALYSIS:
                if(r < 30){ // 30% de chance de causar o efeito
                    Target.statusCondition = Unit.STATUS_CONDITION.PARALYSIS;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("PARALYSED!", skipTime);
                }
                break;

            case Skill.EFFECT.HEALPARALYSIS:
                if(Target.statusCondition == Unit.STATUS_CONDITION.PARALYSIS){
                    Target.statusCondition = Unit.STATUS_CONDITION.NULL;
                    Target.HUD.SetStatusConditions();
                    yield return ShowDialog("Paralysis cured!", skipTime);
                }
                else
                    yield return ShowDialog("It did nothing.", skipTime);
                break;


            default:
            break;
        }
        Target.ShowStatusMods();
        Target.HUD.SetStatusConditions();
    }
    public void InitEnemyPartyRef(List<Unit> Enemies){
        foreach (Unit e in Enemies)
        {
            if(!e.isDead){
                enemyPartyRef.Add(e);
            }
        }
    }
}
