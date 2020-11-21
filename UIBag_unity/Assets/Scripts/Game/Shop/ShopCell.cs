using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///</summary>
public class ShopCell : BasePanel 
{
    private ShopCellInfo info;
    private void Start()
    {
        GetControl<Button>("btnBuy").onClick.AddListener(BuyItem);
    }
    /// <summary>
    /// 买商品
    /// </summary>
    public void BuyItem()
    {
        print("查看数据1"+info.priceType);
        print("查看数据2" + GameDataMgr.GetInstance().playerInfo.money);
        print("查看数据3" + info.price);
        //判断钱够不够
        if (info.priceType == 1 && GameDataMgr.GetInstance().playerInfo.money >= info.price)
        {
            //数据添加到玩家背包
            GameDataMgr.GetInstance().playerInfo.AddItem(info.itemInfo);
            EventCenter.GetInstance().EventTrigger<int>("MoneyChange", -info.price);
            TipMgr.GetInstance().ShowOneTipPanel("用金币购买成功");
        }
        //判断宝石够不够
        else if (info.priceType == 2 && GameDataMgr.GetInstance().playerInfo.gem >= info.price)
        {
            //数据添加到玩家背包
            GameDataMgr.GetInstance().playerInfo.AddItem(info.itemInfo);
            EventCenter.GetInstance().EventTrigger<int>("GemChange", -info.price);
            TipMgr.GetInstance().ShowOneTipPanel("用宝石购买成功");
        }
        //货币不足
        else
        {
            print("查看现在是不是没钱了");
            //这个panel就是OneBtnTipPanel所创建的对象，所以可以通过这个调用内部的方法,panel是在Uimanager里面创建并传过来的
            UIManager.GetInstance().ShowPanel<OneBtnTipPanel>("BuyTipPanel", E_UI_Layer.Top,(panel)=>
            {
                panel.InitTipText("钱不够了");
            });
        }
    }
    /// <summary>
    /// 初始化每一项的信息
    /// </summary>
    /// <param name="info"></param>
    public void InitInfo(ShopCellInfo info)
    {
        this.info = info;
        //根据售卖的道具id可以得到道具表信息
        Item item = GameDataMgr.GetInstance().GetItemInfo(info.itemInfo.id);
        Debug.Log("ahahhaha  "+ info.itemInfo.id);
        //图标
        GetControl<Image>("imageIcon").sprite = ResMgr.GetInstance().Load<Sprite>("Icon/" + item.icon);
        //个数
        GetControl<Text>("txtNum").text = info.itemInfo.num.ToString();
        //名字
        GetControl<Text>("txtName").text =  item.name;
        //价格图标
        GetControl<Image>("imageType").sprite = ResMgr.GetInstance().Load<Sprite>("Icon/" + (info.priceType == 1 ? "5":"6"));
        //价格
        GetControl<Text>("txtPrice").text = item.price.ToString();
        //描述信息
        GetControl<Text>("txtTips").text = info.tips;
    }
}

