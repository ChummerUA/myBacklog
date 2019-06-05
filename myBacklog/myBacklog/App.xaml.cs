using myBacklog.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLitePCL;
using myBacklog.Data;
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

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace myBacklog
{
    public partial class App : PrismApplication
    {
        static BacklogDatabase database;

        public static BacklogDatabase Database
        {
            get
            {
                if(database == null)
                {
                    database = new BacklogDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BacklogDBSQLite.db3"));
                }
                return database;
            }
        }

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


            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<CategoriesPage, CategoriesViewModel>();
            containerRegistry.RegisterForNavigation<SetCategoryPage, SetCategoryViewModel>();
            containerRegistry.RegisterForNavigation<ItemsPage, ItemsViewModel>();
            containerRegistry.RegisterForNavigation<SetColorPage, SetColorViewModel>();
        }
    }
}
