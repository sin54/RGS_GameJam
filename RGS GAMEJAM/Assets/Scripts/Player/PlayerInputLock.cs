using UnityEngine;

public static class PlayerInputLock
{
    public static bool IsLocked { get; private set; } = false;

    public static void LockInput()
    {
        IsLocked = true;
    }

    public static void UnlockInput()
    {
        IsLocked = false;
    }
}
