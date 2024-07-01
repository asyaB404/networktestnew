using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extension;

namespace ChatUI
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI myname;
        [SerializeField] private TextMeshProUGUI text;

        public int Init(string myname, string text)
        {
            this.myname.text = myname;
            this.text.text = text;
            return this.text.ReSetHeightFromText() + 1;
        }
    }
}