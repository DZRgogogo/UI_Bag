using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneBtnTipPanel : BasePanel
{
    public void Start()
    {
        GetControl<Button>("btnSure").onClick.AddListener(() =>
        {
            UIManager.GetInstance().HidePanel("BuyTipPanel");
        });
    }
    public void InitTipText(string info)
    {
        GetControl<Text>("tipText").text = info;
    }
}
