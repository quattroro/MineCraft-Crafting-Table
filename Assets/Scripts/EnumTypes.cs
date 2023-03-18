using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enumµé
public class EnumTypes
{
    public enum ItemCollums
    {
        ItemCode,
        ItemName,
        ResourceName,
        ResourceIndex,
        ItemType,
        CollumMax
    }

    public enum SlotTypes
    {
        Item,
        Craft,
        Result,
        TypeMax
    }

    public enum ItemTypes
    {
        Blocks,
        Equips,
        TypeMax
    }

    public enum RecipeCollums
    {
        ResultItem,
        ResultName,
        Count,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5,
        Slot6,
        Slot7,
        Slot8,
        Slot9,
        CollumMax
    }
}
