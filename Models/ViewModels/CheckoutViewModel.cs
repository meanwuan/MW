namespace THweb.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public int CartId { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}