# DwFramework.MachineLearning

```shell
PM> Install-Package DwFramework.MachineLearning
或
> dotnet add package DwFramework.MachineLearning
```

## DwFramework MachineLearning库

### 0x1 注册服务及初始化

可以参考如下代码：

```c#
// 注册服务
host.RegisterMachineLearningService();
// 初始化
var provider = host.Build();
```

### 0x2 关键操作

#### 0x2.1 数据准备

```c#
// 定义结构
class DataRow
{
		public int A { get; set; }
		public int B { get; set; }
		public bool Res { get; set; }
}

class PredictedDataRow
{
		public float A { get; set; }
		public float B { get; set; }
		public bool Res { get; set; }
		[ColumnName("PredictedLabel")]
		public bool PredictedRes { get; set; }
		public float Probability { get; set; }
}

// 获取服务
var service = provider.GetMachineLearningService();
// 模拟数据
Random random = new Random();
List<DataRow> rows = new List<DataRow>();
for (int i = 0; i < 100; i++)
{
		int a = random.Next(100);
		int b = random.Next(100);
		bool res = a + b < 100;
		rows.Add(new DataRow() { A = a, B = b, Res = res });
}
// 数据加载
var data = service.DataLoad(rows);
// 分割数据
var splitedData = service.DataOperations.TrainTestSplit(data, 0.2);
// 创建数据处理管道
var dataHandlePipeline = service.BuildDataTransformPipeline(data,
		service.Transforms.Conversion.ConvertType(new[]{
				new InputOutputColumnPair("A"),
				new InputOutputColumnPair("B")
		}, DataKind.Single),
		service.Transforms.Concatenate("Features", "A", "B"),
		service.Transforms.NormalizeMinMax(new[] {
				new InputOutputColumnPair("Features")
		})
);
// 数据处理
var trainData = dataHandlePipeline.Transform(splitedData.TrainSet);
var testData = dataHandlePipeline.Transform(splitedData.TestSet);
```

#### 0.2.2 训练模型

本次模拟数据适用二元分类算法处理，所以选择BinaryClassificationTrainers.AveragedPerceptron训练器。

```c#
// 选择训练器
var trainer = service.BinaryClassificationTrainers.SdcaLogisticRegression("Res");
// 训练模型
var model = trainer.Fit(trainData);
```

#### 0.2.3 评估模型

```c#
// 评估模型
var metrics = service.BinaryClassification.Evaluate(model.Transform(testData), "Res");
// 查看评估指标
Console.WriteLine($"Accuracy:{metrics.Accuracy:P2}");
Console.WriteLine($"AreaUnderRocCurve:{metrics.AreaUnderRocCurve:P2}");
Console.WriteLine($"F1Score:{metrics.F1Score:P2}");
```

#### 0.2.4 测试模型

```c#
// 模拟数据
List<DataRow> simpleRows = new List<DataRow>();
for (int i = 0; i < 20; i++)
{
		int a = random.Next(100);
		int b = random.Next(100);
		bool res = a + b < 100;
		simpleRows.Add(new DataRow() { A = a, B = b, Res = res });
}
// 数据处理
var simpleData = service.DataLoad(simpleRows);
var encodedSimpleData = dataHandlePipeline.Transform(simpleData);
var results = model.Transform(encodedSimpleData);
// 对比数据
foreach (var item in service.DataOperations.CreateEnumerable<PredictedDataRow>(results, true))
{
		Console.WriteLine($"{item.A}\t{item.B}\t{item.PredictedRes == item.Res}");
}
```

#### 0.2.5 输出结果

因为这个问题太简单了，结果百分之百的准确率😂...

```c#
Accuracy:100.00%
AreaUnderRocCurve:100.00%
F1Score:100.00%
  
A			B			PredictedRes == item.Res
79		66		True
86		9			True
33		19		True
63		55		True
63		0			True
74		61		True
43		40		True
54		86		True
61		96		True
47		34		True
94		20		True
23		41		True
20		10		True
98		91		True
72		95		True
84		51		True
56		99		True
88		77		True
44		47		True
56		49		True
```