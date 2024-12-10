using UnityEngine;

public static class PlayerInventory
{
    // For simplicity, a static reference.
    public static GameObject CurrentHeldObject;
    private static int woodCount = 10;

    public static bool HasWood(int amount)
    {
        return woodCount >= amount;
    }

    public static void UseWood(int amount)
    {
        woodCount = Mathf.Max(0, woodCount - amount);
    }
}