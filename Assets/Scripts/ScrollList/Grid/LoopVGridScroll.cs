// **************************************************************
// Script id: LoopVGridScroll
// Author: songqz
// Time : 2025/1/9 9:36:12
// Des: 垂直网格滚动虚拟列表, 必须实现挂载好GridLayoutGroup调整好参数,后续所有布局都按照该参数设置
//      不能动态增删添加对象
// 函数说明：
//  InitLoop => 初始化滚动列表
// **************************************************************

using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Game
{
    public class LoopVGridScroll : ScrollRect
    {
        [SerializeField] private Camera UICamera;
        private Func<GridData, ILoopObject> _itemShowAction;
        private int _objectCount;               // 数据数量
        private Queue<LoopGridLine> _linePool = new Queue<LoopGridLine>();
        private List<LoopGridLine> _showLines = new List<LoopGridLine>();       // 当期现实的行
        private LoopGridLayout _layout;
        private int constraintCount => _layout.constraintCount;
        private int lineCount => ((_objectCount + constraintCount - 1) / constraintCount);

        protected override void Awake()
        {
            base.Awake();
            UICamera = GetComponentInParent<Canvas>()?.rootCanvas?.worldCamera;
        }

        /// <summary>
        /// 初始化显示位置
        /// atTop ： 是否从底部开始填满
        /// 以startIndex为第一个分配位置，然后后面自动填满和回收
        /// </summary>
        public void InitLoop(int count, Func<GridData, ILoopObject> showAction, bool atTop = true)
        {
            if (_layout == null)
            {
                var ly = content.GetComponent<GridLayoutGroup>();
                var csf = content.GetComponent<ContentSizeFitter>();
                if (ly == null)
                {
                    Debug.LogError("LoopVGridScroll 需要绑定GridLayoutGroup");
                    return;
                }

                _layout = new LoopGridLayout(ly);
                ly.enabled = false;
                if (csf != null)
                {
                    csf.enabled = false;
                }
            }

            var padding = _layout.padding;
            var spacingNum = _layout.spacing;
            var cellSize = _layout.cellSize;
            //var constraintCount = _layout.constraintCount;

            verticalNormalizedPosition = atTop ? 1 : 0;
            _objectCount = count;
            _itemShowAction = showAction;

            float height = lineCount * cellSize.y + spacingNum.y * Mathf.Max(0, lineCount - 1) + padding.top + padding.bottom;
            content.sizeDelta = new Vector2(content.sizeDelta.x, height);

            foreach (var item in _showLines)
            {
                item.DespawnLine();
                _linePool.Enqueue(item);
            }
            _showLines.Clear();
            CreateGameObject();
        }

        /// <summary>
        /// 刷新实体
        /// </summary>
        private Vector3 _contentPosition = Vector3.zero;
        private Vector3[] _viewCorners = new Vector3[4];
        private void CreateGameObject()
        {
            _contentPosition = content.localPosition;

            viewport.GetWorldCorners(_viewCorners);
            var corner1Sn = RectTransformUtility.WorldToScreenPoint(UICamera, _viewCorners[0]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, corner1Sn, UICamera, out Vector2 leftDown);

            var corner2Sn = RectTransformUtility.WorldToScreenPoint(UICamera, _viewCorners[2]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, corner2Sn, UICamera, out Vector2 rightUp);

            // 计算上面超框数量
            float length = Mathf.Abs(rightUp.y);               // 距离顶部的现实区域
            length = Mathf.Max(0, length - _layout.padding.top);
            int noShowLine = (int)(length / (_layout.spacing.y + _layout.cellSize.y));

            // 计算显示数据+上面超框数量
            float length2 = Mathf.Abs(leftDown.y);
            length2 = Mathf.Max(0, length2 - _layout.padding.top);
            int upShowLine = (int)(length2 / (_layout.spacing.y + _layout.cellSize.y));     // 因为后面半个也要显示，索性都给补1

            // int start = Mathf.Max(0, noShowLine);
            // int end = start + (upShowLine - noShowLine);
            // 从上往下拖拽的安全范围，避免提前回收
            int bufferLineCount = 2;
            int start = Mathf.Max(0, noShowLine - bufferLineCount);
            int end = Mathf.Min(lineCount - 1, upShowLine + bufferLineCount);
            
            // 回收不在显示范围内的
            for (int i = _showLines.Count - 1; i >= 0; i--)
            {
                var item = _showLines[i];
                int lineNum = item.lineNum;
                if (lineNum < start || lineNum > end)
                {
                    _linePool.Enqueue(item);
                    item.DespawnLine();
                    _showLines.RemoveAt(i);
                }
            }

            if (_showLines.Count > 100)
            {
                Debug.LogError("数据异常!!!!");
                return;
            }

            // 生成队列里面没有的
            int lineCnt = lineCount;
            for (int i = start; i <= end && i < lineCnt; i++)
            {
                if (!ContainsLine(i))
                {
                    LoopGridLine line = SpawnLine(i);
                    _showLines.Add(line);
                }
            }
        }

        /// <summary>
        /// 是否包含指定行
        /// </summary>
        private bool ContainsLine(int lineNum)
        {
            for (int i = 0; i < _showLines.Count; i++)
            {
                if (_showLines[i].lineNum == lineNum)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        private LoopGridLine SpawnLine(int lineNum)
        {
            LoopGridLine line = _linePool.Count > 0 ? _linePool.Dequeue() : new LoopGridLine();
            int startIndex = lineNum * constraintCount;
            int remain = _objectCount - startIndex;
            int count = Mathf.Min(constraintCount, remain);
            line.InitLine(content, _layout, lineNum, count, _itemShowAction);
            return line;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (_contentPosition == content.localPosition)
            {
                return;
            }
            CreateGameObject();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (int i = 0; i < _showLines.Count; i++)
            {
                foreach (var item in _showLines[i].items)
                {
                    item.OnDrawGizmos();
                }
            }
        }

#endif
    }
}