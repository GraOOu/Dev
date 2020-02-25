using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MNIST.MNISTViewer;
// using Tensorflow;

namespace MNIST
{
    public class TensorFlowNeuralNetwork : AbstractNeuralNetwork
    {
        public override void Create ( int hiddenLayerSize )
        {
            int image_size = 784; // 28*28
            int num_classes = 10; // ten unique digits
            /*
             * Python
             * 
             model = Sequential ( )

            # The input layer requires the special input_shape parameter which should match
            # the shape of our training data.
            model.add ( Dense ( units = 32, activation = 'sigmoid', input_shape = (image_size,) ) )
            model.add ( Dense ( units = num_classes, activation = 'softmax' ) )
            */
        }

        public override void Learn ( int nbEpoch, int batchSize, List<MNISTDigit> trainingDigits ) { }

        public override void Test ( List<MNISTDigit> testDigits ) { }

        public override double[] Compute ( MNISTDigit digit ) { return null; }
    }
}
