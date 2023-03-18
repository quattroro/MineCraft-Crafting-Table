using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour
{
    Vector2Int Index;

    public bool _isactive;
    public bool _isclicked;

    public RectTransform rectTransform;

    public BaseSlot SettedSlot;

    public BaseSlot PreSlot;//노드가 클릭되어서 마우스를 따라다닐때 원래 있던 슬롯의 위치를 저장해 놓는다.



    public virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //아이템 추가용에 있는 비활성 노드인지 실제 게임 창에서 상호작용하는 활성노드인지 구분하기 위한 프로퍼티
    public bool NodeIsActive
    {
        get
        {
            return _isactive;
        }
        set
        {
            _isactive = value;
        }
    }

    //클릭된 상태인지 아닌지 
    public bool NodeIsClicked
    {
        get
        {
            return _isclicked;
        }
        set
        {
            _isclicked = value;

        }
    }

    //해당 노드를 절반씩 두개의 노드로 나눠준다.
    public virtual BaseNode DivideNode()
    {
        BaseNode temp = null;
        if(IsStackAble()&&GetStack()>=2)
        {
            int cur = GetStack() / 2;
            
            temp = GameObject.Instantiate<BaseNode>(this);
            temp.transform.parent = this.transform.parent;
            temp.NodeIsClicked = true;
            temp.PreSlot = SettedSlot;
            temp.ChangeStack(0);
            temp.ChangeStack(cur);
            this.ChangeStack(-cur);
        }
        else
        {
            temp = SettedSlot.GetSettingNode();
        }

        return temp;
    }
    //해당 아이템을 하나 떼어내서 리턴해준다.
    public virtual BaseNode GetDuplicateNode()
    {
        return null;
    }
    //아이템 병합
    public virtual void ItemMerge(BaseNode node)
    {

    }
    //아이템의 개수를 변경
    public virtual void ChangeStack(int val)
    {
        
    }
    //아이템의 개수를 리턴
    public virtual int GetStack()
    {
        return -1;
    }
    //아이템아이디를 리턴
    public virtual int GetItemID()
    {
        return -1;
    }
    //스택이 가능한 아이템인지 아닌지 구분
    public virtual bool IsStackAble()
    {
        return false;
    }


    public virtual void Update()
    {
        
    }
}
