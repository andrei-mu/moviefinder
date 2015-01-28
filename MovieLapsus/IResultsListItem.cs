using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLapsus
{
    public interface IResultsListItem
    {
        string ItemName();

        string ItemDescription();

        string ItemImageUrl();

        string ItemID();

        float ItemPriority();
    }
}
