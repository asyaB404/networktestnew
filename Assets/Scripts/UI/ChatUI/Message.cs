
using UnityEngine;
using TMPro;
using Extension;
using UnityEngine.Serialization;

namespace ChatUI
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI myname;
        [FormerlySerializedAs("text")] [SerializeField] private TextMeshProUGUI message;

        public int Init(string name, string text)
        {
            this.myname.text = name;
            this.message.text = text;
            return this.message.ReSetHeightFromText() + 1;
        }
    }
}