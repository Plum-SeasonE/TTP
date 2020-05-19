﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TTP.Model;
using TTP.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TTP.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoodsDetailPage : ContentPage
    {
        public GoodsDetailPage()
        {
            InitializeComponent();
            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            GoodsDetailViewModel gdvm = BindingContext as GoodsDetailViewModel;
            if (gdvm.GoodsModel.User.Name == App.StaticUser.Name)
            {
                await App.GoodsManager.DeleteGoodsTaskAsync(gdvm.GoodsModel);
                GoodsViewModel.refresh();
                await DisplayAlert("提示", "删除商品成功", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                if (App.StaticUser.TomatoPoints >= gdvm.GoodsModel.Price)
                {
                    App.StaticUser.TomatoPoints -= gdvm.GoodsModel.Price;
                    await App.UserManager.ModifyUserTaskAsync(App.StaticUser);
                    await DisplayAlert("成功", "购买商品成功", "OK");
                    await Navigation.PopAsync();
                }
                else 
                {
                    await DisplayAlert("失败", "剩余点数不足购买！", "OK");
                }
            }
        }

        protected override void OnAppearing()
        {
            GoodsDetailViewModel gdvm = BindingContext as GoodsDetailViewModel;
            if (gdvm.GoodsModel.User.Name == App.StaticUser.Name)
            {
                bt.Text = "删除";
            }
            else
            {
                bt.Text = "购买";
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            GoodsDetailViewModel gdvm = BindingContext as GoodsDetailViewModel;
            CharPageViewModel.AUser CurrentUser = new CharPageViewModel.AUser()
            {
                UserId = App.StaticUser.UserId,
                Name = App.StaticUser.Name,
                Avatar = App.StaticUser.Imgurl
            };
            CharPageViewModel.AUser SendToUser = new CharPageViewModel.AUser()
            {
                UserId = gdvm.GoodsModel.UserId,
                Name = gdvm.GoodsModel.User.Name,
                Avatar = gdvm.GoodsModel.User.Imgurl
            };
            Navigation.PushAsync(new ChatPage()
            {
                BindingContext = new CharPageViewModel(CurrentUser,SendToUser)
            });
        }
    }
}