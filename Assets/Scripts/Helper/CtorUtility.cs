using System;
using System.Reflection;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 创建私有无参构造函数实例
    /// </summary>
    public static class CtorUtility
    {
        public static T CreatePrivate<T>() where T : class
        {
            return CreatePrivate(typeof(T)) as T;
        }

        public static object CreatePrivate(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < constructors.Length; i++)
            {
                var ctor = constructors[i];
                var ps = ctor.GetParameters();
                if ((ps == null) || (ps.Length == 0))
                {
                    return ctor.Invoke(null);
                }
            }
            Debug.LogError(type.Name + " - 未定义无参私有构造函数");
            return null;
        }

        public static object CreatePublic(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < constructors.Length; i++)
            {
                var ctor = constructors[i];
                var ps = ctor.GetParameters();
                if (((ps == null) || (ps.Length == 0)))
                {
                    return ctor.Invoke(null);
                }
            }
            Debug.LogError(type.Name + " - 未定义无参私有构造函数");
            return null;
        }

    }
}