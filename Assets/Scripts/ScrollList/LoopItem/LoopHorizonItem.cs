using UnityEngine;

namespace Game
{
    /// <summary>
    /// LoopItem 横向列表类 符合开闭原则
    /// </summary>
    public class LoopHorizonItem : LoopItem
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

                if (endPosition == null || localPosition.x != endPosition.Value.x)
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
                sizeDelta.x = rectTransform.rect.width;
            }

            var position = Vector3.zero;
            if (atTop)
            {
                position = new Vector3(_viewportItem.leftDownCornerInContent.x,
                    _viewportItem.leftDownCornerInContent.y); // 左下角位置
                position = position + new Vector3(_padding.left, 0);
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
            var offset = new Vector2(beforeRect.width + _spacingNum, 0);
            localRect = new Rect(beforeRect.position - offset, sizeDelta);
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
            var offset = new Vector2(sizeDelta.x + _spacingNum, 0);
            localRect = new Rect(behindRect.position + offset, sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 设置在制定位置的底部
        /// </summary>
        public override void SetLocalPositionAtDown(Rect behindRect)
        {
            // var offset = new Vector2(sizeDelta.x + _spacingNum, 0);
            // localRect = new Rect(behindRect.position + offset, sizeDelta);
            // SetRectPosition();
            var offset = new Vector2( _spacingNum, 0);
            localRect = new Rect(new Vector2(behindRect.xMax, behindRect.yMin) + offset, sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 根据sizeDelta调整位置
        /// </summary>
        public override void ResetRectBySizeDelta()
        {
            if (rectTransform != null)
            {
                var width = rectTransform.rect.width;
                if (width != sizeDelta.x)
                {
                    var delta = width - sizeDelta.x;
                    sizeDelta.x = width;
                    localRect = new Rect(localRect.position + new Vector2(delta, 0), sizeDelta);
                }
            }
        }

        /// <summary>
        /// 设置在制定位置的底部 已更新横向
        /// </summary>
        public override void AddLocalPosition(float add)
        {
            localRect = new Rect(localRect.position + new Vector2(add, 0), sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 判断对象在现实区域里面的状态
        /// </summary>
        public override LoopObjectStatus GetObjectViewStatus()
        {
            var left = localRect.xMin;
            var right = localRect.xMax;

            if (left > _viewportItem.rightUpCornerInContent.x)
                return LoopObjectStatus.RightOut;
            else if (_viewportItem.leftDownCornerInContent.x > right)
                return LoopObjectStatus.LeftOut;
            else if (_viewportItem.rightUpCornerInContent.x >= right &&
                     _viewportItem.leftDownCornerInContent.x <= left)
                return LoopObjectStatus.Full;
            else
                return LoopObjectStatus.Part;

            return LoopObjectStatus.NotShow;
        }

        /// <summary>
        /// 本身左侧坐标，加上间隙到左侧的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToLeftViewSize()
        {
            var padding = Index == 0 ? _padding.left : _spacingNum;
            var left = _viewportItem.leftDownCornerInContent.x + padding;
            return localRect.xMin - left;
        }

        /// <summary>
        /// 本身底部坐标，加上间隙到底部的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToRightViewSize(int total)
        {
            var padding = Index == total - 1 ? _padding.right : _spacingNum;
            var right = _viewportItem.rightUpCornerInContent.x - padding;
            return right - localRect.xMax;
        }
    }
}