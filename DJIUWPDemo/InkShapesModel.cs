using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// inkshapes

namespace DJIDemo
{
    public sealed class InkshapesModelInput
    {
        public VideoFrame data { get; set; }
    }

    public sealed class InkshapesModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> loss { get; set; }
        public InkshapesModelOutput()
        {
            this.classLabel = new List<string>();
            this.loss = new Dictionary<string, float>()
            {
                { "airplane", float.NaN },
                { "axe", float.NaN },
                { "bike", float.NaN },
                { "bird", float.NaN },
                { "bomb", float.NaN },
                { "cake", float.NaN },
                { "car", float.NaN },
                { "cat", float.NaN },
                { "chair", float.NaN },
                { "doughnut", float.NaN },
                { "duck", float.NaN },
                { "fish", float.NaN },
                { "flower", float.NaN },
                { "guitar", float.NaN },
                { "heart", float.NaN },
                { "house", float.NaN },
                { "poop", float.NaN },
                { "rocket", float.NaN },
                { "shoe", float.NaN },
                { "stick_figure", float.NaN },
                { "sun", float.NaN },
            };
        }
    }

    public sealed class InkshapesModel
    {
        private LearningModelPreview learningModel;
        public static async Task<InkshapesModel> CreateInkshapesModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            InkshapesModel model = new InkshapesModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<InkshapesModelOutput> EvaluateAsync(InkshapesModelInput input) {
            InkshapesModelOutput output = new InkshapesModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("data", input.data);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("loss", output.loss);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
