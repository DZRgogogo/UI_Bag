using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class Items
{
	public List<Item> info;//这个对应json的key
}
//加序列化，如果ItemInfo a ；那么面板中会显示a，那么如果有序列化，那么会给一个默认值，0 0 0 
[System.Serializable]//加这个就是为了让他在读取的时候能够被识别，否则可能报错
public class Item
{
	public int id;
	public string name;
	public string icon;
	public int type;
	public int price;
	public string tips;
	public int equipType;
}
/// <summary>
/// 玩家拥有的道具信息
/// </summary>
public class Player
{
	public string name;
	public int lev;
	public int money;
	public int gem;
	public int pro;
	public List<ItemInfo> items;
	public List<ItemInfo> equips;
	public List<ItemInfo> gems;
	//当前装备的物品
	public List<ItemInfo> nowEquips;
	public Player()
	{
		name = "邓哥";
		lev = 1;
		money = 9999;
		gem = 9999;
		pro = 99;
		items = new List<ItemInfo>() { new ItemInfo() { id = 3, num = 10 } };
		equips = new List<ItemInfo>() { new ItemInfo() { id = 1, num = 10 } };
		gems = new List<ItemInfo>() { new ItemInfo() { id = 4, num = 9 } };
		nowEquips = new List<ItemInfo>() {  };
	}
	/// <summary>
	/// 购买物品后添加物品给玩家
	/// </summary>
	/// <param name="info"></param>
	public void AddItem(ItemInfo info)
	{
		Item item = GameDataMgr.GetInstance().GetItemInfo(info.id);

		switch (item.type)
		{
			case (int)E_Bag_Type.Item:
				items.Add(info);
				break;
			case (int)E_Bag_Type.Equip:
				equips.Add(info);
				break;
			case (int)E_Bag_Type.Gem:
				gems.Add(info);
				break;
		}
	}
	/// <summary>
	/// 进行钱的改变
	/// </summary>
	/// <param name="info"></param>
	public void ChangeMoney(int money)
	{
		if (money < 0 && this.money < money)
		{
			return;
		}
		this.money += money;
	}
	/// <summary>
	/// 进行宝石改变
	/// </summary>
	/// <param name="info"></param>
	public void ChangeGem(int gem)
	{
		if (gem < 0 && this.gem < gem)
		{
			return;
		}
		this.gem += gem;
	}
}
[System.Serializable]//加序列化，如果ItemInfo a ；那么面板中会显示a，那么如果有序列化，那么会给一个默认值，0 0 0 
public class ItemInfo
{
	public int id;
	public int num;
}
/// <summary>
/// 作为json读取的中间数据结构 用装载json内容
/// </summary>
public class Shops
{
	public List<ShopCellInfo> info;
}
/// <summary>
/// 商店售卖物品信息的数据
/// </summary>
[System.Serializable]
public class ShopCellInfo
{
	public int id;
	public ItemInfo itemInfo;
	public int priceType;
	public int price;
	public string tips;
}
///<summary>
///整个商店、背包的数据管理
///</summary>
public class GameDataMgr : BaseManager<GameDataMgr> 
{
	private Dictionary<int, Item> itemInfos = new Dictionary<int, Item>();
	public List<ShopCellInfo> shopInfos;
	/// <summary>
	/// 玩家信息
	/// </summary>
	public Player playerInfo;
	//Application.dataPath 这个就是Assets路径，发布后就不能找到这个路径了，所以要用Application.persistentDataPath
	//玩家信息的存储路径
	private static string PlayerInfo_Url = Application.persistentDataPath + "/PlayerInfo.txt";//unity提供的一个可读可写的路径
	public void Init()
	{
		string info = ResMgr.GetInstance().Load<TextAsset>("Json/ItemInfo").text;
        Items items = JsonUtility.FromJson<Items>(info);
        Debug.Log(items.info.Count);
        for (int i = 0; i < items.info.Count; i++)
        {
			itemInfos.Add(items.info[i].id,items.info[i]);
        }
		Debug.Log(PlayerInfo_Url);
		//初始化 角色信息
		if (File.Exists(PlayerInfo_Url))
        {
            byte[] bytes = File.ReadAllBytes(PlayerInfo_Url);
			string json = Encoding.UTF8.GetString(bytes);
			Debug.Log(json);
            playerInfo = JsonUtility.FromJson<Player>(json);
        }
        else
        {
			//没有玩家数据的时候，给一个默认数据
			playerInfo = new Player();
			//存储玩家信息到json
			SavePlayerInfo();
		}
		//加载Resource文件夹下的json文件 获取它的内容
		string shopInfo = ResMgr.GetInstance().Load<TextAsset>("Json/ShopItem").text;
		Shops shopsInfo = JsonUtility.FromJson<Shops>(shopInfo);
		//记录下 加载解析出来的商店信息
		shopInfos = shopsInfo.info;

		//初始化的时候就开始添加监听事件
		//更改钱
		EventCenter.GetInstance().AddEventListener<int>("MoneyChange", ChangeMoney);
		//更改宝石
		EventCenter.GetInstance().AddEventListener<int>("GemChange", ChangeGem);
	}
	/// <summary>
	/// 通过事件中心进行减钱和保存
	/// </summary>
	/// <param name="info">货币数</param>
	public void ChangeMoney(int money)
	{
		//减钱并且保存
		playerInfo.ChangeMoney(money);
		SavePlayerInfo();
	}
	/// <summary>
	/// 通过事件中心进行减少宝石和保存
	/// </summary>
	/// <param name="info">货币数</param>
	public void ChangeGem(int money)
	{
		//减钱并且保存
		playerInfo.ChangeGem(money);
		SavePlayerInfo();
	}
	/// <summary>
	/// 保存玩家信息
	/// </summary>
	public void SavePlayerInfo()
	{
		string json = JsonUtility.ToJson(playerInfo);
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		File.WriteAllBytes(PlayerInfo_Url, bytes);
	}
	/// <summary>
	/// 根据道具id获取道具的详细信息
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Item GetItemInfo(int id)
	{
        if (itemInfos.ContainsKey(id))
        {
			return itemInfos[id];
        }
		return null;
	}
	
}
