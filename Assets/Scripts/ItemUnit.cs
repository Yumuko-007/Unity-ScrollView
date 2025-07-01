using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// 测试用脚本
    /// </summary>
    public class ItemUnit : UnitBase
    {
        public Image Image;
        public TextMeshProUGUI TMPro;
        public Image OutImage;
        private float width;
        public Button Button;
        private int Index;
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitial()
        {
        }

        protected override void OnSpwan()
        {
            width = rectTransform.sizeDelta.x;
            Button = GetComponent<Button>();
            Button?.onClick.RemoveAllListeners();
            Button?.onClick.AddListener(OnClick);
        }

        protected override void OnDespwan()
        {
            Button?.onClick.RemoveAllListeners();
        }

        public void SetData(int index)
        {
            Index = index;
            TMPro.text = $"{index}";
        }

        private void OnClick()
        {
            Debug.LogError($"点击的是{Index}");
            if (OutImage.gameObject.activeSelf)
            {
                OutImage.gameObject.SetActive(false);
                rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            }
            else
            {
                OutImage.gameObject.SetActive(true);
                rectTransform.sizeDelta = new Vector2(width + 100, rectTransform.sizeDelta.y);
            }
        }
    }
}