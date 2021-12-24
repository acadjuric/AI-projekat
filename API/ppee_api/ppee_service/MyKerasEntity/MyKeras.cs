using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_service.MyKerasEntity
{
    public class MyKeras:KerasOptions
    {

        private Sequential model = null;

        public MyKeras(int inputDim):base()
        {
            this.InputDim = inputDim;
            model = null;
        }


        public Sequential TrainModel(NDarray predictorData, NDarray predictedData)
        {
            var model = this.CreateModel();
            model.Compile(this.Optimizer, this.CostFunction, new string[] { "accuracy" });

            predictorData = np.asarray(predictorData).astype(np.float32);
            predictedData = np.asarray(predictedData).astype(np.float32);

            //ovde trenira, nakon ovoga model je spreman za cuvanje
            model.Fit(predictorData, predictedData, epochs: this.EpochNumber, batch_size: this.BatchSize, verbose: this.Verbose);

            this.model = model;
            return model;
        }

        private BaseModel LoadModel()
        {
            //ucitaj model iz fajla i vrati ga
            
            return null;
        }

        public List<float> Predict(NDarray predictorTest)
        {
            // var model = LoadModel();

            var result = this.model.Predict(predictorTest);

            return KerasHelpers.NDarrayToList(result);
        }
        
        private Sequential CreateModel()
        {
            var model = new Sequential();
            if (this.NumberOfHiddenLayers > 0)
                model.Add(new Dense(this.NumberOfNeuronsInFirstHiddenLayer, this.InputDim, this.ActivationFunction, kernel_initializer: this.KernelInitializer));
            if(this.NumberOfHiddenLayers > 1)
            {
                for(int i =0; i <= this.NumberOfHiddenLayers -1; i++)
                {
                    model.Add(new Dense(this.NumberOfNeuronsInOtherHiddenLayer, activation: this.ActivationFunction, kernel_initializer: this.KernelInitializer));
                }
            }

            model.Add(new Dense(1, kernel_initializer: this.KernelInitializer));

            this.model = model;
            return model;
        }
    }
}
