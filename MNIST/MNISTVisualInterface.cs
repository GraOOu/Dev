using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MNIST
{
    public partial class MNISTViewer : Form
    {
        public MNISTViewer ( )
        {
            InitializeComponent ( );
        }

        private void MNISTViewer_Load ( object sender, EventArgs e )
        {

        }

        static public int inputNbRow = 28;
        static public int inputNbCol = 28;

        int cellSpacing = 20;
        int cellSize    = 20;

        int[,] img = null;

        protected override void OnPaint ( PaintEventArgs e )
        {
            base.OnPaint ( e );
            Graphics g = e.Graphics;

            using ( Pen selPen = new Pen ( Color.LightGreen ) )
            using ( Pen grrrPen = new Pen ( Color.Black ) )
            using ( Pen graoouPen = new Pen ( Color.Red ) )
            {
                for ( int i = 0; i < inputNbRow; i++ )
                {
                    // X positionning
                    int posX = i * cellSpacing + 10;

                    for ( int j = 0; j < inputNbCol; j++ )
                    {
                        // Y positionning
                        int posY = j * cellSpacing + 10;

                        g.DrawRectangle ( selPen, posX, posY, cellSize, cellSize );
                    }
                }

                if ( img != null )
                {
                    List<Point> shape = GetShapePoints ( img, 128 );

                    foreach ( Point pt in shape )
                    {
                        int x = pt.X * cellSpacing + 10;
                        int y = pt.Y * cellSpacing + 10;

                        g.DrawRectangle ( grrrPen, x + 1, y + 1, cellSize - 2, cellSize - 2 );
                    }

                    shape = ComputeBoundingShape ( img, shape, 4, 128 );

                    foreach ( Point pt in shape )
                    {
                        int x = pt.X * cellSpacing + 10;
                        int y = pt.Y * cellSpacing + 10;

                        g.DrawRectangle ( graoouPen, x + 2, y + 2, cellSize - 4, cellSize - 4 );
                    }
                }
            }
        }

        private int BigToLittleEndian ( int bigEndian )
        {
            return ( bigEndian & 0x000000FF ) << 24 |
                   ( bigEndian & 0x0000FF00 ) << 8 |
                   ( bigEndian & 0x00FF0000 ) >> 8 |
                   ( (int) ( bigEndian & 0xFF000000 ) ) >> 24;
        }

        public class Point
        {
            public Point ( int x, int y )
            {
                X = x;
                Y = y;
            }

            public int X, Y;
        }

        private List<Point> GetShapePoints ( int[,] img, int threshold )
        {
            List<Point> result = new List<Point> ( );

            for ( int i = 0; i < img.GetLength ( 0 ); i++ )
            {
                for ( int j = 0; j < img.GetLength ( 1 ); j++ )
                {
                    if ( img[i, j] > threshold )
                    {
                        result.Add ( new Point ( i, j ) );
                    }
                }
            }
            return result;
        }

        private List<Point> ComputeBoundingShape ( int[,] img, List<Point> shape, int nbFace, int threshold )
        {
            // Compute BoundingBox
            // Todo: Create a dimensional abstraction

            int minX = img.GetLength ( 0 );
            int maxX = 0;
            int minY = img.GetLength ( 1 );
            int maxY = 0;

            foreach ( Point pt in shape )
            {
                if ( pt.X < minX ) minX = pt.X;
                if ( pt.X > maxX ) maxX = pt.X;
                if ( pt.Y < minY ) minY = pt.Y;
                if ( pt.Y > maxY ) maxY = pt.Y;
            }

            List<Point> bbox = new List<Point> ( );

            bbox.Add ( new Point ( minX, minY ) );
            bbox.Add ( new Point ( minX, maxY ) );
            bbox.Add ( new Point ( maxX, minY ) );
            bbox.Add ( new Point ( maxX, maxY ) );

            // Todo: Refine bbox.

            return bbox;
        }

        public class TrainingDigit
        {
            public int[,] img;
            public int    label;

            public double[] input;
            public double[] output;
            public double   error;

            public TrainingDigit ( int[,] imgInput, int labelInput )
            {
                img = imgInput;
                label = labelInput;

                int sizeX = img.GetLength ( 0 );
                int sizeY = img.GetLength ( 1 );

                // Fill Input to fed NN

                input = new double[sizeX * sizeY];

                for ( int i = 0; i < sizeX; i++ )
                {
                    for ( int j = 0; j < sizeY; j++ )
                    {
                        input[i * sizeY + j] = ( (double) img[i, j] ) / 256;
                    }
                }

                output = new double[10];
                for ( int i = 0; i < output.Length; i++ )
                {
                    output[i] = 0.0;
                }
                output[label] = 1.0;
            }
        }

        public class NeuronToStringConverter 
        {
            public string ToStringMNIST ( Neuron neuron )
            {
                string result = "W=" + Environment.NewLine;

                int weightCount = neuron.Weights.Count ( );

                for ( int i = 0; i < weightCount; i++ )
                {
                    result += neuron.Weights [ i ].ToString ( "F4" );
                    result += ";";

                    if ( ( ( i + 1 ) % inputNbRow ) == 0 )
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

        

        /// <summary>
        /// Load Dataset and display the first
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadMNISTDataset_Click ( object sender, EventArgs e )
        {
            string picsFileName   = "C:\\Users\\ecaruyer\\Kaggle\\MNIST\\train-images.idx3-ubyte";
            string labelsFileName = "C:\\Users\\ecaruyer\\Kaggle\\MNIST\\train-labels.idx1-ubyte";

            List<TrainingDigit> trainingDigits = LoadMNIST ( picsFileName, labelsFileName );

            int numberOfImages = trainingDigits.Count;

            // Create Learning poll

            double[][] inputs  = new double [numberOfImages][];
            double[][] outputs = new double [numberOfImages][];

            for ( int i = 0; i < numberOfImages; i++ )
            {
                inputs[i] = trainingDigits[i].input;
                outputs[i] = trainingDigits[i].output;
            }

            // Neural network
            // 28x28 input | First Layer 32 | 10 Output.

            ActivationNetwork network = new ActivationNetwork ( new SigmoidFunction ( ), 28*28, 32, 10 );
            network.Randomize ( ); // Not relevant changes
            BackPropagationLearning teacher = new BackPropagationLearning ( network );
            teacher.LearningRate = 2;

            WriteNeuralNetInFile ( network, "InitialNeuralNet.csv" );

            // Learning Loop

            int numberOfEpok = 1;
            List <double> errorTrend = new List<double> ( );

            for ( int i = 0; i < numberOfEpok; i++ )
            {
                // Run epoch of learning procedure
                // double error = teacher.RunEpoch ( inputs, outputs );
                
                double error = 0.0;
                for ( int j = 0; j < numberOfImages; j++ )
                {
                    double[] networkOut = network.Compute ( trainingDigits[j].input );

                    error += teacher.Run ( trainingDigits[j].input, trainingDigits[j].output );
                }
                errorTrend.Add ( error );
            }

            // Save Network

            WriteNeuralNetInFile ( network, "TrainedNeuralNet.csv" );

            // Save Metrics
            string metrics = string.Join ( ";", errorTrend );
            File.WriteAllText ( "LearningMetrics.csv", metrics );

            // Do a test

            picsFileName   = "C:\\Users\\ecaruyer\\Kaggle\\MNIST\\t10k-images.idx3-ubyte";
            labelsFileName = "C:\\Users\\ecaruyer\\Kaggle\\MNIST\\t10k-labels.idx1-ubyte";

            List<TrainingDigit> testDigits = LoadMNIST ( picsFileName, labelsFileName );
            string testMetrics = string.Empty;

            for ( int i = 0; i < testDigits.Count; i++ )
            {
                double[] testResult = network.Compute ( testDigits[i].input );

                double max = testResult.Max ( );
                int predicted = -1;
                for ( int j = 0; j < testResult.Count ( ); j++ )
                {
                    if ( testResult [ j ] == max )
                    {
                        predicted = j; break;
                    }
                }

                testMetrics += ( testDigits[i].label == predicted ) ? "Ok" : "Ko";
                testMetrics += ";" + predicted + ";";
                testMetrics += string.Join ( ";", testResult );
            }
            File.WriteAllText ( "TestMetrics.csv", testMetrics );

            // Display an example

            double[] output = network.Compute ( testDigits[1].input );

            img = testDigits[1].img;

            txtLabel.Text = testDigits[1].label.ToString ( );
            txtOutput.Text = string.Join ( Environment.NewLine, output.Select ( x => x.ToString ( "F3" ) ) );

            Refresh ( );
            Thread.Sleep ( 1000 );
        }

        /// <summary>
        /// Load a MNIST File
        /// </summary>
        /// <param name="picsFileName"></param>
        /// <param name="labelsFileName"></param>
        /// <param name="numberOfImages"></param>
        /// <param name="trainingDigits"></param>
        private List<TrainingDigit> LoadMNIST ( string picsFileName, string labelsFileName )
        {
            BinaryReader imageReader = new BinaryReader ( File.Open ( picsFileName, FileMode.Open ) );
            BinaryReader labelReader = new BinaryReader ( File.Open ( labelsFileName, FileMode.Open ) );

            // Read Picture Hearder

            int magicNumber     = BigToLittleEndian ( imageReader.ReadInt32 ( ) );
            int numberOfImages  = BigToLittleEndian ( imageReader.ReadInt32 ( ) );
            int numberOfRows    = BigToLittleEndian ( imageReader.ReadInt32 ( ) );
            int numberOfColumns = BigToLittleEndian ( imageReader.ReadInt32 ( ) );

            // Read Label Header

            int magicNumberLabel = BigToLittleEndian ( labelReader.ReadInt32 ( ) );
            int numberOfItems    = BigToLittleEndian ( labelReader.ReadInt32 ( ) );

            List<TrainingDigit> digits = new List<TrainingDigit> ( );

            // Read Images

            for ( int trainingDigitIdx = 0; trainingDigitIdx < numberOfImages; trainingDigitIdx++ )
            {
                img = new int[numberOfRows, numberOfColumns];

                for ( int i = 0; i < numberOfRows; i++ )
                {
                    for ( int j = 0; j < numberOfColumns; j++ )
                    {
                        // ReadBytes 256 ???
                        img[j, i] = imageReader.ReadByte ( );
                    }
                }
                int label = labelReader.ReadByte ( );
                digits.Add ( new TrainingDigit ( img, label ) );
            }

            imageReader.Close ( );
            labelReader.Close ( );

            return digits;
        }
    }
}


