using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class MovePool
{
    public struct LevelUp_Move{
        private readonly int level;
        private readonly int skillID;
        public LevelUp_Move (int level, int skillID){
            this.level = level;
            this.skillID = skillID;
        }
        public int Level { get { return level; } }
        public int SkillID { get { return skillID; } }
    }

    public struct Demon_MovePool{
        private readonly string species;
        private List<LevelUp_Move> levelUpMoves;
        public Demon_MovePool (string species, List<LevelUp_Move> l){
            this.species = species;
            this.levelUpMoves = l;
        }
        public string Species { get { return species; } }
        public List<LevelUp_Move> LevelUpMoves { get { return levelUpMoves; } }
    }

    public static readonly IList<Demon_MovePool> MoveDex = new ReadOnlyCollection<Demon_MovePool>
    (new[] {
        new Demon_MovePool("Fire Snake", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/13),
                new LevelUp_Move(/*Level*/6, /*skillID*/2),
                new LevelUp_Move(/*Level*/8, /*skillID*/16),
                new LevelUp_Move(/*Level*/11, /*skillID*/8),
                new LevelUp_Move(/*Level*/15, /*skillID*/3),
                new LevelUp_Move(/*Level*/17, /*skillID*/20),
                new LevelUp_Move(/*Level*/20, /*skillID*/25)
                })),
        new Demon_MovePool("Fire Kindred Flame", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/13),
                new LevelUp_Move(/*Level*/6, /*skillID*/2),
                new LevelUp_Move(/*Level*/8, /*skillID*/16),
                new LevelUp_Move(/*Level*/11, /*skillID*/8),
                new LevelUp_Move(/*Level*/15, /*skillID*/3),
                new LevelUp_Move(/*Level*/17, /*skillID*/20),
                new LevelUp_Move(/*Level*/20, /*skillID*/25)
                })),
        new Demon_MovePool("Fire Dragonspawn", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/13),
                new LevelUp_Move(/*Level*/6, /*skillID*/2),
                new LevelUp_Move(/*Level*/8, /*skillID*/16),
                new LevelUp_Move(/*Level*/11, /*skillID*/8),
                new LevelUp_Move(/*Level*/15, /*skillID*/3),
                new LevelUp_Move(/*Level*/17, /*skillID*/20),
                new LevelUp_Move(/*Level*/20, /*skillID*/25)
                })),
        new Demon_MovePool("Carnivore Plant A", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/3, /*skillID*/1),
                new LevelUp_Move(/*Level*/5, /*skillID*/18),
                new LevelUp_Move(/*Level*/6, /*skillID*/6),
                new LevelUp_Move(/*Level*/9, /*skillID*/39),
                new LevelUp_Move(/*Level*/13, /*skillID*/4),
                new LevelUp_Move(/*Level*/16, /*skillID*/14),
                new LevelUp_Move(/*Level*/18, /*skillID*/26)
                })),
        new Demon_MovePool("Carnivore Plant B", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/3, /*skillID*/1),
                new LevelUp_Move(/*Level*/5, /*skillID*/18),
                new LevelUp_Move(/*Level*/6, /*skillID*/6),
                new LevelUp_Move(/*Level*/9, /*skillID*/39),
                new LevelUp_Move(/*Level*/13, /*skillID*/4),
                new LevelUp_Move(/*Level*/16, /*skillID*/14),
                new LevelUp_Move(/*Level*/18, /*skillID*/26)
                })),
        new Demon_MovePool("Carnivore Plant C", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/3, /*skillID*/1),
                new LevelUp_Move(/*Level*/5, /*skillID*/18),
                new LevelUp_Move(/*Level*/6, /*skillID*/6),
                new LevelUp_Move(/*Level*/9, /*skillID*/39),
                new LevelUp_Move(/*Level*/13, /*skillID*/4),
                new LevelUp_Move(/*Level*/16, /*skillID*/14),
                new LevelUp_Move(/*Level*/18, /*skillID*/26)
                })),
        new Demon_MovePool("Sea Crab", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/10),
                new LevelUp_Move(/*Level*/6, /*skillID*/7),
                new LevelUp_Move(/*Level*/8, /*skillID*/12),
                new LevelUp_Move(/*Level*/10, /*skillID*/11),
                new LevelUp_Move(/*Level*/12, /*skillID*/28),
                new LevelUp_Move(/*Level*/14, /*skillID*/9),
                new LevelUp_Move(/*Level*/18, /*skillID*/27)
                })),
        new Demon_MovePool("Spider Crab", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/10),
                new LevelUp_Move(/*Level*/6, /*skillID*/7),
                new LevelUp_Move(/*Level*/8, /*skillID*/12),
                new LevelUp_Move(/*Level*/10, /*skillID*/11),
                new LevelUp_Move(/*Level*/12, /*skillID*/28),
                new LevelUp_Move(/*Level*/14, /*skillID*/9),
                new LevelUp_Move(/*Level*/18, /*skillID*/27)
                })),
        new Demon_MovePool("Shell Claw", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/10),
                new LevelUp_Move(/*Level*/6, /*skillID*/7),
                new LevelUp_Move(/*Level*/8, /*skillID*/12),
                new LevelUp_Move(/*Level*/10, /*skillID*/11),
                new LevelUp_Move(/*Level*/12, /*skillID*/28),
                new LevelUp_Move(/*Level*/14, /*skillID*/9),
                new LevelUp_Move(/*Level*/18, /*skillID*/27)
                })),
        new Demon_MovePool("Rabbit Bandit", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/5),
                new LevelUp_Move(/*Level*/3, /*skillID*/8),
                new LevelUp_Move(/*Level*/5, /*skillID*/23),
                new LevelUp_Move(/*Level*/10, /*skillID*/3),
                new LevelUp_Move(/*Level*/11, /*skillID*/24),
                new LevelUp_Move(/*Level*/15, /*skillID*/34),
                new LevelUp_Move(/*Level*/17, /*skillID*/20)
                })),
        new Demon_MovePool("Rabbit Knight", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/5),
                new LevelUp_Move(/*Level*/3, /*skillID*/8),
                new LevelUp_Move(/*Level*/5, /*skillID*/23),
                new LevelUp_Move(/*Level*/10, /*skillID*/3),
                new LevelUp_Move(/*Level*/11, /*skillID*/24),
                new LevelUp_Move(/*Level*/15, /*skillID*/34),
                new LevelUp_Move(/*Level*/17, /*skillID*/20)
                })),

        new Demon_MovePool("Ice Spike", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/13),
                new LevelUp_Move(/*Level*/5, /*skillID*/28),
                new LevelUp_Move(/*Level*/7, /*skillID*/16),
                new LevelUp_Move(/*Level*/9, /*skillID*/8),
                new LevelUp_Move(/*Level*/12, /*skillID*/3),
                new LevelUp_Move(/*Level*/15, /*skillID*/20),
                new LevelUp_Move(/*Level*/18, /*skillID*/29)
                })),
        new Demon_MovePool("Ice Kindred Glacier", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/13),
                new LevelUp_Move(/*Level*/5, /*skillID*/28),
                new LevelUp_Move(/*Level*/7, /*skillID*/16),
                new LevelUp_Move(/*Level*/9, /*skillID*/8),
                new LevelUp_Move(/*Level*/12, /*skillID*/3),
                new LevelUp_Move(/*Level*/15, /*skillID*/20),
                new LevelUp_Move(/*Level*/18, /*skillID*/29)
                })),
        new Demon_MovePool("Ice Ogre", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/13),
                new LevelUp_Move(/*Level*/5, /*skillID*/28),
                new LevelUp_Move(/*Level*/7, /*skillID*/16),
                new LevelUp_Move(/*Level*/9, /*skillID*/8),
                new LevelUp_Move(/*Level*/12, /*skillID*/3),
                new LevelUp_Move(/*Level*/15, /*skillID*/20),
                new LevelUp_Move(/*Level*/18, /*skillID*/29)
                })),
        
        new Demon_MovePool("Ice Ball", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/19),
                new LevelUp_Move(/*Level*/6, /*skillID*/28),
                new LevelUp_Move(/*Level*/8, /*skillID*/10),
                new LevelUp_Move(/*Level*/10, /*skillID*/11),
                new LevelUp_Move(/*Level*/15, /*skillID*/29),
                new LevelUp_Move(/*Level*/18, /*skillID*/3)
                })),
        new Demon_MovePool("Snobros", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/19),
                new LevelUp_Move(/*Level*/6, /*skillID*/28),
                new LevelUp_Move(/*Level*/8, /*skillID*/10),
                new LevelUp_Move(/*Level*/10, /*skillID*/11),
                new LevelUp_Move(/*Level*/15, /*skillID*/29),
                new LevelUp_Move(/*Level*/18, /*skillID*/3)
                })),
        new Demon_MovePool("Snowman", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/3, /*skillID*/19),
                new LevelUp_Move(/*Level*/6, /*skillID*/28),
                new LevelUp_Move(/*Level*/8, /*skillID*/10),
                new LevelUp_Move(/*Level*/10, /*skillID*/11),
                new LevelUp_Move(/*Level*/15, /*skillID*/29),
                new LevelUp_Move(/*Level*/18, /*skillID*/3)
                })),
        
        new Demon_MovePool("Seagull", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/30),
                new LevelUp_Move(/*Level*/4, /*skillID*/23),
                new LevelUp_Move(/*Level*/6, /*skillID*/16),
                new LevelUp_Move(/*Level*/8, /*skillID*/7),
                new LevelUp_Move(/*Level*/12, /*skillID*/31),
                new LevelUp_Move(/*Level*/15, /*skillID*/8),
                new LevelUp_Move(/*Level*/18, /*skillID*/29)
                })),
        new Demon_MovePool("Pellican", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/30),
                new LevelUp_Move(/*Level*/4, /*skillID*/23),
                new LevelUp_Move(/*Level*/6, /*skillID*/16),
                new LevelUp_Move(/*Level*/8, /*skillID*/7),
                new LevelUp_Move(/*Level*/12, /*skillID*/31),
                new LevelUp_Move(/*Level*/15, /*skillID*/8),
                new LevelUp_Move(/*Level*/18, /*skillID*/29)
                })),
        
        new Demon_MovePool("Octi", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/0),
                new LevelUp_Move(/*Level*/4, /*skillID*/17),
                new LevelUp_Move(/*Level*/6, /*skillID*/19),
                new LevelUp_Move(/*Level*/8, /*skillID*/7),
                new LevelUp_Move(/*Level*/10, /*skillID*/14),
                new LevelUp_Move(/*Level*/15, /*skillID*/27),
                new LevelUp_Move(/*Level*/18, /*skillID*/22),
                new LevelUp_Move(/*Level*/20, /*skillID*/40)
                })),
        new Demon_MovePool("Octopus", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/0),
                new LevelUp_Move(/*Level*/4, /*skillID*/17),
                new LevelUp_Move(/*Level*/6, /*skillID*/19),
                new LevelUp_Move(/*Level*/8, /*skillID*/7),
                new LevelUp_Move(/*Level*/10, /*skillID*/14),
                new LevelUp_Move(/*Level*/15, /*skillID*/27),
                new LevelUp_Move(/*Level*/18, /*skillID*/22),
                new LevelUp_Move(/*Level*/20, /*skillID*/40)
                })),
        
        new Demon_MovePool("Piranos", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/4, /*skillID*/33),
                new LevelUp_Move(/*Level*/7, /*skillID*/8),
                new LevelUp_Move(/*Level*/9, /*skillID*/16),
                new LevelUp_Move(/*Level*/12, /*skillID*/39),
                new LevelUp_Move(/*Level*/18, /*skillID*/27)
                })),
        new Demon_MovePool("Sea Shark", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/4, /*skillID*/33),
                new LevelUp_Move(/*Level*/7, /*skillID*/8),
                new LevelUp_Move(/*Level*/9, /*skillID*/16),
                new LevelUp_Move(/*Level*/12, /*skillID*/39),
                new LevelUp_Move(/*Level*/18, /*skillID*/27)
                })),
        
        new Demon_MovePool("Wind Bunny", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/18),
                new LevelUp_Move(/*Level*/3, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/31),
                new LevelUp_Move(/*Level*/9, /*skillID*/21),
                new LevelUp_Move(/*Level*/12, /*skillID*/4),
                new LevelUp_Move(/*Level*/15, /*skillID*/23),
                new LevelUp_Move(/*Level*/18, /*skillID*/29),
                new LevelUp_Move(/*Level*/20, /*skillID*/9)
                })),
        new Demon_MovePool("Wind Carbuncle", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/18),
                new LevelUp_Move(/*Level*/3, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/31),
                new LevelUp_Move(/*Level*/9, /*skillID*/21),
                new LevelUp_Move(/*Level*/12, /*skillID*/4),
                new LevelUp_Move(/*Level*/15, /*skillID*/23),
                new LevelUp_Move(/*Level*/18, /*skillID*/29),
                new LevelUp_Move(/*Level*/20, /*skillID*/9)
                })),
        
        new Demon_MovePool("Wind Fairy", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/30),
                new LevelUp_Move(/*Level*/4, /*skillID*/1),
                new LevelUp_Move(/*Level*/7, /*skillID*/35),
                new LevelUp_Move(/*Level*/10, /*skillID*/18),
                new LevelUp_Move(/*Level*/15, /*skillID*/32),
                new LevelUp_Move(/*Level*/17, /*skillID*/4),
                new LevelUp_Move(/*Level*/20, /*skillID*/36)
                })),
        new Demon_MovePool("Wind Harpy", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/2, /*skillID*/30),
                new LevelUp_Move(/*Level*/4, /*skillID*/1),
                new LevelUp_Move(/*Level*/7, /*skillID*/35),
                new LevelUp_Move(/*Level*/10, /*skillID*/18),
                new LevelUp_Move(/*Level*/15, /*skillID*/32),
                new LevelUp_Move(/*Level*/17, /*skillID*/4),
                new LevelUp_Move(/*Level*/20, /*skillID*/36)
                })),
        

    });

    /**
    * Retorna a lista de habilidades pelo nome da espécie do demônio
    */
    public static List<LevelUp_Move> SearchMovePool(string demonName){
        for (int i = 0; i < MoveDex.Count; i++)
        {
            if(MoveDex[i].Species == demonName){
                return MoveDex[i].LevelUpMoves;
            }
        }
        return MoveDex[0].LevelUpMoves;
    }
}
