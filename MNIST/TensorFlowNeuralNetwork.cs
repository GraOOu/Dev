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


namespace MNIST
{
    /// <summary>
    /// 
    /// </summary>
    public class TensorFlowNeuralNetwork : AbstractNeuralNetwork
    {
        Numpy.NDarray xTrain, yTrain;
        Numpy.NDarray xTest, yTest;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hiddenLayerSize"></param>
        public override void Create ( int hiddenLayerSize )
        {
            int image_size = 784; // 28*28
            int num_classes = 10; // ten unique digits

            Sequential model = new Sequential ( );

            // The input layer requires the special input_shape parameter which should match
            // the shape of our training data.

            Dense hiddenLayer = new Dense ( hiddenLayerSize, activation: "sigmoid", input_shape: new Shape ( image_size ) );

            model.Add ( hiddenLayer );
            model.Add ( new Dense ( units: num_classes, activation: "softmax" ) );
        }

        // TODO : Replace this by hand made loading.
        public void PrepareData ( )
        {
            ((xTrain, yTrain), (xTest, yTest)) = Keras.Datasets.MNIST.LoadData ( );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbEpoch"></param>
        /// <param name="batchSize"></param>
        /// <param name="trainingDigits"></param>
        public override void Learn ( int nbEpoch, int batchSize, List<MNISTDigit> trainingDigits )
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testDigits"></param>
        public override void Test ( List<MNISTDigit> testDigits )
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        public override double[] Compute ( MNISTDigit digit )
        {
            return null;
        }
    }
}
