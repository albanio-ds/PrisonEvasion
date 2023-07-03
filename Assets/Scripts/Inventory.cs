using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    private List<Item> Items = new List<Item>();
    public event EventHandler<Item> OnItemEquipped;

    public void EquipItem(Item toEquip)
    {
        if (!Contains(toEquip.GetType()))
        {
            Items.Add(toEquip);
            OnItemEquipped?.Invoke(this, toEquip);
        }
    }

    public bool Contains(Type vtype)
    {
        return Items.Any(item => item.GetType() == vtype);
    }

    public Item Get(Type vtype)
    {
        Item res = Items.FirstOrDefault(item => vtype.IsAssignableFrom(item.GetType()));
        return res;
    }

    public void Unequip(Item item)
    {
        if (Contains(item.GetType()))
        {
            Items.Remove(item);
        }
    }
}
