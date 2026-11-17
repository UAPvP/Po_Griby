using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq;

namespace Po_Griby.Gameplay
{
    public class Quests
    {
        public string Id { get; }
        public string Description { get; }
        public int RequiredAmount { get; }

        public bool IsCompleted(Inventory inventory)
        {
            return GetProgress(inventory) >= RequiredAmount;
        }

        public int GetProgress(Inventory inventory)
        {
            return inventory.Items.Count(item => item.IsQuestMushroom);
        }

        public Quests(string id, string description, int requiredAmount)
        {
            Id = id;
            Description = description;
            RequiredAmount = requiredAmount;
        }

        public Quests Copy()
        {
            return new Quests(Id, Description, RequiredAmount);
        }
    }
}