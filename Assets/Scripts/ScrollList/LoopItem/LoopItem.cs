using DG.Tweening;
using UnityEngine;

namespace Game
{
    public abstract class LoopItem
    {
        public ILoopObject loopObject { private set; get; }
        public RectTransform rectTransform { private set; get; }
        public int Index { set; get; } //序号
        public RectTransform _content { set; get; } //列表Unit父节点
        public RectOffset _padding { set; get; } //padding位移
        public float _spacingNum { set; get; } //间隔数
        public TextAnchor _childAligin { set; get; } //对齐方式
        public ViewportItem _viewportItem { set; get; } //ViewPort可视范围对象

        #region 自身保留的相对于Content的显示区域

        public Rect localRect { set; get; } = new();
        public Vector2 sizeDelta;
        public Vector2 localPosition => rectTransform.localPosition; // 左上角的位置

        #endregion

        public Vector3? endPosition { set; get; } // 最终位置，因为可能会做动画，所以这里需要记录一下最终位置

        public void Init(RectTransform content, ViewportItem viewport, int idx, RectOffset padding, float spacing,
            TextAnchor childAligin)
        {
            Index = idx;
            _content = content;
            _viewportItem = viewport;
            _padding = padding;
            _spacingNum = spacing;
            _childAligin = childAligin;
            sizeDelta = new Vector2(viewport.rectTransform.rect.width,
                viewport.rectTransform.rect.height / 4); // 随便默认一个大小
            endPosition = null;
        }

        /// <summary>
        /// 绑定对象
        /// </summary>
        public void SetObject(ILoopObject obj)
        {
            loopObject = obj;
            rectTransform = obj.objTrans;
            rectTransform.DOKill();
            sizeDelta.y = rectTransform.rect.height;
            sizeDelta.x = rectTransform.rect.width;
        }

        /// <summary>
        /// 刷新索引位置
        /// </summary>
        public void IndexRefresh(int index)
        {
            Index = index;
            loopObject.LoopIndexRefresh(index);
        }

        /// <summary>
        /// 计算位置坐标
        /// </summary>
        protected virtual Vector2 GetContentLocalPosition(Rect rect)
        {
            var localPosition = rect.center;
            localPosition += (rectTransform.pivot - new Vector2(0.5f, 0.5f)) * sizeDelta;
            localPosition -= new Vector2((sizeDelta.x - rectTransform.rect.width) / 2, 0); // 本来在中间，这条是把它移动到左边
            return localPosition;
        }

        public void DespawnSelf()
        {
            loopObject?.DespawnObject();
            rectTransform = null;
            loopObject = null;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var worldMin = _content.TransformPoint(localRect.min);
            var worldMax = _content.TransformPoint(localRect.max);

            Gizmos.DrawLine(worldMin, new Vector2(worldMin.x, worldMax.y));
            Gizmos.DrawLine(new Vector2(worldMin.x, worldMax.y), worldMax);
            Gizmos.DrawLine(worldMax, new Vector2(worldMax.x, worldMin.y));
            Gizmos.DrawLine(new Vector2(worldMax.x, worldMin.y), worldMin);
        }

        #region 子类需重写的方法

        /// <summary>
        /// 设置位置
        /// </summary>
        protected abstract void SetRectPosition();

        /// <summary>
        /// 设置制定Rect位置 已更新横向
        /// </summary>
        public abstract void InitFirstPosition(bool atTop);

        /// <summary>
        /// 设置在制定位置的顶部
        /// </summary>
        public abstract void InitLocalPositionAtUp(Rect beforeRect, bool isInit);

        /// <summary>
        /// 设置在制定位置的底部
        /// </summary>
        public abstract void InitLocalPositionAtDown(Rect behindRect, bool isInit);

        /// <summary>
        /// 设置在制定位置的底部
        /// </summary>
        public abstract void SetLocalPositionAtDown(Rect behindRect);

        /// <summary>
        /// 根据sizeDelta调整位置
        /// </summary>
        public abstract void ResetRectBySizeDelta();

        /// <summary>
        /// 设置在制定位置的底部
        /// </summary>
        public abstract void AddLocalPosition(float add);

        /// <summary>
        /// 判断对象在现实区域里面的状态
        /// </summary>
        public abstract LoopObjectStatus GetObjectViewStatus();

        #endregion
    }
}