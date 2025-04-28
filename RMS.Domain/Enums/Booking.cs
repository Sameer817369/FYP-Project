using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RMS.Domain.Enums
{
    public enum FoodType
    {
        Veg,
        [Display(Name="Non Veg")]
        NonVeg,
        Both
    }
    public enum Quality
    {
        Excellent,
        Good,
        Average,
        Poor
    }
    public enum DrinkType
    {
        [Display(Name = "Hard Drinks")]
        HardDrinks,
        [Display(Name = "Soft Drinks")]
        SoftDrinks,
        Both
    }
    public enum EventType
    {
        Marriage,
        Party,
        Corporate
    }
    public enum Roles
    {
        Manager,
        HeadChef,
        Cook,
        Dishwasher,
        Waiter,
        Bartender,
        Cashier
    }
    public enum Status
    { 
        Pending,
        Complete
    }
    public enum BookingStatus
    {
        Pending, Complete, Approved
    }

}
