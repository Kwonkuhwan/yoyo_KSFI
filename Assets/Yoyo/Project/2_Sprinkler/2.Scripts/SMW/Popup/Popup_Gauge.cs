using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW.Sprinkler
{
    public class Popup_Gauge : Popup
    {
        [SerializeField] RectTransform Needle;
        [SerializeField] RectTransform Room_Needle;

        public void Up()
        {
            Needle.rotation = Quaternion.Euler(new Vector3(0, 0, 135));
            Room_Needle.rotation = Quaternion.Euler(new Vector3(0, 0, 135));
        }

        public void Defalut()
        {
            Needle.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
            Room_Needle.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
        }

        public void ChangeNeedleRotate(int angle)
        {
            Needle.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Room_Needle.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        protected override void Setting()
        {

        }

        protected override void RESET()
        {
            Defalut();
        }
    }
}
