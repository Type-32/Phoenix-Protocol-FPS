[System.Serializable]
public class SettingsOptionsJSON
{
    public float Volume = 0f;
    public float FieldOfView = 75f;
    public float MouseSensitivity = 70f;
    public int ResolutionIndex = -1;
    public bool Fullscreen = true;
    public int QualityIndex = 1;
    public SettingsOptionsJSON()
    {
        Volume = 0f;
        FieldOfView = 75f;
        MouseSensitivity = 100f;
        ResolutionIndex = -1;
        Fullscreen = true;
        QualityIndex = 1;
    }
}
