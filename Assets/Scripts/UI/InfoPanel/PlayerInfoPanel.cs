using System.Collections.Generic;
using Extension;
using FishNet;
using GamePlay.Room;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InfoPanel
{
    public class PlayerInfoPanel : MonoBehaviour
    {
        public static PlayerInfoPanel Instance { get; private set; }
        [SerializeField] private GameObject infoPrefab;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform content;

        private float InfoDuration
        {
            get => verticalLayoutGroup.spacing;
            set => verticalLayoutGroup.spacing = value;
        }


        public void UpdateInfoPanel(IList<PlayerInfo> list)
        {
            if (list.Count <= 0) return;
            // Vector2 size = rectTransform.sizeDelta;
            // float y = ((RectTransform)infoPrefab.transform).sizeDelta.y;
            // size.y = (list.Count - 1) * (InfoDuration + y) + y;
            // rectTransform.sizeDelta = size;
            foreach (var info in list)
            {
                if (info.id != -1)
                {
                    GameObject gobj = Instantiate(infoPrefab, content, false);
                    gobj.GetComponent<PlayerInfoUI>().Init(info);
                }
            }

            rectTransform.ResetSizeFromChilds();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            Clear();
        }

        public void Clear()
        {
            content.DestroyAllChildren();
            Vector3 size = content.sizeDelta;
            size.y = 0;
            content.sizeDelta = size;
            content.localPosition = new Vector3(0, 0);
        }
    }
}