using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Text;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.MachineLearning
{


    public static class RabbitMQServiceExtension
    {
        /// <summary>
        /// 注册MachineLearning服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterMachineLearningService(this ServiceHost host)
        {
            host.RegisterType<IMachineLearningService, MachineLearningService>().SingleInstance();
        }
    }

    public class MachineLearningService : IMachineLearningService
    {
        public readonly MLContext MLContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MachineLearningService()
        {
            MLContext = new MLContext();
        }

        public ITransformer BuildAndEvaluateModel(IDataView data, string inputColumnName, double trainRate = 0.1)
        {
            var trainTestData = MLContext.Data.TrainTestSplit(data, trainRate);
            var estimator = MLContext.Transforms.Text.FeaturizeText("Features", inputColumnName)
                .Append(MLContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            Console.WriteLine("=============== Building Model ===============");
            var model = estimator.Fit(trainTestData.TrainSet);
            Console.WriteLine("=============== Evaluating Model ===============");
            IDataView predictions = model.Transform(trainTestData.TestSet);
            CalibratedBinaryClassificationMetrics metrics = MLContext.BinaryClassification.Evaluate(predictions, "Label");
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("--------------------------------");
            return model;
        }
    }
}
