using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Laplace
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!Directory.Exists("logs"))
                    Directory.CreateDirectory("logs");
                ConsoleKey key;
                LaplaceLogic laplace = new LaplaceLogic();
                do
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Справка:");
                        Console.WriteLine("\tl - включить логирование");
                        Console.WriteLine("\tЕсли нужно вычислить знамение функционала при нескольких N, то просто разделите их запятыми или напишите промежуток через дефис.\nПример: 10, 11, 12 или 13-17.");
                        int N = 0;
                        string s = "";
                        bool logging = false;
                        bool multiInputComma = false;
                        bool multiInputDefis = false;
                        Console.Write("Введите N:");
                        s = Console.ReadLine();

                        logging = s.IndexOf("l") != -1 ? true : false;
                        multiInputComma = s.IndexOf(",") != -1 ? true : false;
                        multiInputDefis = s.IndexOf("-") != -1 ? true : false;
                        Console.ResetColor();

                        if (multiInputComma || multiInputDefis)
                        {
                            if (multiInputComma)
                            {
                                string[] arrN = s.Split(',');
                                for (int i = 0; i < arrN.Length; i++)
                                {
                                    try
                                    {
                                        if (!logging && i == arrN.Length - 1)
                                        {
                                            laplace.setStartParamsFirst(int.Parse(arrN[i].Split(' ')[0]));
                                            laplace.logicFirst(logging);
                                        }
                                        else
                                        {
                                            laplace.setStartParamsFirst(int.Parse(arrN[i]));
                                            laplace.logicFirst(logging);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        viewError(e);
                                    }
                                }
                            }
                            else
                            {
                                string[] arrN = s.Split('-');
                                int begin = int.Parse(arrN[0]);
                                int end = !logging ? int.Parse(arrN.Last().Split(' ')[0]) : int.Parse(arrN.Last());
                                for (int i = begin; i <= end; i++)
                                {
                                    try
                                    {
                                        laplace.setStartParamsFirst(i);
                                        laplace.logicFirst(logging);
                                    }
                                    catch (Exception e)
                                    {
                                        viewError(e);
                                    }
                                }
                            }
                        }
                        else
                        {
                            N = int.Parse(s.Trim().Split(' ')[0]);
                            laplace.setStartParamsFirst(N);
                            laplace.logicFirst(logging);
                        }
                        Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine(String.Format("Jstar = {0:F50}", laplace.Jstar));
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Повторить эксперимент?\n\t y - повторить\n\t n - выйти\n\t s - сохранить информацию на экране и выйти");
                        key = Console.ReadKey().Key;
                        if (key != ConsoleKey.Y && key != ConsoleKey.S)
                            break;
                        if (key == ConsoleKey.S)
                        {
                            StreamWriter sw_log = new StreamWriter("logs/test.txt");
                            bool error = true;
                            while (error)
                            {
                                try
                                {
                                    Console.Write("Введите имя файла (файл будет сохранен в папку со всеми логами): ");
                                    string path = string.Format("logs/{0}.txt", Console.ReadLine());
                                    DateTime now = DateTime.Now;
                                    string date = string.Format("Дата записи {0}h {1}m {2}s {3}ms {4}-{5}-{6}.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString());
                                    StreamWriter writer = new StreamWriter(path, true);
                                    writer.WriteLine(date);
                                    foreach (string str in laplace.screenLog)
                                    {
                                        writer.WriteLine(str);
                                    }
                                    writer.Close();
                                    error = false;
                                    laplace.screenLog.Clear();
                                }
                                catch (Exception e)
                                {
                                    viewError(e);
                                }
                            }
                            sw_log.Close();
                            Console.WriteLine("Повторить эксперимент?\n y - повторить\n n - выйти\n ");
                            key = Console.ReadKey().Key;
                            if (key != ConsoleKey.Y)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        viewError(e);
                    }
                }
                while (true);
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Экранирует ошибки
        /// </summary>
        static void viewError(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ОШИБКА:");
            Console.WriteLine("Сообщение: " + e.Message);
            Console.WriteLine("Место: " + e.StackTrace);
            Console.ReadKey();
        }
    }
}
