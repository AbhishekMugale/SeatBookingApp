using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SeatBooking.Repository.Business_Services.Business_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.Business_Services
{
    public class BookingStatusUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BookingStatusUpdaterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    await bookingService.UpdateBookingStatusAsync();

                    // Wait for 1 minute before the next execution
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
