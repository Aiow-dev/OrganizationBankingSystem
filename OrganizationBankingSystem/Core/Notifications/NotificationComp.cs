﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications.Core;

namespace OrganizationBankingSystem.Core.Notifications
{
    public class NotificationComp : NotificationBase, INotifyPropertyChanged
    {
        private CompletedPropertyNotification _displayPartUI;

        public override NotificationDisplayPart DisplayPart => _displayPartUI
            ??= new CompletedPropertyNotification(this);

        public NotificationComp(string message, MessageOptions messageOptions = null) : base(message, messageOptions)
        {
            Message = message;
        }

        private string _message;

        public new string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}