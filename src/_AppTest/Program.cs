using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Media;

using static Tensorflow.Binding;
using NumSharp;
using OpenCvSharp;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {


                //// 矩阵乘法
                //var matrix1 = tf.constant(np.array(new float[,] { { 1, 2 }, { 3, 4 } }));
                //var matrix2 = tf.constant(np.array(new float[,] { { 5, 6 }, { 7, 8 } }));
                //var product = tf.matmul(matrix1, matrix2);
                //// 类型转换：tensor转换成numpy
                //print("product =", product.numpy());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
