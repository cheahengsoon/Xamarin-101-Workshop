using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Plugin.Messaging;
namespace SalusDemo
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            //Read the Phone Number from Temporary Storage
            if (Application.Current.Properties.ContainsKey("PhoneNo"))
            {
                var phonenumber = (string)Application.Current.Properties["PhoneNo"];
                txtPhoneNumber.Text = phonenumber;
            }
            // AskPermission();

            GetGPS();
            GetTrack();
        }

        private async void GetTrack()
        {
            try
            {
                if (CrossGeolocator.Current.IsListening)
                {
                    await CrossGeolocator.Current.StopListeningAsync();
                    lbllocation.Text = "Stopped tracking";

                }
                else
                {
                    if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(30000), 0))

                    {
                        lbllocation.Text = "Started tracking";

                    }
                }
            }
            catch //(Exception ex)
            {
                //Xamarin.Insights.Report(ex);
                // await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured it in Xamarin Insights! Thanks.", "OK");
            }
        }

        public async void GetGPS()
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 1000;
                lbllocation.Text = "Getting gps";

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10000));

                if (position == null)
                {
                    lbllocation.Text = "null gps :(";
                    return;
                }
                lbllocation.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2}" +
                   " \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                    position.Timestamp, position.Latitude, position.Longitude,
                    position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);

            }
            catch //(Exception ex)
            {
                // Xamarin.Insights.Report(ex);
                // await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured it in Xamarin Insights! Thanks.", "OK");
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
            }
            catch
            {
            }
        }

        void CrossGeolocator_Current_PositionError(object sender, Plugin.Geolocator.Abstractions.PositionErrorEventArgs e)
        {

            lbllocation.Text = "Location error: " + e.Error.ToString();
        }

        void CrossGeolocator_Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var position = e.Position;
            lbllocation.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: "
                + "{3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                position.Timestamp, position.Latitude, position.Longitude,
                position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;
            }
            catch
            {
            }
        }

        private void PhoneNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Temporary Storage for Phone Number from txtxPhoneNumber
            var phonenumber = txtPhoneNumber.Text;
            Application.Current.Properties["PhoneNo"] = phonenumber;
        }

        void OnAlertYesNoClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudio>().PlayAudioFile("policesiren.mp3");
        }

        void SendSMS(object sender, EventArgs e)
        {
            var SmsTask = CrossMessaging.Current.SmsMessenger;

            if (SmsTask.CanSendSms)
                SmsTask.SendSms(txtPhoneNumber.Text, "I'm in Danger,My Location: " + lbllocation.Text);
        }
        void CallPolice(object sender, EventArgs e)
        {
            //Don't forgot to enable ID_CAP_PHONEDAILER on manifest file  
            var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (PhoneCallTask.CanMakePhoneCall)
                PhoneCallTask.MakePhoneCall("999");
            // DisplayAlert("Call Police", "Call 999", "Cancel");
        }
        void CallHospital(object sender, EventArgs e)
        {
            //Don't forgot to enable ID_CAP_PHONEDAILER on manifest file  
            var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (PhoneCallTask.CanMakePhoneCall)
                PhoneCallTask.MakePhoneCall("999");
            // DisplayAlert("Call Ambulance", "Call 999", "Cancel");
        }

    }
}

