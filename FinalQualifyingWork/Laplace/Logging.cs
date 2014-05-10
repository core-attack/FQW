using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FQW
{
    class Logging
    {
        /// <summary>
        /// лог дебага
        /// </summary>
        public StreamWriter sw_log;
        /// <summary>
        /// лог ошибок
        /// </summary>
        public StreamWriter sw_err;
        /// <summary>
        /// Количество узлов сетки (необходимо для наименования лога)
        /// </summary>
        public int N = 0;

        public Logging()
        {
            DateTime now = DateTime.Now;
            string path = string.Format("logs/task 2 log at N={7} {0}h {1}m {2}s {3}ms {4}-{5}-{6}.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString(), N);
            sw_log = new StreamWriter(path, false, Encoding.UTF8, 10000);
        }

        /// <summary>
        /// Экранирует ошибки
        /// </summary>
        public void viewError(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ОШИБКА:");
            Console.WriteLine("Сообщение: " + e.Message);
            Console.WriteLine("Место: " + e.StackTrace);

            DateTime now = DateTime.Now;
            string path = string.Format("logs/errors at {0}h {1}m {2}s {3}ms {4}-{5}-{6}.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString());
            sw_err = new StreamWriter(path, false, Encoding.UTF8, 1000);
            sw_err.WriteLine(e.Message);
            sw_err.WriteLine(e.StackTrace);
            sw_err.Close();

            Console.ReadKey();
        }

        /// <summary>
        /// Выводит в консоль и записывает в лог
        /// </summary>
        /// <param name="s"></param>
        public void writeLine(string s)
        {
            Console.WriteLine(s);
            sw_log.WriteLine(s);
        }

        /// <summary>
        /// Экранирует матрицу в консоль
        /// </summary>
        /// <param name="u"></param>
        public void printMatrix(double[,] matr, string caption, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            writeLine(caption);
            for (int j = 0; j < matr.GetLength(1); j++)
            {
                writeLine(string.Format("{4} -> {0:F10}\t{1:F10}\t{2:F10}\t{3:F10} <- {4}", matr[0, j], matr[1, j], matr[2, j], matr[3, j], j));
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Экранирует массив в консоль
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="caption"></param>
        public void printArray(double[] arr, string caption, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            writeLine("");
            writeLine(caption);
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                writeLine(string.Format("{1} -> {0:F16} <- {1}", arr[i], i));
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Экранирует массив в консоль
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="caption"></param>
        public void printArray(Dictionary<int, double> arr, string caption, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            writeLine("");
            writeLine(caption);
            foreach (int i in arr.Keys)
            {
                writeLine(string.Format("{1} -> {0:F16} <- {1}", arr[i], i));
            }
            Console.ResetColor();
        }
    }
}
