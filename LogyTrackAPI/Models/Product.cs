using System;

namespace LogyTrackAPI.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public string Status { get; set; } // Unassigned, Assigned, InTransit, Delivered, Returned
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Navigation properties (for joins)
        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
    }
}