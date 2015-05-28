using System;
using System.Threading;
using MicroLiquidCrystal;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoMini;
using Toolbox.NETMF;

namespace BAM.TreadmillController
{
    public class Program
    {
        private static PWM treadmillSpeed;
        private static DateTime _lastPush;
        private static float _speed = 1.6F;
        private static uint _period = 3*1000*1000;
        //private static Lcd _lcd;

        public static void Main()
        {
            treadmillSpeed = new PWM(Pins.GPIO_PIN_20);
            
            //var treadmillSpeedRead = new AnalogInput(Pins.GPIO_PIN_5);
            //var treadmillSpeedRead = new InterruptPort(Pins.GPIO_PIN_5, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            var increaseSpeed = new InterruptPort(Pins.GPIO_PIN_6, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
            var decreaseSpeed = new InterruptPort(Pins.GPIO_PIN_5, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);

            //var provider = new GpioLcdTransferProvider(Pins.GPIO_PIN_13, Pins.GPIO_PIN_14, Pins.GPIO_PIN_15, Pins.GPIO_PIN_16, Pins.GPIO_PIN_17, Pins.GPIO_PIN_18, Pins.GPIO_PIN_19);
            //_lcd = new Lcd(provider);
            //_lcd.Begin(16, 2);
            
            //treadmillSpeedRead.OnInterrupt += treadmillSpeedRead_OnInterrupt;
            //increaseSpeed.OnInterrupt += increaseSpeed_OnInterrupt;
            //decreaseSpeed.OnInterrupt += decreaseSpeed_OnInterrupt;

            UpdateSpeed();

            Thread.Sleep(Timeout.Infinite);

            //while (true)
            //{
            //    var speed = treadmillSpeedRead.Read();
            //    Debug.Print(speed.ToString());
            //    Thread.Sleep(500);
            //}
        }

        private static void treadmillSpeedRead_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            Debug.Print("RPM signal received.");
        }

        private static void decreaseSpeed_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if ((time - _lastPush) > new TimeSpan(0, 0, 0, 1) && _speed > 1F)
            {
                _speed = _speed - .2F;
                Debug.Print("Button data1: "+data1);
                Debug.Print("Button data2: " + data2);
                Debug.Print("Speed decreased. New speed: " + _speed.ToString());
                _lastPush = time;
                UpdateSpeed();
            }
        }

        private static void increaseSpeed_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if ((time - _lastPush) > new TimeSpan(0, 0, 0, 1) && _speed < 6F)
            {
                _speed = _speed + .2F;
                Debug.Print("Button data1: " + data1);
                Debug.Print("Button data2: " + data2);
                Debug.Print("Speed increased. New speed: " + _speed.ToString());
                _lastPush = time;
                UpdateSpeed();
            }
        }

        private static void UpdateSpeed()
        {
            var pulseDuration = 601000 + (500*(uint) ((_speed - 1)/.2));
            treadmillSpeed.SetPulse(_period, pulseDuration);
            Debug.Print("New pulse duration: " + pulseDuration);
            var message = "New pulse duration: " + pulseDuration;

            //_lcd.Write(message);
        }
    }
}
