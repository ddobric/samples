//https://github.com/rwaldron/johnny-five/wiki/Getting-Started

var five = require("johnny-five"),
    board = new five.Board();

board.on("ready", function () {
    
    var delay = 250;
    
    // Create an Led on pin 10
    var led = new five.Led(10);
    
    function doIt() {
        
        board.wait(delay, function () {
            if (led.isOn)
                led.off();
            else
                led.on();
        });
    }
    
    board.loop(4500, doIt);
});

