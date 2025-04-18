// **************************************************************
// Script id: LoopHorizonScroll
// Author: Yumuno
// Time : 2025/1/9 9:36:12
// Des: 水平滚动虚拟列表
// 函数说明：
//  InitLoop => 初始化滚动列表
//  GetLoopObject => 获取滚动对象
//  InsertAt => 在指定索引位置添加一个数据
//  RemoveAt => 移除数据
//  AddAtBottom => 在底部添加一定数量的数据
//  ShowAtIndex => 指定那个索引数据显示出来
//  GetLoopItemShowStatus => 获取指定位置对象的显示状态
//  GetObjectInView => 获取当前可见项列表
//  GetFirstIndexInView => 获取可见范围内第一个对象的序号
//  GetLastIndexInView => 获取可见范围内最后一个对象的序号
// **************************************************************

using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class LoopHorizonScroll : LoopScroll, IPointerDownHandler, ILoopScroll
    {
        [SerializeField] private Camera UICamera;

        public Action<PointerEventData> OnBeginDragCallback;
        public Action<PointerEventData> OnDragCallback;
        public Action<PointerEventData> OnEndDragCallback;

        private Func<int, ILoopObject> _itemShowAction;

        private int _objectCount; // 数据数量
        private RectOffset _padding; // 布局编辑间隙
        private float _spacingNum; // 数据间的间隙
        private TextAnchor _childAligin; //对齐方式

        private List<LoopHorizonItem> _items = new(); // 显示列表
        private Queue<LoopHorizonItem> _pool = new(); // 对象池
        private ViewportItem _viewportItem;
        private float contentSize => Mathf.Min(50000, viewport.rect.width * 100); // content大小
        private bool _isDraging = false;
        private bool _isClamping = false; // 首尾超出边界，是否要回弹的标志位

        protected override void Awake()
        {
            base.Awake();
            UICamera = GetComponentInParent<Canvas>()?.rootCanvas?.worldCamera;
            _viewportItem = new ViewportItem(viewport, content, UICamera);
        }

        /// <summary>
        /// 获取挂在节点
        /// </summary>
        /// <returns></returns>
        public RectTransform GetContent()
        {
            return content;
        }

        /// <summary>
        /// 初始化显示位置
        /// atTop ： 是否从左边开始填满
        /// 以startIndex为第一个分配位置，然后后面自动填满和回收
        /// </summary>
        public void InitLoop(int count, Func<int, ILoopObject> showAction, int startIndex = 0, bool atTop = true)
        {
            if (_padding == null)
            {
                var csf = content.GetComponent<ContentSizeFitter>();
                if (csf != null) csf.enabled = false;

                var horizontalLayoutGroup = content.GetComponent<HorizontalLayoutGroup>();
                if (horizontalLayoutGroup != null && horizontalLayoutGroup.enabled)
                {
                    horizontalLayoutGroup.enabled = false;
                    _padding = horizontalLayoutGroup.padding;
                    _spacingNum = horizontalLayoutGroup.spacing;
                    _childAligin = horizontalLayoutGroup.childAlignment;
                }
                else
                {
                    _padding = new RectOffset();
                    _spacingNum = 0;
                }
            }

            content.sizeDelta = new Vector2(contentSize * 2, content.sizeDelta.x);
            horizontalNormalizedPosition = 0.5f;
            _objectCount = count;
            _itemShowAction = showAction;
            InitCreate(startIndex, atTop);
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        private void InitCreate(int startIndex, bool atTop = true)
        {
            InitStatus();
            startIndex = Mathf.Clamp(startIndex, 0, _objectCount - 1);
            if (_objectCount > 0)
            {
                var item = Spwan(startIndex);
                _items.Add(item);
                item.rectTransform.SetAsLastSibling();
                item.InitFirstPosition(atTop);
            }

            LoopHandle(true);
        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        private void InitStatus()
        {
            DespwanRange(0, _items.Count);
            StopMovement();
            _isClamping = true;
            _viewportItem.PreCalculate();
        }


        /// <summary>
        /// 获取制定位置的循环对象
        /// </summary>
        private LoopHorizonItem GetLoopItem(int index)
        {
            for (var i = 0; i < _items.Count; i++)
                if (_items[i].Index == index)
                    return _items[i];

            return null; // 表示找不到
        }


        /// <summary>
        /// 获取制定位置的循环对象
        /// </summary>
        public ILoopObject GetLoopObject(int index)
        {
            return GetLoopItem(index)?.loopObject;
        }

        /// <summary>
        /// 获取制定位置的循环对象
        /// </summary>
        public LoopObjectStatus GetLoopItemShowStatus(int index)
        {
            var item = GetLoopItem(index);
            return item == null ? LoopObjectStatus.NotShow : item.GetObjectViewStatus();
        }

        /// <summary>
        /// 在制定位置添加一些数据
        /// 只能一个个加
        /// </summary>
        public void InsertAt(int dataIndex)
        {
            _objectCount += 1;
            _isClamping = true;
            StopMovement();

            if (_objectCount == 1) // 没有数据时，重新初始化一遍
            {
                InitCreate(0);
            }
            else
            {
                var insertIndex = _items.FindIndex(x => x.Index == dataIndex);
                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (item.Index >= dataIndex) item.IndexRefresh(item.Index + 1);
                }

                if (insertIndex >= 0)
                {
                    // 这里插入，后面的往后怼，并且如果越界会被回收
                    var newItem = Spwan(dataIndex);
                    newItem.InitFirstPosition(true);
                    _items.Insert(insertIndex, newItem);
                    newItem.rectTransform.SetAsLastSibling();
                    newItem.rectTransform.SetSiblingIndex(_items[insertIndex].rectTransform.GetSiblingIndex());
                }
            }

            LoopHandle();
        }

        /// <summary>
        /// 在底部添加一些数据
        /// </summary>
        public void AddAtBottom(int count, bool stop = true)
        {
            if (stop) StopMovement();

            _isClamping = true;
            _objectCount += count;
            if (_items.Count == 0) InitCreate(0, true);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        public void RemoveAt(int dataIndex)
        {
            _objectCount -= 1;
            _isClamping = true;
            StopMovement();

            var insertIndex = _items.FindIndex(x => x.Index == dataIndex);
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item.Index > dataIndex) item.IndexRefresh(item.Index - 1);
            }

            if (insertIndex >= 0) Despwan(_items[insertIndex]);

            LoopHandle();
        }

        /// <summary>
        /// 显示最底部的那条数据
        /// </summary>
        public void ShowAtBottom()
        {
            if (_items.Count > 0)
            {
                _isClamping = true;
                var lastStatus = GetObjectViewStatus(_objectCount - 1);
                // 如果最后一个没有显示，则从新刷新一遍，否则直接移动content
                if (lastStatus == LoopObjectStatus.NotShow)
                {
                    InitCreate(_objectCount - 1, false);
                }
                else
                {
                    // 因为最后一个已经显示，所以底部正常已经夹紧
                    var lastPadding = _items[_items.Count - 1].GetPaddingToRightViewSize(_objectCount);
                    content.anchoredPosition += new Vector2(0, -lastPadding);
                }
            }
        }

        /// <summary>
        /// 显示制定位置
        /// </summary>
        public bool ShowAtIndex(int startIndex)
        {
            // 不存在
            if (startIndex >= _objectCount) return false;

            var item = GetLoopItem(startIndex);
            if (item != null && item.GetObjectViewStatus() == LoopObjectStatus.Full)
                // 在屏幕内，直接显示
                return true;

            InitCreate(startIndex, true);
            return true;
        }

        /// <summary>
        /// 开始处理
        /// </summary>
        private void LoopHandle(bool isInit = false)
        {
            _viewportItem?.PreCalculate();

            //Debug.Log("LoopHandle");
            // 计算一遍item的对象状态
            var count = _items.Count;
            if (count > 0)
            {
                LoopHandleRecycle(isInit);
                LoopHandleCreate(isInit);

                // 位置重新调整，因为可能会在外部调整,一律往后怼
                var anchorItem = _items[0];
                anchorItem.ResetRectBySizeDelta();
                count = _items.Count; // 这里的count可能会在创建和移除的情况下发生变化
                for (var i = 1; i < count; i++)
                {
                    var item = _items[i];
                    item.ResetRectBySizeDelta();
                    item.SetLocalPositionAtDown(anchorItem.localRect);
                    anchorItem = item;
                }
            }
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        private void LoopHandleCreate(bool isInit)
        {
            // 左创建
            for (var i = 0; i < 100; i++)
            {
                var first = _items[0];
                var leftPadding = first.GetPaddingToLeftViewSize();
                if (leftPadding > 0)
                {
                    var index = first.Index - 1;
                    if (index >= 0)
                    {
                        var item = Spwan(index);
                        item.InitLocalPositionAtUp(first.localRect, isInit);
                        _items.Insert(0, item);
                        item.rectTransform.SetAsFirstSibling();
                        continue;
                    }
                }

                break;
            }

            // 右创建
            for (var i = 0; i < 10; i++)
            {
                var last = _items[_items.Count - 1];
                var bottomPadding = last.GetPaddingToRightViewSize(_objectCount);
                if (bottomPadding > 0)
                {
                    var index = last.Index + 1;
                    if (index < _objectCount)
                    {
                        var item = Spwan(index);
                        item.InitLocalPositionAtDown(last.localRect, isInit);
                        _items.Add(item);
                        item.rectTransform.SetAsLastSibling();
                        continue;
                    }
                }

                break;
            }
        }

        /// <summary>
        /// 超出边界回收
        /// </summary>
        private void LoopHandleRecycle(bool isInit)
        {
            // 上回收（只有超出大于两个的情况下才会回收）
            for (var i = 0; i < 100 && _items.Count >= 2; i++)
            {
                var status = _items[0].GetObjectViewStatus();
                if (status == LoopObjectStatus.LeftOut || status == LoopObjectStatus.RightOut)
                {
                    Despwan(_items[0]);
                    continue;
                }

                break;
            }

            // 下回收(只有超出大于两个的情况下才会回收)
            for (var i = 0; i < 100 && _items.Count >= 2; i++)
            {
                var last = _items[_items.Count - 1];
                var status = last.GetObjectViewStatus();
                if (status == LoopObjectStatus.LeftOut || status == LoopObjectStatus.RightOut)
                {
                    Despwan(last);
                    continue;
                }

                break;
            }
        }

        /// <summary>
        /// 判断对象在现实区域里面的状态
        /// </summary>
        public LoopObjectStatus GetObjectViewStatus(int index)
        {
            var item = GetLoopItem(index);
            return item == null ? LoopObjectStatus.NotShow : item.GetObjectViewStatus();
        }

        /// <summary>
        /// 获取当前可见项列表
        /// </summary>
        /// <returns></returns>
        public List<LoopItem> GetObjectInView()
        {
            var result = new List<LoopItem>();
            result.AddRange(_items);
            return result;
        }

        /// <summary>
        /// 获取可见范围内第一个对象的序号
        /// </summary>
        /// <returns></returns>
        public int GetFirstIndexInView(LoopObjectStatus type = LoopObjectStatus.Full)
        {
            foreach (var item in _items)
                if (item.GetObjectViewStatus() == type)
                    return item.Index;

            return 0;
        }

        /// <summary>
        /// 增加监听
        /// </summary>
        /// <param name="action"></param>
        public void AddValueChangedListener(UnityAction<Vector2> action)
        {
            onValueChanged.AddListener(action);
        }

        /// <summary>
        /// 获取可见范围内最后一个对象的序号
        /// </summary>
        /// <param name="type">是全部可见还是部分可见</param>
        /// <returns></returns>
        public int GetLastIndexInView(LoopObjectStatus type = LoopObjectStatus.Full)
        {
            for (var i = _items.Count - 1; i >= 0; i--)
                if (_items[i].GetObjectViewStatus() == type)
                    return _items[i].Index;

            return 0;
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        private LoopHorizonItem Spwan(int idx)
        {
            if (_items.Count >= 400) Debug.LogError("数据异常！！！！");

            var item = _pool.Count > 0 ? _pool.Dequeue() : new LoopHorizonItem();
            item.Init(content, _viewportItem, idx, _padding, _spacingNum, _childAligin);
            var isFirst = _items.Count == 0 || _items[0].Index < idx;
            // 这里考虑捕获异常
            try
            {
                var loopObj = _itemShowAction(idx);
                item.SetObject(loopObj);
            }
            catch (Exception e)
            {
                Debug.LogError("创建对象失败：" + e);
            }

            //Debug.Log("Spawn");
            return item;
        }

        /// <summary>
        /// 回收
        /// </summary>
        private void Despwan(LoopHorizonItem item)
        {
            _pool.Enqueue(item);
            _items.Remove(item);
            item.DespawnSelf();
            //Debug.Log("DespawnSelf");
        }

        /// <summary>
        /// 回收多个
        /// </summary>
        private void DespwanRange(int start, int count)
        {
            for (var i = _items.Count - 1; i >= start; i--) Despwan(_items[i]);
        }

        /// <summary>
        /// 统一在帧里面执行创建和回收
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();

            //_viewportItem?.PreCalculate();
            LoopHandle();
            MovementRecovery();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 开始拖动前，都把content移动到中间
            var x = content.anchoredPosition.x;
            horizontalNormalizedPosition = 0.5f;
            var delta = content.anchoredPosition.x - x;
            foreach (var item in _items) item.AddLocalPosition(-delta);

            _isClamping = false;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _isDraging = true;
            _isClamping = false;
            base.OnBeginDrag(eventData);
            OnBeginDragCallback?.Invoke(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            _viewportItem?.PreCalculate();
            LoopHandle();
            // 由于该脚本必须在ScrollRect执行，但不知道执行顺序，所有放在LateUpdate中执行
            if (movementType == MovementType.Clamped) base.OnDrag(eventData); // Clamped优先执行

            if (MovementElasticHandle(eventData) == false && movementType != MovementType.Clamped)
                base.OnDrag(eventData);

            OnDragCallback?.Invoke(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            _isDraging = false;
            _isClamping = true;
            base.OnEndDrag(eventData);
            // 判断是否需要移动回去
            OnEndDragCallback?.Invoke(eventData);
        }

        /// <summary>
        /// 弹性判断
        /// </summary>
        private bool MovementElasticHandle(PointerEventData eventData)
        {
            if (_items.Count == 0) return false;
            var first = _items[0];
            var last = _items[_items.Count - 1];
            if (first.Index == 0)
            {
                var delta = first.GetPaddingToLeftViewSize();
                if (delta > 0)
                {
                    if (movementType == MovementType.Elastic)
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position,
                            eventData.pressEventCamera, out var p1);
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(content,
                            eventData.position - eventData.delta, eventData.pressEventCamera, out var p2);
                        content.anchoredPosition -= new Vector2((p2.x - p1.x) * elasticity, 0);
                    }
                    else
                    {
                        content.anchoredPosition -= new Vector2(delta, 0);
                    }

                    return true;
                }
            }
            else if (last.Index == _objectCount - 1)
            {
                var delta = last.GetPaddingToRightViewSize(_objectCount);
                if (delta > 0)
                {
                    if (movementType == MovementType.Elastic)
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position,
                            eventData.pressEventCamera, out var p1);
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(content,
                            eventData.position - eventData.delta, eventData.pressEventCamera, out var p2);
                        content.anchoredPosition -= new Vector2((p2.x - p1.x) * elasticity, 0);
                    }
                    else
                    {
                        content.anchoredPosition -= new Vector2(delta, 0);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 位置超框恢复
        /// </summary>
        private void MovementRecovery()
        {
            if (_isDraging || _items.Count == 0)
                return;

            if (_isClamping || velocity != Vector2.zero)
            {
                if (_items[0].Index == 0)
                {
                    var delta = _items[0].GetPaddingToLeftViewSize();
                    if (delta > 0.001f)
                    {
                        StopMovement();
                        _isClamping = true; // 大力拖拽后，靠速度触发，此时暂停移动需要设置标志
                        var localPos = content.localPosition;
                        var conenteEndPos = localPos - new Vector3(delta, 0, 0);
                        content.localPosition = Vector3.Lerp(localPos, conenteEndPos, Time.deltaTime * 10);
                        return;
                    }
                    else if (delta >= -0.01f)
                    {
                        _isClamping = false;
                        return; // 到顶了不需要移动了
                    }
                }

                if (_items[_items.Count - 1].Index == _objectCount - 1)
                {
                    var delta = _items[_items.Count - 1].GetPaddingToRightViewSize(_objectCount);
                    if (delta > 0.001f)
                    {
                        StopMovement();
                        _isClamping = true; // 大力拖拽后，靠速度触发，此时暂停移动需要设置标志
                        var localPos = content.localPosition;
                        var conenteEndPos = localPos + new Vector3(delta, 0, 0);
                        content.localPosition = Vector3.Lerp(localPos, conenteEndPos, Time.deltaTime * 10);
                        return;
                    }

                    _isClamping = false;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (var i = 0; i < _items.Count; i++) _items[i].OnDrawGizmos();
        }
#endif
    }
}