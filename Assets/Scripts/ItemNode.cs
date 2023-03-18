using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNode : BaseNode
{
    public string itemName;
    public int itemCode;
    public Sprite itemSprite;

    public int _Stack;

    public bool _isstackable = false;

    public EnumTypes.ItemTypes itemtype;

    public Text stacktext;

    public override void Awake()
    {
        base.Awake();
        //stacktext = GetComponentInChildren<Text>();
    }

    //초기세팅
    public void InitSetting(int itemcode, string Name, Sprite sprite, Vector3 pos, string _type)
    {
        this.name = Name;
        this.itemName = Name;
        this.itemCode = itemcode;
        this.itemSprite = sprite;
        this.transform.localPosition = pos;

        //Debug.Log($"아이템 타입{_type}///");
        //Debug.Log($"///{EnumTypes.ItemTypes.Blocks.ToString()}///");
        
        if (_type == EnumTypes.ItemTypes.Blocks.ToString())
        {
            //Debug.Log($"욜로들어옴");
            this._isstackable = true;
            this.itemtype = EnumTypes.ItemTypes.Blocks;
        }
        else
        {
            this._isstackable = false;
            this.itemtype = EnumTypes.ItemTypes.Equips;
        }

        ChangeStack(+1);

        GetComponent<Image>().sprite = sprite;
        
    }

    //개수를 변경해준다.
    public override void ChangeStack(int val)
    {
        //0이 들어오면 초기화
        if (val == 0)
        {
            _Stack = 0;
            return;
        }
            
        //아이템은 최대 99개 최소 1개
        _Stack += val;

        if (_Stack > 99)
            _Stack = 99;
        if (_Stack < 1)
            _Stack = 1;

        //1개일때는 숫자가 표시되지 않도록
        if(_Stack == 1)
        {
            if (stacktext.gameObject.activeSelf)
                stacktext.gameObject.SetActive(false);
        }
        else
        {
            if (!stacktext.gameObject.activeSelf)
                stacktext.gameObject.SetActive(true);

            stacktext.text = string.Format("{0}", _Stack);
        }
    }

    //아이템 1개를 나눠서 리턴해준다.
    public override BaseNode GetDuplicateNode()
    {
        BaseNode copynode = null;
        if (GetStack() <= 1)
            return copynode;

        copynode = GameObject.Instantiate<BaseNode>(this);
        copynode.NodeIsActive = true;
        copynode.ChangeStack(0);
        copynode.ChangeStack(1);
        this.ChangeStack(-1);

        return copynode;
    }

    //아이템 병합
    public override void ItemMerge(BaseNode node)
    {
        if(node.GetItemID() == this.GetItemID())
        {
            this.ChangeStack(node.GetStack());
            GameObject.Destroy(node.gameObject);
        }
    }

    //아이템 개수 리턴
    public override int GetStack()
    {
        return _Stack;
    }
    //스택이 가능한 아이템인지
    public override bool IsStackAble()
    {
        return _isstackable;
    }
    //아이템 코드를 리턴
    public override int GetItemID()
    {
        return itemCode;
    }

    //아이템이 클릭되었을땐 마우스를 따라다니게 해준다.
    public override void Update()
    {
        if(NodeIsActive)
        {
            if(NodeIsClicked)
            {
                this.transform.position = Input.mousePosition;
            }
        }
    }
}
