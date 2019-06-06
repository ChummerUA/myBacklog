using myBacklog.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLitePCL;
using System.IO;
using myBacklog.ViewModels;
using myBacklog.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using myBacklog.Services;
using Plugin.CloudFirestore;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace myBacklog
{
    public partial class App : PrismApplication
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/CategoriesPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDialog, DialogService>();
            containerRegistry.Register<IFirebase, FirebaseService>();
            //containerRegistry.Register<IDatabase, BacklogDatabase>();


            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<CategoriesPage, CategoriesViewModel>();
            containerRegistry.RegisterForNavigation<SetCategoryPage, SetCategoryViewModel>();
            containerRegistry.RegisterForNavigation<ItemsPage, ItemsViewModel>();
            containerRegistry.RegisterForNavigation<SetColorPage, SetColorViewModel>();
        }
    }
}
