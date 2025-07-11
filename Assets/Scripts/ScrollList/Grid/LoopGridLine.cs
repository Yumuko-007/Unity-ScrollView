// **************************************************************
// Script id: LoopGridLine
// Author:  songqz
// Time : 2025/1/9 9:36:12
// Des: 一行数据
// **************************************************************

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public struct GridData
    {
        public int line;                // 第几行
        public int column;              // 第几列
        public int totalColumn;         // 总共几列

        public int index => line * totalColumn + column;        // 第几个索引
    }

    public class LoopGridLine
    {
        public int lineNum;
        public List<LoopGridItem> items = null;

        private LoopGridLayout _layout;
        public Vector2 spacing => _layout.spacing;
        public RectOffset padding => _layout.padding;
        public Vector2 cellSize => _layout.cellSize;
        public int constraintCount => _layout.constraintCount;     // 列数据

        public Vector2 Position { private set; get; }            // 左上角的位置
        public float Height { private set; get; }
        private RectTransform content;

        /// <summary>
        /// 初始化行
        /// </summary>
        public void InitLine(RectTransform content, LoopGridLayout layoutGroup, int lineNum, int count, Func<GridData, ILoopObject> itemShowAction)
        {
            this.content = content;
            this.lineNum = lineNum;
            _layout = layoutGroup;
            Position = new Vector2(0, -_layout.padding.top - (_layout.cellSize.y + _layout.spacing.y) * lineNum);
            Height = layoutGroup.cellSize.y;
            int max = layoutGroup.constraintCount;
            if (items == null || items.Count != max)
            {
                items = new List<LoopGridItem>();
                for (int i = 0; i < max; i++)
                {
                    items.Add(new LoopGridItem(content));
                }
            }

            // 创建对象
            for (int i = 0; i < count; i++)
            {
                float x = padding.left + cellSize.x * i + _layout.spacing.x * i;
                Vector2 leftUp = new Vector2(x, Position.y);
                int index = lineNum * max + i;

                var obj = Spwan(i, itemShowAction);
                items[i].InitItem(index, leftUp, layoutGroup.cellSize);
                items[i].BindItem(obj);
            }
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        public ILoopObject Spwan(int offset, Func<GridData, ILoopObject> itemShowAction)
        {
            return itemShowAction.Invoke(new GridData() { column = offset, line = lineNum, totalColumn = constraintCount});
        }

        /// <summary>
        /// 回收行
        /// </summary>
        public void DespawnLine()
        {
            foreach (var item in items)
            {
                item.Despawn();
            }
            items.Clear();
        }
    }
}