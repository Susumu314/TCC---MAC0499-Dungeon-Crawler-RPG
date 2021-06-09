using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/**
* Classe que guarda os valores dos atributos base de todos os demônios do jogos
* Os Atributos de cada demônio do jogo são calculados a partir de seus niveis e de seus atributos base
*/
public class Skill {
    public enum TYPE {NORMAL, FIRE, GRASS, WATER, AIR, DARK, LIGHT, ICE, ROCK, POISON, DRAGON, BUG};
    public enum PRIORITY {LOW, NORMAL, HIGH, MAX};
    public enum TARGET_TYPE {SINGLE, ROW, PARTY, SINGLE_ALLY, ALLY_ROW, ALLY_PARTY};
    public enum STATUS_EFFECT {NULL, ATK_UP, ATK_DOWN, DEF_UP, DEF_DOWN, SPEED_UP, SPEED_DOWN,
                               POISON, BURN, FREEZE, PARALYSIS}
    public struct SkillData{
        private readonly string name;
        private readonly TYPE type;
        private readonly TARGET_TYPE target_type;
        private readonly PRIORITY priority;
        private readonly int power;
        private readonly int accuracy;
        private readonly int cost;
        private readonly STATUS_EFFECT status_effect;
        private readonly bool isSpecial;
        private readonly bool isRanged;
        private readonly int skill_ID;
        private readonly string vfx;
        private readonly Color vfx_color;
        private readonly string description;
        public SkillData (string name, TYPE type, TARGET_TYPE target_type, PRIORITY priority, int power, int accuracy, int cost,
                          STATUS_EFFECT status_effect, bool isSpecial, bool isRanged, int skill_ID, string vfx, Color vfx_color, string description)
        {
            this.name = name;
            this.type = type;
            this.target_type = target_type;
            this.priority = priority;
            this.power = power;
            this.accuracy = accuracy;
            this.cost = cost;
            this.status_effect = status_effect;
            this.isSpecial = isSpecial;
            this.isRanged = isRanged;
            this.skill_ID = skill_ID;
            this.vfx = vfx;
            this.vfx_color = vfx_color;
            this.description = description;
        }
        public string Name { get { return name; } }
        public TYPE Type { get { return type; } }
        public TARGET_TYPE Target_type { get { return target_type; } }
        public PRIORITY Priority { get { return priority; } }
        public int Power { get { return power; } }
        public int Accuracy { get { return accuracy; } }
        public int Cost { get { return cost; } }
        public STATUS_EFFECT Status_effect { get { return status_effect; } }
        public bool IsSpecial { get { return isSpecial; } }
        public bool IsRanged { get { return isRanged; } }
        public string VFX { get { return vfx; } }
        public Color COLOR { get { return vfx_color; } }
        public string DESC { get { return description; } }
    }
    
    public static readonly IList<SkillData> SkillList= new ReadOnlyCollection<SkillData>
        (new[] {
            new SkillData (/*name*/        "Tackle", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        60, 
                           /*accuracy*/     100, 
                           /*cost*/         2,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    false, 
                           /*isRanged*/     false,
                           /*ID*/           0,
                           /*VFX*/          "Punch",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "A full-body charge attack."),

            new SkillData (/*name*/        "Heal", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -40, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           1,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Healing Spell that recovers HP of the target."),

            new SkillData (/*name*/        "Ember", 
                           /*type*/         TYPE.FIRE, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        50, 
                           /*accuracy*/     100, 
                           /*cost*/         6,
                           /*status_effect*/STATUS_EFFECT.BURN, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           2,
                           /*VFX*/          "Flame",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "A weak fire attack that may inflict a burn."),
            
            new SkillData (/*name*/        "H_Slash", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        50, 
                           /*accuracy*/     100, 
                           /*cost*/         20,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    false, 
                           /*isRanged*/     false,
                           /*ID*/           3,
                           /*VFX*/          "Slash",
                           /*VFX_COLOR*/    Color.white,
                           /*DESC*/         "Slashes horizontally with claws, etc. Dealing damage to an entire row"),
            
            new SkillData (/*name*/        "H_Heal", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ALLY_ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -20, 
                           /*accuracy*/     100, 
                           /*cost*/         9,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           4,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Healing Spell that recovers HP of an entire row"),
        });
}