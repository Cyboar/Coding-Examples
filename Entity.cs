using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName = "";
    int hpMax = 0;
    int hp = 0;
    int spd = 0;
    int att = 0;
    int def = 0;
    float critChance = 0;
    List<int> abilities = new List<int>();
    HealthBar healthBar;

    public void Initialise(EntityList.Entity entity)
    {
        entityName = entity.GetName();
        name = entityName;
        hpMax = entity.GetHP();
        hp = hpMax;
        spd = entity.GetSpeed();
        att = entity.GetAttack();
        def = entity.GetDefence();
        critChance = entity.GetCritical();
        abilities = entity.GetAbilities();
    }

    public int RollInitiative()
    {
        int roll;
        roll = Random.Range(0, 20) + spd;
        return roll;
    }

    public List<int> GetAbilities()
    {
        return abilities;
    }

    public int GetAbilityID(int id)
    {
        return abilities[id];
    }

    public void SetHealthBar(HealthBar newHealthBar)
    {
        healthBar = newHealthBar;
        newHealthBar.Initialise(hpMax);
    }

    void Update()
    {
        
    }
}
