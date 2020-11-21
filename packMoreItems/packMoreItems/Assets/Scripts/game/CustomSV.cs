using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 该接口 作为 格子对象 必须继承的类 它用于实现初始化格子的方法
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IItemBase<T>
{
    //注意谁继承了接口，就必须实现下面的方法，而且方法内形参的T是谁，上面类后面的<T>就是要<谁>，因为下面的方法的类型T必须要由上面传过去
    void InitInfo(T info);
}
/// <summary>
/// 自定义sv类 用于节约性能 通过缓存池创建复用对象
/// </summary>
/// <typeparam name="T">代表的 数据来源类</typeparam>
/// <typeparam name="K">代表的 格子类</typeparam>
public class CustomSV<T, K> where K : IItemBase<T>
{
    //通过content获取可视范围的位置
    public RectTransform content;
    //可视范围
    public int viewPortH;
    //当前格子对象
    public Dictionary<int, GameObject> nowShowItems = new Dictionary<int, GameObject>();
    //记录上次的显示范围
    public int oldMinIndex = -1;
    public int oldMaxIndex = -1;
    //格子的间隔宽高
    private int itemW;
    private int itemH;

    //格子的列数
    private int col;
    //预设体资源的路径
    private string itemResName;

    /// <summary>
    /// 初始化格子资源路径
    /// </summary>
    /// <param name="name"></param>
    public void InitItemResName(string name)
    {
        itemResName = name;
    }

    /// <summary>
    /// 初始化Content父对象 以及 我们可视范围的高
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="h"></param>
    public void InitContentAndSVH(RectTransform trans, int h)
    {
        this.content = trans;
        this.viewPortH = h;
    }

    //数据来源
    private List<T> items;
    /// <summary>
    /// 初始化数据来源
    /// </summary>
    /// <param name="items"></param>
    public void InitInfos(List<T> items)
    {
        this.items = items;
        //初始化履带的长度
        content.sizeDelta = new Vector2(0, Mathf.CeilToInt((items.Count + 1) / col * itemH));
    }
    /// <summary>
    /// 初始化格子间隔大小 以及 一行几列
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="col"></param>
    public void InitItemSizeAndCol(int w, int h, int col)
    {
        this.itemW = w;
        this.itemH = h;
        this.col = col;
    }
    public void CheckShowOrHide()
    {
        if (content.anchoredPosition.y < 0)
        {
            return;
        }

        int minIndex = (int)(content.anchoredPosition.y / itemH) * col;
        int maxIndex = (int)((content.anchoredPosition.y + viewPortH) / itemH) * col + 2;

        //最小值判断
        if (minIndex < 0)
            minIndex = 0;

        if (maxIndex >= items.Count)
        {
            maxIndex = items.Count - 1;
        }
        //往下移动，删除上面的东西
        for (int i = oldMinIndex; i < minIndex; ++i)
        {
            if (nowShowItems.ContainsKey(i))
            {
                if (nowShowItems[i] != null)
                    PoolMgr.GetInstance().PushObj(itemResName, nowShowItems[i]);
                nowShowItems.Remove(i);
            }
        }
        ////往上移动，删除下面的东西
        for (int i = maxIndex + 1; i <= oldMaxIndex; i++)
        {
            if (nowShowItems.ContainsKey(i))
            {
                if (nowShowItems[i] != null)
                {
                    PoolMgr.GetInstance().PushObj(itemResName, nowShowItems[i]);
                }
                nowShowItems.Remove(i);
            }
        }
        oldMinIndex = minIndex;
        oldMaxIndex = maxIndex;

        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (nowShowItems.ContainsKey(i))
            {
                continue;
            }
            else
            {
                int index = i;//先占坑，避免异步加载时候坑位不对
                nowShowItems.Add(index, null);//先占坑，避免异步加载时候坑位不对
                PoolMgr.GetInstance().GetObj(itemResName, (obj) =>
                {
                    //当格子创建出来后我们要做什么
                    //设置它的父对象
                    obj.transform.SetParent(content);
                    //重置相对缩放大小
                    obj.transform.localScale = Vector3.one;
                    //重置位置
                    obj.transform.localPosition = new Vector3((index % col) * itemW + 90, -index / col * itemH - 90, 0);
                    //更新格子信息  
                    obj.GetComponent<K>().InitInfo(items[index]);
                    //判断有没有这个坑位
                    if (nowShowItems.ContainsKey(index))
                    {
                        nowShowItems[index] = obj;
                    }
                    else //这里是，如果往下移动的特别快，那么就会出现，前面Add占完坑了，但是还没异步加载出来资源，nowShowItems就已经在update里被Remove删除了，这时候再加载进来就会走进else
                    //就是拖得比创建的还要快
                    {
                        PoolMgr.GetInstance().PushObj(itemResName, obj);
                    }
                });
            }

        }
    }
}

