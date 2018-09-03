'use strict';
// Define the objects you will be working with
var five = require("johnny-five");
var Shield = require("j5-sparkfun-weather-shield")(five);
var device = require('azure-iot-device');
var deviceId = "damir's box";

// location is simply a string that you can filter on later
var location = 'DAENET office';

console.log("Device ID: " + deviceId);

// Create a Johnny-Five board instance to represent your Particle Photon
// Board is simply an abstraction of the physical hardware, whether is is a 
// Photon, Arduino, Raspberry Pi or other boards. 
var board = new five.Board({ port: "COM3" });

// The board.on() executes the anonymous function when the 
// board reports back that it is initialized and ready.
board.on("ready", function () {
    console.log("Board connected...");

    var weather = new Shield({
        variant: "ARDUINO", // or PHOTON
        freq: 1000,         // Set the callback frequency to 1-second
        elevation: 100      // Go to http://www.WhatIsMyElevation.com to get your current elevation
    });

    // The weather.on("data", callback) function invokes the anonymous callback function 
    // whenever the data from the sensor changes (no faster than every 25ms). The anonymous 
    // function is scoped to the object (e.g. this == the instance of Weather class object). 
    weather.on("data", function () {
        console.log("weather data event fired...");
        var payload = JSON.stringify({
            deviceId: deviceId,
            location: location,
            // celsius & fahrenheit are averages taken from both sensors on the shield
            celsius: this.celsius,
            fahrenheit: this.fahrenheit,
            relativeHumidity: this.relativeHumidity,
            pressure: this.pressure,
            feet: this.feet,
            meters: this.meters
        });

        // Create the message based on the payload JSON
        var message = new Message(payload);
        // For debugging purposes, write out the message payload to the console
        console.log("Telemetry: " + message.getData());
        
    });

});

// Helper function to print results in the console
function printResultFor(op) {
    return function printResult(err, res) {
        if (err) console.log(op + ' error: ' + err.toString());
    };
}