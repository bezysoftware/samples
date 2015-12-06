namespace NameDayClient
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using Windows.ApplicationModel.AppService;
    using Windows.ApplicationModel.Contacts;
    using Windows.Foundation.Collections;
    using Windows.System;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Calendar.Date = DateTime.Today;
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!Calendar.Date.HasValue)
            {
                return;
            }

            this.Names.Items.Clear();
            this.Contacts.Items.Clear();

            // connection details
            AppServiceConnection app = new AppServiceConnection();
            app.AppServiceName = "NameDayService";
            app.PackageFamilyName = "9AABFA2B.10465E12B64C8_grzn081tst3he";

            // open connection
            AppServiceConnectionStatus status = await app.OpenAsync();

            if (status == AppServiceConnectionStatus.Success)
            {
                // target service keeps the connection open until it receives an "exit" message - it can be part of a single ValueSet, or it can be sent separately
                // this allows you to send multiple messages with different dates
                ValueSet message = new ValueSet();

                // make sure that the date is in correct format, e.g. 2015-12-31
                message.Add("date", Calendar.Date.Value.ToString("yyyy-MM-dd"));
                message.Add("exit", "");

                // send message and await result
                AppServiceResponse response = await app.SendMessageAsync(message);

                if (response.Status == AppServiceResponseStatus.Success)
                {
                    // the result is a json, this will deserialize it to object of type ServiceResult
                    var result = JsonConvert.DeserializeObject<ServiceResult>(response.Message["result"].ToString());

                    result.Names.Select(n => n.DisplayValue).ToList().ForEach(this.Names.Items.Add);

                    if (result.Contacts.Any())
                    {
                        // get access to the list of contacts. This also requires Contacts capability in app manifest
                        var store = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
                        var contacts = await store.FindContactsAsync();

                        // translate the returned ids to names
                        result.Contacts.Select(id => contacts.FirstOrDefault(c => c.Id == id)).Select(c => c.DisplayName).ToList().ForEach(this.Contacts.Items.Add);
                    }
                }
            }
            else if (status == AppServiceConnectionStatus.AppNotInstalled)
            {
                // app is not installed, redirect user to the store to download it first
                await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:navigate?appid={0}", "1340547d-bde8-4b63-a827-07272b267558")));
            }
        }
    }
}
