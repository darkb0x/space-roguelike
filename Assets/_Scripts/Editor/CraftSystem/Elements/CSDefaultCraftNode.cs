using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CraftSystem.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class CSDefaultCraftNode : CSNode
    {
        public override void Initialize(string nodeName, CSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            CraftType = CSCraftType.DefaultCraft;

            CSChoiceSaveData choiceData = new CSChoiceSaveData()
            {
                Text = "Next Craft"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            foreach (CSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
