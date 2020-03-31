using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.ML;

using DwFramework.Core;

namespace DwFramework.MachineLearning
{
    public class MachineLearningService : ServiceApplication
    {
        public readonly MLContext MlContext;
        public DataOperationsCatalog DataOperations { get { return MlContext.Data; } }
        public TransformsCatalog Transforms { get { return MlContext.Transforms; } }
        public ModelOperationsCatalog ModelOperations { get { return MlContext.Model; } }

        // 异常情况检测
        public AnomalyDetectionCatalog AnomalyDetection { get { return MlContext.AnomalyDetection; } }
        public AnomalyDetectionCatalog.AnomalyDetectionTrainers AnomalyDetectionTrainers { get { return MlContext.AnomalyDetection.Trainers; } }
        // 二元分类
        public BinaryClassificationCatalog BinaryClassification { get { return MlContext.BinaryClassification; } }
        public BinaryClassificationCatalog.BinaryClassificationTrainers BinaryClassificationTrainers { get { return MlContext.BinaryClassification.Trainers; } }
        // 聚类分析
        public ClusteringCatalog Clustering { get { return MlContext.Clustering; } }
        public ClusteringCatalog.ClusteringTrainers ClusteringTrainers { get { return MlContext.Clustering.Trainers; } }
        // 预测
        public ForecastingCatalog Forecasting { get { return MlContext.Forecasting; } }
        public ForecastingCatalog.Forecasters ForecasterTrainers { get { return MlContext.Forecasting.Trainers; } }
        // 排名
        public RankingCatalog Ranking { get { return MlContext.Ranking; } }
        public RankingCatalog.RankingTrainers RankingTrainers { get { return MlContext.Ranking.Trainers; } }
        // 回归测试
        public RegressionCatalog Regression { get { return MlContext.Regression; } }
        public RegressionCatalog.RegressionTrainers RegressionTrainers { get { return MlContext.Regression.Trainers; } }
        // 建议
        public RecommendationCatalog Recommendation { get { return MlContext.Recommendation(); } }
        public RecommendationCatalog.RecommendationTrainers RecommendationTrainers { get { return MlContext.Recommendation().Trainers; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MachineLearningService(IServiceProvider provider, IRunEnvironment environment) : base(provider, environment)
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
            var sourceData = DataOperations.LoadFromEnumerable(data);
            return handle == null ? sourceData : handle(sourceData, DataOperations);
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
            var sourceData = DataOperations.LoadFromTextFile<T>(path, separatorChar, hasHeader, allowQuoting, trimWhitespace, allowSparse);
            return handle == null ? sourceData : handle(sourceData, DataOperations);
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
            var loader = DataOperations.CreateTextLoader<T>(separatorChar, hasHeader, null, allowQuoting, trimWhitespace, allowSparse);
            var sourceData = loader.Load(paths);
            return handle == null ? sourceData : handle(sourceData, DataOperations);
        }

        /// <summary>
        /// 创建数据转换处理管道
        /// </summary>
        /// <param name="data"></param>
        /// <param name="estimators"></param>
        /// <returns></returns>
        public ITransformer BuildDataTransformPipeline(IDataView data, params IEstimator<ITransformer>[] estimators)
        {
            if (estimators.Length <= 0)
                throw new Exception("参数错误");
            var estimatorList = estimators.ToArray();
            var estimator = estimators.First();
            for (int i = 1; i < estimators.Count(); i++)
                estimator = estimator.Append(estimatorList[i]);
            return estimator.Fit(data);
        }
    }
}
