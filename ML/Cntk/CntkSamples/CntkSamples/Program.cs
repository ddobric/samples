using CNTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNTK.NET.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var device = DeviceDescriptor.CPUDevice;

                // Arrays and ranks
                Ranks();

                // Demonstrates how to work wirh Shapes.
                Shapes(device);

                // Demonstrates how to work with values.
                Values(device);
                    
                //
                // Run Feed Froward Deep Network Sample
                FeedForwardSample feedFwdSample = new FeedForwardSample();
                feedFwdSample.Run(device);
                feedFwdSample.Evaluate(device);

                //
                // Run IRIS sample
                IrisSample irisSample = new IrisSample();
                irisSample.Run(device);

                return;

                TrainerSample(device);
               
               
                Values(device);
                DataBatchSample(device);



                LinearRegressionSample linearSample = new LinearRegressionSample();

                linearSample.Go(device);

                // new NDShape()
                var a = Variable.InputVariable(new int[] { 0 }, DataType.Float, "sas", null, false /*isSparse*/);


                var feature = Variable.InputVariable(new int[] { 1 }, DataType.Float, "sas", null, false /*isSparse*/);
                var label = Variable.InputVariable(new int[] { 2 }, DataType.Float, "ss", new List<CNTK.Axis>() { CNTK.Axis.DefaultBatchAxis() }, false);
            }
            catch (Exception e)
            {

            }
        }

        private static Array getRow<T>(Array array, int dimension = 0)
        {

            var row = Array.CreateInstance(typeof(T), array.GetUpperBound(dimension) + 1);
            for (int i = 0; i <= array.GetUpperBound(dimension); i++)
                row.SetValue()
                Console.WriteLine(a.GetValue(0, i));
        }

        private static void Ranks()
        {
            //
            // Rank 2.  el[i,j]. el[1,1] = 6
            int[,] array2Da = new int[,] { { 2, 3, 3 }, { 5, /**/6, 4 }, { 7, 9, 5 }, { 11, 12, 6 }, { 11, 12, 7 }, { 131, 1442, 8 } };
            int[,] array2Db = new int[,] { { 2, 3 }, { 5, 6 }, { 7, 9 }, { 11, 12 }, { 11, 12 }, { 131, 1442 } };

            Array a = array2Da;
           
            var six = a.GetValue(1,1);//5,6,4
            for (int i = 0; i <= a.GetUpperBound(1); i++)
                Console.WriteLine(a.GetValue(0, i));

            //
            // Rank 3. el[i,j,k]. el[1,1,1] = 11
            int[,,] array3D = new int[,,] {
                                            { { 1, 2, 3 }, { 4, 5, 6 } },
                                            { { 7, 8, 9 }, { 10, /**/11, 12 } },
                                            { { 7, 8, 9 }, { 10, 11, 12 } }
                                           };

            //
            // Rank 4. el[i,j,k,l]. el[1,1,1,1] = 11
            int[,,,] array4D = new int[,,,]
                                            {
                                               {
                                                    { { 1, 2, 3 }, { 4, 5, 6 } },
                                                    { { 7, 8, 9 }, { 10, 11, 12 } },
                                                    { { 7, 8, 9 }, { 10, 11, 12 } }
                                               },
                                               {
                                                    { { 1, 2, 3 }, { 4, 5, 6 } },
                                                    { { 7, 8, 9 }, { 10, /**/11, 12 } },
                                                    { { 7, 8, 9 }, { 10, 11, 12 } }
                                               } };
        }


        private static void Shapes(DeviceDescriptor device)
        {
            var s = new NDShape(1, 1);
            s = new NDShape(1, 2);
            s = new NDShape(2, 3);
            s = new NDShape(2, 4);
            s = new NDShape(3, 2);
            s = new NDShape(3, 4);
            s = new NDShape(5, 2);            
         
        }

        /// <summary>
        /// Demonstrates how to work with Values and variables.
        /// </summary>
        /// <param name="device"></param>

        private static void Values(DeviceDescriptor device)
        {
            var f = new float[3][];
            f[0] = new float[] { 10.0f, 11.0f };
            f[1] = new float[] { 21.0f, 22.0f };
            f[2] = new float[] { 31.0f, 33.0f };

            var v1 = Value.Create<float>(new NDShape(1, 2), f, new bool[] { true, true, true }, device);

            var input = Variable.InputVariable(new NDShape(1, 2), DataType.Float);

            var data = v1.GetDenseData<float>(input);
        }


        private static void TrainerSample(DeviceDescriptor device)
        {
            int outDim = 2;
            int inDim = 4;

            //default network initialization
            var glorotInit = CNTKLib.GlorotUniformInitializer(
                    CNTKLib.DefaultParamInitScale,
                    CNTKLib.SentinelValueForInferParamInitRank,
                    CNTKLib.SentinelValueForInferParamInitRank, 1);

            // Variable is input manipulated by YOU
            Variable X = Variable.InputVariable(new int[] { inDim }, DataType.Float);
            Variable R = Variable.InputVariable(new int[] { 1 }, DataType.Float);

            //Define Weight matrix with input dimension x outDim dimensions
            var W = new Parameter(new int[] { outDim, inDim }, DataType.Float, glorotInit, device, "W");

            //Define bias with outputDimension
            var bias = new Parameter(new int[] { outDim }, 0.0f, device, "bias");

            //Perform layer creation by calculating W*X+b
            var layer = CNTKLib.Plus(CNTKLib.Times(W, X), bias, "layer");

            //once the layer is created perform activation function
            var layerWithActivation = CNTKLib.Sigmoid(layer);

            var label = Variable.InputVariable(new int[] { outDim }, DataType.Float, "label");

            Function trainingLoss = CNTKLib.SquaredError(layerWithActivation, label, "squarederrorLoss");

            Function prediction = CNTKLib.SquaredError(layerWithActivation, label, "squarederrorEval");

            TrainingParameterScheduleDouble learningRatePerSample = new TrainingParameterScheduleDouble(0.0005, 1);

            TrainingParameterScheduleDouble momentumTimeConstant = CNTKLib.MomentumAsTimeConstantSchedule(256);

            IList<Learner> parameterLearners = new List<Learner>() {
                Learner.MomentumSGDLearner(layerWithActivation.Parameters(), learningRatePerSample, momentumTimeConstant, /*unitGainMomentum = */true)  };

            var trainer = Trainer.CreateTrainer(layerWithActivation, trainingLoss, prediction, parameterLearners);

            Random rnd = new Random();
            List<float> x = new List<float>();
            for (int i = 0; i < 1000; i++)
            {
                x.Add((float)rnd.NextDouble());
                x.Add((float)rnd.NextDouble());
                x.Add((float)rnd.NextDouble());
                x.Add((float)rnd.NextDouble());
            }

            List<float> y = new List<float>();
            for (int i = 0; i < 1000; i++)
            {
                x.Add(rnd.NextDouble() > 0.5 ? 1 : 0);
            }

            var xValues = Value.CreateBatch<float>(new NDShape(1, inDim), x.ToArray(), device);
            var yValues = Value.CreateBatch<float>(new NDShape(1, outDim), y.ToArray(), device);

            var batchData = new Dictionary<Variable, Value>();
            batchData.Add(X, xValues);
            batchData.Add(R, yValues);

            trainer.TrainMinibatch(batchData, false, device);

        }

        private static void DataBatchSample(DeviceDescriptor device)
        {
            var data = GenerateRandomData(25, 2, 2);

            Random rnd = new Random();
            int[] data1 = new int[1000];
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = rnd.Next();
            }

            var intValues = Value.CreateBatch<float>(new NDShape(1, 2), data1.Select(i => (float)i), device);


            #region Enumerable of floats as batch
            //
            // Provide correct multiply
            //

            double[] data2 = new double[5 * 1000];
            for (int i = 0; i < data1.Length; i++)
            {
                data2[i] = rnd.NextDouble();
            }

            var floatValues = Value.CreateBatch<double>(new NDShape(1, 5), data2, device);
            #endregion

            #region Correct Multiply
            //
            // Provide correct multiply
            //

            double[] data3 = new double[1000];
            for (int i = 0; i < data1.Length; i++)
            {
                data2[i] = rnd.NextDouble();
            }

            //
            // System.ArgumentOutOfRangeException: 'The number of elements (1000) 
            // in the vector containing batch data must
            // be a multiple of the size (3) of the sample shape '[3]'.
            //floatValues = Value.CreateBatch<double>(new NDShape(1, 3), data2, device);
            #endregion
        }

        private void go()
        {
            int featureDim = 2;
            int numClasses = 2;


            var fetures = Variable.InputVariable(new int[] { featureDim }, DataType.Float);

            var labels = Variable.InputVariable(new int[] { numClasses }, DataType.Float);

            //CNTKLib.lay
            //  Value.Create()
        }


        // X1 - 1,0,0 
        // X2 - 0,1,0 
        // X3 - 0,0,1
        /// <summary>
        /// Encodes the number in to th ebit array.
        /// 0, 1, 2, 3 converts to: [0,0],[0,1], [1,0], [1,1] respectively.
        /// </summary>
        /// <param name="num">Number to be encoded.</param>
        /// <param name="classes">Bits</param>
        /// <returns></returns>
        private static float[] bitEncode(float num, int classes, out float codedLabels)
        {
            if (num == 0)
            {
                codedLabels = 0;
                return new float[] { 1, 0, 0 };
            }
            else if (num == 1)
            {
                codedLabels = 1;
                return new float[] { 0, 1, 0 };
            }
            else if (num == 2)
            {
                codedLabels = 2;
                return new float[] { 0, 0, 1 };
            }
            else
                throw new NotImplementedException("");
            /*
            float[] res = new float[classes];

            for (int i = 0; i < classes; i++)
            {
                res[i] = (((int)num) & (2 >> i)) > 0 ? 1 : 0;
            }

            return res;*/
        }

        public static (float[] X, float[] Y, float[] Labels) GenerateRandomData(int sampleSize, int featureDim,
            int numClasses, string fileName = null)
        {
            float[] markers = new float[] { 5.0f, 10.0f, 15.0f };

            var rnd = new Random();
            List<float> yLst = new List<float>(); // encoded labels as 1 0 1, array.
            List<float> xLst = new List<float>(); // training data
            List<float> lLst = new List<float>(); // Labels as classified by algorithm

            for (int i = 0; i < sampleSize; i++)
            {
                float[] yComps = new float[numClasses];

                // Get random label value.
                var y = rnd.Next(0, markers.Length);

                float encodedClassifiedLabel;

                // Peek random marker and convert it to bit array.
                yComps = bitEncode(y, numClasses, out encodedClassifiedLabel);
                lLst.Add(encodedClassifiedLabel);
                yLst.AddRange(yComps);

                float[] xComponents = new float[featureDim];

                for (int j = 0; j < featureDim; j++)
                {
                    var min = y == 0 ? 0 : (int)markers[(int)y - 1];

                    xComponents[j] = (rnd.Next(min, (int)markers[(int)y]) + (float)rnd.NextDouble());
                }

                xLst.AddRange(xComponents);
            }

            if (fileName != null)
            {
                saveData(featureDim, xLst);
            }

            return (xLst.ToArray(), yLst.ToArray(), lLst.ToArray());

        }

        private static void saveData(int featureDim, List<float> xLst)
        {
            using (var sw = new StreamWriter("data.csv"))
            {
                int k = 0;

                string line = String.Empty;

                foreach (var row in xLst)
                {
                    line += row.ToString() + ";";
                    if (++k >= featureDim)
                    {
                        sw.WriteLine(line);
                        line = String.Empty;
                        k = 0;
                    }
                }
            }
        }
    }
}
