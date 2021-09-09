using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbiltyList : MonoBehaviour
{
    public class Ability
    {
        string name, desc;
        int target;
        Dictionary<string, int> abilityMods = new Dictionary<string, int>();
        int dur;

        public Ability(string abilityName, string abilityDescription, int designatedTarget, int duration, string affectedStatistic, int statisticModifier)
        {
            name = abilityName;
            desc = abilityDescription;
            target = designatedTarget;
            dur = duration;
            abilityMods.Add(affectedStatistic, statisticModifier);
        }

        public Ability(string abilityName, string abilityDescription, int designatedTarget, int duration, string affectedStatistic1, int statisticModifier1, string affectedStatistic2, int statisticModifier2)
        {
            name = abilityName;
            desc = abilityDescription;
            target = designatedTarget;
            dur = duration;
            abilityMods.Add(affectedStatistic1, statisticModifier1);
            abilityMods.Add(affectedStatistic2, statisticModifier2);
        }

        public string GetName()
        {
            return name;
        }

        public string GetDescription()
        {
            return desc;
        }

        public int GetTarget()
        {
            return target;
        }

        public Dictionary<string, int> GetAbilityMod()
        {
            return abilityMods;
        }

        public int GetDuration()
        {
            return dur;
        }
    }

    Dictionary<int, Ability> abilities = new Dictionary<int, Ability>();
    int index = 0;

    public Ability GetAbility(int id)
    {
        Ability abil;
        abilities.TryGetValue(id, out abil);
        return abil;
    }

    public void Load()
    {
                                    //name, description, target, duration, affected stat, affect mod
        abilities.Add(1, new Ability("Pistol Shot", "Ranged attack against single opponent. +15% damage against outlaws.", 1, 0, "hp", 15));
        abilities.Add(2, new Ability("Stab", "Melee attack against single opponent.", 1, 0, "hp", 0));
        abilities.Add(3, new Ability("Reload", "+5 defence for 1 turn. +15% critical chance for 1 turn.", 0, 1, "defence", 5, "crit chance", 15));
        abilities.Add(4, new Ability("Spray Fire", "Hit all enemies. -15% damage per shot.", 3, 0, "hp", -15));
        abilities.Add(5, new Ability("For the Badge", "+3 speed and +5 attack for 2 turns.", 0, 2, "speed", 3, "attack", 5));
        abilities.Add(6, new Ability("WITNESS ME", "Self inflict 10 damage. +3 speed and +5 attack for 1 turn.", 0, 1, "speed", 3, "attack", 5));
        abilities.Add(7, new Ability("Headbutt", "Stun for 1 turn.", 1, 1, "stun", 0));
    }

    int Index()
    {
        index += 1;
        return index;
    }
}
