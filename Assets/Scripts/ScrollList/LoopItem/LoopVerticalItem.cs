using UnityEngine;

namespace Game
{
    /// <summary>
    /// LoopItem 纵向列表类 符合开闭原则
    /// </summary>
    public class LoopVerticalItem : LoopItem
    {
        /// <summary>
        /// 设置位置
        /// </summary>
        protected override void SetRectPosition()
        {
            if (rectTransform != null)
            {
                // 中心位置+中心点偏移
                var localPosition = GetContentLocalPosition(localRect);

                if (endPosition == null || localPosition.y != endPosition.Value.y)
                {
                    endPosition = localPosition;
                    loopObject.SetLoopObjectLocalPosition(localPosition);
                }
            }
        }

        /// <summary>
        /// 设置制定Rect位置
        /// </summary>
        public override void InitFirstPosition(bool atTop)
        {
            if (rectTransform != null)
            {
                sizeDelta.y = rectTransform.rect.height;
            }

            var position = Vector3.zero;
            if (atTop)
            {
                position = new Vector3(_viewportItem.leftDownCornerInContent.x,
                    _viewportItem.rightUpCornerInContent.y); // 左上角位置
                position = position - new Vector3(0, sizeDelta.y + _padding.top);
            }
            else
            {
                position = _viewportItem.leftDownCornerInContent;
            }

            localRect = new Rect(position, sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 设置在制定位置的顶部
        /// </summary>
        public override void InitLocalPositionAtUp(Rect beforeRect, bool isInit)
        {
            // 做动画用，把其实坐标放到上面显示区域外面
            //lastLocalPostion = null;
            //if (!isInit)
            //{
            //    var viewLeftUp = new Rect(new Vector2(_viewportItem.leftDownCornerInContent.x, _viewportItem.rightUpCornerInContent.y), sizeDelta);
            //    lastLocalPostion = GetContentLocalPosition(viewLeftUp);
            //}
            var offset = new Vector2(0, beforeRect.height + _spacingNum);
            localRect = new Rect(beforeRect.position + offset, sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 设置在制定位置的底部
        /// </summary>
        public override void InitLocalPositionAtDown(Rect behindRect, bool isInit)
        {
            // 做动画用，把其实坐标放到下面显示区域外面
            //lastLocalPostion = null;
            //if (!isInit)
            //{
            //    var viewLeftDown = new Rect((Vector2)_viewportItem.leftDownCornerInContent - new Vector2(0, sizeDelta.y), sizeDelta);
            //    lastLocalPostion = GetContentLocalPosition(viewLeftDown);
            //}
            var offset = new Vector2(0, sizeDelta.y + _spacingNum);
            localRect = new Rect(behindRect.position - offset, sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 设置在制定位置的底部
        /// </summary>
        public override void SetLocalPositionAtDown(Rect behindRect)
        {
            var offset = new Vector2(0, sizeDelta.y + _spacingNum);
            localRect = new Rect(behindRect.position - offset, sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 根据sizeDelta调整位置
        /// </summary>
        public override void ResetRectBySizeDelta()
        {
            if (rectTransform != null)
            {
                var height = rectTransform.rect.height;
                if (height != sizeDelta.y)
                {
                    var delta = height - sizeDelta.y;
                    sizeDelta.y = height;
                    localRect = new Rect(localRect.position - new Vector2(0, delta), sizeDelta);
                }
            }
        }

        /// <summary>
        /// 设置在制定位置的底部
        /// </summary>
        public override void AddLocalPosition(float add)
        {
            localRect = new Rect(localRect.position + new Vector2(0, add), sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 判断对象在现实区域里面的状态
        /// </summary>
        public override LoopObjectStatus GetObjectViewStatus()
        {
            var up = localRect.yMax;
            var down = localRect.yMin;

            if (down > _viewportItem.rightUpCornerInContent.y)
                return LoopObjectStatus.DownOut;
            else if (_viewportItem.leftDownCornerInContent.y > up)
                return LoopObjectStatus.UpOut;
            else if (_viewportItem.leftDownCornerInContent.y <= down &&
                     _viewportItem.rightUpCornerInContent.y >= up)
                return LoopObjectStatus.Full;
            else
                return LoopObjectStatus.Part;
            return LoopObjectStatus.NotShow;
        }

        /// <summary>
        /// 本身顶部坐标，加上间隙到顶部的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToTopViewSize()
        {
            var padding = Index == 0 ? _padding.top : _spacingNum;
            var up = _viewportItem.rightUpCornerInContent.y - padding;
            return up - localRect.yMax;
        }

        /// <summary>
        /// 本身底部坐标，加上间隙到底部的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToBottomViewSize(int total)
        {
            var padding = Index == total - 1 ? _padding.bottom : _spacingNum;
            var down = _viewportItem.leftDownCornerInContent.y + padding;
            return localRect.yMin - down;
        }
    }
}