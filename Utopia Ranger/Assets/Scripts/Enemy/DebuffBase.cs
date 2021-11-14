using System.Collections.Generic;

// duration: Time (in seconds) the debuff will last. Afterwards, worn_off will be true, ready for the victim to remove it.
// affect_rate: Effect activates every x seconds. If 0, it only activates once but lasts until it's worn out.
public class DebuffBase
{
    public DebuffBase(EnemyBase victim, float duration, float affect_rate = 0f, string name = "Default")
    {
        this.victim = victim;
        this.name = name;
        duration_given = duration;
        this.affect_rate = affect_rate;
    }

    public EnemyBase victim;
    public string name{get; private set;}
    
    public float duration_given{get; private set;}
    public float duration_current{get; private set;}

    public float affect_rate{get; set;}
    public float affect_elapsed{get; private set;}

    public bool worn_off;

    // This is not Unity's. It's run by UpdateDebuff(), which requires Update() in the victim to run it.
    protected virtual void Update(float deltaTime)
    {
        // This guarantees that the effect will activate once no matter what.
        if (affect_rate >= 0)
        {
            affect_elapsed += deltaTime;
            if (affect_elapsed >= affect_rate) { affect_elapsed -= affect_rate; OnActivate(); }
            if (affect_rate == 0) affect_rate = -1f; // This is what stops OnActivate from running again.
        }
        

        duration_current += deltaTime;
        if (duration_current >= duration_given) { worn_off = true; OnWearOff(); }
    }

    public virtual void OnActivate()
    {

    }

    public virtual void OnWearOff()
    {

    }

    public void Extend(float new_duration)
    {
        duration_given = new_duration;
        duration_current = 0;
    }

    public static DebuffBase GetDebuff(string name, List<DebuffBase> debuffs)
    {
        foreach (var i in debuffs) if (i.name == name) return i;
        return null;
    }

    // You're to run this every tick.
    public static void UpdateDebuffs(List<DebuffBase> debuffs, float deltaTime)
    {
        Stack<int> worn_off_debuffs = new Stack<int>(); // Using stack because we're doing RemoveAt from the end.
        for (var i = 0; i < debuffs.Count; i++)
        {
            debuffs[i].Update(deltaTime);
            if (debuffs[i].worn_off) worn_off_debuffs.Push(i);
        }
        while (worn_off_debuffs.Count > 0) { debuffs.RemoveAt(worn_off_debuffs.Pop()); }
    }
}
