using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
#if UNITY_EDITOR
    public class LoopVerticalScrollEditor
    {
        [UnityEditor.MenuItem("CONTEXT/ScrollRect/替换LoopVerticalScroll")]
        private static void CONTEXT_ScrollRect_Instead()
        {
            var selectGo = UnityEditor.Selection.activeObject as GameObject;
            var sr = selectGo.GetComponent<ScrollRect>();
            if (sr != null && sr.GetType() != typeof(LoopVerticalScroll))
            {
                var content = sr.content;
                var horizontal = sr.horizontal;
                var movementType = sr.movementType;
                var inertia = sr.inertia;
                var decelerationRate = sr.decelerationRate;
                var scrollSensitivity = sr.scrollSensitivity;
                var viewport = sr.viewport;
                var horizontalScrollbar = sr.horizontalScrollbar;
                var verticalScrollbar = sr.verticalScrollbar;
                UnityEngine.Object.DestroyImmediate(sr);
                var xr = selectGo.AddComponent<LoopVerticalScroll>();
                xr.content = content;
                xr.horizontal = horizontal;
                xr.movementType = movementType;
                xr.inertia = inertia;
                xr.decelerationRate = decelerationRate;
                xr.scrollSensitivity = scrollSensitivity;
                xr.viewport = viewport;
                xr.horizontalScrollbar = horizontalScrollbar;
                xr.verticalScrollbar = verticalScrollbar;

                if (selectGo.name.StartsWith("Sv_", StringComparison.OrdinalIgnoreCase))
                    selectGo.name = "LoopVSv_" + selectGo.name.Substring(3);
                else if (!selectGo.name.StartsWith("LoopVSv_")) selectGo.name = "LoopVSv_View";
            }
        }
    }

    public class LoopHorizonScrollEditor
    {
        [UnityEditor.MenuItem("CONTEXT/ScrollRect/替换LoopHorizonScroll")]
        private static void CONTEXT_ScrollRect_Instead()
        {
            var selectGo = UnityEditor.Selection.activeObject as GameObject;
            var sr = selectGo.GetComponent<ScrollRect>();
            if (sr != null && sr.GetType() != typeof(LoopHorizonScroll))
            {
                var content = sr.content;
                var horizontal = sr.horizontal;
                var movementType = sr.movementType;
                var inertia = sr.inertia;
                var decelerationRate = sr.decelerationRate;
                var scrollSensitivity = sr.scrollSensitivity;
                var viewport = sr.viewport;
                var horizontalScrollbar = sr.horizontalScrollbar;
                var verticalScrollbar = sr.verticalScrollbar;
                UnityEngine.Object.DestroyImmediate(sr);
                var xr = selectGo.AddComponent<LoopHorizonScroll>();
                xr.content = content;
                xr.horizontal = true;
                xr.vertical = false;
                xr.movementType = movementType;
                xr.inertia = inertia;
                xr.decelerationRate = decelerationRate;
                xr.scrollSensitivity = scrollSensitivity;
                xr.viewport = viewport;
                xr.horizontalScrollbar = horizontalScrollbar;
                xr.verticalScrollbar = verticalScrollbar;

                if (selectGo.name.StartsWith("Sv_", StringComparison.OrdinalIgnoreCase))
                    selectGo.name = "LoopHSv_" + selectGo.name.Substring(3);
                else if (!selectGo.name.StartsWith("LoopHSv_")) selectGo.name = "LoopHSv_View";
            }
        }
    }
#endif
}