using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    // Start is called before the first frame update
    void Start()
    {
        //背包打开
        this.GetControl<Button>("btn_role").onClick.AddListener(()=>
        {
            UIManager.GetInstance().ShowPanel<BagPanel>("BagPanel", E_UI_Layer.Top);
            UIManager.GetInstance().ShowPanel<RolePanel>("RolePanel", E_UI_Layer.Bot);

        });
        //商店打开
        this.GetControl<Button>("btn_shop").onClick.AddListener(() =>
        {
            UIManager.GetInstance().ShowPanel<ShopPanel>("ShopPanel", E_UI_Layer.Top);

        });
        //加钱
        this.GetControl<Button>("addMoney").onClick.AddListener(() =>
        {
            EventCenter.GetInstance().EventTrigger<int>("MoneyChange", 1000);
        });
        //加宝石
        this.GetControl<Button>("addGem").onClick.AddListener(() =>
        {
            EventCenter.GetInstance().EventTrigger<int>("GemChange", 1000);
        });
    }
    public override void ShowMe()
    {
        base.ShowMe();
        //更新名字和钱、等级等等
        GetControl<Text>("player_name").text = GameDataMgr.GetInstance().playerInfo.name;
        GetControl<Text>("player_level").text = GameDataMgr.GetInstance().playerInfo.lev.ToString();
        GetControl<Text>("textMoney").text = GameDataMgr.GetInstance().playerInfo.money.ToString();
        GetControl<Text>("textGem").text = GameDataMgr.GetInstance().playerInfo.gem.ToString();
        GetControl<Text>("textPro").text = GameDataMgr.GetInstance().playerInfo.pro.ToString();
        EventCenter.GetInstance().AddEventListener<int>("MoneyChange", UpdatePanelText);
        EventCenter.GetInstance().AddEventListener<int>("GemChange", UpdatePanelText);
    }
    public override void HideMe()
    {
        base.HideMe();
        EventCenter.GetInstance().RemoveEventListener<int>("MoneyChange", UpdatePanelText);
        EventCenter.GetInstance().RemoveEventListener<int>("GemChange", UpdatePanelText);
    }
    public void UpdatePanelText(int money)
    {
        GetControl<Text>("textMoney").text = GameDataMgr.GetInstance().playerInfo.money.ToString();
        GetControl<Text>("textGem").text = GameDataMgr.GetInstance().playerInfo.gem.ToString();
    }
}
