using System.Collections.Generic;

namespace CraftSystem.Data.Error
{
    using Elements;

    public class CSGroupErrorData
    {
        public CSErrorData ErrorData { get; set; }
        public List<CSGroup> Groups { get; set; }

        public CSGroupErrorData()
        {
            ErrorData = new CSErrorData();
            Groups = new List<CSGroup>();
        }
    }
}