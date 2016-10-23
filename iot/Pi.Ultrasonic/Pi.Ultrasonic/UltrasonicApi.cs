using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Daenet.Iot
{
    /// <summary>
    /// The API used to simplify using of ultrasonic sensor.
    /// </summary>
    public class UltrasonicApi
    {
        /// <summary>
        /// GPIO used to trigger UT signal.
        /// </summary>
        private GpioPin m_GpioUTScannTrigger;

        /// <summary>
        /// GPIO used to control receiving of UT echo.
        /// </summary>
        private GpioPin m_Echo;

        private GpioController m_Controller;

        private Stopwatch m_Stopwatch = new Stopwatch();

        private Int32 m_TriggerIoPort;

        private Int32 m_EchoIoPort;

        private bool m_IsSensorRunnuing = false;

        /// <summary>
        /// Distance measured in the previous step.
        /// </summary>
        private double m_PrevDistance;

        /// <summary>
        /// Time of previous measuring
        /// </summary>
        private double m_PrevTime;

        /// <summary>
        /// Invoked when the measuring is completed.
        /// </summary>
        private Action<double, double> m_OnScanComplete;

        private SlidingWindow m_SLidingWindow = new SlidingWindow(3, 1);

        private CancellationTokenSource m_TSrc = new CancellationTokenSource();

        private Task m_Task;

        #region Initialization

        /// <summary>
        /// Creates the instancce of the UltrasonicApi.
        /// </summary>
        /// <param name="triggerGpioPort">The GPIO port connected to trigger pin of UT sensor.</param>
        /// <param name="echoGpioPort">The GPIO port connected to trigger pin of UT sensor.</param>
        public UltrasonicApi(Int32 triggerGpioPort, Int32 echoGpioPort)
        {
            m_TriggerIoPort = triggerGpioPort;
            m_EchoIoPort = echoGpioPort;

            initialize();
        }

        private void initialize()
        {
            m_Controller = GpioController.GetDefault();
            if (m_Controller == null)
                throw new NotSupportedException("GPIO controller not supported.");

            //Open Gpio pins for triggering and the sensor, and listening the echo
            m_GpioUTScannTrigger = m_Controller.OpenPin(m_TriggerIoPort);
            m_Echo = m_Controller.OpenPin(m_EchoIoPort);

            // For UT initialization. Send UT signal
            m_GpioUTScannTrigger.SetDriveMode(GpioPinDriveMode.Output);

            // For UT echo reading.
            m_Echo.SetDriveMode(GpioPinDriveMode.Input);

            m_Echo.ValueChanged += gpioEchoValueChanged;

            m_GpioUTScannTrigger.Write(GpioPinValue.Low);

            //Delay of 2 milliseconds for the sensor to stabilize
            Task.Delay(2).Wait();
        }
        #endregion

        #region Public Methods

        public Task StopScan()
        {
            return Task.Run(() =>
            {
                if (m_Task != null)
                {
                    if (m_TSrc != null)
                        m_TSrc.Cancel();
                }

                m_Task.Wait();

                m_IsSensorRunnuing = true;
            });
        }


        /// <summary>
        /// Stars UT scanning process.
        /// </summary>
        /// <param name="onScanComplete">Action invoked after the measuring of distance and 
        /// velocity has completed.
        /// First argument in action is distance in meter. Second argument is velocity.</param>
        /// <returns></returns>
        public Task StartScan(Action<double, double> onScanComplete)
        {
            m_OnScanComplete = onScanComplete;

            m_IsSensorRunnuing = true;
            m_PrevDistance = 0;
            m_PrevTime = 0;

            m_Task = Task.Run(() =>
            {
                m_TSrc = new CancellationTokenSource();

                if (m_IsSensorRunnuing)
                {
                    // With this signal On/Off we send UT signal.
                    // We send high signal for 10ms. After waiting for 300ms
                    // we will start getting echo (if echo presented)
                    m_GpioUTScannTrigger.Write(GpioPinValue.High);
                    Task.Delay(TimeSpan.FromMilliseconds(0.01)).Wait();
                    m_GpioUTScannTrigger.Write(GpioPinValue.Low);
                    //Task.Delay(TimeSpan.FromMilliseconds(300)).Wait();
                }
            }, m_TSrc.Token);

            return m_Task;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Invoked when the value of echo port ischanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void gpioEchoValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            double distance;
            double velocity;

            if (m_IsSensorRunnuing)
            {
                //
                // On Rising Edge we start time counting,
                // because on rising edge the signal is sent.
                if (args.Edge == GpioPinEdge.RisingEdge)
                {
                    m_Stopwatch.Start();
                }

                //
                // On Falling edge we stop time counter, because
                // the echo of the signal has arrived.
                else if (args.Edge == GpioPinEdge.FallingEdge)
                {
                    m_Stopwatch.Stop();

                    // do calculattion
                    calcParameters(out distance, out velocity);

                    m_Stopwatch.Reset();

                    m_OnScanComplete?.Invoke(distance, velocity);
                }

            }
        }

        /// <summary>
        /// Calculates distance and velocity.
        /// </summary>
        /// <param name="distance">Distance to be calculated.</param>
        /// <param name="velocity">Velocity to be calculated.</param>
        private void calcParameters(out double distance, out double velocity)
        {
            distance = getCurrentDistance();

            velocity = getCurrentVelocity(distance);

            double avgVelocity = m_SLidingWindow.returnAvgValues(velocity);
        }

        private double getCurrentVelocity(double distance)
        {
            double time = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            double velocity = Math.Abs(m_PrevDistance - distance) / Math.Abs(time - m_PrevTime);
            //Debug.WriteLine("time {0}, velocity {1}, distance {2}", Math.Abs(time - m_PrevTime), velocity, distance);
            m_PrevDistance = distance;
            m_PrevTime = time;
            return velocity;
        }

        private double getCurrentDistance()
        {
            long microseconds = m_Stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));

            double distance = microseconds * 0.0001715;

            return distance;
        }

        #endregion
    }
}
