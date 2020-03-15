using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MNIST.MNISTViewer;

namespace MNIST
{
    public class AccordNeuralNetwork : AbstractNeuralNetwork
    {
        ActivationNetwork _neuralNet = null;

        public class NeuronToStringConverter
        {
            public string ToStringMNIST ( Neuron neuron )
            {
                string result = "W=" + Environment.NewLine;

                int weightCount = neuron.Weights.Count ( );

                for ( int i = 0; i < weightCount; i++ )
                {
                    result += neuron.Weights[i].ToString ( "F4" );
                    result += ";";

                    // Ugly constant
                    if ( ( ( i + 1 ) % MNISTViewer.inputNbRow ) == 0 )
                    {
                        result += Environment.NewLine;
                    }
                }
                // Bias ???

                result += Environment.NewLine;
                return result;
            }
        }

        private void WriteNeuralNetInFile ( ActivationNetwork nn, string fileName )
        {
            string result = string.Empty;

            for ( int l = 0; l < nn.Layers.Count ( ); l++ )
            {
                result += "Layer " + l + Environment.NewLine;

                Layer layer = nn.Layers [ l ];
                for ( int n = 0; n < layer.Neurons.Count ( ); n++ )
                {
                    result += "Neuron " + n + Environment.NewLine;

                    Neuron neuron = layer.Neurons [ n ];
                    result += new NeuronToStringConverter ( ).ToStringMNIST ( neuron );
                }
            }
            File.WriteAllText ( fileName, result );
        }


        public override void Create ( int hiddenLayerSize )
        {
            // 28x28 input | First Layer 32 | 10 Output.

            _neuralNet = new ActivationNetwork ( new SigmoidFunction ( ), 28*28, 32, 10 );
            _neuralNet.Randomize ( ); // Not relevant changes

            WriteNeuralNetInFile ( _neuralNet, "InitialNeuralNet.csv" );
        }

        public override void Learn ( int nbEpoch, int batchSize, List<MNISTDigit> trainingDigits )
        {
            BackPropagationLearning teacher = new BackPropagationLearning ( _neuralNet );
            teacher.LearningRate = 2;

            // Learning Loop

            int numberOfEpok = 1;
            List <double> errorTrend = new List<double> ( );

            for ( int i = 0; i < numberOfEpok; i++ )
            {
                // Run epoch of learning procedure
                // double error = teacher.RunEpoch ( inputs, outputs );

                double error = 0.0;
                for ( int j = 0; j < trainingDigits.Count; j++ )
                {
                    double[] networkOut = _neuralNet.Compute ( trainingDigits[j].input );

                    error += teacher.Run ( trainingDigits[j].input, trainingDigits[j].output );
                }
                errorTrend.Add ( error );
            }

            // Save Network

            WriteNeuralNetInFile ( _neuralNet, "TrainedNeuralNet.csv" );

            // Save Metrics
            string metrics = string.Join ( ";", errorTrend );
            File.WriteAllText ( "LearningMetrics.csv", metrics );
        }



        public override void Test ( List<MNISTDigit> testDigits )
        {
            string testMetrics = string.Empty;

            for ( int i = 0; i < testDigits.Count; i++ )
            {
                double[] testResult = _neuralNet.Compute ( testDigits[i].input );

                double max = testResult.Max ( );
                int predicted = -1;
                for ( int j = 0; j < testResult.Count ( ); j++ )
                {
                    if ( testResult[j] == max )
                    {
                        predicted = j; break;
                    }
                }

                testMetrics += ( testDigits[i].label == predicted ) ? "Ok" : "Ko";
                testMetrics += ";" + predicted + ";";
                testMetrics += string.Join ( ";", testResult );
            }
            File.WriteAllText ( "TestMetrics.csv", testMetrics );
        }

        public override double[] Compute ( MNISTDigit digit )
        {
            double[] output = _neuralNet.Compute ( digit.input );
            return output;
        }

    }
}
