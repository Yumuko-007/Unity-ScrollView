using UnityEngine;

namespace Game
{
    public class ViewportItem
    {
        public RectTransform rectTransform { get; private set; }
        private RectTransform conetentRectTrans;
        public Vector3 leftDownCornerInContent { get; private set; } // 左下角在Content上的局部坐标
        public Vector3 rightUpCornerInContent { get; private set; } // 左上角在Content上的局部坐标
        private Vector3[] _corners = new Vector3[4];
        private Camera uiCamera;

        public ViewportItem(RectTransform trans, RectTransform conetent, Camera camera)
        {
            rectTransform = trans;
            uiCamera = camera;
            conetentRectTrans = conetent;
        }

        /// <summary>
        /// 计算当前UI元素在内容区域中的左下角和右上角的局部坐标。
        /// 必须要先计算的函数，减少反复计算工作量
        /// </summary>
        public void PreCalculate()
        {
            rectTransform.GetWorldCorners(_corners);
            var screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, _corners[0]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(conetentRectTrans, screenPos, uiCamera,
                out Vector2 localPos);
            leftDownCornerInContent = localPos;

            screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, _corners[2]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(conetentRectTrans, screenPos, uiCamera,
                out Vector2 localPos2);
            rightUpCornerInContent = localPos2;
        }
    }
}