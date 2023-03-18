using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//여기서 모든 마우스 입력을 담당한다.
//마우스가 클릭되면 어떤 객체가 선택되었고 그래그되었는지 판단해서 동작해준다.
public class InputManager : MonoBehaviour
{
    GraphicRaycaster graphicRaycaster;

    [Header("current vals")]
    bool NowClicked;

    BaseNode ClickedObj;

    bool IsControl = false;
    bool RightButtonDrag = false;

    private void Awake()
    {
        graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
    }

    //현재 클릭된 노드가 있는지 없는지 확인한다.
    //현재 클릭된 노드가 있고 클릭된 곳에 슬롯이 있고 해당 슬롯이 비어있으면 클릭된 노드를 해당 슬롯에 등록
    //현재 클릭된 노드가 없고 클릭된 곳에 슬록이 있고 해당 슬록에 노드가 있으면 슬롯에 있는 노드를 받아온다.

    public void MouseDown(Vector2 pos)
    {
        //canvas에 있는 graphicraycast를 이용해 클릭된 위치에 있는 객체들의 정보들을 받아온다.
        PointerEventData ped = new PointerEventData(null);
        ped.position = pos;
        List<RaycastResult> result = new List<RaycastResult>();
        graphicRaycaster.Raycast(ped, result);

        BaseNode node = null;
        BaseSlot slot = null;


        if (ClickedObj==null)
        {
            foreach (var a in result)
            {
                if (a.gameObject.tag == "Node")
                {
                    node = a.gameObject.GetComponent<BaseNode>();
                    if (node.NodeIsActive)//활성노드
                    {
                        if (!node.NodeIsClicked)//클릭된 노드가 아닐때
                        {
                            if (IsControl)
                            {
                                ClickedObj = node.DivideNode();
                            }
                            else
                            {
                                ClickedObj = node.SettedSlot.GetSettingNode();
                            }
                        }
                    }
                    else//비활성노드
                    {
                        //해당 노드가 클릭되면 아이템창에 해당 아이템을 추가해준다.
                        BaseNode copyNode = GameObject.Instantiate<BaseNode>(node);
                        copyNode.NodeIsActive = true;
                        ItemBag.Instance.InsertItem(copyNode);
                    }

                    Debug.Log($"{a.gameObject.name} clicked");
                }
            }
        }
        else
        {
            //아무것도 없는 공간에 노드를 두면 해당 노드는 파괴
            if (result.Count == 1)
            {
                if (ClickedObj != null)
                {
                    GameObject.Destroy(ClickedObj.gameObject);
                    ClickedObj = null;
                    return;
                }
            }

            //무언가 있으면 해당 위치에 있는 오브젝트들 확인
            foreach (var a in result)
            {
                //그래그하는 노드가 있을때
                if (ClickedObj != null)
                {
                    //해당 마우스의 위치에서 슬롯을 찾는다.
                    if (a.gameObject.tag == "Slot")
                    {
                        slot = a.gameObject.GetComponent<BaseSlot>();
                        if (slot.GetSlotTypes() != EnumTypes.SlotTypes.Result)
                        {
                            //찾은 슬롯이 빈 슬롯일때 해당 아이템 노드를 해당 노드에 세팅해준다.
                            if (slot.SettingNode == null)
                            {
                                slot.SetNode(ClickedObj);
                                //ClickedObj = null;
                            }
                            else
                            {
                                //클릭된 곳에 아이템이 들어있는 슬롯이 있고 해당 아이템이 드래그하고 있는 아이템과 같은 아이템이면 아이템을 병합한다.
                                if (slot.SettingNode.GetItemID() == ClickedObj.GetItemID() && ClickedObj.IsStackAble())
                                {
                                    slot.SettingNode.ItemMerge(ClickedObj);
                                    ClickedObj = null;
                                }
                                slot = null;
                                //ClickedObj = null;
                            }
                        }
                        else
                        {
                            slot = null;
                        }
                    }

                }
            }
            //드래그 중인 노드가 있는데 마우스의 위치에 슬롯이 없으면 노드는 원래있었던 자리로 돌아간다.
            if (ClickedObj != null && slot == null)
            {
                ClickedObj.PreSlot.SetNode(ClickedObj);
                ClickedObj = null;
            }

            if (ClickedObj != null)
                ClickedObj = null;
        }

    }


    //현재 마우스 위치에 비어있는 슬롯이있으면 손에 들고있는 아이템을 복사해서 해당 슬롯에 복사해준다. 마지막 하나남았을떄는 그걸 등록해준다.
    public void MouseDrag(Vector2 pos)
    {
        PointerEventData ped = new PointerEventData(null);
        ped.position = pos;
        List<RaycastResult> result = new List<RaycastResult>();
        graphicRaycaster.Raycast(ped, result);

        BaseNode node = null;
        BaseSlot slot = null;

        if (ClickedObj == null)
            return;

        foreach (var a in result)
        {
            //해당 마우스의 위치에서 슬롯을 찾는다.
            if (a.gameObject.tag == "Slot")
            {
                slot = a.gameObject.GetComponent<BaseSlot>();
                //손에 들고있는 아이템이 2개 이상이고 해당 슬롯이 비어있는 슬롯이면 손에 들고있는 슬롯에 아이템을 하나 등록시켜 준다.
                if (ClickedObj.GetStack()>1&&slot.SettingNode==null)
                {
                    node = ClickedObj.GetDuplicateNode();
                    if(node!=null)
                        slot.SetNode(node);
                    

                }
                //마지막 한개나맜을때는 해당 노드를 슬롯에 넣어주고 드래그모드 끝 클릭도 끝
                else if (slot.SettingNode==null&&ClickedObj.GetStack()==1)
                {
                    slot.SetNode(ClickedObj);
                    ClickedObj = null;
                    RightButtonDrag = false;
                    return;
                }
                


            }
        }



    }

    public void MouseUp(Vector2 pos)
    {
        //canvas에 있는 graphicraycast를 이용해 클릭된 위치에 있는 객체들의 정보들을 받아온다.
        PointerEventData ped = new PointerEventData(null);
        ped.position = pos;
        List<RaycastResult> result = new List<RaycastResult>();
        graphicRaycaster.Raycast(ped, result);

        BaseNode node = null;
        BaseSlot slot = null;

        Debug.Log($"개수{result.Count}");

        //아무것도 없는 공간에 노드를 두면 해당 노드는 파괴
        if(result.Count==1)
        {
            if (ClickedObj != null)
            {
                GameObject.Destroy(ClickedObj.gameObject);
                ClickedObj = null;
                return;
            }
        }

        //무언가 있으면 해당 위치에 있는 오브젝트들 확인
        foreach (var a in result)
        {
            //그래그하는 노드가 있을때
            if(ClickedObj !=null)
            {
                //해당 마우스의 위치에서 슬롯을 찾는다.
                if(a.gameObject.tag == "Slot")
                {
                    slot = a.gameObject.GetComponent<BaseSlot>();
                    if (slot.GetSlotTypes()!=EnumTypes.SlotTypes.Result)
                    {
                        //찾은 슬롯이 빈 슬롯일때 해당 아이템 노드를 해당 노드에 세팅해준다.
                        if (slot.SettingNode == null)
                        {
                            slot.SetNode(ClickedObj);
                            //ClickedObj = null;
                        }
                        else
                        {
                            //클릭된 곳에 아이템이 들어있는 슬롯이 있고 해당 아이템이 드래그하고 있는 아이템과 같은 아이템이면 아이템을 병합한다.
                            if(slot.SettingNode.GetItemID()==ClickedObj.GetItemID()&&ClickedObj.IsStackAble())
                            {
                                slot.SettingNode.ItemMerge(ClickedObj);
                                ClickedObj = null;
                            }
                            slot = null;
                            //ClickedObj = null;
                        }
                    }
                    else
                    {
                        slot = null;
                    }
                }
                
            }
        }

        //드래그 중인 노드가 있는데 마우스의 위치에 슬롯이 없으면 노드는 원래있었던 자리로 돌아간다.
        if(ClickedObj != null&&slot == null)
        {
            ClickedObj.PreSlot.SetNode(ClickedObj);
            ClickedObj = null;
        }

        if (ClickedObj != null)
            ClickedObj = null;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseDown(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            //MouseUp(Input.mousePosition);
        }

        if(Input.GetMouseButtonDown(1))
        {
            if (ClickedObj != null)
                RightButtonDrag = true;
            //MouseRightDown(Input.mousePosition);
        }

        if(Input.GetMouseButtonUp(1))
        {
            if (RightButtonDrag)
                RightButtonDrag = false;
        }

        if(RightButtonDrag)
        {
            MouseDrag(Input.mousePosition);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            IsControl = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            IsControl = false;
        }
    }
}
