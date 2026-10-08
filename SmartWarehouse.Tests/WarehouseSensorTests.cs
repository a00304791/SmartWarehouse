using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWarehouse.Core;

namespace SmartWarehouse.Tests;

[TestClass]
public class WarehouseSensorTests
{
    [TestMethod]
    public void Constructor_SetsInitialState()
    {
        // Arrange
        string sensorId = "WS-001";
        string locationTag = "Cold-Vault-01";

        // Act
        WarehouseSensor sensor = new WarehouseSensor(sensorId, locationTag);

        // Assert
        Assert.AreEqual(sensorId, sensor.SensorId);
        Assert.AreEqual(locationTag, sensor.LocationTag);
        Assert.AreEqual(0.0, sensor.CurrentTemperature);
        Assert.IsFalse(sensor.IsActive);
        Assert.IsFalse(sensor.IsAlertTriggered);
        Assert.AreEqual(4.0, sensor.CriticalThresholdCelsius);
    }

    [TestMethod]
    public void Constructor_NullSensorId_ThrowsArgumentException()
    {
        // Arrange
        string sensorId = null!;

        // Act
        Action act = () => new WarehouseSensor(sensorId, "Cold-Vault-01");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [TestMethod]
    public void Constructor_EmptySensorId_ThrowsArgumentException()
    {
        // Arrange
        string sensorId = "";

        // Act
        Action act = () => new WarehouseSensor(sensorId, "Cold-Vault-01");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [TestMethod]
    public void Constructor_WhitespaceSensorId_ThrowsArgumentException()
    {
        // Arrange
        string sensorId = "   ";

        // Act
        Action act = () => new WarehouseSensor(sensorId, "Cold-Vault-01");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [TestMethod]
    public void Constructor_NullLocationTag_ThrowsArgumentException()
    {
        // Arrange
        string locationTag = null!;

        // Act
        Action act = () => new WarehouseSensor("WS-001", locationTag);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [TestMethod]
    public void Constructor_EmptyLocationTag_ThrowsArgumentException()
    {
        // Arrange
        string locationTag = "";

        // Act
        Action act = () => new WarehouseSensor("WS-001", locationTag);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [TestMethod]
    public void Constructor_WhitespaceLocationTag_ThrowsArgumentException()
    {
        // Arrange
        string locationTag = "   ";

        // Act
        Action act = () => new WarehouseSensor("WS-001", locationTag);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [TestMethod]
    public void Activate_SetsSensorToActive()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        // Act
        sensor.Activate();

        // Assert
        Assert.IsTrue(sensor.IsActive);
    }

    [TestMethod]
    public void Deactivate_SetsSensorToInactiveAndResetsAlert()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();
        sensor.RecordReading(10.0);

        // Act
        sensor.Deactivate();

        // Assert
        Assert.IsFalse(sensor.IsActive);
        Assert.IsFalse(sensor.IsAlertTriggered);
    }

    [TestMethod]
    public void RecordReading_WhenInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        // Act
        Action act = () => sensor.RecordReading(5.0);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [DataTestMethod]
    [DataRow(3.9, false)]
    [DataRow(4.0, true)]
    [DataRow(4.1, true)]
    public void RecordReading_BelowExactlyAndAboveThreshold(
        double temperature,
        bool expectedAlert)
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();

        // Act
        sensor.RecordReading(temperature);

        // Assert
        Assert.AreEqual(temperature, sensor.CurrentTemperature);
        Assert.AreEqual(expectedAlert, sensor.IsAlertTriggered);
    }

    [TestMethod]
    public void RecordReading_AtMinus50Degrees_IsAccepted()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();

        // Act
        sensor.RecordReading(-50.0);

        // Assert
        Assert.AreEqual(-50.0, sensor.CurrentTemperature);
    }

    [TestMethod]
    public void RecordReading_At80Degrees_IsAccepted()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();

        // Act
        sensor.RecordReading(80.0);

        // Assert
        Assert.AreEqual(80.0, sensor.CurrentTemperature);
    }

    [TestMethod]
    public void RecordReading_BelowMinus50Degrees_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();

        // Act
        Action act = () => sensor.RecordReading(-50.1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [TestMethod]
    public void RecordReading_Above80Degrees_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();

        // Act
        Action act = () => sensor.RecordReading(80.1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [TestMethod]
    public void UpdateThreshold_UpdatesThreshold()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        // Act
        sensor.UpdateThreshold(15.0);

        // Assert
        Assert.AreEqual(15.0, sensor.CriticalThresholdCelsius);
    }

    [TestMethod]
    public void UpdateThreshold_BelowMinus30Degrees_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        // Act
        Action act = () => sensor.UpdateThreshold(-30.1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [TestMethod]
    public void UpdateThreshold_Above50Degrees_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        // Act
        Action act = () => sensor.UpdateThreshold(50.1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [DataTestMethod]
    [DataRow(-30.0)]
    [DataRow(50.0)]
    public void UpdateThreshold_AtBoundaries_IsAccepted(double threshold)
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        // Act
        sensor.UpdateThreshold(threshold);

        // Assert
        Assert.AreEqual(threshold, sensor.CriticalThresholdCelsius);
    }

    [TestMethod]
    public void UpdateThreshold_WhenNewThresholdIsAboveTemperature_ClearsAlert()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();
        sensor.RecordReading(10.0);

        // Act
        sensor.UpdateThreshold(15.0);

        // Assert
        Assert.IsFalse(sensor.IsAlertTriggered);
    }

    [TestMethod]
    public void UpdateThreshold_WhenNewThresholdEqualsTemperature_TriggersAlert()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();
        sensor.RecordReading(10.0);

        // Act
        sensor.UpdateThreshold(10.0);

        // Assert
        Assert.IsTrue(sensor.IsAlertTriggered);
    }

    [TestMethod]
    public void UpdateThreshold_WhenNewThresholdIsBelowTemperature_TriggersAlert()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor(
            "WS-001",
            "Cold-Vault-01");

        sensor.Activate();
        sensor.RecordReading(10.0);

        // Act
        sensor.UpdateThreshold(5.0);

        // Assert
        Assert.IsTrue(sensor.IsAlertTriggered);
    }
}