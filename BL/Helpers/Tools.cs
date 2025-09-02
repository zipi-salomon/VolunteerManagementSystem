using DalApi;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace Helpers
{
    static internal class Tools
    {
        private static IDal s_dal = Factory.Get; //stage 4

        public static string ToStringProperty<T>(this T t)
        {
            string str = "";
            foreach (PropertyInfo item in typeof(T).GetProperties())
            {
                var value = item.GetValue(t, null);
                str += item.Name + ": ";
                if (value is not string && value is IEnumerable)
                {
                    str += "\n";
                    foreach (var it in (IEnumerable<object>)value)
                    {
                        str += it.ToString() + '\n';
                    }
                }
                else
                    str += value?.ToString() + '\n';
            }
            return str;
        }

        /// <summary>
        /// Function to convert degrees to radians
        /// </summary>
        /// <param name="degrees">Degrees to convert: double</param>
        /// <returns>Degrees in radians</returns>
        internal static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        /// <summary>
        /// Function that calculates the longitude and latitude of an address
        /// </summary>
        /// <param name="address">Address for calculation: string</param>
        /// <returns>An array of length 2 where the first index is width and the second index is length</returns>
        public static async Task<double[]?> CalcCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            string link = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=679a8da6c01a6853187846vomb04142";

            try
            {
                using HttpClient client = new();
                string response = await client.GetStringAsync(link);

                var result = JsonSerializer.Deserialize<GeocodeResponse[]>(response);
                if (result == null || result.Length == 0)
                {
                    throw new BO.BlInvalidValueException("Invalid address.");
                }

                double latitude = double.Parse(result[0].lat);
                double longitude = double.Parse(result[0].lon);
                return [latitude, longitude];
            }
            catch
            {
                return null;
            }
        }

        public class GeocodeResponse
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }
        /// <summary>
        /// Function to calculate distance between volunteer and call
        /// </summary>
        /// <param name="addressVol">Volunteer address</param>
        /// <param name="addressCall">Call address</param>
        /// <returns>distance</returns>
        /// <exception cref="BO.BlInvalidValueException">Invalid addresses</exception>
        internal static double CalcDistance(DO.Volunteer vol, DO.Call call)
        {
            if (vol.Address == null)
                throw new BO.BlInvalidValueException("the value of volunteer address can not be null");
            //double[]? volunteerLonLat = CalcCoordinates(addressVol);
            //double[]? callLonLat = CalcCoordinates(addressCall);
            if (vol?.Latitude == null || vol?.Longitude== null || call.Longitude == null || call.Latitude == null)
                return 0;
            const double R = 6371; // רדיוס כדור הארץ בק"מ
            double dLat = DegreesToRadians((double)vol.Latitude - call.Latitude);
            double dLon = DegreesToRadians((double)vol.Longitude - call.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(call.Latitude)) * Math.Cos(DegreesToRadians((double)vol.Latitude)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // המרחק בקילומטרים
        }

    }
}