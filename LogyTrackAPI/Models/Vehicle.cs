using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogyTrackAPI.Models
{    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Vehicle number is required")]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100)]
        public string Model { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public decimal CapacityKg { get; set; }

        public int? DriverId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Available";

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public Driver Driver { get; set; }
    }
}