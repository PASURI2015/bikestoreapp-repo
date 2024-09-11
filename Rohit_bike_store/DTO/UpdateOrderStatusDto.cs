namespace Rohit_bike_store.DTO
{
    public class UpdateOrderStatusDto
    {


        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public bool OrderApproved { get; set; }


    }
}