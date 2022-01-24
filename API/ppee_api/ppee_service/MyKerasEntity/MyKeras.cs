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
        //path -> saving trained models
        private string path = string.Empty;

        //pathToBestModel -> load best trained model from separate folder
        private string pathToBestModel = string.Empty;

        public MyKeras(int inputDim):base()
        {
            this.InputDim = inputDim;

            string path = AppDomain.CurrentDomain.BaseDirectory;
            char[] charsToTrim = { '\\', ' ' };
            path = path.TrimEnd(charsToTrim);
            path = path.Substring(0, path.LastIndexOf('\\'));
            this.pathToBestModel = path;

            path += "\\TrainedModels\\";

            this.pathToBestModel += "\\BestModel\\";
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

            return model;
        }

        public void SaveModel(Sequential model, string mapeError)
        {
            if (!this.path.Equals(string.Empty) && double.Parse(mapeError) < 17)
            {
                if (mapeError.Contains('.'))
                    mapeError = mapeError.Replace('.', '_');
                else if (mapeError.Contains(','))
                    mapeError = mapeError.Replace(',', '_');

                string modelName = "model_" + mapeError;

                string jsonModel = model.ToJson();
                File.WriteAllText(this.path + modelName +".json", jsonModel);
                model.SaveWeight(this.path + modelName +".h5");
            }
        }

        public BaseModel LoadModel()
        {
            if (!this.pathToBestModel.Equals(string.Empty))
            {
                Keras.Backend.ClearSession();

                var model = Sequential.ModelFromJson(File.ReadAllText(this.pathToBestModel + "model.json"));
                model.LoadWeight(this.pathToBestModel + "model.h5");

                return model;
            }
            return null;
        }

        public List<float> Predict(NDarray predictorTest)
        {

            var model = LoadModel();
            
            if (model == null) return null;

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
