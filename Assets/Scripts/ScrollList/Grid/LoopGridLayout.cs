// **************************************************************
// Script id: LoopGridLayout
// Author: songqz
// Time : 2025/1/9 9:36:12
// Des: 布局
// **************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LoopGridLayout
    {
        public Vector2 spacing { private set; get; }
        public RectOffset padding { private set; get; }
        public Vector2 cellSize { private set; get; }
        public int constraintCount { private set; get; }    // 列数据

        public LoopGridLayout(GridLayoutGroup layout)
        {
            spacing = layout.spacing;
            padding = layout.padding;
            cellSize = layout.cellSize;
            constraintCount = Mathf.Max(1, layout.constraintCount);
        }
    }
}