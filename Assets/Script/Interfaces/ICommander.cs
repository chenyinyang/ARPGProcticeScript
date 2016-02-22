using UnityEngine;
using System.Collections.Generic;

public interface ICommander {
    //Transform myTransform { get; }
    List<NPCController> Followers { get; set; }
    void CommandFollowersInvade();
    void CommandFollowerInvade(NPCController follower);
    void ChangeFollower(ICommander commander, NPCController follower);
    void AddFollower(NPCController follower);
}
