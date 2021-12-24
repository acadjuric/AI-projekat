using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_service.MyKerasEntity
{
    public abstract class KerasOptions
    {

        public int BatchSize { get; set; }
        public int EpochNumber { get; set; }
        public string CostFunction { get; set; }
        public string Optimizer { get; set; }
        public string KernelInitializer { get; set; } 
        public string ActivationFunction { get; set; }
        public int NumberOfHiddenLayers { get; set; }
        public int NumberOfNeuronsInFirstHiddenLayer { get; set; }
        public int NumberOfNeuronsInOtherHiddenLayer { get; set; }
        public int Verbose { get; set; }
        public int InputDim { get; set; }


        public KerasOptions()
        {
            BatchSize = 6;
            EpochNumber = 500;
            CostFunction = "mean_squared_error";
            Optimizer = "adamax";
            KernelInitializer = "normal";
            ActivationFunction = "sigmoid";
            NumberOfHiddenLayers = 5;
            NumberOfNeuronsInFirstHiddenLayer = 8;
            NumberOfNeuronsInOtherHiddenLayer = 10;
            Verbose = 2;

        }
    }
}
