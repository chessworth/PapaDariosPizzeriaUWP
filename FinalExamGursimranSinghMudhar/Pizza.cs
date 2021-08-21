using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExamGursimranSinghMudhar
{
    public class Pizza : IFoodItem
    {
        public char Size { get; set; }
        public Pizza(int id, string name, string category, double baseprice, string description, string img, char size = 'd')
        {
            FoodID = id;
            Name = name;
            CategoryName = category;
            BasePrice = baseprice;
            Description = description;
            Image = img;
            Size = size;
            switch (size)
            {
                case 's':
                    FinalPrice = BasePrice;
                    break;
                case 'm':
                    FinalPrice = BasePrice + 3;
                    break;
                case 'l':
                    FinalPrice = BasePrice + 5;
                    break;
                default:
                    break;
            }
        }
        //TODO: function to change size
    }
}
