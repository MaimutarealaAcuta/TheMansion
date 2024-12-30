using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInventory
{
    // For simplicity, a static reference.
    public static GameObject CurrentHeldObject;
    public static Action<int> OnWoodCountChange;

    private static int woodCount = 15;
    private static int woodAmount = 0;

    public static bool hasWoodenSpike = false;
    public static int monstersBurned = 0;

    private static List<string> keys = new List<string>();

    public static bool HasWood(int amount)
    {
        return woodAmount >= amount;
    }

    public static bool CanGatherWood()
    {
        return woodAmount < woodCount;
    }

    public static void GatherWood(int amount)
    {
        woodAmount += amount;

        if (woodAmount >= woodCount)
        {
            woodAmount = woodCount;
        }

        OnWoodCountChange?.Invoke(woodAmount);
    }

    public static void UseWood(int amount)
    {
        woodAmount = Mathf.Max(0, woodAmount - amount);
        OnWoodCountChange?.Invoke(woodAmount);
    }

    public static bool HasKey(string keyName)
    {
        return keys.Contains(keyName);
    }

    public static void AddKey(string keyName)
    {
        if (!keys.Contains(keyName))
        {
            keys.Add(keyName);
        }
    }

    public static bool UseKey(string keyName)
    {
        // If you want to remove the key after using it
        if (keys.Contains(keyName))
        {
            keys.Remove(keyName);
            return true;
        }
        return false;
    }
}