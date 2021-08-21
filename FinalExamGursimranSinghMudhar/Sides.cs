using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalExamGursimranSinghMudhar
{
    public class Sides : IFoodItem
    {
        public int Pieces { get; set; }
        public Sides(int id, string name, string category, double baseprice, string description, string img, int pcs = 0)
        {
            FoodID = id;
            Name = name;
            CategoryName = category;
            BasePrice = baseprice;
            Description = description;
            Image = "assets/" + img;
            Pieces = pcs;
            switch (pcs)
            {
                case 8:
                    FinalPrice = BasePrice;
                    break;
                case 16:
                    FinalPrice = (BasePrice * 2) - 1;
                    break;
                case 32:
                    FinalPrice = BasePrice * 3;
                    break;
                default:
                    break;
            }
        }
        public new string GetPrice
        {
            get => BasePrice + "$/8pcs";
            set { }
        }
        //TODO: function to change size
    }
}
