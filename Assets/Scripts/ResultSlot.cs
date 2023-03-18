using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSlot : BaseSlot
{
    public override void Start()
    {
        base.Start();
        slottype = EnumTypes.SlotTypes.Result;
    }

    //결과물을 가져갔다는 이벤트를 발생시킨다.
    public override BaseNode GetSettingNode()
    {
        SettingNode.NodeIsClicked = true;
        SettingNode.PreSlot = this;
        BaseNode temp = SettingNode;
        SettingNode = null;
        pickupevent();
        return temp;
    }
}
