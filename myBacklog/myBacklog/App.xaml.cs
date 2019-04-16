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

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace myBacklog
{
    public partial class App : Application
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

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new CategoriesPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
