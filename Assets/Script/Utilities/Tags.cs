using UnityEngine;
using System.Collections;

public static class Tags
{
    public static string Player = "Player";
    public static string Monster = "Monster";
    public static string DeadBody = "DeadBody";
    public static string Friend = "Friend";
    public static string Enemy= "Enemy";
    public static string Build = "Build";


    public static bool IsCompanion(Component self, Component other)
    {   
        //Player -> Friend
        if (self.CompareTag(Tags.Player))
            return other.CompareTag(Tags.Friend) || other.CompareTag(Tags.Player) ;
        //Friend ->player, friend
        if (self.CompareTag(Tags.Friend))
            return other.CompareTag(Tags.Friend) || other.CompareTag(Tags.Player);
        //Enemy -> Enemy
        if (self.CompareTag(Tags.Enemy))
            return other.CompareTag(Tags.Enemy);
        //Monster -> Monster
        if (self.CompareTag(Tags.Monster))
            return other.CompareTag(Tags.Monster);
        return false;
    }
}
public static class Layers
{
    public static int Default = 0;
    public static int Monster = 8;
    public static int Player = 9;
    public static int Clickable = 14;
}