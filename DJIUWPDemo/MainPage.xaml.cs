using DJIDemo.Controls;
using DJISDK;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace DJIDemo
{
    public sealed partial class MainPage : Page
    {
        DJIClient djiClient = DJIClient.Instance;
	
        public MainPage()
        {        
            this.InitializeComponent();
            viewModel = new MainPageViewModel(Dispatcher, djiClient);
        }

        private MainPageViewModel viewModel;
        public MainPageViewModel ViewModel => viewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Setup Joystick listener
            Task.Run(() => { ListenForJoystick(); });
        }

        #region Joystick Controls
        // Take off button
        private void ButtonTakeOff_Click(object sender, RoutedEventArgs e)
        {
            SetJoyStickValue(new JoyStickValues(-1, -1, -1, 1));
        }

        // Take off button in Landing mode
        private void ButtonLand_Click(object sender, RoutedEventArgs e)
        {
            SetJoyStickValue(new JoyStickValues(-1, 0, 0, 0));
        }

        // Go home button
        private void ButtonGoHome_Click(object sender, RoutedEventArgs e)
        {
            // todo:add gohome from djiclient
            bool goingHome = ((sender as CircularToggleButton).IsChecked == true);
        }

        private void JoystickLeft_OnJoystickReleased(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSL Released");
            SetJoyStickValue(new JoyStickValues(0, null, null, 0));
        }

        private void JoystickLeft_OnJoystickMoved(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSL Moved");
            SetJoyStickValue(new JoyStickValues(e.YValue, null, null, e.XValue));
        }

        private void JoystickRight_OnJoystickReleased(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSR Released");
            SetJoyStickValue(new JoyStickValues(null, 0, 0, null));
        }

        private void JoystickRight_OnJoystickMoved(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSR Moved");
            SetJoyStickValue(new JoyStickValues(null, e.XValue, e.YValue, null));
        }

        static BlockingCollection<JoyStickValues> JoyStickValuesQueue = new BlockingCollection<JoyStickValues>();

        private void SetJoyStickValue(JoyStickValues newValues)
        {
            JoyStickValuesQueue.TryAdd(newValues);
        }

        private void ListenForJoystick()
        {
            JoyStickValues current = new JoyStickValues(0, 0, 0, 0);
            foreach (var joystickItem in JoyStickValuesQueue.GetConsumingEnumerable())
            {
                current.throttle = joystickItem.throttle ?? current.throttle;
                current.roll = joystickItem.roll ?? current.roll;
                current.pitch = joystickItem.pitch ?? current.pitch;
                current.yaw = joystickItem.yaw ?? current.yaw;
                djiClient.SetJoyStickValue((float)current.throttle, (float)current.roll, (float)current.pitch, (float)current.yaw);
            }
        }
        #endregion //Joystick Controls
    }
}
