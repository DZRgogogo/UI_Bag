using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameDataMgr.GetInstance().Init();
        Debug.Log(GameDataMgr.GetInstance().GetItemInfo(1).name);
        BagMgr.GetInstance().Init();
        //显示主面板
        UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel", E_UI_Layer.Bot);
     
    }

}
