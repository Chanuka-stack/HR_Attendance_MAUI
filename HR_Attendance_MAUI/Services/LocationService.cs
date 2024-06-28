using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Attendance_MAUI.Services
{
    public class LocationService
    {
        private static CancellationTokenSource _cancelTokenSource;
        private static bool _isCheckingLocation;

        public async Task<Dictionary<string, double>> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    return new Dictionary<string, double>
                    {
                        { "Latitude", location.Latitude },
                        { "Longitude", location.Longitude }
                    };
                }
                else
                {
                    return new Dictionary<string, double> { { "Error", 0 } };
                }

            }

            catch (Exception ex)
            {
                return new Dictionary<string, double> { { "Error", 0 } };
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }

    }
}
