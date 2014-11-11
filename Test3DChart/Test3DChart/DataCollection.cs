using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Test3DChart
{
    public class DataCollection : Collection<SalesData>
    {
        public DataCollection()
        {
            Add(new SalesData { Actions = "Dogs", WestStoreQuantity = 5, EastStoreQuantity = 7 });
            Add(new SalesData { Actions = "Cats", WestStoreQuantity = 5, EastStoreQuantity = 6 });
            Add(new SalesData { Actions = "Birds", WestStoreQuantity = 3, EastStoreQuantity = 8 });
            Add(new SalesData { Actions = "Fish", WestStoreQuantity = 6, EastStoreQuantity = 9 });
            Add(new SalesData { Actions = "Fish2", WestStoreQuantity = 8, EastStoreQuantity = 19 });


        }
    }
}
