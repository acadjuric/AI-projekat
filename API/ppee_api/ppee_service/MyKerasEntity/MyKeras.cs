using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_service.MyKerasEntity
{
    public class MyKeras:KerasOptions
    {

        private string path = string.Empty;

        public MyKeras(int inputDim):base()
        {
            this.InputDim = inputDim;

            string path = AppDomain.CurrentDomain.BaseDirectory;
            char[] charsToTrim = { '\\', ' ' };
            path = path.TrimEnd(charsToTrim);
            path = path.Substring(0, path.LastIndexOf('\\'));
            path += "\\TrainedModels\\";

            this.path = path;
        }

        public Sequential TrainModel(NDarray predictorData, NDarray predictedData)
        {
            var model = this.CreateModel();
            model.Compile(this.Optimizer, this.CostFunction, new string[] { "accuracy" });

            predictorData = np.asarray(predictorData).astype(np.float32);
            predictedData = np.asarray(predictedData).astype(np.float32);

            //ovde trenira, nakon ovoga model je spreman za cuvanje
            model.Fit(predictorData, predictedData, epochs: this.EpochNumber, batch_size: this.BatchSize, verbose: this.Verbose);



            if (!this.path.Equals(string.Empty))
            {
                string jsonModel = model.ToJson();
                File.WriteAllText(this.path + "model.json", jsonModel);
                model.SaveWeight(this.path + "model.h5");
            }


            return model;
        }

        private BaseModel LoadModel()
        {
            var model = Sequential.ModelFromJson(File.ReadAllText(this.path + "model.json"));
            model.LoadWeight(this.path + "model.h5");
            
            return model;
        }

        public List<float> Predict(NDarray predictorTest)
        {
            var model = LoadModel();

            var result = model.Predict(predictorTest);

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

            return model;
        }
    }
}
