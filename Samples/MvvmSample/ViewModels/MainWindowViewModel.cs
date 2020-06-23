﻿using NIntercept;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmSample.ViewModels
{

    public class MainWindowViewModel
    {
        private DelegateCommand updateTitleCommand;

        [PropertyChanged]
        public virtual bool IsBusy { get; set; }

        public MainWindowViewModel()
        {
            Title = "Main title";
        }

        [PropertyChanged]
        public virtual string Title { get; set; }

        public DelegateCommand UpdateTitleCommand
        {
            get
            {
                if (updateTitleCommand == null)
                    updateTitleCommand = new DelegateCommand(ExecuteUpdateTitleCommand);
                return updateTitleCommand;
            }
        }

        [MethodInterceptor(typeof(MyAsyncInterceptor))]
        [MethodInterceptor(typeof(CanExecuteCommandInterceptor))]
        protected virtual void ExecuteUpdateTitleCommand()
        {
            Title += "!";
        }
    }

    public class MyAsyncInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var context = invocation.GetAwaitableContext();

            var viewModel = invocation.Proxy as MainWindowViewModel;
            if (viewModel != null)
                viewModel.IsBusy = true;

            Task.Delay(3000).Await(() =>
            {
                if (viewModel != null)
                    viewModel.IsBusy = false;
                context.Proceed();
            }, ex => { });
        }
    }

    public class CanExecuteCommandInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (MessageBox.Show("Can Execute?", $"Execute '{invocation.Member.Name}'", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                invocation.Proceed();
            }
            else
            {
                MessageBox.Show("Cancelled.", $"Execute '{invocation.Member.Name}'", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
    public class PropertyChangedAttribute : Attribute, IPropertySetInterceptorProvider
    {
        public Type InterceptorType => typeof(PropertyChangedInterceptor);
    }

    public interface IPropertyChangedNotifier : INotifyPropertyChanged
    {
        void OnPropertyChanged(object target, string propertyName);
    }

    [Serializable]
    public class PropertyChangedNotifier : IPropertyChangedNotifier
    {
        public void OnPropertyChanged(object target, string propertyName)
        {
            PropertyChanged?.Invoke(target, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PropertyChangedInterceptor : Interceptor
    {
        protected override void OnEnter(IInvocation invocation) { }
        protected override void OnException(IInvocation invocation, Exception exception)
        {
            Console.WriteLine($"Error: {exception.Message}");
        }

        protected override void OnExit(IInvocation invocation)
        {
            IPropertyChangedNotifier propertyChangedNotifier = invocation.Proxy as IPropertyChangedNotifier;
            if (propertyChangedNotifier != null)
            {
                string propertyName = invocation.Member.Name;
                propertyChangedNotifier.OnPropertyChanged(invocation.Proxy, propertyName);
            }
        }
    }
}
