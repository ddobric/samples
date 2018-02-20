using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNTK.NET.Samples
{
    public class FeedForwardSample
    {
        private const string m_cModelPath = "trainedmodel.json";

        /// <summary>
        /// https://cntk.ai/pythondocs/CNTK_102_FeedForward.html
        public void Run(DeviceDescriptor device)
        {
            int outDim = 3;

            int inDim = 2;

            int num_hidden_layers = 3;
            int hidden_layers_dim = 50;

            var input = Variable.InputVariable(new int[] { inDim }, DataType.Float);
            var label = Variable.InputVariable(new int[] { outDim }, DataType.Float);

            var trainingData = Program.GenerateRandomData(1000000, 2, 2);

            var testingData = Program.GenerateRandomData(100, 2, 2);

            var model = createModel(input, num_hidden_layers, outDim, hidden_layers_dim, CNTKLib.Sigmoid, device);

            var trainer = createTrainer(model, label);

            train(trainer, input, label, trainingData.X, trainingData.Y, device);

            test(trainer, input, label, testingData.X, testingData.Y, device);

            evaluate(model, input, testingData.X, testingData.Labels, device);

            model.Save(m_cModelPath);
        }

        internal void Evaluate(DeviceDescriptor device)
        {
            var testingData = Program.GenerateRandomData(100, 2, 2);

            var model = Function.Load(m_cModelPath, device);

            var input = model.Arguments.First();

            evaluate(model, input, testingData.X, testingData.Labels, device);
        }

        private void evaluate(Function model, Variable input, float[] testingData, float[] expectedLabels, DeviceDescriptor device)
        {
            var batch = Value.CreateBatch<float>(new NDShape(1, input.Shape[0]), testingData, device);
            var inputDataMap = new Dictionary<Variable, Value>() { { input, batch } };
            var outputDataMap = new Dictionary<Variable, Value>() { { model.Output, null } };

            model.Evaluate(inputDataMap, outputDataMap, device);

            var outputValue = outputDataMap[model.Output];
            IList<IList<float>> actualLabelSoftMax = outputValue.GetDenseData<float>(model.Output);
            var actualLabels = actualLabelSoftMax.Select((IList<float> l) => l.IndexOf(l.Max())).ToList();
            int misMatches = actualLabels.Zip(expectedLabels, (a, b) =>
            {
                if (a == b)
                    return 0;
                else
                    return 1;
            }).Sum();

            // Console.WriteLine($"Validating Model: Total Samples = {testSize}, Misclassify Count = {misMatches}");
        }

        private Function getLinearLayer(Variable input, int outDim, DeviceDescriptor device)
        {
            var glorotInit = CNTKLib.GlorotUniformInitializer(
                CNTKLib.DefaultParamInitScale,
                CNTKLib.SentinelValueForInferParamInitRank,
                CNTKLib.SentinelValueForInferParamInitRank, 1);

            var W = new Parameter(new int[] { outDim, input.Shape[0] }, DataType.Float, glorotInit, device, "W");

            var bias = new Parameter(new int[] { outDim }, 0.0f, device, "bias");

            return CNTKLib.Plus(CNTKLib.Times(W, input), bias, "layer");
        }

        private Function getDenseLayer(Variable input, int outDim, Func<Variable, Function> activationFunction, DeviceDescriptor device)
        {
            var linearLayer = getLinearLayer(input, outDim, device);
            return activationFunction(linearLayer);
        }

        private Function createModel(Variable input, int numOfHiddenLayers, int outDim, int hiddenLayersDim, Func<Variable, Function> activationFnc, DeviceDescriptor device)
        {
            //var firstHiddenLayer = getDenseLayer(input, numOfHiddenLayers, activationFnc, device);

            //var hiddenLayer = firstHiddenLayer;

            //for (int i = 1; i < numOfHiddenLayers; i++)
            //{
            //    hiddenLayer = getDenseLayer(hiddenLayer, hiddenLayersDim, activationFnc, device);
            //}

            //var lastLayer = getDenseLayer(hiddenLayer, outDim, activationFnc, device);

            //return lastLayer;

            // Model with single layer.
            var lastLayer = getLinearLayer(input, outDim, device);
            return lastLayer;
        }

        private Trainer createTrainer(Function model, Variable label)
        {
            //Function loss = CNTKLib.SquaredError(model, label, "Softmax");
            Function loss = CNTKLib.CrossEntropyWithSoftmax(model, label, "CrossEntropyWithSoftmax");
            Function prediction = CNTKLib.ClassificationError(model, label, "ClassificationError");

            //TrainingParameterScheduleDouble learningRatePerSample = new TrainingParameterScheduleDouble(0.01, 1);
            TrainingParameterScheduleDouble learningRatePerSample = new TrainingParameterScheduleDouble(0.01, 1);

            TrainingParameterScheduleDouble momentumTimeConstant = CNTKLib.MomentumAsTimeConstantSchedule(25);

            IList<Learner> parameterLearners = new List<Learner>() {
                Learner.MomentumSGDLearner(model.Parameters(),
                learningRatePerSample, momentumTimeConstant, /*unitGainMomentum = */true)  };

            var trainer = Trainer.CreateTrainer(model, loss, prediction, parameterLearners);

            return trainer;
        }


        private void train(Trainer trainer, Variable input,
            Variable label,
            float[] inputData, float[] labelData,
            DeviceDescriptor device)
        {
            int batchSize = 1000;
            int numOfBatches = inputData.Length / batchSize / input.Shape[0];

            for (int i = 0; i < numOfBatches; i++)
            {
                List<float> batchInputArr = new List<float>();
                List<float> batchLblArr = new List<float>();

                batchInputArr.AddRange(inputData.Skip(i * batchSize * input.Shape[0]).Take(batchSize * input.Shape[0]));
                batchLblArr.AddRange(labelData.Skip(i * batchSize * label.Shape[0]).Take(batchSize * label.Shape[0]));

                var xValues = Value.CreateBatch<float>(new NDShape(1, input.Shape[0]), batchInputArr.ToArray(), device);
                var yValues = Value.CreateBatch<float>(new NDShape(1, label.Shape[0]), batchLblArr.ToArray(), device);

                var batchData = new Dictionary<Variable, Value>();
                batchData.Add(input, xValues);
                batchData.Add(label, yValues);

                var res = trainer.TrainMinibatch(batchData, false, device);

                Console.WriteLine($"Iteration: {i} - Count: {trainer.PreviousMinibatchSampleCount()} - EvalAvg: {trainer.PreviousMinibatchEvaluationAverage()} - Loss: {trainer.PreviousMinibatchLossAverage()}");
            }
        }

        private void test(Trainer trainer, Variable testVariable,
          Variable label,
          float[] testData, float[] labelData,
          DeviceDescriptor device)
        {
            var xValues = Value.CreateBatch<float>(new NDShape(1, testVariable.Shape[0]), testData, device);
            var yValues = Value.CreateBatch<float>(new NDShape(1, label.Shape[0]), labelData, device);

            MinibatchData d = new MinibatchData();
            d.data = xValues;

            UnorderedMapVariableMinibatchData batch = new UnorderedMapVariableMinibatchData();
            batch.Add(testVariable, new MinibatchData(xValues));
            batch.Add(label, new MinibatchData(yValues));


            var res = trainer.TestMinibatch(batch, device);
        }
    }
}
