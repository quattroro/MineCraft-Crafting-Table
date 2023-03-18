using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : BaseSlot
{
    public override void Start()
    {
        base.Start();
        slottype = EnumTypes.SlotTypes.Item;
    }
}
