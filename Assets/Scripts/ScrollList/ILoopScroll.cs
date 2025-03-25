using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    /// <summary>
    /// 滚动列表接口
    /// </summary>
    public interface ILoopScroll
    {
        /// <summary>
        /// 获取挂在节点
        /// </summary>
        /// <returns></returns>
        public RectTransform GetContent();

        /// <summary>
        /// 初始化显示位置
        /// atTop ： 是否从左边开始填满
        /// 以startIndex为第一个分配位置，然后后面自动填满和回收
        /// </summary>
        public void InitLoop(int count, Func<int, ILoopObject> showAction, int startIndex = 0, bool atTop = true);

        /// <summary>
        /// 获取制定位置的循环对象
        /// </summary>
        public ILoopObject GetLoopObject(int index);

        /// <summary>
        /// 在制定位置添加一些数据
        /// 只能一个个加
        /// </summary>
        public void InsertAt(int dataIndex);

        /// <summary>
        /// 移除数据
        /// </summary>
        public void RemoveAt(int dataIndex);

        /// <summary>
        /// 在底部添加一些数据
        /// </summary>
        public void AddAtBottom(int count, bool stop = true);

        /// <summary>
        /// 显示制定位置
        /// </summary>
        public bool ShowAtIndex(int startIndex);

        /// <summary>
        /// 获取制定位置的循环对象
        /// </summary>
        public LoopObjectStatus GetLoopItemShowStatus(int index);

        /// <summary>
        /// 获取当前可见项列表
        /// </summary>
        /// <returns></returns>
        public List<LoopItem> GetObjectInView();

        /// <summary>
        /// 获取可见范围内第一个对象的序号
        /// </summary>
        /// <returns></returns>
        public int GetFirstIndexInView(LoopObjectStatus type = LoopObjectStatus.Full);

        /// <summary>
        /// 获取可见范围内最后一个对象的序号
        /// </summary>
        /// <param name="type">是全部可见还是部分可见</param>
        /// <returns></returns>
        public int GetLastIndexInView(LoopObjectStatus type = LoopObjectStatus.Full);

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="action"></param>
        public void AddValueChangedListener(UnityAction<Vector2> action);
    }
}