using UnityEngine;
using System.Collections;

public interface IDamageMaker {

    float GetCurDamage(DamageType type, out bool isCritical, out float additionHit);
}
