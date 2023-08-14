using System.Collections.Generic;

namespace Game.CraftSystem.Research
{
    using global::CraftSystem.ScriptableObjects;
    using System.Linq;

    public class ResearchTreeCraft
    {
        public CSCraftContainerSO craftContainer { get; set; }
        public CSCraftGroupSO group { get; set; }
        public IReadOnlyList<CSTreeCraftSO> crafts { get; set; }

        public int maxLevel { get { return _maxLevel; } }
        public int level { get { return _level; } }

        private int _maxLevel;
        private int _level;

        public ResearchTreeCraft(CSCraftContainerSO craftContainer, CSCraftGroupSO group, List<CSTreeCraftSO> crafts)
        {
            this.craftContainer = craftContainer;
            this.group = group;
            this.crafts = GetSortedCrafts(crafts);

            _maxLevel = crafts.Count - 1;
            _level = 0;
        }

        public bool Upgradable()
        {
            if((_level + 1) > maxLevel)
            {
                return false;
            }
            return true;
        }
        public void Upgrade()
        {
            _level++;
        }

        public bool IsStartCraft()
        {
            return crafts[0].IsStartingCraft;
        }

        public CSTreeCraftSO GetPreviousCraft()
        {
            if (_level == 0)
                return null;

            return crafts[_level - 1];
        }
        public CSTreeCraftSO GetCurrentCraft()
        {
            return crafts[_level];
        }
        public CSTreeCraftSO GetNextCraft()
        {
            if (_level == maxLevel)
                return null;

            return crafts[level + 1];
        }

        public int IndexOf(CSTreeCraftSO craft)
        {
            for (int i = 0; i < crafts.Count; i++)
            {
                if (crafts[i] == craft)
                    return i;
            }
            return -1;
        }

        private void LoadData(CSTreeCraftSO current)
        {
            _level = IndexOf(current);
        }
        public bool LoadData(List<CSTreeCraftSO> current)
        {
            var list = current.Where(result => crafts.Contains(result)).ToArray();

            if(list.Length == 1)
            {
                LoadData(list[0]);
                return true;
            }
            else if (list.Length > 1)
            {
                Dictionary<CSTreeCraftSO, int> results = new Dictionary<CSTreeCraftSO, int>();
                foreach (var item in list)
                {
                    results.Add(item, IndexOf(item));
                }
                LoadData(results.OrderByDescending(pair => pair.Value).First().Key);
                return true;
            }

            return false;
        }

        private List<CSTreeCraftSO> GetSortedCrafts(List<CSTreeCraftSO> input)
        {
            CSTreeCraftSO current = System.Array.Find(input.ToArray(), result => result.IsStartingCraftInGroup | result.IsStartingCraft);
            List<CSTreeCraftSO> output = new List<CSTreeCraftSO>() { current };
            for (int i = 0; i < input.Count; i++)
            {
                current = current.Choices[0].Next;

                if (current == null)
                    break;
                if (!input.Contains(current))
                    continue;

                output.Add(current);
            }
            return output;
        }
    }
}
