using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Build_a_PC_Sales_Deal_Hunter.Models
{
    public class SummaryItemModel
    {
        public int Count { get; set; }
        public string Query { get; set; }
        public static List<SummaryItemModel> GetSummaryResults(List<TaskModel> Tasks)
        {
            var returnObject = new List<SummaryItemModel>();
            var uniqueQueries = new Dictionary<string, int>();
            var items = new HashSet<string>();
            foreach (var item in Tasks)
            {
                items.Add(item.Query);
            }

            foreach (var _item in items)
            {
                uniqueQueries.Add(_item, Tasks.Count(item => item.Query == _item));
            }

            var myList = uniqueQueries.ToList();

            myList.Sort((x, y) => x.Value.CompareTo(y.Value));
            myList.Reverse();

            uniqueQueries = myList.ToDictionary(pair => pair.Key, pair => pair.Value);

            var returnList = uniqueQueries.Keys.ToList();
            int increment = 0;
            foreach (var i in returnList)
            {
                if (increment == 5)
                    break;
                returnObject.Add(new SummaryItemModel()
                {
                    Count = Convert.ToInt32((double)uniqueQueries[i] / (double)uniqueQueries.Count() * (double)100),
                    Query = i
                });
                increment++;
            }
            return returnObject;
        }
    }
}