﻿using myBacklog.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace myBacklog.ViewModels
{
    public abstract class BaseViewModel : INavigationAware
    {
        protected INavigationService NavigationService { get; }
        protected IDialog DialogService { get; }

        public BaseViewModel(INavigationService navigationService, IDialog dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
            System.Diagnostics.Debug.WriteLine(this.GetType().Name);
        }
    }
}
