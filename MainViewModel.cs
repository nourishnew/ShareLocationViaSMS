using Imhere.UWP;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json;


namespace ImHere
{
    public class MainViewModel : BaseViewModel
    {
        string message = "";
        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        string phoneNumbers = "";
        public string PhoneNumbers
        {
            get => phoneNumbers;
            set => Set(ref phoneNumbers, value);
        }

        public MainViewModel()
        {
            SendLocationCommand = new Command(async () => await SendLocation());
        }


        public ICommand SendLocationCommand { get; }
        HttpClient client = new HttpClient();
        const string baseUrl = "https://imherefunctions20191003113917.azurewebsites.net";

        async Task SendLocation()
        {
            Location location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                Message = $"Location found: {location.Latitude}, {location.Longitude}.";
                Imhere.Data.PostData postData = new Imhere.Data.PostData
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    ToNumbers = PhoneNumbers.Split('\n')
                };

                string data = JsonConvert.SerializeObject(postData);
                StringContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync($"{baseUrl}/api/SendLocation",
                                                                    content);

                if (result.IsSuccessStatusCode)
                    Message = "Location sent successfully";
                else
                    Message = $"Error - {result.ReasonPhrase}";
            }
        }
    }
}