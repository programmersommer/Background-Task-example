using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// added
using Windows.ApplicationModel.Background;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BGTaskMD
{
 
    public sealed class ExampleBackgroundTask : IBackgroundTask
    {

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        volatile bool _cancelRequested = false;


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            int taskRunCount = 0;
            try
            {
                taskRunCount = (int)localSettings.Values["AppRunCount"];

            }
            catch { }

            taskRunCount++;

            try
            {
                localSettings.Values["AppRunCount"] = taskRunCount;
            }
            catch { }

            if (_cancelRequested != true)
            {
                ShowToast("It's my Task - " + taskRunCount);
            }

        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // you can use sender.Task.Name to identity the task  
            _cancelRequested = true;
        }

        // snippet to show toast notification
        public void ShowToast(string whattext)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(whattext));
            ToastNotification toast = new ToastNotification(toastXml);

            toast.Activated += ToastActivated;
            toast.Dismissed += ToastDismissed;
            toast.Failed += ToastFailed;

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs args) { }
        private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs args) { }
        private void ToastActivated(ToastNotification sender, object args) { }

    }


    
    public sealed class AppUpdateServicingCompleteTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BGTaskMD.RegisterTask r = new BGTaskMD.RegisterTask();
            r.RegisterIt();
        }
    }



    public sealed class RegisterTask 
    {

        public void RegisterIt()
        {

            foreach (var _task in BackgroundTaskRegistration.AllTasks)
            {
                if (_task.Value.Name == "My demo task")
                {
                    _task.Value.Unregister(true);
                }
            }

            TimeTrigger taskTrigger = new TimeTrigger(15, false);
            var bgTaskBuilder = new BackgroundTaskBuilder();
            bgTaskBuilder.Name = "My demo task";
            bgTaskBuilder.TaskEntryPoint = "BGTaskMD.ExampleBackgroundTask";
            bgTaskBuilder.SetTrigger(taskTrigger);
            SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);
            bgTaskBuilder.AddCondition(internetCondition);
            BackgroundTaskRegistration task = bgTaskBuilder.Register();

            task.Completed += task_Completed;

        }


        void task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            if (sender.Name == "My demo task")
            {

                try
                {
                    args.CheckResult();

                    BGTaskMD.ExampleBackgroundTask b = new BGTaskMD.ExampleBackgroundTask();
                    b.ShowToast(sender.Name+" is completed");          
                }
                catch{}

            }
        }


    }




}
