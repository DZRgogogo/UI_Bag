using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包面板 主要是用来更新背包逻辑
/// </summary>
public class BagPanel : BasePanel
{
    CustomSV<Item, BagItem> sv;
    public RectTransform content;
    // Use this for initialization
    void Start () {
        //CustomSV里面限制的BagItem必须要实现接口IItemBase
        sv = new CustomSV<Item, BagItem>();
        //初始预设体名
        sv.InitItemResName("UI/BagItem");
        //初始化格子间隔大小 以及 一行几列
        sv.InitItemSizeAndCol(300, 250, 2);
        sv.InitContentAndSVH(content,925);
        sv.InitInfos(BagMgr.GetInstance().items);
    
    }
    void Update()
    {
        sv.CheckShowOrHide();
    }
}
