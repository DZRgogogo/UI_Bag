using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum E_Item_Type
{
    Bag = 0,//代表背包内的物品
    Head,
    Neck,
    Weapon,
    Cloth,
    Trousers,
    Shoes,
}
///<summary>
///道具格子对象
///</summary>
public class ItemCell : BasePanel 
{
    private ItemInfo _itemInfo;
    public E_Item_Type type = E_Item_Type.Bag;//默认是背包里面的
    public Image imgBK;
    public Image imgIcon;
    public bool isOpenDrag = false;
    public ItemInfo itemInfo
    {
        get 
        {
            return _itemInfo;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        imgBK = GetControl<Image>("imgBK");
        imgIcon = GetControl<Image>("imgIcon");
        //开始先将图标进行隐藏，应该是初始化了信息在去显示
        GetControl<Image>("imgIcon").gameObject.SetActive(false);

        EventTrigger trigger = GetControl<Image>("imgBK").gameObject.AddComponent<EventTrigger>();
       
        //声明一个鼠标进入的事件类对象
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener(EnterItemCell);
        trigger.triggers.Add(enter);

        //声明一个鼠标移出的事件类对象
        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener(ExitItemCell);
        trigger.triggers.Add(exit);


    }
    /// <summary>
    /// 开启检测鼠标拖动相关的事件
    /// </summary>
    private void OpenDragEvent()
    {
        if (isOpenDrag)
        {
            return;
        }
        isOpenDrag = true;
        EventTrigger trigger = GetControl<Image>("imgBK").gameObject.AddComponent<EventTrigger>();
        //声明拖动相关
        EventTrigger.Entry beginDrag = new EventTrigger.Entry();
        beginDrag.eventID = EventTriggerType.BeginDrag;
        beginDrag.callback.AddListener(BeginDragItemCell);
        trigger.triggers.Add(beginDrag);

        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener(DragItemCell);
        trigger.triggers.Add(drag);

        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener(EndDragItemCell);
        trigger.triggers.Add(endDrag);
    }
    private void BeginDragItemCell(BaseEventData data)
    {
        EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellBeginDrag", this);
    }
    private void DragItemCell(BaseEventData data)
    {
        EventCenter.GetInstance().EventTrigger<BaseEventData>("ItemCellDrag", data);
    }
    private void EndDragItemCell(BaseEventData data)
    {
        EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellEndDrag", this);
    }
    private void EnterItemCell(BaseEventData data)
    {
        EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellEnter", this);
    }                                                                                        
    private void ExitItemCell(BaseEventData data)
    {
        if (itemInfo == null)
        {
            return;
        }
        EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellExit", this);
      
    }
    /// <summary>
    /// 根据道具信息初始化 格子信息
    /// </summary>
    public void InitInfo(ItemInfo info)
    {
       
        this._itemInfo = info;
        if (info == null)
        {
            imgIcon.gameObject.SetActive(false);
            return;
        }
        GetControl<Image>("imgIcon").gameObject.SetActive(true);
        //根据道具信息，更新格子对象
        print("得到了id" + info.id + "  " + info.num);
        Item itemData = GameDataMgr.GetInstance().GetItemInfo(info.id);
        //使用我们的道具表中的数据
        GetControl<Image>("imgIcon").sprite =ResMgr.GetInstance().Load<Sprite>("Icon/"+itemData.icon);
        if (type == E_Item_Type.Bag)
        {
            //数量
            GetControl<Text>("txtNum").text = info.num.ToString();
        }
        //只要装备才可以进行拖动
        if (itemData.type == (int)E_Bag_Type.Equip)
        {
            OpenDragEvent();
        }
    }
}

