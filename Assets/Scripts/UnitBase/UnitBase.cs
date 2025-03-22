using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class UnitBase : MonoBehaviour, ILoopObject, IRegisterUnitPoolVisitor
    {
        // 所属对象池
        private IUnitPool _unitPool;
        private RectTransform rectTrans;
        public RectTransform rectTransform => rectTrans = rectTrans ?? GetComponent<RectTransform>();
        public RectTransform objTrans => GetComponent<RectTransform>();
        public bool IsDespwaned { private set; get; } = true; // 对象是否已经被回收了

        protected virtual void OnInitial()
        {
        }

        protected virtual void OnSpwan()
        {
        }

        protected virtual void OnDespwan()
        {
        }

        /// <summary>
        /// 绑定对象
        /// </summary>
        public void Initial(IUnitPool pool)
        {
            _unitPool = pool;
            // Initial();
        }

        /// <summary>
        /// 生成时被调用
        /// </summary>
        public void SpwanCall()
        {
            IsDespwaned = false;
            OnSpwan();
        }

        /// <summary>
        /// 回收时被调用
        /// </summary>
        public void DespwanCall()
        {
            IsDespwaned = true;
            OnDespwan();
        }

        public void DespawnObject()
        {
            rectTransform.DOKill();
            _unitPool.Despwan(this);
        }

        /// <summary>
        /// 索引刷新，一般是插入或者移除后发生
        /// </summary>
        public void LoopIndexRefresh(int index)
        {
        }

        /// <summary>
        /// 设置位置坐标，可以重写做动画
        /// </summary>
        public void SetLoopObjectLocalPosition(Vector3 localPos)
        {
            rectTransform.localPosition = localPos;
        }
    }
}