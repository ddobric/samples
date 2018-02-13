
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CNTK.NET.Samples
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CNTK_TrainIrisData
    {
        public static void TrainIriswithBatch(DeviceDescriptor device)
        {
            //data file path
            var iris_data_file = "Data/iris_with_hot_vector.csv";

            //Network definition
            int inputDim = 4;
            int numOutputClasses = 3;
            int numHiddenLayers = 1;
            int hidenLayerDim = 6;
            int sampleSize = 120;

            //load data in to memory
            var dataSet = loadIrisDataset(iris_data_file, inputDim, numOutputClasses);

            // build a NN model
            //define input and output variable
            var xValues = Value.CreateBatch<float>(new NDShape(1, inputDim), dataSet.Item1, device);
            var yValues    = Value.CreateBatch<float>(new NDShape(1, numOutputClasses), dataSet.Item2, device);

            // build a NN model
            //define input and output variable and connecting to the stream configuration
            var feature = Variable.InputVariable(new NDShape(1, inputDim), DataType.Float);
            var label = Variable.InputVariable(new NDShape(1, numOutputClasses), DataType.Float);

            //Combine variables and data in to Dictionary for the training
            var dic = new Dictionary<Variable, Value>();
            dic.Add(feature, xValues);
            dic.Add(label, yValues);


            //Build simple Feed Froward Neural Network model
            // var ffnn_model = CreateMLPClassifier(device, numOutputClasses, hidenLayerDim, feature, classifierName);
            var ffnn_model = createFFNN(feature, numHiddenLayers, hidenLayerDim, numOutputClasses, Activation.Tanh, "IrisNNModel", device);

            //Loss and error functions definition
            var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(new Variable(ffnn_model), label, "lossFunction");
            var classError = CNTKLib.ClassificationError(new Variable(ffnn_model), label, "classificationError");
            
            // set learning rate for the network
            var learningRatePerSample = new TrainingParameterScheduleDouble(0.001125, 1);

            //define learners for the NN model
            var ll = Learner.SGDLearner(ffnn_model.Parameters(), learningRatePerSample);

            //define trainer based on ffnn_model, loss and error functions , and SGD learner 
            var trainer = Trainer.CreateTrainer(ffnn_model, trainingLoss, classError, new Learner[] { ll });

            //Preparation for the iterative learning process
            //used 800 epochs/iterations. Batch size will be the same as sample size since the data set is small
            int epochs = 800;
            int i = 0;
            while (epochs > -1)
            {
                trainer.TrainMinibatch(dic, device);

                //print progress
                //printTrainingProgress(trainer, i++, 50);

                //
                epochs--;
            }
            //Summary of training
            double acc = Math.Round((1.0 - trainer.PreviousMinibatchEvaluationAverage()) * 100, 2);
            Console.WriteLine($"------TRAINING SUMMARY--------");
            Console.WriteLine($"The model trained with the accuracy {acc}%");
        }


        static (float[], float[]) loadIrisDataset(string filePath, int featureDim, int numClasses)
        {
            var rows = File.ReadAllLines(filePath);
            var features = new List<float>();
            var label = new List<float>();
            for (int i = 1; i < rows.Length; i++)
            {
                var row = rows[i].Split(',');
                var input = new float[featureDim];
                for (int j = 0; j < featureDim; j++)
                {
                    input[j] = float.Parse(row[j], CultureInfo.InvariantCulture);
                }
                var output = new float[numClasses];
                for (int k = 0; k < numClasses; k++)
                {
                    int oIndex = featureDim + k;
                    output[k] = float.Parse(row[oIndex], CultureInfo.InvariantCulture);
                }

                features.AddRange(input);
                label.AddRange(output);
            }

            return (features.ToArray(), label.ToArray());
        }

        public enum Activation
        {
            None,
            ReLU,
            Sigmoid,
            Tanh
        }
    }
}
