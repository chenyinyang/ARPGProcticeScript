using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FactionManager
{
    public static FactionManager PlayerFactionManager = new FactionManager();
    public static FactionManager EnemyFactionManager = new FactionManager();
    public static FactionManager MonsterFactionManager = new FactionManager();

    const float EXP_TO_POOL_RATE = .1f;
    List<NPCController> NPCs;
    float ExpPool;
    private FactionManager() {
        NPCs = new List<NPCController>();        
    }
    public void AddNPC(NPCController npc) {
        NPCs.Add(npc);
        npc.status.GetExp(ExpPool);
        npc.onGetExp += ExpToPool;
        npc.OnDead += Npc_OnDead;
    }

    private void Npc_OnDead(BaseCharacterBehavior npc)
    {
        NPCController npcController = (npc as NPCController);
        npcController.onGetExp -= ExpToPool;
        NPCs.Remove(npcController);
    }

    void ExpToPool(float mount) {
        ExpPool += mount * EXP_TO_POOL_RATE;
    }
}
