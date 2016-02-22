using UnityEngine;
using System.Collections;

public interface IClosablePanel {
    bool IsOpen { get; }
    void Close();
}
