// **************************************************************
// Script id: LoopGridItem
// Author: songqz
// Time : 2025/1/9 9:36:12
// Des: 虚拟对象
// **************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LoopGridItem
    {
        public int Index { set; get; }
        public ILoopObject loopObject { private set; get; }
        private RectTransform _content;
        public Vector2 min { private set; get; }
        public Vector2 max { private set; get; }
        public Vector2 center { private set; get; }
        public Vector2 size { private set; get; }

        public LoopGridItem(RectTransform content)
        {
            _content = content;
        }

        public void InitItem(int index, Vector2 leftUp, Vector2 cellSize)
        {
            Index = index;
            min = new Vector2(leftUp.x, leftUp.y - cellSize.y);
            max = new Vector2(leftUp.x + cellSize.x, leftUp.y);
            center = (min + max) / 2;
            size = cellSize;
            Despawn();
        }

        /// <summary>
        /// 关联实体对象
        /// </summary>
        public void BindItem(ILoopObject loopObj)
        {
            loopObject = loopObj;
            loopObj.objTrans.localPosition = center + (loopObj.objTrans.pivot - new Vector2(0.5f, 0.5f)) * center;
        }

        public void Despawn()
        {
            loopObject?.DespawnObject();
            loopObject = null;
        }

        public void OnDrawGizmos()
        {
            if (loopObject != null)
            {
                Gizmos.color = Color.red;
                Vector3 worldMin = _content.TransformPoint(min);
                Vector3 worldMax = _content.TransformPoint(max);
                Gizmos.DrawLine(worldMin, new Vector2(worldMin.x, worldMax.y));
                Gizmos.DrawLine(new Vector2(worldMin.x, worldMax.y), worldMax);
                Gizmos.DrawLine(worldMax, new Vector2(worldMax.x, worldMin.y));
                Gizmos.DrawLine(new Vector2(worldMax.x, worldMin.y), worldMin);
            }
        }
    }
}