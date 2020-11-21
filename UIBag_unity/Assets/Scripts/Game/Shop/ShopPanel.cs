using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///</summary>
public class ShopPanel : BasePanel 
{
    public Transform content;
    private void Start()
    {
        this.GetControl<Button>("btnClose").onClick.AddListener(() =>
        {
            UIManager.GetInstance().HidePanel("ShopPanel");
        });
    }
    public override void ShowMe()
    {
        base.ShowMe();
        //根据数据进行初始化
        for (int i = 0; i < GameDataMgr.GetInstance().shopInfos.Count; i++)
        {
            //实例化出来shopcell对象
            ShopCell cell = ResMgr.GetInstance().Load<GameObject>("UI/ShopCell").GetComponent<ShopCell>();
            //设置父对象
            cell.transform.SetParent(content);
            //设置相对大小
            cell.transform.localScale = Vector3.one;
            
            cell.InitInfo(GameDataMgr.GetInstance().shopInfos[i]);
        }
    }
}

