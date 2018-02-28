using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamReaderFunctionApp
{
    public class PositionMsg
    {
        public string rideId { get; set; }

        public string trainId { get; set; }

        public string correlationId { get; set; }

        public string accelX { get; set; }

        public string accelY { get; set; }

        public string accelZ { get; set; }

        public string rotX { get; set; }

        public string rotY { get; set; }

        public string rotZ { get; set; }

        public string deviceTime { get; set; }
    }

    //{"rideId":"2327F177-8079-43E1-BA50-569455E2FADD","trainId":"6FA812B3-FAE8-4875-8D80-62B037CD3528","correlationId":"3D57E64E-EA34-4BA2-BD15-1070A51A0961","accelX":"0.0205841","accelY":"1.03023","accelZ":"0.558151","rotX":"0.353375","rotY":"0.188923","rotZ":"0.0900415","deviceTime":"2018-02-27T11:04:52.7300000Z"}
}
