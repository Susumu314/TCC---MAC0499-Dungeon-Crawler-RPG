using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
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

    public PlayerBattleHUD HUD;
    private bool isInit = false;
    public int isBackLine = 0;
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
    public Unit(string species, string unitName, int unitLevel/*, int hp, int mp*/){
        this.species = species;
        this.unitName = unitName;
        this.unitLevel = unitLevel;
        InitStats();
        // this.currentHP = hp;
        // this.currentMana = mp;
    }

    void Awake(){
        skillList = new int[4];
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
        if(isSpecial){
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

    public void BattleSystemReference(Battle_System b){
        BS = b;
    }

    public void SetGuard(bool guard){ // setter para definir se o personagem esta defendendo
        OnGuard = guard;
        if(OnGuard)
            print(unitName + ": I'm On Guard\n");
    }

    public void Escape(){
        print(unitName + ": Trying to escape battle\n");
    }

    public void Dead(){
        isDead = true;
        this.transform.GetChild(0).gameObject.SetActive(false);//desabilita o sprite
        //incrementa o expEarned no Battle_System se for uma unidade inimiga
        if(!isPlayerUnit){
            BS.expEarned += xp_yield*unitLevel/7;
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
        skillList[0] = 0;
        skillList[1] = 1;
        skillList[2] = 4;
        skillList[3] = 3;
        
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
                baseExp = 5/4;
            break;
            case BaseStats.GROWTH_RATE.MEDIUM_SLOW:
                baseExp = 6/5;
            break;
            case BaseStats.GROWTH_RATE.MEDIUM:
                baseExp = 1;
            break;
            case BaseStats.GROWTH_RATE.FAST:
                baseExp = 4/5;
            break;
            default:
                baseExp = 1;
            break;
        }
        totalExp = Mathf.CeilToInt(baseExp*Mathf.Pow(unitLevel,exponent));
        expForNextLevel = Mathf.CeilToInt(baseExp*Mathf.Pow(unitLevel + 1,exponent));
        isInit = true;
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
    public IEnumerator Capture(float bonus = 10){
        if (isPlayerUnit)
        {
            yield return null;
        }
        else{
            int rng = UnityEngine.Random.Range(0, 255);
            int a = Mathf.CeilToInt((3*maxHP - 2*currentHP)*catch_rate*bonus/(10*3*maxHP)); //quando a vida está cheia, a chance de captura é de apenas 1/3 e com 1 de vida, a chance é 1
            Debug.Log(a);
            Debug.Log(rng);
            if(a > rng){
                GameManager.Instance.CapturedUnit(this);
                BS.dialogueText.text = "You tamed " + unitName + "!";
                GameObject Anim = Instantiate(Resources.Load("VFX/Skill_Animation"), transform.position, Quaternion.identity) as GameObject; 
                Animator animator = Anim.GetComponent<Animator>();
                Anim.GetComponent<SpriteRenderer>().color =  new Color(0.25f, 0.75f, 0.96f, 1f);
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
}

