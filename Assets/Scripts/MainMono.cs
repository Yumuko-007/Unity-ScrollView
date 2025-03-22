using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

/// <summary>
/// 测试用脚本
/// </summary>
public class MainMono : MonoBehaviour
{
    public LoopVerticalScroll LoopList;
    public LoopHorizonScroll LoopHorizonList;
    public UnitPool<ItemUnit> UnitPool;
    public UnitPool<ItemUnit> UnitPool2;

    public ItemUnit Unit;
    public ItemUnit Unit2;
    // Start is called before the first frame update
    void Start()
    {
        UnitPool = UIHelper.RegisterUnitPool(Unit);
        UnitPool2 = UIHelper.RegisterUnitPool(Unit2);
        LoopList.InitLoop(100, ScrollFunc);
        
        LoopHorizonList.InitLoop(100, ScrollHorizonFunc);
    }

    private ILoopObject ScrollFunc(int index)
    {
        var item = UnitPool.Spwan(LoopList.content);
        item.SetData(index);
        return item;
    }
    
    private ILoopObject ScrollHorizonFunc(int index)
    {
        var item = UnitPool2.Spwan(LoopHorizonList.content);
        item.SetData(index);
        return item;
    }
    
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoopList.ShowAtIndex(50);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoopHorizonList.ShowAtIndex(50);
        }
    }
}
