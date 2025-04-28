using System.ComponentModel.DataAnnotations;

namespace RMS.Domain.Enums
{
    public enum MenuCategory
    {
        Veg,
        [Display(Name ="Non Veg")]
        Non_Veg,
        [Display(Name = "Kimchi Special")]
        Kimchi_Special
    }
    public enum InventoryCategory
    {
        Vegetable,
        Herb,
        Fruit,
        Milk,
        Cheese,
        Butter,
        Cream,
        Pork,
        Chicken,
        Buff,
        Mutton,
        Fish,
        Rice,
        Pasta,
        Sugar,
        Flour,
        Spices,
        [Display(Name = "Soft Drinks")]
        SoftDrink,
        [Display(Name = "Hard Drinks")]
        HardDrink,
        Tea,
        Coffee,
        [Display(Name = "Cooking Oil")]
        CookingOil,
        Napkins,
        Detergents,
    }
    public enum InventoryStatus
    {
        Low,
        Medium,
        High,
        OutOfStock
    }
    public enum InventoryUnit
    {
        Kg,
        Box,
        Pieces
    }
}
