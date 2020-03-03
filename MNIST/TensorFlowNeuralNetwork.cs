using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MNIST.MNISTViewer;

using Tensorflow;
using static Tensorflow.Binding;

using Keras;
using Keras.Models;
using Keras.Layers;
using Keras.Callbacks;

namespace MNIST
{
    /// <summary>
    /// 
    /// </summary>
    public class TensorFlowNeuralNetwork : AbstractNeuralNetwork
    {
        Numpy.NDarray xTrain, yTrain;
        Numpy.NDarray xTest, yTest;

        Sequential model = null;

        int image_size = 784; // 28*28
        int num_classes = 10; // ten unique digits

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hiddenLayerSize"></param>
        public override void Create ( int hiddenLayerSize )
        {
            

            model = new Sequential ( );

            // The input layer requires the special input_shape parameter which should match
            // the shape of our training data.

            Dense hiddenLayer = new Dense ( hiddenLayerSize, activation: "sigmoid", input_shape: new Shape ( image_size ) );

            model.Add ( hiddenLayer );
            model.Add ( new Dense ( units: num_classes, activation: "softmax" ) );

            model.Compile ( "sgd", "categorical_crossentropy", new string[] { "accuracy" } );

            PrepareData ( );
        }

        // TODO : Replace this by hand made loading.
        public void PrepareData ( )
        {
            ((xTrain, yTrain), (xTest, yTest)) = Keras.Datasets.MNIST.LoadData ( );

            xTrain = xTrain.flatten ( );
            xTest  = xTest.flatten ( );

            yTrain = Keras.Utils.Util.ToCategorical ( yTrain, num_classes );
            yTest  = Keras.Utils.Util.ToCategorical ( yTest, num_classes );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbEpoch"></param>
        /// <param name="batchSize"></param>
        /// <param name="trainingDigits"></param>
        public override void Learn ( int nbEpoch, int batchSize, List<MNISTDigit> trainingDigits )
        {
            History history = model.Fit ( xTrain, yTrain, batch_size: 128, epochs: 5, verbose: 1, validation_split: 0.1f );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testDigits"></param>
        public override void Test ( List<MNISTDigit> testDigits )
        {
            model.Evaluate ( xTest, yTest, verbose: 1 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        public override double[] Compute ( MNISTDigit digit )
        {
            return model.Predict ( xTrain [ 1 ] ).GetData < double > ( );
        }
    }
}
