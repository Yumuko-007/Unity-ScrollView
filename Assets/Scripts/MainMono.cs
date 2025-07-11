using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

/// <summary>
/// 测试用脚本
/// </summary>
public class MainMono : MonoBehaviour
{
    public LoopVerticalScroll _loopList;
    public LoopHorizonScroll _loopHorizonList;
    public LoopVGridScroll _LoopVGridList;
    public ILoopScroll LoopList;
    public ILoopScroll LoopHorizonList;
    public UnitPool<ItemUnit> UnitPool;
    public UnitPool<ItemUnit> UnitPool2;

    public ItemUnit Unit;

    public ItemUnit Unit2;

    // Start is called before the first frame update
    private void Start()
    {
        LoopList = _loopList as ILoopScroll;
        LoopHorizonList = _loopHorizonList as ILoopScroll;
        UnitPool = UIHelper.RegisterUnitPool(Unit);
        UnitPool2 = UIHelper.RegisterUnitPool(Unit2);
        LoopList.InitLoop(100, ScrollFunc);

        LoopHorizonList.InitLoop(100, ScrollHorizonFunc);
        LoopHorizonList.AddValueChangedListener(OnScoll);
        
        _LoopVGridList.InitLoop(100, ScrollVGridFunc);
    }

    private ILoopObject ScrollFunc(int index)
    {
        var item = UnitPool.Spwan(LoopList.GetContent());
        item.SetData(index);
        return item;
    }

    private ILoopObject ScrollHorizonFunc(int index)
    {
        var item = UnitPool2.Spwan(LoopHorizonList.GetContent());
        item.SetData(index);
        return item;
    }
    
    private ILoopObject ScrollVGridFunc(int index)
    {
        var item = UnitPool2.Spwan(_LoopVGridList.content);
        item.SetData(index);
        return item;
    }

    private void OnScoll(Vector2 pos)
    {
    }


    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoopList.ShowAtIndex(50);
            LoopHorizonList.ShowAtIndex(50);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.LogError($"当前横向可见范围内第一个全显示的Item序号是 {LoopHorizonList.GetFirstIndexInView()}");
            Debug.LogError($"当前横向可见范围内最后一个全显示的Item序号是 {LoopHorizonList.GetLastIndexInView()}");

            Debug.LogError($"当前纵向可见范围内第一个全显示的Item序号是 {LoopList.GetFirstIndexInView()}");
            Debug.LogError($"当前纵向可见范围内最后一个全显示的Item序号是 {LoopList.GetLastIndexInView()}");
        }
    }
}