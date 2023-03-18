using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSlot : BaseSlot
{
    public override void Start()
    {
        base.Start();
        slottype = EnumTypes.SlotTypes.Craft;
    }
    //public delegate void InsertCraftSlotEvent(Vector2Int index);

    //basenode와 똑같이 동작하고 무언가 삽입되었을때는 itemcrafttable에 알려준다.
    public override void SetNode(BaseNode node)
    {
        //노드를 슬록 자신의 하위로 두고 크기를 맞춰준다.
        node.transform.parent = this.transform;
        node.rectTransform.sizeDelta = this.rectTransform.sizeDelta;
        node.transform.localPosition = new Vector3(0, 0, 0);
        if (node.NodeIsClicked)
            node.NodeIsClicked = false;
        node.SettedSlot = this;
        this.SettingNode = node;

        insertevent();
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
