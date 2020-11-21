using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
public class RolePanel : BasePanel 
{
    public ItemCell itemHead;
    public ItemCell itemNeck;
    public ItemCell itemWeapon;
    public ItemCell itemCloth;
    public ItemCell itemTrousers;
    public ItemCell itemShoes;
    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        switch (btnName)
        {
            case "btnClose":
                UIManager.GetInstance().HidePanel("RolePanel");
                break;
        }
    }
    public void UpdateRolePanel()
    {
        List<ItemInfo> nowEquips = GameDataMgr.GetInstance().playerInfo.nowEquips;
        Item itemInfo;
        itemHead.InitInfo(null);
        itemNeck.InitInfo(null);
        itemWeapon.InitInfo(null);
        itemCloth.InitInfo(null);
        itemTrousers.InitInfo(null);
        itemShoes.InitInfo(null);
        for (int i = 0; i < nowEquips.Count; i++)
        {
            Debug.Log("查看一下" + nowEquips[i].id);
            itemInfo = GameDataMgr.GetInstance().GetItemInfo(nowEquips[i].id);
            switch (itemInfo.equipType)
            {
                case (int)E_Item_Type.Head:
                    itemHead.InitInfo(nowEquips[i]);
                    break;
                case (int)E_Item_Type.Neck:
                    itemNeck.InitInfo(nowEquips[i]);
                    break;
                case (int)E_Item_Type.Weapon:
                    itemWeapon.InitInfo(nowEquips[i]);
                    break;
                case (int)E_Item_Type.Cloth:
                    itemCloth.InitInfo(nowEquips[i]);
                    break;
                case (int)E_Item_Type.Trousers:
                    itemTrousers.InitInfo(nowEquips[i]);
                    break;
                case (int)E_Item_Type.Shoes:
                    itemShoes.InitInfo(nowEquips[i]);
                    break;
            }
        }
    }
}

