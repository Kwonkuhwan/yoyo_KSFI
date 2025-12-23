using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioObj : MonoBehaviour
{
    /// <summary>
    /// 1. 배경
    /// 2. 경고 메시지
    /// 3. 팝업
    /// 4. 인터렉션 버튼
    /// 5. 서브 팝업
    /// 6. 시나리오 인덱싱
    /// </summary>
    public int index;
    public Sprite[] bgSprites;
    public Sprite[] eventPopupSprites;
    public Image backgroundImage;
    public DefaultPopupObj[] popupObjs;
    public DefaultPopupObj eventPopupObj;
    public Button[] interactableBtns;
    public Button nextBtn;
}
