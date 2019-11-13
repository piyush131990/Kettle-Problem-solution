using BDL.Kettle.Model.Inputs;
using BDL.Kettle.Model.Outputs;
using BDL.Kettle.Model.Sensors;
using BDL.Kettle.Model.Workers;
using System;


namespace BDL.Kettle.Controller
{
    public class KettleController
    {
        private readonly IPowerSwitch powerSwitch;
        private readonly IPowerLamp powerLamp;
        private readonly IHeatingElement heatingElement;
        private readonly IWaterSensor waterSensor;
        private readonly ITemperatureSensor temperatureSensor;

        public KettleController(IPowerSwitch powerSwitch,
            IPowerLamp powerLamp,
            IHeatingElement heatingElement,
            IWaterSensor waterSensor,
            ITemperatureSensor temperatureSensor)
        {
            this.powerSwitch = powerSwitch ?? throw new ArgumentNullException(nameof(powerSwitch));
            this.powerLamp = powerLamp ?? throw new ArgumentNullException(nameof(powerLamp));
            //this.heatingElement = heatingElement ?? throw new ArgumentNullException(nameof(heatingElement));
            this.waterSensor = waterSensor ?? throw new ArgumentNullException(nameof(waterSensor));
            this.temperatureSensor = temperatureSensor ?? throw new ArgumentNullException(nameof(temperatureSensor));
            this.heatingElement = heatingElement ?? throw new HeatingElementException();
            this.powerSwitch.SwitchedOn += PowerSwitch_SwitchedOn;
            this.powerSwitch.SwitchedOff += PowerSwitch_SwitchedOff;
            this.temperatureSensor.ValueChanged += TemperatureSensor_ValueChanged;
            this.waterSensor.ValueChanged += WaterSensor_ValueChanged;


        }

        private async void PowerSwitch_SwitchedOn(object sender, EventArgs e)
        {
            // (POINT1) when power button is on below code will activate lamp
            await powerLamp.SwitchOnAsync();
                try
                {
                //(POINT1) when power button is on below code will actiavte heating element
                await heatingElement.SwitchOnAsync();
                }
                catch(HeatingElementException ex)
            {
                //(POINT7)When the heating element has a fault, it will raise HeatingElementException when switching on and all the components should be off.
                await heatingElement.SwitchOffAsync();
                await powerLamp.SwitchOffAsync();
                await powerSwitch.SwitchOffAsync();
            }
            //TODO: Implement
        }

        private async void PowerSwitch_SwitchedOff(object sender, EventArgs e)
        {
            //(POINT 3)When the power switch is switched off, all components must be deactivated.
            await heatingElement.SwitchOffAsync();
            await powerLamp.SwitchOffAsync();
            //(POINT 4)The power switch is a toggle switch and must be switched off by the controller in any case where the controller decides to turn off the kettle.
            await powerSwitch.SwitchOffAsync();
        }

        private void WaterSensor_ValueChanged(object sender, Model.ValueChangedEventArgs<bool> e)
        {
            //(POINT 6)The kettle controller should not attempt to heat if there is no water present and should switch off if the water is removed during heating.
            if (e.Value==false)
            {
                heatingElement.SwitchOffAsync();
            }
            else if(  heatingElement.IsOn && e.Value == true)
            {
                powerSwitch.SwitchOffAsync();
            }
           
        }

        private void TemperatureSensor_ValueChanged(object sender, Model.ValueChangedEventArgs<int> e)
        {
            //(POINT 5)When the water temperature reaches 100 degrees Celsius, the power must be switched off
            if (e.Value >= 100)
            {
                powerSwitch.SwitchOffAsync();
            }
          
        }

    }
}
