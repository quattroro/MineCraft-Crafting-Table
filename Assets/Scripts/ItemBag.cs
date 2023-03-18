using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이후에 재사용을 고려하여
//아이템 가방은 제작테이블과 독립적으로 동작한다.
public class ItemBag : Singleton<ItemBag>
{
    [SerializeField]
    private Vector2Int InventorySlotSize = new Vector2Int(9, 3);
    [SerializeField]
    private int QuickInvectoryNum = 9;

    public BaseSlot InventorySlot;
    public BaseSlot QuickSlot;

    public BaseSlot[] InventorySlotArr;
    public BaseSlot[] QuickSlotArr;

    private void Awake()
    {
        InventorySlotInit(72, 78);
    }

    //초기화 작업 인텐토리 슬롯들을 만들어 준다.
    public void InventorySlotInit(int xval, int yval)
    {
        BaseSlot temp;
        InventorySlotArr = new BaseSlot[InventorySlotSize.x * InventorySlotSize.y];
        InventorySlotArr[0] = InventorySlot;
        for (int y = 0; y < InventorySlotSize.y; y++)
        {
            for (int x = 0; x < InventorySlotSize.x; x++)
            {
                if (x != 0 || y != 0)
                {
                    temp = GameObject.Instantiate<BaseSlot>(InventorySlot);
                    temp.transform.parent = InventorySlot.transform.parent;
                    temp.transform.position = InventorySlot.transform.position + new Vector3(x * xval, -y * yval);
                    temp.SlotIndex = new Vector2Int(x, y);
                    InventorySlotArr[x + (y * InventorySlotSize.x)] = temp;

                }
            }
        }
        QuickSlotArr = new BaseSlot[QuickInvectoryNum];
        QuickSlotArr[0] = QuickSlot;
        for (int x = 1; x < QuickInvectoryNum; x++)
        {
            temp = GameObject.Instantiate<BaseSlot>(QuickSlot);
            temp.transform.parent = QuickSlot.transform.parent;
            temp.transform.position = QuickSlot.transform.position + new Vector3(x * xval, 0);
            temp.SlotIndex = new Vector2Int(x, 0);
            
            QuickSlotArr[x] = temp;
        }

    }

    //해당 노드 파괴
    public void DeleteItem(BaseSlot slot, BaseNode node)
    {
        slot.RemoveNode(node);
    }

    //미리 만들어진 노드 객체를 넘겨받아서 세팅
    public void InsertItem(BaseSlot slot, BaseNode node)
    {
        if(slot.SettingNode==null)
        {
            slot.SetNode(node);
        }
    }

    //미리 만들어진 노드 객체를 넘겨받아서 세팅
    public void InsertItem(BaseNode node)
    {
        BaseSlot tempslot = null;
        bool flag = false;


        //만약 같은 종류의 아이템이 있으면 해당 아이템의 개수를 증가시켜 준다.
        for (int y = 0; y < InventorySlotSize.y; y++)
        {
            for (int x = 0; x < InventorySlotSize.x; x++)
            {
                if(InventorySlotArr[x + y * (InventorySlotSize.x)].SettingNode != null)
                {
                    
                    if (InventorySlotArr[x + y * (InventorySlotSize.x)].GetSettingNodeID() == node.GetItemID())
                    {
                        if (node.IsStackAble())
                        {
                            InventorySlotArr[x + y * (InventorySlotSize.x)].SettingNode.ChangeStack(+1);
                            GameObject.Destroy(node.gameObject);
                            return;
                        }
                    }
                }
            }
        }

        for (int x = 0; x < QuickInvectoryNum; x++)
        {
            if(QuickSlotArr[x + 0].GetSettingNodeID() == node.GetItemID())
            {
                if (node.IsStackAble())
                {
                    QuickSlotArr[x + 0].SettingNode.ChangeStack(+1);
                    GameObject.Destroy(node.gameObject);
                    return;
                }
            }
            
        }


        //비어있는 슬롯 을 찾는다.
        for (int y = 0; y < InventorySlotSize.y; y++)
        {
            for (int x = 0; x < InventorySlotSize.x; x++)
            {
                //슬롯이 비어있으면 해당 슬롯에 아이템 노드를 넣어준다.
                if (InventorySlotArr[x + y * (InventorySlotSize.x)].SettingNode == null)
                {
                    tempslot = InventorySlotArr[x + y * (InventorySlotSize.x)];
                    flag = true;
                    break;
                }

            }
            if (flag)
            {
                break;
            }
        }

        //인벤토리가 전부 찼으면 퀵슬롯에서 찾는다.
        if (tempslot == null)
        {
            for (int x = 0; x < QuickInvectoryNum; x++)
            {
                if (QuickSlotArr[x + 0].SettingNode == null)
                {
                    tempslot = QuickSlotArr[x + 0];
                    break;
                }
            }
        }

        if (tempslot == null)
        {
            GameObject.Destroy(node.gameObject);
            return;
        }

        tempslot.SetNode(node);
        
    }

}
