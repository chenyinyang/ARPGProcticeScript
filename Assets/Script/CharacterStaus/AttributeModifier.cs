using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttributeModifier {
    Dictionary<PrimaryAttribute, float> sourceRatio;
    public AttributeModifier() {
        sourceRatio = new Dictionary<PrimaryAttribute, float>();
    }
    public void AddRatio(PrimaryAttribute attr,float ratio) {
        if (sourceRatio.ContainsKey(attr))
            sourceRatio[attr] += ratio;
        else
            sourceRatio.Add(attr, ratio);
    }
    public void RemoveRatio(PrimaryAttribute attr, float ratio)
    {
        if (sourceRatio.ContainsKey(attr))
        {
            if (sourceRatio[attr] > ratio)
                sourceRatio[attr] -= ratio;
            else
                sourceRatio.Remove(attr);
        }
    }
    public int Value {
        get {
            float sum = 0;
            foreach (var item in sourceRatio)
            {
                sum += item.Key.AdjustedValue * item.Value;
            }
            return (int)sum;
        }
    }
}
