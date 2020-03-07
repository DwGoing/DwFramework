using System;
using System.Collections.Generic;

using Microsoft.ML;

namespace DwFramework.MachineLearning
{
    public class MachineLearningService : IMachineLearningService
    {
        public readonly MLContext MlContext;
        public DataOperationsCatalog Data { get { return MlContext.Data; } }
        public TransformsCatalog Transforms { get { return MlContext.Transforms; } }
        public ModelOperationsCatalog Model { get { return MlContext.Model; } }

        public BinaryClassificationCatalog BinaryClassification { get { return MlContext.BinaryClassification; } }
        public BinaryClassificationCatalog.BinaryClassificationTrainers BinaryClassificationTrainers { get { return MlContext.BinaryClassification.Trainers; } }
        public RegressionCatalog Regression { get { return MlContext.Regression; } }
        public RegressionCatalog.RegressionTrainers RegressionTrainers { get { return MlContext.Regression.Trainers; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MachineLearningService()
        {
            MlContext = new MLContext();
        }

        /// <summary>
        /// 数据加载处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public IDataView DataLoad<T>(IEnumerable<T> data, Func<IDataView, DataOperationsCatalog, IDataView> handle = null) where T : class
        {
            var sourceData = Data.LoadFromEnumerable(data);
            return handle == null ? sourceData : handle(sourceData, Data);
        }

        /// <summary>
        /// 数据加载处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="handle"></param>
        /// <param name="separatorChar"></param>
        /// <param name="hasHeader"></param>
        /// <param name="allowQuoting"></param>
        /// <param name="trimWhitespace"></param>
        /// <param name="allowSparse"></param>
        /// <returns></returns>
        public IDataView DataLoad<T>(string path, Func<IDataView, DataOperationsCatalog, IDataView> handle = null, char separatorChar = '\t', bool hasHeader = false, bool allowQuoting = false, bool trimWhitespace = false, bool allowSparse = false) where T : class
        {
            var sourceData = Data.LoadFromTextFile<T>(path, separatorChar, hasHeader, allowQuoting, trimWhitespace, allowSparse);
            return handle == null ? sourceData : handle(sourceData, Data);
        }

        /// <summary>
        /// 数据加载处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paths"></param>
        /// <param name="handle"></param>
        /// <param name="separatorChar"></param>
        /// <param name="hasHeader"></param>
        /// <param name="allowQuoting"></param>
        /// <param name="trimWhitespace"></param>
        /// <param name="allowSparse"></param>
        /// <returns></returns>
        public IDataView DataLoad<T>(string[] paths, Func<IDataView, DataOperationsCatalog, IDataView> handle = null, char separatorChar = '\t', bool hasHeader = false, bool allowQuoting = false, bool trimWhitespace = false, bool allowSparse = false) where T : class
        {
            var loader = Data.CreateTextLoader<T>(separatorChar, hasHeader, null, allowQuoting, trimWhitespace, allowSparse);
            var sourceData = loader.Load(paths);
            return handle == null ? sourceData : handle(sourceData, Data);
        }

        /// <summary>
        /// 数据转换处理
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public IEstimator<ITransformer> BuildDataTranferPipeline(Func<TransformsCatalog, IEstimator<ITransformer>> handle) => handle(Transforms);
    }
}
