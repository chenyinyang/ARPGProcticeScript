using UnityEngine;
using System.Collections;


public interface ICharacterAnimation {
    
    void PlayIdle();
    void PlayScan();
    void PlayWalk();
    void PlayRun();
    void PlayJump();
    void PlayCast();    
    void Skill(int number);
}
