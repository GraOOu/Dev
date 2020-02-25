using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MNIST.MNISTViewer;

namespace MNIST
{
    /// <summary>
    /// AbstractInterface to handle different Neural Network
    /// Specific to MNIST Sand box.
    /// </summary>
    public class AbstractNeuralNetwork
    {
        public virtual void Create ( int hiddenLayerSize ) { }

        public virtual void Learn (  int nbEpoch, int batchSize, List<MNISTDigit> trainingDigits ) { }

        public virtual void Test ( List<MNISTDigit> testDigits ) { }

        public virtual double[] Compute ( MNISTDigit digit ) { return null; }
    }
}
