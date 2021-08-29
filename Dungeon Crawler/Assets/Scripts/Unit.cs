using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public enum STATUS_CONDITION {NULL, POISON, BURN, FREEZE, PARALYSIS};
    public STATUS_CONDITION statusCondition;
    public int[] modStages = new int[7]{0,0,0,0,0,0,0};
    public float attackModifier;
    public float defenceModifier;
    public float special_attackModifier;
    public float special_defenceModifier;
    public float speedModifier;
    public float accuracyModifier;
    public float evasion; //no inicio das lutas sempre começa em 1, age como um dos statsMods
    public int MAX_SKILL_NUMBER;
    public string species;
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHP;
    public int currentHP;

    public int maxMana;
    public int currentMana;

    public int attack;
    public int defence;
    public int special_attack;
    public int special_defence;
    public int speed;

    public BaseStats.TYPE type;

    public int catch_rate;
    public int xp_yield;
    public BaseStats.GROWTH_RATE growth_rate;

    public bool isPlayerUnit;

    public bool isDead = false;

    public BattleAction Move;
    public EnemyBattleAI AI; //used only for NonPlayableUnits

    private Battle_System BS;

    private OW_MenuSystem MS;

    public PlayerBattleHUD HUD;
    private bool isInit = false;
    public int isBackLine = 0; //0 is frontline, 1 is backline
    public int totalExp = 0;
    private int expForNextLevel;
    private BaseStats.DemonStats stats;
    private bool OnGuard = false; // devo resetar isso alguma hora depois do BattleTurn,
                                 // provavelmente quando o personagem for selecionado para escolher a sua proxima ação
    /**
    * Cria uma unidade com vida e mana atual igual a hp e mp
    */
    private float baseExp;
    private float exponent;
    public int[] skillList;// por enquanto cada unidade tem 4 skills, essa lista contem os ID de cada skill
    private List<MovePool.LevelUp_Move> movePool;
    private Unit switchTarget;
    public Unit(string species, string unitName, int unitLevel/*, int hp, int mp*/){
        this.species = species;
        this.unitName = unitName;
        this.unitLevel = unitLevel;
        InitStats();
        // this.currentHP = hp;
        // this.currentMana = mp;
    }

    void Awake(){
        MAX_SKILL_NUMBER = 4;
        skillList = new int[MAX_SKILL_NUMBER];
        for (int i = 0; i < MAX_SKILL_NUMBER; i++)
        {
            skillList[i] = -1;
        }
    }
    void Update(){
    }
    public void TakeDamage(int d){
        int damage;
        if (!OnGuard){
            damage = d;
        }
        else{
            damage = Mathf.CeilToInt(d/2f);
        }
        StartCoroutine(HUD.SetHP(Math.Max(0, currentHP - damage)));
        GameObject damagePopup = Instantiate(Resources.Load("VFX/DamagePopup"), HUD.transform.position, Quaternion.identity) as GameObject;
        damagePopup.GetComponent<DamagePopup>().Setup(damage);
        if(isPlayerUnit){//se for unidade do player, tremer a camera, se for do inimigo, tremer o inimigo (acho que na vdd eh melhor tremer o canvas)
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ShakeScript>().TriggerShake();
        }
        else{
            this.GetComponent<ShakeScript>().TriggerShake();
        }
        if (currentHP == 0){
            Dead();
        }
    }

    public void HealDamage(int d){
        int damage = d;
        StartCoroutine(HUD.SetHP(Math.Min(currentHP + damage, maxHP)));
        GameObject damagePopup = Instantiate(Resources.Load("VFX/HealPopup"), HUD.transform.position, Quaternion.identity) as GameObject;
        damagePopup.GetComponent<DamagePopup>().Setup(damage);
    }

    /**
    * Gasta mana ou vida para utilizar habilidades,
    * 
    * @param c Custo para utilizar uma habilidade
    * @param isSpecial se a habilidade é especial ou física
    */
    public bool PaySkillCost(int c, bool isSpecial){
        if(isSpecial && isPlayerUnit){
            if(currentMana < c){
                return false;
            }
            HUD.SetMP(currentMana - c);
        }
        else{
            //o custo de HP para realizar ataques é uma porcentagem fixa da vida maxima
            int HPcost = (maxHP*c)/100;
            if(HPcost == 0){
                HPcost = 1;
            }
            if(currentHP < HPcost){
                return false;
            }
            StartCoroutine(HUD.SetHP(currentHP - HPcost));
        }
        return true;
    }

    /**
    * Procura um item com ID "itemID" na mochila, e o utiliza se existir
    * 
    */
    public bool PayItemCost(int itemID){
        if(MS == null){
            return BS.bag.UseItem(itemID);
        }
        return MS.bag.UseItem(itemID);
    }

    public void BattleSystemReference(Battle_System b){
        BS = b;
        Move.InitBattleSystemRef(b);
    }

    public void MenuSystemReference(OW_MenuSystem m){
        MS = m;
        Move.InitMenuSystemRef(m);
    }

    public void SetGuard(bool guard){ // setter para definir se o personagem esta defendendo
        OnGuard = guard;
        if(OnGuard)
            print(unitName + ": I'm On Guard\n");
    }
    /**
    *   Faz o calculo para determinar se o grupo consegue fugir da luta ou não
    */
    public bool Escape(){
        int B;
        if(speed < BS.fastestEnemySpeed*0.75f){
            B = 0;
        }
        else if(speed > BS.fastestEnemySpeed*1.25f){
            B = 20;
        }
        else{
            B = 5;
        }
        int T = BS.turno;
        int R = T*UnityEngine.Random.Range(T, 2*T+1);
        int D = 6* BS.Allies_Deaths;
        if(UnityEngine.Random.Range(0,100) < B+R+D){
            return true;
        }
        return false;
    }

    public void Dead(){
        isDead = true;
        this.transform.GetChild(0).gameObject.SetActive(false);//desabilita o sprite
        //incrementa o expEarned no Battle_System se for uma unidade inimiga
        if(!isPlayerUnit){
            BS.expEarned += xp_yield*unitLevel/7;
        }
        else{
            BS.Allies_Deaths += 1;
        }
        //avisa o Battle_System que uma unidade morreu para checar o fim da batalha
        BS.TestBattleEnd();
    }

    /**
    * Inicializa as variáveis necessárias para inicialização dos status de uma inimiga no inicio de um combate
    */
    public void InitEnemyUnit(string s, int lvl, int backline){
        species = s;
        unitLevel = lvl;
        unitName = s;
        isBackLine = backline;
    }

    public void InitStats(){
        if(isInit){
            return;
        }
        // setando skills para teste
        movePool = MovePool.SearchMovePool(species);
        int m = 0;
        foreach (MovePool.LevelUp_Move move in movePool)
        {
            if(move.Level <= unitLevel){
                skillList[m%MAX_SKILL_NUMBER] = move.SkillID;
                m++;
            }
        }
        
        this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Demon Sprites/" + species);
        stats = BaseStats.SearchDex(species);

        maxHP = Mathf.FloorToInt(3*stats.Hp * unitLevel/100) + unitLevel + 10;
        currentHP = maxHP;

        maxMana = Mathf.FloorToInt(3*stats.Mp * unitLevel/100) + unitLevel + 10;
        currentMana = maxMana;

        attack = Mathf.FloorToInt(3*stats.atk * unitLevel/100) + 5;
        defence = Mathf.FloorToInt(3*stats.def * unitLevel/100) + 5;
        special_attack = Mathf.FloorToInt(3*stats.spatk * unitLevel/100) + 5;
        special_defence = Mathf.FloorToInt(3*stats.spdef * unitLevel/100) + 5;
        speed = Mathf.FloorToInt(3*stats.spd * unitLevel/100) + 5;
        type = stats.Type;
        xp_yield = stats.xpYield; //isso aqui tem que mudar
        growth_rate = stats.growthRate; 
        catch_rate = stats.catchRate;
        //calculo da exp
        exponent = 3;
        switch (growth_rate)
        {
            case BaseStats.GROWTH_RATE.SLOW:
                baseExp = 5f/4f;
            break;
            case BaseStats.GROWTH_RATE.MEDIUM_SLOW:
                baseExp = 6f/5f;
            break;
            case BaseStats.GROWTH_RATE.MEDIUM:
                baseExp = 1;
            break;
            case BaseStats.GROWTH_RATE.FAST:
                baseExp = 4f/5f;
            break;
            default:
                baseExp = 1;
            break;
        }
        totalExp = Mathf.CeilToInt(baseExp*Mathf.Pow(unitLevel,exponent));
        expForNextLevel = Mathf.CeilToInt(baseExp*Mathf.Pow(unitLevel + 1,exponent));
        isInit = true;
        statusCondition = STATUS_CONDITION.NULL;
    }

    public IEnumerator GainExp(int exp){//depois pensar num jeito de mostrar numa tela as mudanças de stats (provavelmente fazer toda parte de ganhar xp e upar em uma cena nova) 
        totalExp += exp;
        if (totalExp > expForNextLevel){
            LevelUp();
        }
        yield return null;
    }
    public void LevelUp(){ 
        unitLevel += 1;
        maxHP = Mathf.FloorToInt(3*stats.Hp * unitLevel/100) + unitLevel + 10;
        maxMana = Mathf.FloorToInt(3*stats.Mp * unitLevel/100) + unitLevel + 10;
        attack = Mathf.FloorToInt(3*stats.atk * unitLevel/100) + 5;
        defence = Mathf.FloorToInt(3*stats.def * unitLevel/100) + 5;
        special_attack = Mathf.FloorToInt(3*stats.spatk * unitLevel/100) + 5;
        special_defence = Mathf.FloorToInt(3*stats.spdef * unitLevel/100) + 5;
        speed = Mathf.FloorToInt(3*stats.spd * unitLevel/100) + 5;
        expForNextLevel = Mathf.CeilToInt(baseExp*Mathf.Pow(unitLevel + 1,exponent));
    }
    /**
    * Quando tentar capturar uma unidade selvagem, essa função determina se a captura foi efetiva ou nao
    * caso seja, transfere a unidade para equipe do jogador (se tiver espaço) ou para o banco
    */
    public IEnumerator Capture(int bonus, Color color){
        if (isPlayerUnit)
        {
            yield return null;
        }
        else{
            int rng = UnityEngine.Random.Range(0, 255);
            int a = Mathf.CeilToInt((3*maxHP - 2*currentHP)*catch_rate*bonus/(10*3*maxHP)); //quando a vida está cheia, a chance de captura é de apenas 1/3 e com 1 de vida, a chance é 1
            Debug.Log("a = "+ a);
            Debug.Log("rng = "+ rng);
            if(a > rng){
                GameManager.Instance.CapturedUnit(this);
                BS.dialogueText.text = "You tamed " + unitName + "!";
                GameObject Anim = Instantiate(Resources.Load("VFX/Skill_Animation"), transform.position, Quaternion.identity) as GameObject; 
                Animator animator = Anim.GetComponent<Animator>();
                Anim.GetComponent<SpriteRenderer>().color = color;
                animator.Play("Barrier");
                //Fetch the current Animation clip information for the base layer
                AnimatorClipInfo[] m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
                yield return new WaitForSeconds(m_CurrentClipInfo[0].clip.length);
                Dead();
            }
            else{
                BS.dialogueText.text = "You failed to tame " + unitName + "."; 
                yield return null;
            }
        }
    }

    /**
    * Deve ser chamada todo começo de batalha, reseta todos os modificadores de stats a 1
    */
    public void ResetStatMods(){
        modStages = new int[7]{0,0,0,0,0,0,0};
        attackModifier = 1;
        defenceModifier = 1;
        special_attackModifier = 1;
        special_defenceModifier = 1;
        speedModifier = 1;
        accuracyModifier = 1;
        evasion = 1;
    }
    public void ShowStatusMods(){
        HUD.SetStatusModText(modStages);
    }

    public void Switch(Unit target){
        int old_index = transform.GetSiblingIndex();
        int new_index = target.transform.GetSiblingIndex();
        transform.SetSiblingIndex(new_index);
        target.transform.SetSiblingIndex(old_index);
        MS.SetupGUI();
    }

    public void Switch(int targetPosition){
        int old_index = transform.GetSiblingIndex();
        Transform target = transform.parent.GetChild(targetPosition);
        transform.SetSiblingIndex(targetPosition);
        target.transform.SetSiblingIndex(old_index);
        MS.SetupGUI();
    }

    public void FullHeal(){
        Debug.Log("FullHeal");
        isDead = false;
        statusCondition = STATUS_CONDITION.NULL;
        int damage = maxHP - currentHP;
        currentHP = maxHP;
        currentMana = maxMana;
        //por algum motivo nao tocando nem o SFX ou VFX da cura
        GameObject Anim = Instantiate(Resources.Load("VFX/Skill_Animation"), HUD.transform.position, Quaternion.identity) as GameObject;
        Animator animator = Anim.GetComponent<Animator>();
        animator.Play("Heal");
        StartCoroutine(HUD.SetHP(currentHP));
        HUD.SetMP(currentMana);
    }
}

