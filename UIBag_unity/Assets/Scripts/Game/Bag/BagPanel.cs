using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 背包的页签状态
/// </summary>
public enum E_Bag_Type
{
    Item = 1,
    Equip = 2,
    Gem = 3
}
public class BagPanel : BasePanel
{
    public Transform content;
    public List<ItemCell> list;
    Dictionary<int, ItemInfo> temp_items = new Dictionary<int, ItemInfo>();
    void Start()
    {
        this.GetControl<Button>("btn_close").onClick.AddListener(() =>
        {
            UIManager.GetInstance().HidePanel("BagPanel");
        });
        //为toggle添加事件监听 来触发 数据更新
        GetControl<Toggle>("togItem").onValueChanged.AddListener(ToggleValueChange);
        GetControl<Toggle>("togEquip").onValueChanged.AddListener(ToggleValueChange);
        GetControl<Toggle>("togGem").onValueChanged.AddListener(ToggleValueChange);
    }
    public override void ShowMe()
    {
        base.ShowMe();
        print("这里是每次都执行吗");
        ChangeType(E_Bag_Type.Item);
    }
    private void ToggleValueChange(bool value)
    {
        if (GetControl<Toggle>("togItem").isOn)
        {
            ChangeType(E_Bag_Type.Item);
        }
        else if (GetControl<Toggle>("togEquip").isOn)
        {
            ChangeType(E_Bag_Type.Equip);
        }
        else
        {
            ChangeType(E_Bag_Type.Gem);
        }
    }
    /// <summary>
    /// 页签切换的函数
    /// </summary>
    /// <param name="type"></param>
    public void ChangeType(E_Bag_Type type)
    {
        //默认值是道具列表信息
        List<ItemInfo> tempInfo = GameDataMgr.GetInstance().playerInfo.items;
        switch (type)
        {
            case E_Bag_Type.Equip:
                tempInfo = GameDataMgr.GetInstance().playerInfo.equips;
                break;
            case E_Bag_Type.Gem:
                tempInfo = GameDataMgr.GetInstance().playerInfo.gems;
                break;
        }
        //更新前先删除之前的格子
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
        list.Clear();
        temp_items.Clear();

        int totalNum = 0;
        ItemInfo item_info = new ItemInfo();
        for (int i = 0; i < tempInfo.Count; i++)
        {
            if (temp_items.ContainsKey(tempInfo[i].id))
            {
                totalNum += tempInfo[i].num;
                temp_items[tempInfo[i].id] = new ItemInfo() { id = tempInfo[i].id, num = totalNum };
            }
            else
            {
                totalNum = tempInfo[i].num;
                temp_items.Add(tempInfo[i].id, new ItemInfo() { id = tempInfo[i].id, num = totalNum });
            }
        }
        //更新现在的数据
        //动态创建 ItemCell 预设体 并且将它存放到list里面
        foreach (var item in temp_items)
        {
            ItemCell cell = ResMgr.GetInstance().Load<GameObject>("UI/ItemCell").GetComponent<ItemCell>();
            cell.transform.parent = content;
            cell.transform.localScale = Vector3.one;
            cell.InitInfo(temp_items[item.Key]);
            list.Add(cell);
        }
    }

}
