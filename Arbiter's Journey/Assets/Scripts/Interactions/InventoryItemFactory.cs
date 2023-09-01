using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interactions
{
    public static class InventoryItemFactory
    {
        public static InventoryItem MakeGold(int value)
        {
            InventoryItem gold = new InventoryItem();
            gold.value = value;
            gold.name = value.ToString() + "gp";
            gold.description = "A pile of gold.";

            return gold;
        }

        public static InventoryItem MakeRandomGold(int max)
        {
            InventoryItem gold = new InventoryItem();
            gold.value = UnityEngine.Random.Range(0, max);
            gold.name = gold.value.ToString() + "gp";
            gold.description = "A pile of gold.";

            return gold;
        }
    }
}
