﻿using ToastNotifications.Core;

namespace OrganizationBankingSystem.Core
{
    /// <summary>
    /// Логика взаимодействия для CompletedPropertyNotification.xaml
    /// </summary>
    public partial class CompletedPropertyNotification : NotificationDisplayPart
    {
        public CompletedPropertyNotification(NotificationComp notificationComp)
        {
            InitializeComponent();
            Bind(notificationComp);
        }
    }
}