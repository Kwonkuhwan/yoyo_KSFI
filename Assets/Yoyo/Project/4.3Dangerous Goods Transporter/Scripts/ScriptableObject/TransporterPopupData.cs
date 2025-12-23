using UnityEngine;

[CreateAssetMenu(fileName = "TransporterPopupData", menuName = "ScriptableObjects/TransporterPopupData", order = 1)]
public class TransporterPopupData : ScriptableObject
{
    public TransporterPopup[] transporterPopups;

    private void OnValidate()
    {
        for (int i = 0; i < transporterPopups.Length; i++)
        {
            transporterPopups[i].index = i;
        }
    }
}

[System.Serializable]
public class TransporterPopup
{
    public string titleText;
    public int index;
    public bool sideFormat;
    public bool setImageMiddle;
    [TextArea]
    public string descriptionText;

    public Sprite exampleImage_1;
    public Sprite exampleImage_2;
    public string documentTitleText;
    [TextArea]
    public string documentDescriptionText;
    public int popupBgNumber;
    public ButtonPosition buttonPosition;
    public AudioClip audioClip;
}

public enum ButtonPosition
{
    None,
    Middle,
    Right
}


