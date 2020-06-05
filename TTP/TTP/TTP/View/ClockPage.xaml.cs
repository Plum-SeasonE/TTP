﻿using CHD;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TTP.Services;
using TTP.Model;
using TTP.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TTP.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClockPage : PopupPage
    {
        public DateTime startTime;
        public TimeSpan recordLength;
        public TimeSpan dynamicLength;
        public string description;
        public ClockPage()
        {
            InitializeComponent();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                dynamicLength = dynamicLength.Subtract(TimeSpan.FromSeconds(1));
                header.Text = dynamicLength.ToString();
                range.EndValue = (dynamicLength.TotalSeconds / recordLength.TotalSeconds) * 360;
                if (dynamicLength.ToString().Equals("00:00:00")) // 结束了
                {
                    Finish();
                }
                if (!App.IsClockPageOn)
                {
                    return false;
                }
                Console.WriteLine(DateTime.Now.ToString());
                return true;
            });
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.IsClockPageOn = true;
            bool OnBackButtonPressed()
            {
                return false;
            }
            HWBackButtonManager.OnBackButtonPressedDelegate onBackButtonPressedDelegate = new HWBackButtonManager.OnBackButtonPressedDelegate(OnBackButtonPressed);
            HWBackButtonManager.Instance.SetHWBackButtonListener(onBackButtonPressedDelegate);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.IsClockPageOn = false;
            if (App.IsLogIn)
            {
                DependencyService.Get<IToastService>().LongAlert("锁机结束");
            }
            else
            {
                DependencyService.Get<IToastService>().LongAlert("锁机结束。由于你没有登录，此次锁机将不计算番茄点");
            }
            HWBackButtonManager.Instance.RemoveHWBackButtonListener();
        }

        private void btnQuit_Clicked(object sender, EventArgs e)
        {
            Finish();
        }

        private void btnLaunch_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new WhiteListPage());
        }

        private async void Finish()
        {
            DateTime currentTime = DateTime.Now;
            TomatoTime time = new TomatoTime()
            {
                BeginTime = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UserId = App.StaticUser.UserId,
                Description = description,
                BeginTimeDate = startTime.Date.ToLongDateString(),
                SpanString = startTime.ToShortTimeString() + " → " + currentTime.ToShortTimeString()
            };
            TomatoTimeViewModel.addRecord(time);
            int p = (DateTime.Now.Subtract(startTime).Minutes + 1) / 15+1;
            if (App.IsLogIn)
            {
                await App.TomatoTimeManager.AddTomatoTimeTaskAsync(time);
                App.StaticUser.TomatoPoints += p;
                await App.UserManager.ModifyUserTaskAsync(App.StaticUser);
            }
            // TODO: 加一个铃声提醒
            await PopupNavigation.Instance.PopAsync();
        }
    }
}