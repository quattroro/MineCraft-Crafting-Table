using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSlot : MonoBehaviour
{
    public Vector2Int SlotIndex;

    public BaseNode SettingNode;

    public RectTransform rectTransform;

    public delegate void InsertSlotEvent();
    protected InsertSlotEvent insertevent;

    public delegate void PickUpSlotEvent();
    protected PickUpSlotEvent pickupevent;

    protected EnumTypes.SlotTypes slottype;

    public EnumTypes.SlotTypes GetSlotTypes()
    {
        return slottype;
    }
    public virtual void SetNode(BaseNode node)
    {
        //노드를 슬록 자신의 하위로 두고 크기를 맞춰준다.
        node.transform.parent = this.transform;
        node.rectTransform.sizeDelta = this.rectTransform.sizeDelta;
        node.transform.localPosition = new Vector3(0, 0, 0);
        if (node.NodeIsClicked)
            node.NodeIsClicked = false;
        node.SettedSlot = this;
        this.SettingNode = node;
    }
    //슬롯에 어떠한 아이템이 삽입되었을떄 실행될 이벤트
    public virtual void InsertEvent(InsertSlotEvent _event)
    {
        insertevent += _event;
    }
    //슬롯에 있던 아이템을 가져갔을 때 실행될 이벤트
    public virtual void PickUpEvent(PickUpSlotEvent _event)
    {
        pickupevent += _event;
    }
    //현재 슬롯에 들어가있는 아이템을 삭제
    public void RemoveNode()
    {
        if(SettingNode!=null)
        {
            GameObject.Destroy(SettingNode.gameObject);
            SettingNode = null;
        }
    }
    //현재 슬롯에 들어와있는 아이템을 삭제
    public void RemoveNode(BaseNode node)
    {
        if(SettingNode == node)
        {
            GameObject.Destroy(SettingNode.gameObject);
            SettingNode = null;
        }
    }
    //현재 슬롯에 들어와있는 아이템을 리턴해준다.
    public virtual BaseNode GetSettingNode()
    {
        SettingNode.NodeIsClicked = true;
        SettingNode.PreSlot = this;
        BaseNode temp = SettingNode;
        SettingNode = null;
        return temp;
    }

    //현재 슬롯에 들어와있는 아이템의 코드를 넘겨준다.
    public int GetSettingNodeID()
    {
        if (SettingNode == null)
            return -1;

        return SettingNode.GetItemID();
    }


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
