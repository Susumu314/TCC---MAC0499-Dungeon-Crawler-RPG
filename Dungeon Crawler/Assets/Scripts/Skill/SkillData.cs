using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/**
* Classe que guarda os valores dos atributos base de todos os demônios do jogos
* Os Atributos de cada demônio do jogo são calculados a partir de seus niveis e de seus atributos base
*/
public class Skill {
    public enum PRIORITY {LOW, NORMAL, HIGH, MAX};
    public enum TARGET_TYPE {SINGLE, ROW, PARTY, SINGLE_ALLY, ALLY_ROW, ALLY_PARTY, SELF};
    public enum EFFECT {NULL, ATK_UP, ATK_DOWN, DEF_UP, DEF_DOWN,
                        SPATK_UP, SPATK_DOWN, SPDEF_UP, SPDEF_DOWN,
                        SPEED_UP, SPEED_DOWN, ACC_UP, ACC_DOWN, EVASION_UP, EVASION_DOWN,
                        POISON, BURN, FREEZE, PARALYSIS, LIFESTEAL}
    public struct SkillData{
        private readonly string name;
        private readonly BaseStats.TYPE type;
        private readonly TARGET_TYPE target_type;
        private readonly PRIORITY priority;
        private readonly int power;
        private readonly int accuracy;
        private readonly int cost;
        private readonly EFFECT effect;
        private readonly bool isSpecial;
        private readonly bool isRanged;
        private readonly int skill_ID;
        private readonly string vfx;
        private readonly Color vfx_color;
        private readonly string description;
        public SkillData (string name, BaseStats.TYPE type, TARGET_TYPE target_type, PRIORITY priority, int power, int accuracy, int cost,
                          EFFECT effect, bool isSpecial, bool isRanged, int skill_ID, string vfx, Color vfx_color, string description)
        {
            this.name = name;
            this.type = type;
            this.target_type = target_type;
            this.priority = priority;
            this.power = power;
            this.accuracy = accuracy;
            this.cost = cost;
            this.effect = effect;
            this.isSpecial = isSpecial;
            this.isRanged = isRanged;
            this.skill_ID = skill_ID;
            this.vfx = vfx;
            this.vfx_color = vfx_color;
            this.description = description;
        }
        public string Name { get { return name; } }
        public BaseStats.TYPE Type { get { return type; } }
        public TARGET_TYPE Target_type { get { return target_type; } }
        public PRIORITY Priority { get { return priority; } }
        public int Power { get { return power; } }
        public int Accuracy { get { return accuracy; } }
        public int Cost { get { return cost; } }
        public EFFECT Effect { get { return effect; } }
        public bool IsSpecial { get { return isSpecial; } }
        public bool IsRanged { get { return isRanged; } }
        public string VFX { get { return vfx; } }
        public Color COLOR { get { return vfx_color; } }
        public string DESC { get { return description; } }
    }
    
    public static readonly IList<SkillData> SkillList= new ReadOnlyCollection<SkillData>
        (new[] {
            new SkillData (/*name*/        "Tackle", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        60, 
                           /*accuracy*/     95, 
                           /*cost*/         10,
                           /*effect*/       EFFECT.NULL, 
                           /*isSpecial*/    false, 
                           /*isRanged*/     false,
                           /*ID*/           0,
                           /*VFX*/          "Punch",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "A full-body charge attack."),

            new SkillData (/*name*/        "Heal", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -40, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           1,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Healing Spell that recovers HP of the target."),

            new SkillData (/*name*/        "Ember", 
                           /*type*/         BaseStats.TYPE.FIRE, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        60, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.BURN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           2,
                           /*VFX*/          "Small_Flame",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "A weak fire attack that may inflict a burn."),
            
            new SkillData (/*name*/        "H_Slash", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        50, 
                           /*accuracy*/     100, 
                           /*cost*/         20,
                           /*effect*/       EFFECT.NULL, 
                           /*isSpecial*/    false, 
                           /*isRanged*/     false,
                           /*ID*/           3,
                           /*VFX*/          "Slash",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Slashes horizontally with claws, etc. Dealing damage to an entire row."),
            
            new SkillData (/*name*/        "H_Heal", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ALLY_ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -20, 
                           /*accuracy*/     100, 
                           /*cost*/         9,
                           /*effect*/       EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           4,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Healing Spell that recovers HP of an entire row."),
            
            new SkillData (/*name*/        "Scratch", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        60, 
                           /*accuracy*/     100, 
                           /*cost*/         10,
                           /*effect*/       EFFECT.NULL, 
                           /*isSpecial*/    false, 
                           /*isRanged*/     false,
                           /*ID*/           5,
                           /*VFX*/          "Slash",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Scratches the foe with sharp claws."),
            
            new SkillData (/*name*/        "Absorb", 
                           /*type*/         BaseStats.TYPE.GRASS, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        30, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.LIFESTEAL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           6,
                           /*VFX*/          "Absorb",
                           /*VFX_COLOR*/    new Color(0, 1, 0.93f, 1),
                           /*DESC*/         "An attack that absorbs half the damage inflicted."),
            
            new SkillData (/*name*/        "Water Gun", 
                           /*type*/         BaseStats.TYPE.WATER, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        60, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           7,
                           /*VFX*/          "Burst",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Squirts water to attack the target."),

            new SkillData (/*name*/        "Fight Stance", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SELF, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*effect*/       EFFECT.ATK_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           8,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Assumes a serious fighting stance increasing it's Attack."),
            
            new SkillData (/*name*/        "Encourage", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.ATK_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           9,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Encourage an ally boosting their Attack."),
            
            new SkillData (/*name*/        "Growl", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.ATK_DOWN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           10,
                           /*VFX*/          "Debuff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Growls cutely to reduce the target's Attack."),
            
            new SkillData (/*name*/        "Guard Stance", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SELF, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*effect*/       EFFECT.DEF_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           11,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Raise it's guard increasing the user Defence."),
            
            new SkillData (/*name*/        "Alert", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.DEF_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           12,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Alert an ally boosting their Defence."),
            
            new SkillData (/*name*/        "Leer", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.DEF_DOWN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           13,
                           /*VFX*/          "Debuff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Gives the target an intimidating leer that lowers it's Defense."),

            new SkillData (/*name*/        "Concentrate", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SELF, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*effect*/       EFFECT.SPATK_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           14,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Quietly focuses its mind to raise its Sp. Atk."),
            
            new SkillData (/*name*/        "Acupressure", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.SPATK_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           15,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "The user applies pressure to stress points boosting its target Sp.Atk stat."),
            
            new SkillData (/*name*/        "Mock", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.SPATK_DOWN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           16,
                           /*VFX*/          "Debuff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Mocks the target making it lose focus lowering it's Sp. Atk."),
            
            new SkillData (/*name*/        "Meditate", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SELF, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*effect*/       EFFECT.SPDEF_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           17,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Quietly calms its spirit to raise its Sp. Def stats."),
            
            new SkillData (/*name*/        "Aromatic Mist", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.SPDEF_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           18,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Raises the Sp. Def stat of an ally Pokémon by using a mysterious aroma."),
            
            new SkillData (/*name*/        "Fake Tears", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*effect*/       EFFECT.SPDEF_DOWN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           19,
                           /*VFX*/          "Debuff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Feigns crying to lower the target's Sp. Def."),
            
            new SkillData (/*name*/        "Agility", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SELF, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*effect*/       EFFECT.SPEED_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           20,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Relaxes the body to boost Speed."),
            
            new SkillData (/*name*/        "Motivate", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ALLY_ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         9,
                           /*effect*/       EFFECT.SPEED_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           21,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Boost target row moral raising its Speed."),
            
            new SkillData (/*name*/        "Sticky Web", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         9,
                           /*effect*/       EFFECT.SPEED_DOWN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           22,
                           /*VFX*/          "Debuff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Weaves a sticky net around the target row, which lowers their Speed stats."),
            
            new SkillData (/*name*/        "Double Team", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SELF, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*effect*/       EFFECT.EVASION_UP, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           23,
                           /*VFX*/          "Buff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Creates illusory copies to raise evasiveness."),
            
            new SkillData (/*name*/        "Sand Attack", 
                           /*type*/         BaseStats.TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        0, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*effect*/       EFFECT.ACC_DOWN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           24,
                           /*VFX*/          "Debuff",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Lowers accuracy of target by hurling sand in its face."),
            

        });
}