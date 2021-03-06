﻿using UnityEngine;
using System.Collections;
using System;

public class StaticState
{

    public static int currentLevel = 0;
    public static int permHealthBonus = 0;
    public static int permRollBonus = 0;
    public static int permShieldBonus = 0;
    public static int infoCounter = 0;
        
    public void embark()
    {
        currentLevel = 0;
    }

    internal static void AddPermHealth()
    {
        permHealthBonus += 1;
    }

    internal static void AddPermRollBonus()
    {
        permRollBonus += 1;
    }

    internal static void AddPermShieldBonus()
    {
        permShieldBonus += 1;
    }

    internal static void Reset()
    {
        currentLevel = 0;
        permHealthBonus = 0;
        permRollBonus = 0;
        permShieldBonus = 0;
    }
}
