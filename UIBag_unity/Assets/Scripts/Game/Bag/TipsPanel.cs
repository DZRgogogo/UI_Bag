using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
///<summary>
///道具装备详细信息面板
///</summary>
public class TipsPanel : BasePanel 
{
    public void InitInfo(ItemInfo info)
    {
        Item itemData = GameDataMgr.GetInstance().GetItemInfo(info.id);
        //使用我们的道具表中的数据
        GetControl<Image>("imageIcon").sprite = ResMgr.GetInstance().Load<Sprite>("Icon/" + itemData.icon);
        //数量
        GetControl<Text>("txtNum").text = "数量: " + info.num.ToString();

        GetControl<Text>("txtName").text = itemData.name;

        GetControl<Text>("txtTips").text = itemData.tips;

    }
}

