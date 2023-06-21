using System.Collections.Generic;

namespace CraftSystem.Data.Error
{
    using Elements;

    public class CSNodeErrorData
    {
        public CSErrorData ErrorData { get; set; }
        public List<CSNode> Nodes { get; set; }

        public CSNodeErrorData()
        {
            ErrorData = new CSErrorData();
            Nodes = new List<CSNode>();
        }
    }
}