using TMPro;
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

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitial()
        {
        }

        protected override void OnSpwan()
        {
        }

        protected override void OnDespwan()
        {
        }

        public void SetData(int index)
        {
            TMPro.text = $"{index}";
        }
    }
}