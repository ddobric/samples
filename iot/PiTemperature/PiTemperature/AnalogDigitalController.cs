using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using TemperatureReader.Logic.Utilities;

using System.Diagnostics;

namespace TemperatureReader.Logic.Devices
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="https://www.jameco.com/Jameco/Products/ProdDS/831200.pdf"/>
    public class AnalogDigitalController
    {
        public const int AdcDefCsPinId = 5;
        public const int AdcDefClkPinId = 6;
        public const int AdcDefDigitalIoPinId = 13;
        public const int DefaultReadingInterval = 5000;
        public const int DefMaxRetries = 10;

        private readonly int cAdcCsPin;
        private readonly int cAdcClkPi;
        private readonly int cAdcDigitalIoPin;
         
        private GpioPin m_Cs;
        private GpioPin m_Clk;
        private GpioPin m_InOut;

        private Task m_Task;
        private CancellationTokenSource m_CancellationTokenSource;
        private DateTimeOffset m_LastExecTime = DateTimeOffset.MinValue;
        private readonly int m_ReadingDelayInMilliSeconds;
        private readonly int m_MaxRetries;
        private GpioController m_GpioCtrl;

        private Action<int> m_OnDataAvailable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onDataAvailable">Callback function invoked after reading of data from ADC.</param>
        /// <param name="adcCsPinId"></param>
        /// <param name="adcClkPinId"></param>
        /// <param name="adcDigitalIoPinId"></param>
        /// <param name="delayMilliSeconds"></param>
        /// <param name="maxRetries"></param>
        public AnalogDigitalController(
          Action<int> onDataAvailable,
          int adcCsPinId = AdcDefCsPinId,
          int adcClkPinId = AdcDefClkPinId,
          int adcDigitalIoPinId = AdcDefDigitalIoPinId,
          int delayMilliSeconds = DefaultReadingInterval,
          int maxRetries = DefMaxRetries)
        {
            m_OnDataAvailable = onDataAvailable;
            m_GpioCtrl = GpioController.GetDefault();
            cAdcCsPin = adcCsPinId;
            cAdcClkPi = adcClkPinId;
            cAdcDigitalIoPin = adcDigitalIoPinId;
            m_ReadingDelayInMilliSeconds = delayMilliSeconds;
            m_MaxRetries = maxRetries;
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }


        /// <summary>
        /// Starts reading process.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (m_GpioCtrl != null)
            {
                if (m_InOut == null)
                {
                    m_Cs = m_GpioCtrl.OpenPin(cAdcCsPin);
                    m_Clk = m_GpioCtrl.OpenPin(cAdcClkPi);
                    m_InOut = m_GpioCtrl.OpenPin(cAdcDigitalIoPin);
                }

                if (m_Task == null)
                {
                    m_CancellationTokenSource = new CancellationTokenSource();
                    initReading();
                    m_Task = new Task(async () =>
                      await readDataAsync(m_CancellationTokenSource.Token));
                    m_Task.Start();
                }

                IsRunning = true;

            }

            return IsRunning;
        }


        /// <summary>
        /// Stops reading process
        /// </summary>
        public void Stop()
        {
            m_CancellationTokenSource.Cancel();
            m_Cs.Dispose();
            m_Cs = null;
            m_Clk.Dispose();
            m_Clk = null;
            m_InOut.Dispose();
            m_InOut = null;
            IsRunning = false;
        }

        private async Task readDataAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var timePassed = DateTimeOffset.UtcNow - m_LastExecTime;
                if (timePassed > TimeSpan.FromMilliseconds(m_ReadingDelayInMilliSeconds))
                {
                    var retries = 0;
                    var readStatus = false;

                    while (!readStatus && retries++ < m_MaxRetries)
                    {
                        readStatus = readDataFromAdc();

                        m_LastExecTime = DateTimeOffset.UtcNow;
                    }

                    if (retries >= m_MaxRetries)
                    {
                        Debug.WriteLine("Failed to obtain data from ADC.");
                       // OnTemperatureMeasured?.Invoke(this, new TemperatureData { IsValid = false });
                    }
                    m_LastExecTime = DateTimeOffset.UtcNow;
                }
                else
                {
                    var waitTime = m_ReadingDelayInMilliSeconds - timePassed.TotalMilliseconds;

                    if (waitTime > 0)
                    {
                        await Task.Delay(Convert.ToInt32(waitTime), cancellationToken);
                    }
                }
            }
        }

        private void initReading()
        {
            // On CLK we give a clock signal.
            m_Clk.SetDriveMode(GpioPinDriveMode.Output);

            // Channel Select activate channel selection.
            m_Cs.SetDriveMode(GpioPinDriveMode.Output);

            // First we use this PIN as output to write channel number, 
            // before we start reading.
            m_InOut.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void selectChannel(SynchronousWaiter waiter)
        {
        
            m_Cs.Write(GpioPinValue.Low);       
            m_InOut.Write(GpioPinValue.High);
            // 1. Takt
            m_Clk.Write(GpioPinValue.Low);
            waiter.Wait(2);
            m_Clk.Write(GpioPinValue.High);
            waiter.Wait(2);

            // 2. Takt
            m_Clk.Write(GpioPinValue.Low);
            m_InOut.Write(GpioPinValue.High);
            waiter.Wait(2);
            m_Clk.Write(GpioPinValue.High);
            waiter.Wait(2);

            // 3 Takt
            m_Clk.Write(GpioPinValue.Low);
            m_InOut.Write(GpioPinValue.Low);
            waiter.Wait(2);
            m_Clk.Write(GpioPinValue.High);
            m_InOut.Write(GpioPinValue.High);
            waiter.Wait(2);

            // 4 Takt
            m_Clk.Write(GpioPinValue.Low);
            //m_InOut.Write(GpioPinValue.High);
            waiter.Wait(2);

            m_InOut.SetDriveMode(GpioPinDriveMode.Input);
        }

        private bool readDataFromAdc()
        {
            int sequence1 = 0, sequence2 = 0;
            m_Cs.Write(GpioPinValue.Low);

            initReading();

            var waiter = new SynchronousWaiter();

            selectChannel(waiter);

            //Read the first sequence
            for (var i = 0; i < 8; i++)
            {
                m_Clk.Write(GpioPinValue.High);
                waiter.Wait(2);
                m_Clk.Write(GpioPinValue.Low);
                waiter.Wait(2);
                sequence1 = sequence1 << 1 | (int)m_InOut.Read();
            }

            //Read the second sequence
            for (var i = 0; i < 8; i++)
            {
                sequence2 = sequence2 | (int)m_InOut.Read() << i;

                m_Clk.Write(GpioPinValue.High);
                waiter.Wait(2);
                m_Clk.Write(GpioPinValue.Low);
                waiter.Wait(2);
            }

            m_Cs.Write(GpioPinValue.High);

            if (sequence1 == sequence2)
            {
                m_OnDataAvailable?.Invoke(sequence1);

                return true;
            }

            return false;
        }
    }
}
