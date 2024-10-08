using System.Data;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace EvolutionTest
{
    public class Matrix
    {
        public int rows, cols, maxrows;
        double[] data;
        public Matrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            this.maxrows = rows;
            data = new double[cols * rows];
        }
        public Matrix(int rows, int cols, double[] data)
        {
            this.rows = rows;
            this.cols = cols;
            this.maxrows = rows;
            this.data = data;
        }
        public Span<double> this[int row] => this.data.AsSpan(row * cols, cols);
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.cols != b.cols) throw new ApplicationException("invalid dimensions");
            int indexA = 0;
            int indexB;
            int indexRes = 0;
            double sum;
            double[] result = new double[a.rows * b.rows];
            for (int i = 0; i < a.rows; i++)
            {
                var rowA = a.data.AsSpan(indexA, a.cols);
                indexA += a.cols;
                var resRow = result.AsSpan(indexRes, b.rows);
                indexRes += b.rows;
                indexB = 0;
                for (int j = 0; j < b.rows; j++)
                {
                    var rowB = b.data.AsSpan(indexB, b.cols);
                    indexB += b.cols;
                    sum = 0;
                    for (int k = 0; k < rowA.Length; k++) sum += rowA[k] * rowB[k];
                    resRow[j] = sum;
                }
            }
            return new Matrix(a.rows, b.rows, result);
        }

        public Matrix Preferences()
        {
            int index = 0;
            double sum;
            double scale;
            double val;
            Span<double> row;
            for (int i = 0; i < rows; i++)
            {
                row = data.AsSpan(index, cols);
                sum = 0;
                for (int j = 0; j < cols; j++)
                {
                    val = row[j];
                    if (val >= 0) sum += val;
                    else row[j] = 0;
                }
                if (sum > 0)
                {
                    scale = 1 / sum;
                    for (int j = 0; j < cols; j++) row[j] *= scale;
                }
                index += cols;
            }
            return this;
        }

        public static Matrix RandomInit(int rows, int cols, double scale = 1, double shift = 0)
        {
            double[] result = new double[rows * cols];
            Random rand = new Random();
            if (shift == 0 && scale == 1)
            {
                for (int i = 0; i < result.Length; i++) result[i] = rand.NextDouble();
            }
            else
            {
                for (int i = 0; i < result.Length; i++) result[i] = rand.NextDouble() * scale + shift;
            }
            return new Matrix(rows, cols, result);
        }
        public override string ToString() => ToString(3);
        public string ToString(int decimals)
        {
            StringBuilder sb = new StringBuilder();
            string format = $"F{decimals}";
            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                sb.Append("[");
                for (int j = 0; j < cols - 1; j++) sb.Append($"{data[index++].ToString(format)}, ");
                sb.Append($"{data[index++].ToString(format)}]\n");
            }
            sb[sb.Length - 1] = ' ';
            return sb.ToString();
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            int resCount = 5;
            int prodCount = 10;
            int prefCount = 6;
            int consCount = 10;
            Matrix resources = Matrix.RandomInit(1, resCount);
            Matrix products = Matrix.RandomInit(prodCount,prefCount);
            Matrix costs = Matrix.RandomInit(prodCount,resCount);
            Matrix consumers = Matrix.RandomInit(consCount, prefCount, 2,-1);
            Console.WriteLine("Resources");
            Console.WriteLine(resources);
            Console.WriteLine("Costs");
            Console.WriteLine(costs);
            Console.WriteLine("Products");
            Console.WriteLine(products);
            Console.WriteLine("Consumers");
            Console.WriteLine(consumers);
            Matrix preferences = consumers * products;
            Console.WriteLine("Preferences with negatives");
            Console.WriteLine(preferences);
            Console.WriteLine("Preferences scaled");
            Console.WriteLine(preferences.Preferences());
        }
    }
}
