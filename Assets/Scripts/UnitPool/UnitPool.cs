// **************************************************************
// Script Name: 
// Author: songqz
// Time : 2022/9/22 10:44:42
// Des: 描述
// **************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace Game
{
    public class UnitPool<T> : IUnitPool where T : UnitBase, new()
    {
        public int Count => _units.Count; // 数量
        public IReadOnlyList<T> Units => _units; // 已经分配对象访问接口

        protected T baseUnit; // 母体对象
        private T _singleUnit; // 母体cell
        private List<T> _units = new(); // 已经分配的对象
        private Queue<T> _poolUnits = new(); // 对象池
        private int _capacity = 2000; // 对象池容量

        private UnitPool()
        {
        }

        /// <summary>
        /// 初始化 isPrefabUnit 是否是挂在ui上的unit
        /// </summary>
        public void Init(T ub)
        {
            baseUnit = ub;
            baseUnit.gameObject.SetActive(ub.transform.parent == null);
        }

        /// <summary>
        /// 设置对象池容量
        /// </summary>
        public void SetPoolCapacity(int value)
        {
            _capacity = value;
        }

        // 获取母体
        public T GetBase()
        {
            if (_singleUnit == false)
            {
                _singleUnit = baseUnit;
                _singleUnit.Initial(this);
            }

            return _singleUnit;
        }

        // 分配对象
        // isCoonectToPool是否加入对象池内置列表
        public T Spwan(Transform rectTrans, bool isCoonectToPool = true)
        {
            T newUnit;
            if (_poolUnits.Count > 0)
            {
                newUnit = _poolUnits.Dequeue();
            }
            else
            {
                newUnit = UnityEngine.Object.Instantiate(baseUnit.gameObject, rectTrans).GetComponent<T>();
                newUnit.Initial(this);
            }

            newUnit.gameObject.SetActive(true);
            newUnit.rectTransform.SetParent(rectTrans);
            if (isCoonectToPool) _units.Add(newUnit);
            newUnit.SpwanCall();
            return newUnit;
        }

        /// <summary>
        /// 放入对象池，如果对象池容器满了，直接销毁
        /// </summary>
        private void SetToPool(T unit)
        {
            unit.DespwanCall();
            if (_poolUnits.Count >= _capacity)
            {
                Debug.LogError("对象池内容太多，执行销毁！！，必要的话，请设置SetPoolCapacity()容器");
                UnityEngine.Object.Destroy(unit.gameObject);
            }
            else
            {
                unit.gameObject.SetActive(false);
                if (_poolUnits.Contains(unit))
                    Debug.LogError("对象被回收多次！！！，不影响后续逻辑，但需要处理！！！");
                else
                    _poolUnits.Enqueue(unit as T);
            }
        }

        // 回收对象
        public void Despwan(UnitBase unit)
        {
            var curUnit = unit as T;
            if (curUnit != null)
            {
                _units.Remove(curUnit);
                SetToPool(curUnit);
            }
            else
            {
                Debug.LogError("非对象池中产物，不能回收！！！！");
            }
        }

        /// <summary>
        /// 查找位置
        /// </summary>
        public int IndexOf(UnitBase unit)
        {
            return _units.IndexOf(unit as T);
        }

        /// <summary>
        /// 遍历
        /// </summary>
        public void Foreach(Action<T> action)
        {
            foreach (var unit in _units) action.Invoke(unit);
        }

        /// <summary>
        /// 回收所有
        /// </summary>
        public void DespwanAll()
        {
            try
            {
                foreach (var unit in _units) SetToPool(unit);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            _units.Clear();
        }
    }
}