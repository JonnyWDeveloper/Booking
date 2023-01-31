using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.ViewModels
{
    public class GymClassesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public bool Attending { get; set; }

    }
}
