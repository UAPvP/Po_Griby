using Po_Griby.Gameplay;
using Po_Griby.MapSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po_Griby.Gameplay
{
    internal class QuestNPC: Entity
    {
        private readonly string dialogue;
        private readonly Quests quest;

        public QuestNPC(
            char symbol,
            Vector2D position,
            string dialogue,
            string questId,
            string questDescription,
            int questAmount)
            : base(symbol, position)
        {
            this.dialogue = dialogue;
            quest = new Quests(questId, questDescription, questAmount);
        }

        public override string? Interact(Player player)
        {
            if (player.CompletedQuests.Contains(quest.Id))
                return dialogue + "\nЗавдання виконано!";

            if (player.CurrentQuest == null)
            {
                player.CurrentQuest = quest;
                return dialogue;
            }

            if (player.CurrentQuest.Id != quest.Id)
                return "Треба виконати завдяння!";

            int required = player.CurrentQuest.RequiredAmount;
            int count = player.CurrentQuest.GetProgress(player.PlayerInventory);

            if (count < required)
            {
                return "Тобі потрібно ще " + (required - count) + " грибів!";
            }

            if (player.PlayerInventory.RemoveQuestItems(required))
            {
                player.CompletedQuests.Add(quest.Id);
                player.CurrentQuest = null;

                player.PlayerInventory.AddItem(
                    new MushroomItem("Золотий гриб",ItemType.Heal,50,"Рідкісний золотий гриб."));

                return "Квест виконано! Ти отримуєш Золотий гриб!";
            }

            return "Не вдалося здати гриби.";
        }

    }
}
