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

    //�ʱ⼼��
    public void InitSetting(int itemcode, string Name, Sprite sprite, Vector3 pos, string _type)
    {
        this.name = Name;
        this.itemName = Name;
        this.itemCode = itemcode;
        this.itemSprite = sprite;
        this.transform.localPosition = pos;

        //Debug.Log($"������ Ÿ��{_type}///");
        //Debug.Log($"///{EnumTypes.ItemTypes.Blocks.ToString()}///");
        
        if (_type == EnumTypes.ItemTypes.Blocks.ToString())
        {
            //Debug.Log($"��ε���");
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

    //������ �������ش�.
    public override void ChangeStack(int val)
    {
        //0�� ������ �ʱ�ȭ
        if (val == 0)
        {
            _Stack = 0;
            return;
        }
            
        //�������� �ִ� 99�� �ּ� 1��
        _Stack += val;

        if (_Stack > 99)
            _Stack = 99;
        if (_Stack < 1)
            _Stack = 1;

        //1���϶��� ���ڰ� ǥ�õ��� �ʵ���
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

    //������ 1���� ������ �������ش�.
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

    //������ ����
    public override void ItemMerge(BaseNode node)
    {
        if(node.GetItemID() == this.GetItemID())
        {
            this.ChangeStack(node.GetStack());
            GameObject.Destroy(node.gameObject);
        }
    }

    //������ ���� ����
    public override int GetStack()
    {
        return _Stack;
    }
    //������ ������ ����������
    public override bool IsStackAble()
    {
        return _isstackable;
    }
    //������ �ڵ带 ����
    public override int GetItemID()
    {
        return itemCode;
    }

    //�������� Ŭ���Ǿ����� ���콺�� ����ٴϰ� ���ش�.
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
