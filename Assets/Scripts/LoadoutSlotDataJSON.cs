[System.Serializable]
public class LoadoutSlotDataJSON
{
    public string SlotName;
    public bool EquippedByDefault;
    public int Weapon1;
    public int Weapon2;
    public int Equipment1;
    public int Equipment2;

    public int WA_Sight1;
    public int WA_Sight2;
    public int WA_Barrel1;
    public int WA_Barrel2;
    public int WA_Underbarrel1;
    public int WA_Underbarrel2;
    public int WA_Rightbarrel1;
    public int WA_Rightbarrel2;
    public int WA_Leftbarrel1;
    public int WA_Leftbarrel2;
    internal LoadoutSlotDataJSON()
    {
        SlotName = "Custom Loadout";
        EquippedByDefault = false;
        Weapon1 = 0;
        Weapon2 = 1;
        Equipment1 = 0;
        Equipment2 = 0;
        WA_Sight1 = -1;
        WA_Sight2 = -1;
        WA_Barrel1 = -1;
        WA_Barrel2 = -1;
        WA_Underbarrel1 = -1;
        WA_Underbarrel2 = -1;
        WA_Rightbarrel1 = -1;
        WA_Rightbarrel2 = -1;
        WA_Leftbarrel1 = -1;
        WA_Leftbarrel2 = -1;
    }
}
