using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// Added
using Windows.ApplicationModel.Background;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BGTaskExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            foreach (var _task in BackgroundTaskRegistration.AllTasks)
            {
                if (_task.Value.Name == "My demo task")
                {
                    _task.Value.Unregister(true);
                }
            }


            // asking to register on lock screen
            BackgroundAccessStatus accessresult = await BackgroundExecutionManager.RequestAccessAsync();

            BGTaskMD.ExampleBackgroundTask b = new BGTaskMD.ExampleBackgroundTask();
            b.ShowToast(accessresult.ToString());

            if ((accessresult == BackgroundAccessStatus.Denied)||(accessresult == BackgroundAccessStatus.Unspecified))
            {
                return;
            }

            // just to show result of lock screen registration
            BGTaskMD.RegisterTask r = new BGTaskMD.RegisterTask();
            r.RegisterIt();


            // unregister ServicingComplete task if it was already registered
            foreach (var _task in BackgroundTaskRegistration.AllTasks)
            {
                if (_task.Value.Name == "My demo task updated")
                {
                    _task.Value.Unregister(true);
                }
            }


            // register ServicingComplete task
            SystemTrigger taskTrigger = new SystemTrigger(SystemTriggerType.ServicingComplete,false);
            var bgTaskBuilder = new BackgroundTaskBuilder();
            bgTaskBuilder.Name = "My demo task updated";
            bgTaskBuilder.TaskEntryPoint = "BGTaskMD.AppUpdateServicingCompleteTask";
            bgTaskBuilder.SetTrigger(taskTrigger);
            BackgroundTaskRegistration task = bgTaskBuilder.Register();

        }


    }
}
