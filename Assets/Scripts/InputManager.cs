using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//���⼭ ��� ���콺 �Է��� ����Ѵ�.
//���콺�� Ŭ���Ǹ� � ��ü�� ���õǾ��� �׷��׵Ǿ����� �Ǵ��ؼ� �������ش�.
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

    //���� Ŭ���� ��尡 �ִ��� ������ Ȯ���Ѵ�.
    //���� Ŭ���� ��尡 �ְ� Ŭ���� ���� ������ �ְ� �ش� ������ ��������� Ŭ���� ��带 �ش� ���Կ� ���
    //���� Ŭ���� ��尡 ���� Ŭ���� ���� ������ �ְ� �ش� ���Ͽ� ��尡 ������ ���Կ� �ִ� ��带 �޾ƿ´�.

    public void MouseDown(Vector2 pos)
    {
        //canvas�� �ִ� graphicraycast�� �̿��� Ŭ���� ��ġ�� �ִ� ��ü���� �������� �޾ƿ´�.
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
                    if (node.NodeIsActive)//Ȱ�����
                    {
                        if (!node.NodeIsClicked)//Ŭ���� ��尡 �ƴҶ�
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
                    else//��Ȱ�����
                    {
                        //�ش� ��尡 Ŭ���Ǹ� ������â�� �ش� �������� �߰����ش�.
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
            //�ƹ��͵� ���� ������ ��带 �θ� �ش� ���� �ı�
            if (result.Count == 1)
            {
                if (ClickedObj != null)
                {
                    GameObject.Destroy(ClickedObj.gameObject);
                    ClickedObj = null;
                    return;
                }
            }

            //���� ������ �ش� ��ġ�� �ִ� ������Ʈ�� Ȯ��
            foreach (var a in result)
            {
                //�׷����ϴ� ��尡 ������
                if (ClickedObj != null)
                {
                    //�ش� ���콺�� ��ġ���� ������ ã�´�.
                    if (a.gameObject.tag == "Slot")
                    {
                        slot = a.gameObject.GetComponent<BaseSlot>();
                        if (slot.GetSlotTypes() != EnumTypes.SlotTypes.Result)
                        {
                            //ã�� ������ �� �����϶� �ش� ������ ��带 �ش� ��忡 �������ش�.
                            if (slot.SettingNode == null)
                            {
                                slot.SetNode(ClickedObj);
                                //ClickedObj = null;
                            }
                            else
                            {
                                //Ŭ���� ���� �������� ����ִ� ������ �ְ� �ش� �������� �巡���ϰ� �ִ� �����۰� ���� �������̸� �������� �����Ѵ�.
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
            //�巡�� ���� ��尡 �ִµ� ���콺�� ��ġ�� ������ ������ ���� �����־��� �ڸ��� ���ư���.
            if (ClickedObj != null && slot == null)
            {
                ClickedObj.PreSlot.SetNode(ClickedObj);
                ClickedObj = null;
            }

            if (ClickedObj != null)
                ClickedObj = null;
        }

    }


    //���� ���콺 ��ġ�� ����ִ� ������������ �տ� ����ִ� �������� �����ؼ� �ش� ���Կ� �������ش�. ������ �ϳ����������� �װ� ������ش�.
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
            //�ش� ���콺�� ��ġ���� ������ ã�´�.
            if (a.gameObject.tag == "Slot")
            {
                slot = a.gameObject.GetComponent<BaseSlot>();
                //�տ� ����ִ� �������� 2�� �̻��̰� �ش� ������ ����ִ� �����̸� �տ� ����ִ� ���Կ� �������� �ϳ� ��Ͻ��� �ش�.
                if (ClickedObj.GetStack()>1&&slot.SettingNode==null)
                {
                    node = ClickedObj.GetDuplicateNode();
                    if(node!=null)
                        slot.SetNode(node);
                    

                }
                //������ �Ѱ����������� �ش� ��带 ���Կ� �־��ְ� �巡�׸�� �� Ŭ���� ��
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
        //canvas�� �ִ� graphicraycast�� �̿��� Ŭ���� ��ġ�� �ִ� ��ü���� �������� �޾ƿ´�.
        PointerEventData ped = new PointerEventData(null);
        ped.position = pos;
        List<RaycastResult> result = new List<RaycastResult>();
        graphicRaycaster.Raycast(ped, result);

        BaseNode node = null;
        BaseSlot slot = null;

        Debug.Log($"����{result.Count}");

        //�ƹ��͵� ���� ������ ��带 �θ� �ش� ���� �ı�
        if(result.Count==1)
        {
            if (ClickedObj != null)
            {
                GameObject.Destroy(ClickedObj.gameObject);
                ClickedObj = null;
                return;
            }
        }

        //���� ������ �ش� ��ġ�� �ִ� ������Ʈ�� Ȯ��
        foreach (var a in result)
        {
            //�׷����ϴ� ��尡 ������
            if(ClickedObj !=null)
            {
                //�ش� ���콺�� ��ġ���� ������ ã�´�.
                if(a.gameObject.tag == "Slot")
                {
                    slot = a.gameObject.GetComponent<BaseSlot>();
                    if (slot.GetSlotTypes()!=EnumTypes.SlotTypes.Result)
                    {
                        //ã�� ������ �� �����϶� �ش� ������ ��带 �ش� ��忡 �������ش�.
                        if (slot.SettingNode == null)
                        {
                            slot.SetNode(ClickedObj);
                            //ClickedObj = null;
                        }
                        else
                        {
                            //Ŭ���� ���� �������� ����ִ� ������ �ְ� �ش� �������� �巡���ϰ� �ִ� �����۰� ���� �������̸� �������� �����Ѵ�.
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

        //�巡�� ���� ��尡 �ִµ� ���콺�� ��ġ�� ������ ������ ���� �����־��� �ڸ��� ���ư���.
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