using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>
///
///</summary>
public class BagMgr : BaseManager<BagMgr> 
{
	//当前拖动着的格子
	private ItemCell nowSelItem;
	//当前鼠标进入的格子
	private ItemCell nowInItem;
	//当前选中装备的图片信息
	private Image nowSelItemImg;
	//判断是不是在拖动
	private bool isDraging;
	public void Init()
	{
		EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellBeginDrag", BeginDragItemCell);
		EventCenter.GetInstance().AddEventListener<BaseEventData>("ItemCellDrag", DragItemCell);
		EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellEndDrag", EndDragItemCell);
		EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellEnter", EnterItemCell);
		EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellExit", ExitItemCell);
	}
	private void BeginDragItemCell(ItemCell itemCell)
	{
		isDraging = true;
		//记录当前选中的格子
		nowSelItem = itemCell;
		//创建一个拖动的图片
		PoolMgr.GetInstance().GetObj("UI/imgIcon", (obj) =>
		 {
			 nowSelItemImg = obj.GetComponent<Image>();
			 nowSelItemImg.sprite = itemCell.imgIcon.sprite;

			 //设置父对象 改变缩小大小相关
			 nowSelItemImg.transform.SetParent(UIManager.GetInstance().canvas);
			 nowSelItemImg.transform.localScale = Vector3.one;
			 //入过异步加载结束，拖动就已经结束了，那么就直接隐藏
             if (!isDraging)
             {
				 PoolMgr.GetInstance().PushObj(nowSelItemImg.name, nowSelItemImg.gameObject);
			 }
		 });

	}
	private void DragItemCell(BaseEventData eventData)
	{
        //拖动中 更新这个图片的位置
        //把鼠标位置 转换到 UI相关的位置 让 图片跟随鼠标移动
        if (nowSelItemImg == null)
        {
			return;
        }
		Vector2 localPos;
		//用于坐标的转换的api
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			UIManager.GetInstance().canvas,//希望得到坐标结果对象的父对象
			(eventData as PointerEventData).position,// 相当于 鼠标位置 eventData as PointerEventData是父类BaseEventData转化为子类
			(eventData as PointerEventData).pressEventCamera,//相当于 UI摄像机
			out localPos
		);
		nowSelItemImg.transform.localPosition = localPos;
	}
	private void EndDragItemCell(ItemCell itemCell)
	{
		isDraging = false;
		ChangeEquip();
		//清空当前选中的格子
		nowSelItem = null;
        nowInItem = null;
        //结束拖动 移除这个图片
        if (nowSelItemImg == null)
        {
			return;
        }
		PoolMgr.GetInstance().PushObj(nowSelItemImg.name, nowSelItemImg.gameObject);
	}
	private void EnterItemCell(ItemCell itemCell)
	{
        if (isDraging)
        {
			nowInItem = itemCell;
			return;
        }
		//如果格子是空的，就别显示tip面板。例如装备面板中的格子
        if (itemCell.itemInfo == null)
        {
			return;
        }
		UIManager.GetInstance().ShowPanel<TipsPanel>("TipsPanel", E_UI_Layer.Top, (panel) =>
		{
			//异步加载结束 初始化信息
			panel.InitInfo(itemCell.itemInfo);
			//更新位置
			panel.transform.position = itemCell.imgBK.transform.position;
			//如果面板加载完了，发现已经开始拖动了，直接隐藏tips
            if (isDraging)
            {
				UIManager.GetInstance().HidePanel("TipsPanel");
            }
		});
	}
	private void ExitItemCell(ItemCell itemCell)
	{
        if (isDraging)
        {
			nowInItem = null;
        }
		//如果格子是空的，直接都没显示，所以没必要隐藏
		if (itemCell.itemInfo == null)
		{
			return;
		}
		UIManager.GetInstance().HidePanel("TipsPanel");
	}
	public void ChangeEquip()
	{
		//从装备拖装备
        if (nowSelItem.type == E_Item_Type.Bag)
        {
			Debug.Log("1111"+ nowInItem   );
			if (nowInItem != null && nowInItem.type != E_Item_Type.Bag)//进入了格子，并且不是拖到背包内的格子
            {
				Debug.Log("2222");
				//读表
				Item info = GameDataMgr.GetInstance().GetItemInfo(nowSelItem.itemInfo.id);
                //装备交换
                //1、判断 格子类型和装备类型是否一致
                if (nowInItem.itemInfo == null)
                {
					Debug.Log("3333"+"  "+ nowSelItem.itemInfo.id);
					//直接装备 直接从背包中移除 更新面板
					GameDataMgr.GetInstance().playerInfo.equips.Remove(nowSelItem.itemInfo);
					GameDataMgr.GetInstance().playerInfo.nowEquips.Add(nowSelItem.itemInfo);
					List<ItemInfo> nowEquips = GameDataMgr.GetInstance().playerInfo.nowEquips;
					Debug.Log("现在就看看" + "  " + nowEquips[0].id);
					
                }
                else//交换
                {
					Debug.Log("4444");
					//直接装备 直接从背包中移除 更新面板
					GameDataMgr.GetInstance().playerInfo.nowEquips.Remove(nowInItem.itemInfo);
					GameDataMgr.GetInstance().playerInfo.nowEquips.Add(nowSelItem.itemInfo);
					GameDataMgr.GetInstance().playerInfo.equips.Remove(nowSelItem.itemInfo);
					GameDataMgr.GetInstance().playerInfo.equips.Add(nowInItem.itemInfo);
				}
				//更新背包
				UIManager.GetInstance().GetPanel<BagPanel>("BagPanel").ChangeType(E_Bag_Type.Equip);
				//更新人物
				UIManager.GetInstance().GetPanel<RolePanel>("RolePanel").UpdateRolePanel();
				//保存数据
				GameDataMgr.GetInstance().SavePlayerInfo();
			}
        }
        else//从装备栏往外拖动
        {
            //当前从装备栏往外拖出去一个装备 并且没有进入任何装备
            if (nowInItem == null || nowInItem.type != E_Item_Type.Bag)
            {
				GameDataMgr.GetInstance().playerInfo.nowEquips.Remove(nowSelItem.itemInfo);
				GameDataMgr.GetInstance().playerInfo.equips.Add(nowSelItem.itemInfo);
				//更新背包
				UIManager.GetInstance().GetPanel<BagPanel>("BagPanel").ChangeType(E_Bag_Type.Equip);
				//更新人物
				UIManager.GetInstance().GetPanel<RolePanel>("RolePanel").UpdateRolePanel();
				//保存数据
				GameDataMgr.GetInstance().SavePlayerInfo();
            }
            else if (nowInItem != null && nowInItem.type == E_Item_Type.Bag)
            {
				//读表
				Item info = GameDataMgr.GetInstance().GetItemInfo(nowSelItem.itemInfo.id);
                if ((int)nowSelItem.type == info.equipType)
                {
					//直接装备 直接从背包中移除 更新面板
					GameDataMgr.GetInstance().playerInfo.nowEquips.Remove(nowSelItem.itemInfo);
					GameDataMgr.GetInstance().playerInfo.nowEquips.Add(nowInItem.itemInfo);
					GameDataMgr.GetInstance().playerInfo.equips.Remove(nowInItem.itemInfo);
					GameDataMgr.GetInstance().playerInfo.equips.Add(nowSelItem.itemInfo);
					//更新背包
					UIManager.GetInstance().GetPanel<BagPanel>("BagPanel").ChangeType(E_Bag_Type.Equip);
					//更新人物
					UIManager.GetInstance().GetPanel<RolePanel>("RolePanel").UpdateRolePanel();
					//保存数据
					GameDataMgr.GetInstance().SavePlayerInfo();
				}
			}
			
		}
	}
}

