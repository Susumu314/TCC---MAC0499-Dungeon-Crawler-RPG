using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PartyManager {
    private static PartyManager instance;
    public static PartyManager Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
    }
}