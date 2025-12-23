using LJS;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{

    [SerializeField] private int index;
    [FormerlySerializedAs("zoneStr")]
    [SerializeField] private string srtzone;
    [FormerlySerializedAs("nameStr")]
    [SerializeField] private string strName;
    
    [SerializeField] private TextMeshProUGUI indexText;
    [FormerlySerializedAs("zoneText")]
    [SerializeField] private TextMeshProUGUI areaText;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private Image checkMarkImage;
    
    private bool _checked = false;

    public void InitTwoLine(bool isChecked)
    {
        OnCheck(isChecked);
        indexText.text = index.ToString();
        //areaText.text = srtzone;
        //nameText.text = !string.IsNullOrEmpty(Util.GetObjectName(gameObject.name)) ? Util.GetObjectName(gameObject.name) : strName;
    }

    public void InitOneLine(bool isChecked)
    {
        OnCheck(isChecked);
        //nameText.text = !string.IsNullOrEmpty(Util.GetObjectName(gameObject.name)) ? Util.GetObjectName(gameObject.name) : strName;
    }

    public void OnCheck(bool isChecked)
    {
        if (_checked.Equals(isChecked))
            return;
        _checked = isChecked;
        checkMarkImage.gameObject.SetActive(_checked);
        //string isOn = isChecked ? "On" : "Off";
        //string panelName = Util.RemoveWhitespaceUsingRegex(Util.GetObjectName(nameText.text));
        //ControlPanel.Instance?.SetReceiverLog($"{panelName} {isOn}");
    }

    public bool IsChecked()
    {
        return _checked;
    }

}
