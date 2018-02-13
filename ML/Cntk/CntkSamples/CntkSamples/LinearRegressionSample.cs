using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNTK.NET.Samples
{
    public class LinearRegressionSample
    {
        public Function Dense(Variable X, int outDim, DeviceDescriptor device)
        {
           // default network initialization
            var glorotInit = CNTKLib.GlorotUniformInitializer(
                    CNTKLib.DefaultParamInitScale,
                    CNTKLib.SentinelValueForInferParamInitRank,
                    CNTKLib.SentinelValueForInferParamInitRank, 1);

            //Define Weight matrix with input dimension x outDim dimensions
            var weightMatrix = new Parameter(new int[] { outDim, X.Shape[0]}, DataType.Float, glorotInit, device, "Weight");

            //Define bias with outputDimension
            var bias = new Parameter(new int[] { outDim }, 0.0f, device, "bias");

            var wx = CNTKLib.Times(weightMatrix, X);

            //Perform layer creation by calculating W*X+b
            var layer = CNTKLib.Plus(wx, bias, "layer");

            layer =  CNTKLib.Sigmoid(layer);

            //once the layer is created perform activation function
            //var layerWithActivation = aFun(layer);

            return layer;
        }

        internal void Go(DeviceDescriptor device)
        {
            Variable X = Variable.InputVariable(new int[] { 4 }, DataType.Float);

            var layer = Dense(X, 2, device);
        }
    }
}
