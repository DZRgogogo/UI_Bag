using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipMgr : BaseManager<TipMgr>
{
    /// <summary>
    /// 再次封装，只需要传入info
    /// </summary>
    /// <param name="info"></param>
    public void ShowOneTipPanel(string info)
    {
        UIManager.GetInstance().ShowPanel<OneBtnTipPanel>("BuyTipPanel", E_UI_Layer.Top, (panel) =>
        {
            panel.InitTipText(info);
        });
    }
}
