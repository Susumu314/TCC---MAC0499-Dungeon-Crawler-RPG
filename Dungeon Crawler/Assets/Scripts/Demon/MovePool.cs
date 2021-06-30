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
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/2, /*skillID*/8),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Fire Kindred Flame", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Fire Dragonspawn", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Toxic Carnivorous Plant A", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/9),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/7, /*skillID*/6),
                })),
        new Demon_MovePool("Toxic Carnivorous Plant B", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Toxic Carnivorous Plant C", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Sea Crab", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/10),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/7),
                })),
        new Demon_MovePool("Spider Crab", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Shell Claw", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/0),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Rabbit Bandit", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                })),
        new Demon_MovePool("Rabbit Knight", 
            new List<LevelUp_Move>(new[] {
                new LevelUp_Move(/*Level*/1, /*skillID*/5),
                new LevelUp_Move(/*Level*/2, /*skillID*/3),
                new LevelUp_Move(/*Level*/4, /*skillID*/2),
                new LevelUp_Move(/*Level*/5, /*skillID*/1),
                new LevelUp_Move(/*Level*/6, /*skillID*/4),
                }))

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
