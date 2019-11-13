using BDL.Kettle.Controller;
using BDL.Kettle.Model.Inputs;
using BDL.Kettle.Model.Outputs;
using BDL.Kettle.Model.Sensors;
using BDL.Kettle.Model.Workers;
using Moq;
using System;
using Xunit;

namespace Kettle.Controller.Tests
{
    public class KettleControllerTests
    {
        private readonly Mock<IPowerSwitch> powerSwitch;
        private readonly Mock<IPowerLamp> powerLamp;
        private readonly Mock<IHeatingElement> heatingElement;
        private readonly Mock<IWaterSensor> waterSensor;
        private readonly Mock<ITemperatureSensor> temperatureSensor;
        private readonly KettleController controller;

        public KettleControllerTests()
        {
            this.powerSwitch = new Mock<IPowerSwitch>();
            this.powerLamp = new Mock<IPowerLamp>();
            this.heatingElement = new Mock<IHeatingElement>();
            this.waterSensor = new Mock<IWaterSensor>();
            this.temperatureSensor = new Mock<ITemperatureSensor>();

            this.controller = new KettleController(this.powerSwitch.Object, 
                this.powerLamp.Object, 
                this.heatingElement.Object,
                this.waterSensor.Object,
                this.temperatureSensor.Object);
        }

        [Fact]
        public void KettleController_PowerSwitchActivatesHeatingElementAndPowerLamp()
        {
            this.powerSwitch.Raise(powerSwitch => powerSwitch.SwitchedOn += null, EventArgs.Empty);

            this.heatingElement.Verify(h => h.SwitchOnAsync());
            this.powerLamp.Verify(p => p.SwitchOnAsync());
        }
    }
}
