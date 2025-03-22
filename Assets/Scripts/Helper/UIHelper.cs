namespace Game
{
    public static class UIHelper
    {
        /// <summary>
        /// 创建 Unit Pool，这里可能会做一些额外的操作,比如收集节点
        /// 方法不能动！！！！
        /// <summary>
        public static UnitPool<T> RegisterUnitPool<T>(this IRegisterUnitPoolVisitor self, T ub)
            where T : UnitBase, new()
        {
            var pool = CtorUtility.CreatePrivate<UnitPool<T>>();
            pool.Init(ub);
            return pool;
        }

        public static UnitPool<T> RegisterUnitPool<T>(T unit) where T : UnitBase, new()
        {
            var pool = CtorUtility.CreatePrivate<UnitPool<T>>();
            pool.Init(unit);
            return pool;
        }
    }
}