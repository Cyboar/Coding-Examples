using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LoadOrder))]
public class EntityList : MonoBehaviour
{
    public class Entity
    {
        string name;
        int hp, spd, att, def, critChance, radDam, stunChance, stunRes;
 
        List<int> abilities = new List<int>();

        public Entity(string newName, int newHp, int newSpd, int newAtt, int newDef, int newCritChance)
        {
            name = newName;
            hp = newHp;
            spd = newSpd;
            att = newAtt;
            def = newDef;
            critChance = newCritChance;
        }

        // 0 abilities, basic attack overflow
        public Entity(string newName, int newHp, int newSpd, int newAtt, int newDef, int newCritChance, int newRad, int newStun, int newStunRes, int basicAtt)
        {
            name = newName;
            hp = newHp;
            spd = newSpd;
            att = newAtt;
            def = newDef;
            critChance = newCritChance;
            radDam = newRad;
            stunChance = newStun;
            stunRes = newStunRes;
            abilities.Add(basicAtt);
        }

        // 1 abilities overflow
        public Entity(string newName, int newHp, int newSpd, int newAtt, int newDef, int newCritChance, int newRad, int newStun, int newStunRes, int basicAtt, int abil1)
        {
            name = newName;
            hp = newHp;
            spd = newSpd;
            att = newAtt;
            def = newDef;
            critChance = newCritChance;
            radDam = newRad;
            stunChance = newStun;
            stunRes = newStunRes;
            abilities.Add(basicAtt);
            abilities.Add(abil1);
        }

        // 2 abilities overflow
        public Entity(string newName, int newHp, int newSpd, int newAtt, int newDef, int newCritChance, int newRad, int newStun, int newStunRes, int basicAtt, int abil1, int abil2)
        {
            name = newName;
            hp = newHp;
            spd = newSpd;
            att = newAtt;
            def = newDef;
            critChance = newCritChance;
            radDam = newRad;
            stunChance = newStun;
            stunRes = newStunRes;
            abilities.Add(basicAtt);
            abilities.Add(abil1);
            abilities.Add(abil2);
        }

        // 3 abilities overflow
        public Entity(string newName, int newHp, int newSpd, int newAtt, int newDef, int newCritChance, int newRad, int newStun, int newStunRes, int basicAtt, int abil1, int abil2, int abil3)
        {
            name = newName;
            hp = newHp;
            spd = newSpd;
            att = newAtt;
            def = newDef;
            critChance = newCritChance;
            radDam = newRad;
            stunChance = newStun;
            stunRes = newStunRes;
            abilities.Add(basicAtt);
            abilities.Add(abil1);
            abilities.Add(abil2);
            abilities.Add(abil3);
        }

        public string GetName()
        {
            return name;
        }

        public int GetHP()
        {
            return hp;
        }

        public int GetSpeed()
        {
            return spd;
        }

        public int GetAttack()
        {
            return att;
        }

        public int GetDefence()
        {
            return def;
        }

        public int GetCritical()
        {
            return critChance;
        }

        public int GetRadDamage()
        {
            return radDam;
        }

        public int GetStun()
        {
            return stunChance;
        }

        public int GetStunRes()
        {
            return stunRes;
        }

        public List<int> GetAbilities()
        {
            return abilities;
        }
    }


    //Dictionary of all the currently loaded enemies
    Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
    int index = 0;


    //Placeholder Load function, to be replaced by database fetch function.
    public void Load()
    {
        entities.Add(900, new Entity("Sheriff", 100, 10, 10, 10, 15, 0, 55, 35, 1, 3, 4, 5));
        entities.Add(901, new Entity("Mutant", 150, 8, 12, 8, 5, 15, 15, 5, 1));
        entities.Add(Index(), new Entity("Grunt - Small", 60, 8, 5, 5, 15, 0, 15, 10, 2, 6, 7));
        entities.Add(Index(), new Entity("Grunt - Medium", 90, 7, 9, 7, 15));
        entities.Add(Index(), new Entity("Grunt - Large", 120, 5, 7, 9, 15));
        entities.Add(Index(), new Entity("Assassin", 70, 14, 12, 5, 3));
        entities.Add(Index(), new Entity("Boss", 150, 10, 12, 10, 2));
    }

    int Index()
    {
        index += 1;
        return index;
    }

    public Entity GetEntity(int id)
    {
        Entity temp = null;
        if (entities.TryGetValue(id, out temp))
        {
            //success!
            return temp;
        }
        else
        {
            //failure!
            Debug.Log("An entity for the given ID (" + id + ") cannot be found.");
            return null;
        }
    }
}
