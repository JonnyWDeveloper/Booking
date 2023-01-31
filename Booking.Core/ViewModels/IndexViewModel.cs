using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<GymClassesViewModel> GymClasses { get; set; } = new List<GymClassesViewModel>();
        public bool ShowHistory { get; set; }
    }
}
