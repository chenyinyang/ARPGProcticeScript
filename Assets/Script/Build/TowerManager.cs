using UnityEngine;
using System.Collections.Generic;
//using System.Linq;
using System;

public static class TowerManager{
    static List<Tower> towerInMap;

    public static void AddTower(Tower tower) {
        if (towerInMap == null)
            towerInMap = new List<Tower>();
        towerInMap.Add(tower);
        tower.OnBuildDestroied += Tower_OnBuildDestroied;
    }
    private static void Tower_OnBuildDestroied(Build build)
    {
        towerInMap.Remove(build as Tower);
    }

    private static Tower GetNearestCompanionTower(Tower from) {
        List<Tower> companionTowers = new List<Tower>();
        for (int i = 0; i < towerInMap.Count; i++)
        {
            if (towerInMap[i] == from)
                continue;
            if (Tags.IsCompanion(from, towerInMap[i]))
                companionTowers.Add(towerInMap[i]);
        }            
        companionTowers.Sort(new TowerDistanceComparer(from,false));
        if (companionTowers.Count == 0)
            return null;
        return companionTowers[0];
    }
    private static Tower GetNearestEnemyTower(Tower from)
    {
        List<Tower> enemyTowers = new List<Tower>();
        for (int i = 0; i < towerInMap.Count; i++)
        {
            if (!Tags.IsCompanion(from, towerInMap[i]))
                enemyTowers.Add(towerInMap[i]);
        }       
        enemyTowers.Sort(new TowerDistanceComparer(from, false));
        if (enemyTowers.Count == 0)
            return null;
        return enemyTowers[0];
    }
    public class TowerDistanceComparer : IComparer<Tower>
    {
        private Vector3 from;
        bool isDecending;
        public TowerDistanceComparer(Tower from,bool decending) {
            this.from = from.transform.position;
            isDecending = decending;
        }
        public int Compare(Tower x, Tower y)
        {
            return (from- x.transform.position).magnitude
                .CompareTo((from - y.transform.position).magnitude)* (isDecending?-1:0);
        }
    }
}
