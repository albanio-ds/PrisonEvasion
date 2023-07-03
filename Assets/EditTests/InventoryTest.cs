using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InventoryTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void InventoryTestEquip()
    {
        Inventory inventory = new Inventory();
        Assert.IsFalse(inventory.Contains(typeof(ExitKey)));
        inventory.EquipItem(new ExitKey());
        Assert.IsTrue(inventory.Contains(typeof(ExitKey)));
        Assert.NotNull(inventory.Get(typeof(ExitKey)));
    }
    
    // A Test behaves as an ordinary method
    [Test]
    public void InventoryTestGet()
    {
        Inventory inventory = new Inventory();
        Assert.IsNull(inventory.Get(typeof(ExitKey)));
        inventory.EquipItem(new ExitKey());
        Assert.NotNull(inventory.Get(typeof(ExitKey)));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator InventoryTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
