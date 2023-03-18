using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemCraftTable : Singleton<ItemCraftTable>
{
    [Header("제작테이블 정보")]
    [SerializeField]
    private Vector2Int CraftingSlotSize = new Vector2Int(3, 3);
    [SerializeField]
    private int CraftResultSlotNum = 1;
    [SerializeField]
    private Vector2Int InventorySlotSize = new Vector2Int(9, 3);
    [SerializeField]
    private int QuickInvectoryNum = 9;

    public List<CraftRecipe> recipelist;

    [Header("슬롯 정보")]
    public BaseSlot CraftSlot;
    public BaseSlot ResultSlot;

    public ItemBag itemBag;

    ////////////////////////////////////////////////////
    //public BaseSlot InventorySlot;
    //public BaseSlot QuickSlot;


    //public BaseSlot[] InventorySlotArr;
    //public BaseSlot[] QuickSlotArr;
    ////////////////////////////////////////////////////
    
    public BaseSlot[] CraftSlotArr;

    public BaseNode ClickedNode;

    private void Awake()
    {
        CraftSlotInit(72, 78);
        ResultSlot.PickUpEvent(ResultPickUp);
    }

    //결과창에 있는 아이템을 가져갔을때는 제작창에 있는 아이템들을 개수를 하나 줄이거나 없애준다.
    public void ResultPickUp()
    {
        for (int i = 0; i < CraftSlotArr.Length; i++)
        {
            if (CraftSlotArr[i].SettingNode != null)
            {
                if (CraftSlotArr[i].SettingNode.GetStack()>=2)
                {
                    CraftSlotArr[i].SettingNode.ChangeStack(-1);
                }
                else
                {
                    CraftSlotArr[i].RemoveNode();
                }
            }
        }
        CraftInsert();
    }

    //제작창에있는 아이템을 꺼냈을때는 결과창에 이미 아이템이 있을때는 해당 아이템 노드를 없애주고, 해당 아이템이 사라진 상태에서의 제작물을 검색하고 결과를 표시해준다. 
    public void CraftItemPickUp()
    {
        if (ResultSlot.SettingNode!=null)
        {
            //Debug.Log("결과 없애줌");
            ResultSlot.RemoveNode();
        }

        CraftInsert();
    }

    public void CraftSlotInit(int xval, int yval)
    {
        BaseSlot temp;
        CraftSlotArr = new BaseSlot[CraftingSlotSize.x * CraftingSlotSize.y];
        CraftSlotArr[0] = CraftSlot;
        CraftSlotArr[0].InsertEvent(CraftInsert);
        CraftSlotArr[0].PickUpEvent(CraftItemPickUp);

        for (int y = 0; y < CraftingSlotSize.y; y++)
        {
            for (int x = 0; x < CraftingSlotSize.x; x++)
            {
                if (x != 0 || y != 0)
                {
                    temp = GameObject.Instantiate<BaseSlot>(CraftSlot);
                    temp.transform.parent = CraftSlot.transform.parent;
                    temp.transform.position = CraftSlot.transform.position + new Vector3(x * xval, -y * yval);
                    temp.SlotIndex = new Vector2Int(x, y);
                    temp.InsertEvent(CraftInsert);//제작슬롯은 슬롯에 무언가 아이템이 들러왔을때 실행될 함수를 대리자에 등록해준다.
                    temp.PickUpEvent(CraftItemPickUp);

                    CraftSlotArr[x + (y * CraftingSlotSize.x)] = temp;
                }
            }
        }
    }

    //무언가 하나라도 들어오면 모든 슬롯을 조사해서 레시피테이블에 넘겨줘서 검색한다.
    public void CraftInsert()
    {
        Debug.Log("검색 시작?");
        int[] inputs = new int[CraftSlotArr.Length];
        int count = 0;

        for (int i = 0; i < CraftSlotArr.Length; i++)
        {
            if(CraftSlotArr[i].SettingNode!=null)
            {
                inputs[i] = CraftSlotArr[i].SettingNode.GetItemID();
                count++;
            }
            else
            {
                inputs[i] = 0;
            }
        }

        //해당 함수가 들어왔지만 아이템 창에 아무것도 없으면 서칭을 하지 않는다.
        if (count == 0)
            return;

        BaseNode copyresultnode = CraftRecipe.SearchRecipe(recipelist, inputs);

        if(copyresultnode != null)
        {
            //아이템 노드들 중에서 해당 노드를 찾아서 결과슬롯에 넣어준다.
            //BaseNode copyresultnode = GameObject.Instantiate<BaseNode>(ItemDataLoader.Instance.itemnodes.Find(x => x.GetItemID() == resultcode));
            //copyresultnode.NodeIsActive = true;
            ResultSlot.SetNode(copyresultnode);
        }

    }




    public Vector2Int GetInventorySize
    {
        get
        {
            return InventorySlotSize;
        }
    }

    public Vector2Int GetSlotSize
    {
        get
        {
            return CraftingSlotSize;
        }
    }

    ////아이템 가방에서 제작테이블로 옮겨준다.
    //public void CraftSetItem(BaseSlot slot, BaseNode node)
    //{

    //}
    
    ////제작 테이블에서 다시 아이템 가방으로 보내준다.
    //public void CraftUnSetItem(BaseSlot slot, BaseNode node)
    //{

    //}



    // Update is called once per frame
    void Update()
    {
        

    }
}
