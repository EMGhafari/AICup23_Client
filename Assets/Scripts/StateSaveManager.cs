using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateSaveManager 
{
    public void SaveState();

    public void LoadState(int stateIndex);
}

