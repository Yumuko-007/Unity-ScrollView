using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class LoopItem
    {
        public ILoopObject loopObject { private set; get; }
        public RectTransform rectTransform { private set; get; }

        public LoopObjectType CurLoopType { private set; get; }
        public int Index { set; get; }
        private RectTransform _content;

        private RectOffset _padding;
        private float _spacingNum;

        private ViewportItem _viewportItem;

        #region 自身保留的相对于Content的显示区域

        public Rect localRect { private set; get; } = new Rect();
        private Vector2 sizeDelta;
        public Vector2 localPosition => rectTransform.localPosition; // 左上角的位置

        #endregion

        private Vector3? endPosition; // 最终位置，因为可能会做动画，所以这里需要记录一下最终位置


        public void Init(RectTransform content, ViewportItem viewport, int idx, RectOffset padding, float spacing,
            LoopObjectType type)
        {
            Index = idx;
            _content = content;
            _viewportItem = viewport;
            _padding = padding;
            _spacingNum = spacing;
            sizeDelta = new Vector2(viewport.rectTransform.rect.width,
                viewport.rectTransform.rect.height / 4); // 随便默认一个大小
            endPosition = null;
            CurLoopType = type;
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
        /// 设置位置
        /// </summary>
        private void SetRectPosition()
        {
            if (rectTransform != null)
            {
                // 中心位置+中心点偏移
                Vector2 localPosition = GetContentLocalPosition(localRect);

                if (CurLoopType == LoopObjectType.Vertical)
                {
                    if (endPosition == null || localPosition.y != endPosition.Value.y)
                    {
                        endPosition = localPosition;
                        loopObject.SetLoopObjectLocalPosition(localPosition);
                    }
                }

                if (CurLoopType == LoopObjectType.Horizontal)
                {
                    if (endPosition == null || localPosition.x != endPosition.Value.x)
                    {
                        endPosition = localPosition;
                        loopObject.SetLoopObjectLocalPosition(localPosition);
                    }
                }
            }
        }

        /// <summary>
        /// 计算位置坐标
        /// </summary>
        private Vector2 GetContentLocalPosition(Rect rect)
        {
            Vector2 localPosition = rect.center;
            localPosition += (rectTransform.pivot - new Vector2(0.5f, 0.5f)) * sizeDelta;
            localPosition -= new Vector2((sizeDelta.x - rectTransform.rect.width) / 2, 0); // 本来在中间，这条是把它移动到左边
            return localPosition;
        }

        /// <summary>
        /// 设置制定Rect位置 已更新横向
        /// </summary>
        public void InitFirstPosition(bool atTop)
        {
            if (rectTransform != null)
            {
                if (CurLoopType == LoopObjectType.Vertical)
                {
                    sizeDelta.y = rectTransform.rect.height;
                }

                if (CurLoopType == LoopObjectType.Horizontal)
                {
                    sizeDelta.x = rectTransform.rect.width;
                }
            }

            Vector3 position = Vector3.zero;
            if (atTop)
            {
                if (CurLoopType == LoopObjectType.Vertical)
                {
                    position = new Vector3(_viewportItem.leftDownCornerInContent.x,
                        _viewportItem.rightUpCornerInContent.y); // 左上角位置
                    position = position - new Vector3(0, sizeDelta.y + _padding.top);
                }

                if (CurLoopType == LoopObjectType.Horizontal)
                {
                    position = new Vector3(_viewportItem.leftDownCornerInContent.x,
                        _viewportItem.leftDownCornerInContent.y); // 左下角位置
                    position = position + new Vector3(_padding.left, 0);
                }
            }
            else
            {
                position = _viewportItem.leftDownCornerInContent;
            }

            localRect = new Rect(position, sizeDelta);
            SetRectPosition();
        }

        /// <summary>
        /// 设置在制定位置的顶部 已更新横向
        /// </summary>
        public void InitLocalPositionAtUp(Rect beforeRect, bool isInit)
        {
            // 做动画用，把其实坐标放到上面显示区域外面
            //lastLocalPostion = null;
            //if (!isInit)
            //{
            //    var viewLeftUp = new Rect(new Vector2(_viewportItem.leftDownCornerInContent.x, _viewportItem.rightUpCornerInContent.y), sizeDelta);
            //    lastLocalPostion = GetContentLocalPosition(viewLeftUp);
            //}


            if (CurLoopType == LoopObjectType.Vertical)
            {
                var offset = new Vector2(0, beforeRect.height + _spacingNum);
                localRect = new Rect(beforeRect.position + offset, sizeDelta);
            }

            if (CurLoopType == LoopObjectType.Horizontal)
            {
                var offset = new Vector2(beforeRect.width + _spacingNum, 0);
                localRect = new Rect(beforeRect.position - offset, sizeDelta);
            }

            SetRectPosition();
        }

        /// <summary>
        /// 设置在制定位置的底部 已更新横向
        /// </summary>
        public void InitLocalPositionAtDown(Rect behindRect, bool isInit)
        {
            // 做动画用，把其实坐标放到下面显示区域外面
            //lastLocalPostion = null;
            //if (!isInit)
            //{
            //    var viewLeftDown = new Rect((Vector2)_viewportItem.leftDownCornerInContent - new Vector2(0, sizeDelta.y), sizeDelta);
            //    lastLocalPostion = GetContentLocalPosition(viewLeftDown);
            //}

            if (CurLoopType == LoopObjectType.Vertical)
            {
                var offset = new Vector2(0, sizeDelta.y + _spacingNum);
                localRect = new Rect(behindRect.position - offset, sizeDelta);
            }

            if (CurLoopType == LoopObjectType.Horizontal)
            {
                var offset = new Vector2(sizeDelta.x + _spacingNum, 0);
                localRect = new Rect(behindRect.position + offset, sizeDelta);
            }

            SetRectPosition();
        }

        /// <summary>
        /// 设置在制定位置的底部 已更新横向
        /// </summary>
        public void SetLocalPositionAtDown(Rect behindRect)
        {
            if (CurLoopType == LoopObjectType.Vertical)
            {
                var offset = new Vector2(0, sizeDelta.y + _spacingNum);
                localRect = new Rect(behindRect.position - offset, sizeDelta);
            }

            if (CurLoopType == LoopObjectType.Horizontal)
            {
                var offset = new Vector2(sizeDelta.x + _spacingNum, 0);
                localRect = new Rect(behindRect.position + offset, sizeDelta);
            }

            SetRectPosition();
        }

        /// <summary>
        /// 根据sizeDelta调整位置 已更新横向
        /// </summary>
        public void ResetRectBySizeDelta()
        {
            if (rectTransform != null)
            {
                if (CurLoopType == LoopObjectType.Vertical)
                {
                    float height = rectTransform.rect.height;
                    if (height != sizeDelta.y)
                    {
                        float delta = height - sizeDelta.y;
                        sizeDelta.y = height;
                        localRect = new Rect(localRect.position - new Vector2(0, delta), sizeDelta);
                    }
                }

                if (CurLoopType == LoopObjectType.Horizontal)
                {
                    float width = rectTransform.rect.width;
                    if (width != sizeDelta.x)
                    {
                        float delta = width - sizeDelta.x;
                        sizeDelta.x = width;
                        localRect = new Rect(localRect.position + new Vector2(delta, 0), sizeDelta);
                    }
                }
            }
        }

        /// <summary>
        /// 设置在制定位置的底部 已更新横向
        /// </summary>
        public void AddLocalPosition(float add)
        {
            if (CurLoopType == LoopObjectType.Vertical)
            {
                localRect = new Rect(localRect.position + new Vector2(0, add), sizeDelta);
            }

            if (CurLoopType == LoopObjectType.Horizontal)
            {
                localRect = new Rect(localRect.position + new Vector2(add, 0), sizeDelta);
            }

            SetRectPosition();
        }

        /// <summary>
        /// 判断对象在现实区域里面的状态 已更新横向
        /// </summary>
        public LoopObjectStatus GetObjectViewStatus()
        {
            float up = localRect.yMax;
            float down = localRect.yMin;
            float left = localRect.xMin;
            float right = localRect.xMax;

            if (CurLoopType == LoopObjectType.Vertical)
            {
                if (down > _viewportItem.rightUpCornerInContent.y)
                {
                    return LoopObjectStatus.DownOut;
                }
                else if (_viewportItem.leftDownCornerInContent.y > up)
                {
                    return LoopObjectStatus.UpOut;
                }
                else if ((_viewportItem.leftDownCornerInContent.y >= down) &&
                         (_viewportItem.rightUpCornerInContent.y <= up))
                {
                    return LoopObjectStatus.Full;
                }
                else
                {
                    return LoopObjectStatus.Part;
                }
            }

            if (CurLoopType == LoopObjectType.Horizontal)
            {
                if (left > _viewportItem.rightUpCornerInContent.x)
                {
                    return LoopObjectStatus.RightOut;
                }
                else if (_viewportItem.leftDownCornerInContent.x > right)
                {
                    return LoopObjectStatus.LeftOut;
                }
                else if ((_viewportItem.rightUpCornerInContent.x >= right) &&
                         (_viewportItem.leftDownCornerInContent.x <= left))
                {
                    return LoopObjectStatus.Full;
                }
                else
                {
                    return LoopObjectStatus.Part;
                }
            }

            return LoopObjectStatus.NotShow;
        }

        /// <summary>
        /// 本身顶部坐标，加上间隙到顶部的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToTopViewSize()
        {
            float padding = Index == 0 ? _padding.top : _spacingNum;
            float up = _viewportItem.rightUpCornerInContent.y - padding;
            return up - localRect.yMax;
        }

        /// <summary>
        /// 本身底部坐标，加上间隙到底部的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToBottomViewSize(int total)
        {
            float padding = Index == (total - 1) ? _padding.bottom : _spacingNum;
            float down = _viewportItem.leftDownCornerInContent.y + padding;
            return localRect.yMin - down;
        }

        /// <summary>
        /// 本身左侧坐标，加上间隙到左侧的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToLeftViewSize()
        {
            float padding = Index == 0 ? _padding.left : _spacingNum;
            float left = _viewportItem.leftDownCornerInContent.x + padding;
            return localRect.xMin - left;
        }

        /// <summary>
        /// 本身底部坐标，加上间隙到底部的距离
        /// 大于等于0：在显示区域内部，小于0：在外部
        /// 因为加了间隙，所以，如果在内部就表示可以创建对象，在外部则可以回收对象
        /// </summary>
        public float GetPaddingToRightViewSize(int total)
        {
            float padding = Index == (total - 1) ? _padding.right : _spacingNum;
            float right = _viewportItem.rightUpCornerInContent.x - padding;
            return right - localRect.xMax;
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

            Vector3 worldMin = _content.TransformPoint(localRect.min);
            Vector3 worldMax = _content.TransformPoint(localRect.max);

            Gizmos.DrawLine(worldMin, new Vector2(worldMin.x, worldMax.y));
            Gizmos.DrawLine(new Vector2(worldMin.x, worldMax.y), worldMax);
            Gizmos.DrawLine(worldMax, new Vector2(worldMax.x, worldMin.y));
            Gizmos.DrawLine(new Vector2(worldMax.x, worldMin.y), worldMin);
        }
    }
}