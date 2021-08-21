using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExamGursimranSinghMudhar
{
    public abstract class IFoodItem
    {
        public int FoodID { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public double BasePrice { get; set; }
        public double FinalPrice { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string GetPrice { get; set; }
        public string ShowBasePrice
        {
            get => "Base Price: " + BasePrice + "$";
            set { }
        }
        public string ShowTotalPrice
        {
            get => "Total Price: " + FinalPrice + "$";
            set { }
        }
        public Windows.UI.Xaml.Visibility isRealEntry
        {
            get => FoodID == 9999 ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
            set { }
        }
    }
}
