[System.Serializable]
public class LoadoutDataJSON
{
    public int SelectedSlot = 0;
    public LoadoutSlotDataJSON[] Slots;
    /*
    internal LoadoutDataJSON()
    {
        Slots.SetValue(new LoadoutSlotDataJSON(), 0);
        Slots.SetValue(new LoadoutSlotDataJSON(), 1);
        Slots.SetValue(new LoadoutSlotDataJSON(), 2);
        Slots.SetValue(new LoadoutSlotDataJSON(), 3);
        Slots.SetValue(new LoadoutSlotDataJSON(), 4);
        Slots.SetValue(new LoadoutSlotDataJSON(), 5);
        Slots.SetValue(new LoadoutSlotDataJSON(), 6);
        Slots.SetValue(new LoadoutSlotDataJSON(), 7);
    }*/
}
