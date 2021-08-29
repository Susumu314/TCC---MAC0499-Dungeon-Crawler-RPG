using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat_Screen : MonoBehaviour
{
    public Text NAME;
    public Text SPECIES;
    public Text LVL;
    public Text HP;
    public Text MP;
    public Text ATK;
    public Text SPATK;
    public Text DEF;
    public Text SPDEF;
    public Text SPEED;
    public Text TYPE;
    public Text SKILL1;
    public Text SKILL2;
    public Text SKILL3;
    public Text SKILL4;
    public Image SPRITE;

    public void UpdateStatScreen(Unit u){
        NAME.text = u.name;
        SPECIES.text = u.species;
        LVL.text = "" + u.unitLevel;
        HP.text = "" + u.maxHP;
        MP.text = "" + u.maxMana;
        ATK.text = "" + u.attack;
        SPATK.text = "" + u.special_attack;
        DEF.text = "" + u.defence;
        SPDEF.text = "" + u.special_defence;
        SPEED.text = "" + u.speed;
        TYPE.text = u.type.ToString();
        SKILL1.text = Skill.SkillList[u.skillList[0]].Name;
        SKILL2.text = Skill.SkillList[u.skillList[1]].Name;
        SKILL3.text = Skill.SkillList[u.skillList[2]].Name;
        SKILL4.text = Skill.SkillList[u.skillList[3]].Name;
        SPRITE.sprite = Resources.Load<Sprite>("Demon Sprites/" + u.species);
    }
}
