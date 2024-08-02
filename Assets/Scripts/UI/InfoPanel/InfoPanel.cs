using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.InfoPanel
{
    public class InfoPanel : MonoBehaviour
    {
        public static InfoPanel Instance { get; private set; }
        public List<UIBehaviour> infoList;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private RectTransform rectTransform;


        private float InfoDuration
        {
            set => verticalLayoutGroup.spacing = value;
        }

        private float TotalHeight => infoList.Sum(info => ((RectTransform)info.transform).sizeDelta.y);

        public void UpdateInfoPanel(IList<UIBehaviour> list)
        {
            this.infoList = list as List<UIBehaviour>;
            Vector2 size = rectTransform.sizeDelta;
            size.y = TotalHeight;
            rectTransform.sizeDelta = size;
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}