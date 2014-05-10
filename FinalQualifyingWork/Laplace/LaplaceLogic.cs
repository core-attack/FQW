using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Laplace
{
    class LaplaceLogic
    {
        #region[переменные]
        ///// <summary>
        ///// лог дебага
        ///// </summary>
        //static StreamWriter sw_log;
        ///// <summary>
        ///// лог ошибок
        ///// </summary>
        //static StreamWriter sw_err;

        /// <summary>
        /// Логирование
        /// </summary>
        FQW.Logging log;
        /// <summary>
        /// подсчет времени выполнения расчетов
        /// </summary>
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        /// <summary>
        /// Log сохранения, вызванного пользователем
        /// </summary>
        public List<string> screenLog = new List<string>();
        public bool logging = false;

        /// <summary>
        /// значение функционала
        /// </summary>
        public double Jstar = 0;

        //N, λ, γ, τ, ε, ω, ξ, ϕ, ρ, σ, θ, α, β, δ, Δ
        /// <summary>
        /// Количество узлов
        /// </summary>
        int N = 0;
        /// <summary>
        /// γ0
        /// </summary>
        double gamma0 = 0;
        /// <summary>
        /// γ1
        /// </summary>
        double gamma1 = 0;
        /// <summary>
        /// λ
        /// </summary>
        double lambda = 0;
        /// <summary>
        /// τ
        /// </summary>
        double TAU = 0;
        /// <summary>
        /// τ[]
        /// </summary>
        double[] tau;
        /// <summary>
        /// ε
        /// </summary>
        double epsilon = 0;
        /// <summary>
        /// ω0
        /// </summary>
        Dictionary<int, double> omega0;
        /// <summary>
        /// ω1
        /// </summary>
        Dictionary<int, double> omega1;
        /// <summary>
        /// ξ
        /// </summary>
        double xi = 0;
        /// <summary>
        /// ϕ0
        /// </summary>
        string phi0 = "ξ^4";
        /// <summary>
        /// ϕ1
        /// </summary>
        string phi1 = "1 - 6 * ξ^2 + ξ^4";
        /// <summary>
        /// ρ0
        /// </summary>
        string ro0 = "t^4";
        /// <summary>
        /// ρ1
        /// </summary>
        string ro1 = "t^4 - 6*t^2 + 1";
        /// <summary>
        /// σ
        /// </summary>
        double sigma = 0;
        /// <summary>
        /// θ
        /// </summary>
        double teta = 0;
        /// <summary>
        /// α0
        /// </summary>
        double alpha0 = 0;
        /// <summary>
        /// α1
        /// </summary>
        double alpha1 = 0;
        /// <summary>
        /// β0
        /// </summary>
        double beta0 = 0;
        /// <summary>
        /// β1
        /// </summary>
        double beta1 = 0;
        /// <summary>
        /// δ0
        /// </summary>
        double delta0 = 0;
        /// <summary>
        /// δ1
        /// </summary>
        double delta1 = 0;
        /// <summary>
        /// h
        /// </summary>
        double H = 0;
        /// <summary>
        /// h[]
        /// </summary>
        double[] h;
        /// <summary>
        /// трехдиагональная матрица u
        /// </summary>
        double[,] u;

        double[,] z;

        int n = 0;
        double a = 0;
        double b = 0;
        double X = 0;
        double Y = 0;
        double[,] A;
        double[,] B;
        double[] Cheb;
        Dictionary<int, double> x;
        Dictionary<int, double> X0;
        Dictionary<int, double> X1;
        Dictionary<int, double> Y0;
        Dictionary<int, double> Y1;
        Dictionary<int, double> y;
        Dictionary<int, double> v0;
        Dictionary<int, double> v1;
        Dictionary<int, double> z0;
        Dictionary<int, double> z1;

        /// <summary>
        /// Парсер математических выражений
        /// </summary>
        FQW.Parser parser = new FQW.Parser();

        #endregion

        #region[конструкторы]
        public LaplaceLogic()
        { }

        public LaplaceLogic(int N)
        { 
            //setStartParamsFirst(N);
            setStartParamsSecond(N); 
        }
        #endregion

        #region[задача 1]
        /// <summary>
        /// Задать начальные параметры для задачи 1
        /// </summary>
        /// <param name="N">количество узлов</param>
        public void setStartParamsFirst(int N)
        {
            //DateTime now = DateTime.Now;
            //string path = string.Format("logs/task 1 log at N={7} {0}h {1}m {2}s {3}ms {4}-{5}-{6}.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString(), N);
            //sw_log = new StreamWriter(path, false, Encoding.UTF8, 10000);
            log = new FQW.Logging();
            this.N = N;
            n = N - 1;
            tau = new double[3 * N + 1];
            h = new double[4];
            u = new double[4, 3 * N + 1];
            x = new Dictionary<int, double>(); //new double[3 * N + 1];
            y = new Dictionary<int, double>(); //new double[3 * N + 1];
            Jstar = 0;
        }

        /// <summary>
        /// Логика метода Лапласа для задачи 1
        /// </summary>
        public void logicFirst(bool logging)
        {
            try
            {
                this.logging = logging;
                watch.Start();

                TAU = 1.0 / (3 * N);
                H = 1.0 / 3.0;
                for (int i = 0; i < tau.Length; i++)
                    tau[i] = i * TAU;
                for (int j = 0; j < h.Length; j++)
                    h[j] = j * H;

                double ro0_0 = parser.Evaluate(ro0, 0);
                double ro0_1 = parser.Evaluate(ro0, 1);
                double ro1_0 = parser.Evaluate(ro1, 0);
                double ro1_1 = parser.Evaluate(ro1, 1);

                for (int i = 0; i <= 3 * N; i++)
                {
                    //varx = (double)i / (3 * N);
                    //u[i, 0] = parser.Evaluate(phi0, varx) - parser.Evaluate(phi0, 0) * (1 - varx) - parser.Evaluate(phi0, 1) * varx;
                    //u[i, 1] = parser.Evaluate(phi1, varx) - parser.Evaluate(phi1, 0) * (1 - varx) - parser.Evaluate(phi1, 1) * varx;

                    u[0, i] = parser.Evaluate(ro0, tau[i]) - ro0_0 * (1 - tau[i]) - ro0_1 * tau[i];//parser.Evaluate("t^4 - t", varx);//
                    u[3, i] = parser.Evaluate(ro1, tau[i]) - ro1_0 * (1 - tau[i]) - ro1_1 * tau[i];//parser.Evaluate("t^4 - 6 * t^2 + 5 * t", varx);//
                }

                a = 1; //?
                b = 1; //?

                teta = (b * TAU * TAU) / (a * H * H); //+
                //вспомогательные переменные
                double sqr_teta = teta * teta;
                v0 = new Dictionary<int, double>();//new double[N];
                v1 = new Dictionary<int, double>();//new double[N];
                z0 = new Dictionary<int, double>();//new double[N + 1];
                z1 = new Dictionary<int, double>();//new double[N + 1];
                omega0 = new Dictionary<int, double>(); //new double[N + 1];
                omega1 = new Dictionary<int, double>(); //new double[N + 1];
                X0 = new Dictionary<int, double>(); //new double[N + 1];
                X1 = new Dictionary<int, double>(); //new double[N + 1];
                Y0 = new Dictionary<int, double>(); //new double[N + 1];
                Y1 = new Dictionary<int, double>(); //new double[N + 1];


                for (int k = 1; k <= N; k++) //+
                {
                    z0[k] = (1.0 / (2 * teta)) * (u[0, 3 * k - 3] - u[0, 3 * k - 2] - u[0, 3 * k - 1] + u[0, 3 * k]
                                                + u[3, 3 * k - 3] - u[3, 3 * k - 2] - u[3, 3 * k - 1] + u[3, 3 * k]); //+

                    omega0[k] = (3.0 / (2 * teta)) * (u[0, 3 * k - 3] - 3 * u[0, 3 * k - 2] + 3 * u[0, 3 * k - 1] - u[0, 3 * k]
                                                    + u[3, 3 * k - 3] - 3 * u[3, 3 * k - 2] + 3 * u[3, 3 * k - 1] - u[3, 3 * k]); //+

                    z1[k] = (1.0 / (6 * teta)) * (u[0, 3 * k - 3] - u[0, 3 * k - 2] - u[0, 3 * k - 1] + u[0, 3 * k]
                                                - u[3, 3 * k - 3] + u[3, 3 * k - 2] + u[3, 3 * k - 1] - u[3, 3 * k]); //+

                    omega1[k] = (1.0 / (2 * teta)) * (u[0, 3 * k - 3] - 3 * u[0, 3 * k - 2] + 3 * u[0, 3 * k - 1] - u[0, 3 * k]
                                                    - u[3, 3 * k - 3] + 3 * u[3, 3 * k - 2] - 3 * u[3, 3 * k - 1] + u[3, 3 * k]); //+
                }

                alpha0 = 0.5 * (1 + sqr_teta) / (3 + 5 * teta + 3 * sqr_teta); //+
                beta0 = (35 + 3 * sqr_teta) / (30 * (21 + 7 * teta + sqr_teta)); //+
                Y = (alpha0 + beta0) / (alpha0 - beta0); //+
                for (int k = 1; k <= n; k++) //+
                    v0[k] = (1 + Y) * (z0[k] + z0[k + 1]) + (1 - Y) * (omega0[k] - omega0[k + 1]); //+

                alpha1 = (9 + 105 * sqr_teta) / (10 + 70 * teta + 210 * sqr_teta); //+
                beta1 = (3 + 3 * sqr_teta) / (10 + 14 * teta + 10 * sqr_teta); //+
                X = (alpha1 + beta1) / (alpha1 - beta1); //+
                for (int k = 1; k <= n; k++) //+
                    v1[k] = (1 + X) * (z1[k] + z1[k + 1]) + (1 - X) * (omega1[k] - omega1[k + 1]); //+

                gamma0 = (10 * teta + 10 * sqr_teta) / (9 + 15 * teta + 9 * sqr_teta); //+
                delta0 = (70 * teta + 14 * sqr_teta) / (189 + 63 * teta + 9 * sqr_teta); //+

                gamma1 = (14 * teta + 70 * sqr_teta) / (3 + 21 * teta + 63 * sqr_teta); //+
                delta1 = (70 * teta + 70 * sqr_teta) / (45 + 63 * teta + 45 * sqr_teta); //+

                progon(ref x, 1.0, 2 * Y, 1.0, v0);
                progon(ref y, 1.0, 2 * X, 1.0, v1);

                for (int k = 1; k <= N; k++)
                {
                    X0[k] = gamma0 * (x[3 * k - 3] + x[3 * k] + 2 * z0[k]);
                    Y0[k] = delta0 * (x[3 * k - 3] - x[3 * k] + 2 * omega0[k]);
                    X1[k] = gamma1 * (y[3 * k - 3] + y[3 * k] + 2 * z1[k]);
                    Y1[k] = delta1 * (y[3 * k - 3] - y[3 * k] + 2 * omega1[k]);
                }

                double prod = (9 * H) / (64 * Math.Pow(TAU, 3));
                for (int k = 1; k <= N; k++)
                {
                    Jstar += prod *
                                (
                                    4 * sqr_teta * Math.Pow((x[3 * k - 3] + x[3 * k] + 2 * z0[k]), 2)
                                       - 6 * teta * (1 + teta) * (x[3 * k - 3] + x[3 * k] + 2 * z0[k]) * X0[k]
                                           + 0.9 * (3 + 5 * teta + 3 * sqr_teta) * Math.Pow(X0[k], 2) //+

                                    +

                                    (4.0 / 3.0) * sqr_teta * Math.Pow((x[3 * k - 3] - x[3 * k] + 2 * omega0[k]), 2)
                                       - (6.0 / 5.0) * teta * (5 + teta) * (x[3 * k - 3] - x[3 * k] + 2 * omega0[k]) * X1[k]
                                           + (27.0 / 70.0) * (21 + 7 * teta + sqr_teta) * Math.Pow(Y0[k], 2) //+

                                    +

                                    12 * sqr_teta * Math.Pow((y[3 * k - 3] + y[3 * k] + 2 * z1[k]), 2)
                                       - (18.0 / 5.0) * teta * (1 + 5 * teta) * (y[3 * k - 3] + y[3 * k] + 2 * z1[k]) * Y0[k]
                                           + (27.0 / 70.0) * (1 + 7 * teta + 21 * sqr_teta) * Math.Pow(X1[k], 2) //+

                                    +

                                    4 * sqr_teta * Math.Pow((y[3 * k - 3] - y[3 * k] + 2 * omega1[k]), 2)
                                       - (18.0 / 5.0) * teta * (1 + teta) * (y[3 * k - 3] - y[3 * k] + 2 * omega1[k]) * Y1[k]
                                           + (81.0 / 350.0) * (5 + 7 * teta + 5 * sqr_teta) * Math.Pow(Y1[k], 2) //+
                                );
                    #region [логирование]
                    if (logging)
                    {
                        double var2 = 4 * sqr_teta * Math.Pow((x[3 * k - 3] + x[3 * k] + 2 * z0[k]), 2)
                                               - 6 * teta * (1 + teta) * (x[3 * k - 3] + x[3 * k] + 2 * z0[k]) * X0[k]
                                                   + 0.9 * (3 + 5 * teta + 3 * sqr_teta) * Math.Pow(X0[k], 2);
                        double var3 = (4.0 / 3.0) * sqr_teta * Math.Pow((x[3 * k - 3] - x[3 * k] + 2 * omega0[k]), 2)
                                               - (6.0 / 5.0) * teta * (5 + teta) * (x[3 * k - 3] - x[3 * k] + 2 * omega0[k]) * X1[k]
                                                   + (27.0 / 70.0) * (21 + 7 * teta + sqr_teta) * Math.Pow(Y0[k], 2);
                        double var4 = 12 * sqr_teta * Math.Pow((y[3 * k - 3] + y[3 * k] + 2 * z1[k]), 2)
                                           - (18.0 / 5.0) * teta * (1 + 5 * teta) * (y[3 * k - 3] + y[3 * k] + 2 * z1[k]) * Y0[k]
                                               + (27.0 / 70.0) * (1 + 7 * teta + 21 * sqr_teta) * Math.Pow(X1[k], 2);
                        double var5 = 4 * sqr_teta * Math.Pow((y[3 * k - 3] - y[3 * k] + 2 * omega1[k]), 2)
                                           - (18.0 / 5.0) * teta * (1 + teta) * (y[3 * k - 3] - y[3 * k] + 2 * omega1[k]) * Y1[k]
                                               + (81.0 / 350.0) * (5 + 7 * teta + 5 * sqr_teta) * Math.Pow(Y1[k], 2);

                        log.writeLine(string.Format("prod = {0}", prod));
                        Console.ForegroundColor = ConsoleColor.White;
                        log.writeLine(string.Format("1 = {0}", var2));
                        Console.ForegroundColor = ConsoleColor.Green;
                        log.writeLine(string.Format("2 = {0}", var3));
                        Console.ForegroundColor = ConsoleColor.Blue;
                        log.writeLine(string.Format("3 = {0}", var4));
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        log.writeLine(string.Format("4 = {0}", var5));
                        Console.ResetColor();
                        log.writeLine(string.Format("sum 1-4 = {0}", var2 + var3 + var4 + var5));
                        Console.ForegroundColor = ConsoleColor.Red;
                        log.writeLine(string.Format("Jstar[{0}] = {1}", k, Jstar));
                        Console.ResetColor();
                    }
                    #endregion
                }
                Jstar *= a * a;
                #region [логирование]
                if (logging)
                {
                    log.printMatrix(u, string.Format("u[{0},{1}]: ", u.GetLength(0), u.GetLength(1)), ConsoleColor.White);
                    log.writeLine(string.Format("a = {0}", a));
                    log.writeLine(string.Format("b = {0}", b));
                    log.writeLine(string.Format("tau = {0}", TAU));
                    log.printArray(tau, string.Format("tau[]: ", tau.Length), ConsoleColor.White);
                    log.writeLine(string.Format("h = {0}", H));
                    log.printArray(h, string.Format("h[]: ", h.Length), ConsoleColor.White);
                    log.writeLine(string.Format("teta = {0}", teta));
                    log.printArray(z0, string.Format("z0[]: ", z0.Count), ConsoleColor.White);
                    log.printArray(z1, string.Format("z1[]: ", z1.Count), ConsoleColor.White);
                    /*
                    for (int k = 1; k <= N; k++)
                    {
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k - 3, u[0, 3 * k - 3]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k - 2, u[0, 3 * k - 2]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k - 1, u[0, 3 * k - 1]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k, u[0, 3 * k]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k - 3, u[3, 3 * k - 3]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k - 2, u[3, 3 * k - 2]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k - 1, u[3, 3 * k - 1]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k, u[3, 3 * k]);

                        Console.WriteLine("1.0 / (6 * teta) = {0}", 1.0 / (6 * teta));

                        Console.WriteLine("{0:F16}\n- {1:F16}\n- {2:F16}\n+ {3:F16}\n- {4:F16}\n+ {5:F16}\n+ {6:F16}\n- {7:F16}\n=\n{8:F16}",
                            u[0, 3 * k - 3],
                            u[0, 3 * k - 2],
                            u[0, 3 * k - 1],
                            u[0, 3 * k],
                            u[3, 3 * k - 3],
                            u[3, 3 * k - 2],
                            u[3, 3 * k - 1],
                            u[3, 3 * k],
                            u[0, 3 * k - 3] - u[0, 3 * k - 2] - u[0, 3 * k - 1] + u[0, 3 * k]
                            - u[3, 3 * k - 3] + u[3, 3 * k - 2] + u[3, 3 * k - 1] - u[3, 3 * k]);

                        Console.WriteLine("z1[k] = {0}", z1[k]);
                    }
                    */
                    log.printArray(omega0, string.Format("omega0[]: ", omega0.Count), ConsoleColor.White);
                    log.printArray(omega1, string.Format("omega1[]: ", omega1.Count), ConsoleColor.White);
                    log.writeLine(string.Format("alpha0 = {0}", alpha0));
                    log.writeLine(string.Format("beta0 = {0}", beta0));
                    log.writeLine(string.Format("y = {0}", Y));
                    log.printArray(v0, string.Format("v0[]: ", v0.Count), ConsoleColor.White);
                    log.writeLine(string.Format("alpha1 = {0}", alpha1));
                    log.writeLine(string.Format("beta1 = {0}", beta1));
                    log.writeLine(string.Format("x = {0}", X));
                    log.printArray(v1, string.Format("v1[]: ", v1.Count), ConsoleColor.White);
                    log.writeLine(string.Format("gamma0 = {0}", gamma0));
                    log.writeLine(string.Format("delta0 = {0}", delta0));
                    log.writeLine(string.Format("gamma1 = {0}", gamma1));
                    log.writeLine(string.Format("delta1 = {0}", delta1));
                    log.printArray(x, string.Format("x[]: ", x.Count), ConsoleColor.White);
                    progonCheck("Проверяем массив x:", ref x, Y, v0);
                    log.printArray(y, string.Format("y[]: ", y.Count), ConsoleColor.White);
                    progonCheck("Проверяем массив y:", ref y, X, v1);
                    log.printArray(X0, string.Format("X0[]: ", X0.Count), ConsoleColor.White);
                    log.printArray(Y0, string.Format("Y0[]: ", Y0.Count), ConsoleColor.White);
                    log.printArray(X1, string.Format("X1[]: ", X1.Count), ConsoleColor.White);
                    log.printArray(Y1, string.Format("Y1[]: ", Y1.Count), ConsoleColor.White);
                    //X0X1Y0Y1Check("Проверяем массивы X0, Y0, X1, Y1:", ref X0, ref X1, ref Y0, ref Y1, ref x, ref y);
                }
                log.writeLine(string.Format("Jstar = {0}", Jstar));
                screenLog.Add(string.Format("N = {1} Jstar = {0}", Jstar, N));
                #endregion
            }
            catch (Exception e)
            {
                log.viewError(e);
            }
            finally
            {
                watch.Stop();
                Console.ForegroundColor = ConsoleColor.Yellow;
                log.writeLine(string.Format("Затраченное время: {0}", watch.Elapsed.ToString(@"hh\:mm\:ss\.FFFFFFF")));
                Console.ResetColor();
                watch.Reset();
                log.sw_log.Close();
            }
        }
        #endregion
        
        #region[задача 2]
        /// <summary>
        /// Задать начальные параметры для задачи 2
        /// </summary>
        /// <param name="N">количество узлов</param>
        public void setStartParamsSecond(int N)
        {
            //DateTime now = DateTime.Now;
            //string path = string.Format("logs/task 2 log at N={7} {0}h {1}m {2}s {3}ms {4}-{5}-{6}.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString(), N);
            //sw_log = new StreamWriter(path, false, Encoding.UTF8, 10000);
            this.N = N;
            n = N - 1;
            tau = new double[4];
            h = new double[3 * N + 1];
            u = new double[3 * N + 1, 4];
            x = new Dictionary<int, double>(); //new double[3 * N + 1];
            y = new Dictionary<int, double>(); //new double[3 * N + 1];
            Jstar = 0;
        }

        /// <summary>
        /// Логика метода Лапласа для задачи 2
        /// </summary>
        public void logicSecond(bool logging)
        {
            try
            {
                this.logging = logging;
                watch.Start();

                TAU = 1.0 / 3;
                H = 1.0 / (3.0 * N);
                for (int i = 0; i < tau.Length; i++)
                    tau[i] = i * TAU;
                for (int j = 0; j < h.Length; j++)
                    h[j] = j * H;

                double ro0_0 = parser.Evaluate(ro0, 0);
                double ro0_1 = parser.Evaluate(ro0, 1);
                double ro1_0 = parser.Evaluate(ro1, 0);
                double ro1_1 = parser.Evaluate(ro1, 1);

                for (int i = 0; i <= 3 * N; i++)
                {
                    u[i, 0] = parser.Evaluate(phi0, h[i]) - parser.Evaluate(phi0, 0) * (1 - h[i]) - parser.Evaluate(phi0, 1) * h[i];
                    u[i, 3] = parser.Evaluate(phi1, h[i]) - parser.Evaluate(phi1, 0) * (1 - h[i]) - parser.Evaluate(phi1, 1) * h[i];

                    //u[0, i] = parser.Evaluate(ro0, tau[i]) - ro0_0 * (1 - tau[i]) - ro0_1 * tau[i];//parser.Evaluate("t^4 - t", varx);//
                    //u[3, i] = parser.Evaluate(ro1, tau[i]) - ro1_0 * (1 - tau[i]) - ro1_1 * tau[i];//parser.Evaluate("t^4 - 6 * t^2 + 5 * t", varx);//
                }

                a = 1; //?
                b = 1; //?

                teta = (b * TAU * TAU) / (a * H * H); //+
                //вспомогательные переменные
                double sqr_teta = teta * teta;
                v0 = new Dictionary<int, double>();//new double[N];
                v1 = new Dictionary<int, double>();//new double[N];
                z0 = new Dictionary<int, double>();//new double[N + 1];
                z1 = new Dictionary<int, double>();//new double[N + 1];
                omega0 = new Dictionary<int, double>(); //new double[N + 1];
                omega1 = new Dictionary<int, double>(); //new double[N + 1];
                X0 = new Dictionary<int, double>(); //new double[N + 1];
                X1 = new Dictionary<int, double>(); //new double[N + 1];
                Y0 = new Dictionary<int, double>(); //new double[N + 1];
                Y1 = new Dictionary<int, double>(); //new double[N + 1];


                for (int k = 1; k <= N; k++) //+
                {
                    z0[k] = (1.0 / (2 * teta)) * (u[0, 3 * k - 3] - u[0, 3 * k - 2] - u[0, 3 * k - 1] + u[0, 3 * k]
                                                + u[3, 3 * k - 3] - u[3, 3 * k - 2] - u[3, 3 * k - 1] + u[3, 3 * k]); //+

                    omega0[k] = (3.0 / (2 * teta)) * (u[0, 3 * k - 3] - 3 * u[0, 3 * k - 2] + 3 * u[0, 3 * k - 1] - u[0, 3 * k]
                                                    + u[3, 3 * k - 3] - 3 * u[3, 3 * k - 2] + 3 * u[3, 3 * k - 1] - u[3, 3 * k]); //+

                    z1[k] = (1.0 / (6 * teta)) * (u[0, 3 * k - 3] - u[0, 3 * k - 2] - u[0, 3 * k - 1] + u[0, 3 * k]
                                                - u[3, 3 * k - 3] + u[3, 3 * k - 2] + u[3, 3 * k - 1] - u[3, 3 * k]); //+

                    omega1[k] = (1.0 / (2 * teta)) * (u[0, 3 * k - 3] - 3 * u[0, 3 * k - 2] + 3 * u[0, 3 * k - 1] - u[0, 3 * k]
                                                    - u[3, 3 * k - 3] + 3 * u[3, 3 * k - 2] - 3 * u[3, 3 * k - 1] + u[3, 3 * k]); //+
                }

                alpha0 = 0.5 * (1 + sqr_teta) / (3 + 5 * teta + 3 * sqr_teta); //+
                beta0 = (35 + 3 * sqr_teta) / (30 * (21 + 7 * teta + sqr_teta)); //+
                Y = (alpha0 + beta0) / (alpha0 - beta0); //+
                for (int k = 1; k <= n; k++) //+
                    v0[k] = (1 + Y) * (z0[k] + z0[k + 1]) + (1 - Y) * (omega0[k] - omega0[k + 1]); //+

                alpha1 = (9 + 105 * sqr_teta) / (10 + 70 * teta + 210 * sqr_teta); //+
                beta1 = (3 + 3 * sqr_teta) / (10 + 14 * teta + 10 * sqr_teta); //+
                X = (alpha1 + beta1) / (alpha1 - beta1); //+
                for (int k = 1; k <= n; k++) //+
                    v1[k] = (1 + X) * (z1[k] + z1[k + 1]) + (1 - X) * (omega1[k] - omega1[k + 1]); //+

                gamma0 = (10 * teta + 10 * sqr_teta) / (9 + 15 * teta + 9 * sqr_teta); //+
                delta0 = (70 * teta + 14 * sqr_teta) / (189 + 63 * teta + 9 * sqr_teta); //+

                gamma1 = (14 * teta + 70 * sqr_teta) / (3 + 21 * teta + 63 * sqr_teta); //+
                delta1 = (70 * teta + 70 * sqr_teta) / (45 + 63 * teta + 45 * sqr_teta); //+

                progon(ref x, 1.0, 2 * Y, 1.0, v0);
                progon(ref y, 1.0, 2 * X, 1.0, v1);

                for (int k = 1; k <= N; k++)
                {
                    X0[k] = gamma0 * (x[3 * k - 3] + x[3 * k] + 2 * z0[k]);
                    Y0[k] = delta0 * (x[3 * k - 3] - x[3 * k] + 2 * omega0[k]);
                    X1[k] = gamma1 * (y[3 * k - 3] + y[3 * k] + 2 * z1[k]);
                    Y1[k] = delta1 * (y[3 * k - 3] - y[3 * k] + 2 * omega1[k]);
                }

                double prod = (9 * H) / (64 * Math.Pow(TAU, 3));
                for (int k = 1; k <= N; k++)
                {
                    Jstar += prod *
                                (
                                    4 * sqr_teta * Math.Pow((x[3 * k - 3] + x[3 * k] + 2 * z0[k]), 2)
                                       - 6 * teta * (1 + teta) * (x[3 * k - 3] + x[3 * k] + 2 * z0[k]) * X0[k]
                                           + 0.9 * (3 + 5 * teta + 3 * sqr_teta) * Math.Pow(X0[k], 2) //+

                                    +

                                    (4.0 / 3.0) * sqr_teta * Math.Pow((x[3 * k - 3] - x[3 * k] + 2 * omega0[k]), 2)
                                       - (6.0 / 5.0) * teta * (5 + teta) * (x[3 * k - 3] - x[3 * k] + 2 * omega0[k]) * X1[k]
                                           + (27.0 / 70.0) * (21 + 7 * teta + sqr_teta) * Math.Pow(Y0[k], 2) //+

                                    +

                                    12 * sqr_teta * Math.Pow((y[3 * k - 3] + y[3 * k] + 2 * z1[k]), 2)
                                       - (18.0 / 5.0) * teta * (1 + 5 * teta) * (y[3 * k - 3] + y[3 * k] + 2 * z1[k]) * Y0[k]
                                           + (27.0 / 70.0) * (1 + 7 * teta + 21 * sqr_teta) * Math.Pow(X1[k], 2) //+

                                    +

                                    4 * sqr_teta * Math.Pow((y[3 * k - 3] - y[3 * k] + 2 * omega1[k]), 2)
                                       - (18.0 / 5.0) * teta * (1 + teta) * (y[3 * k - 3] - y[3 * k] + 2 * omega1[k]) * Y1[k]
                                           + (81.0 / 350.0) * (5 + 7 * teta + 5 * sqr_teta) * Math.Pow(Y1[k], 2) //+
                                );
                    #region [логирование]
                    if (logging)
                    {
                        double var2 = 4 * sqr_teta * Math.Pow((x[3 * k - 3] + x[3 * k] + 2 * z0[k]), 2)
                                               - 6 * teta * (1 + teta) * (x[3 * k - 3] + x[3 * k] + 2 * z0[k]) * X0[k]
                                                   + 0.9 * (3 + 5 * teta + 3 * sqr_teta) * Math.Pow(X0[k], 2);
                        double var3 = (4.0 / 3.0) * sqr_teta * Math.Pow((x[3 * k - 3] - x[3 * k] + 2 * omega0[k]), 2)
                                               - (6.0 / 5.0) * teta * (5 + teta) * (x[3 * k - 3] - x[3 * k] + 2 * omega0[k]) * X1[k]
                                                   + (27.0 / 70.0) * (21 + 7 * teta + sqr_teta) * Math.Pow(Y0[k], 2);
                        double var4 = 12 * sqr_teta * Math.Pow((y[3 * k - 3] + y[3 * k] + 2 * z1[k]), 2)
                                           - (18.0 / 5.0) * teta * (1 + 5 * teta) * (y[3 * k - 3] + y[3 * k] + 2 * z1[k]) * Y0[k]
                                               + (27.0 / 70.0) * (1 + 7 * teta + 21 * sqr_teta) * Math.Pow(X1[k], 2);
                        double var5 = 4 * sqr_teta * Math.Pow((y[3 * k - 3] - y[3 * k] + 2 * omega1[k]), 2)
                                           - (18.0 / 5.0) * teta * (1 + teta) * (y[3 * k - 3] - y[3 * k] + 2 * omega1[k]) * Y1[k]
                                               + (81.0 / 350.0) * (5 + 7 * teta + 5 * sqr_teta) * Math.Pow(Y1[k], 2);

                        log.writeLine(string.Format("prod = {0}", prod));
                        Console.ForegroundColor = ConsoleColor.White;
                        log.writeLine(string.Format("1 = {0}", var2));
                        Console.ForegroundColor = ConsoleColor.Green;
                        log.writeLine(string.Format("2 = {0}", var3));
                        Console.ForegroundColor = ConsoleColor.Blue;
                        log.writeLine(string.Format("3 = {0}", var4));
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        log.writeLine(string.Format("4 = {0}", var5));
                        Console.ResetColor();
                        log.writeLine(string.Format("sum 1-4 = {0}", var2 + var3 + var4 + var5));
                        Console.ForegroundColor = ConsoleColor.Red;
                        log.writeLine(string.Format("Jstar[{0}] = {1}", k, Jstar));
                        Console.ResetColor();
                    }
                    #endregion
                }
                Jstar *= a * a;
                #region [логирование]
                if (logging)
                {
                    log.printMatrix(u, string.Format("u[{0},{1}]: ", u.GetLength(0), u.GetLength(1)), ConsoleColor.White);
                    log.writeLine(string.Format("a = {0}", a));
                    log.writeLine(string.Format("b = {0}", b));
                    log.writeLine(string.Format("tau = {0}", TAU));
                    log.printArray(tau, string.Format("tau[]: ", tau.Length), ConsoleColor.White);
                    log.writeLine(string.Format("h = {0}", H));
                    log.printArray(h, string.Format("h[]: ", h.Length), ConsoleColor.White);
                    log.writeLine(string.Format("teta = {0}", teta));
                    log.printArray(z0, string.Format("z0[]: ", z0.Count), ConsoleColor.White);
                    log.printArray(z1, string.Format("z1[]: ", z1.Count), ConsoleColor.White);
                    /*
                    for (int k = 1; k <= N; k++)
                    {
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k - 3, u[0, 3 * k - 3]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k - 2, u[0, 3 * k - 2]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k - 1, u[0, 3 * k - 1]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 0, 3 * k, u[0, 3 * k]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k - 3, u[3, 3 * k - 3]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k - 2, u[3, 3 * k - 2]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k - 1, u[3, 3 * k - 1]);
                        Console.WriteLine("u[{0}, {1}] =  {2}", 3, 3 * k, u[3, 3 * k]);

                        Console.WriteLine("1.0 / (6 * teta) = {0}", 1.0 / (6 * teta));

                        Console.WriteLine("{0:F16}\n- {1:F16}\n- {2:F16}\n+ {3:F16}\n- {4:F16}\n+ {5:F16}\n+ {6:F16}\n- {7:F16}\n=\n{8:F16}",
                            u[0, 3 * k - 3],
                            u[0, 3 * k - 2],
                            u[0, 3 * k - 1],
                            u[0, 3 * k],
                            u[3, 3 * k - 3],
                            u[3, 3 * k - 2],
                            u[3, 3 * k - 1],
                            u[3, 3 * k],
                            u[0, 3 * k - 3] - u[0, 3 * k - 2] - u[0, 3 * k - 1] + u[0, 3 * k]
                            - u[3, 3 * k - 3] + u[3, 3 * k - 2] + u[3, 3 * k - 1] - u[3, 3 * k]);

                        Console.WriteLine("z1[k] = {0}", z1[k]);
                    }
                    */
                    log.printArray(omega0, string.Format("omega0[]: ", omega0.Count), ConsoleColor.White);
                    log.printArray(omega1, string.Format("omega1[]: ", omega1.Count), ConsoleColor.White);
                    log.writeLine(string.Format("alpha0 = {0}", alpha0));
                    log.writeLine(string.Format("beta0 = {0}", beta0));
                    log.writeLine(string.Format("y = {0}", Y));
                    log.printArray(v0, string.Format("v0[]: ", v0.Count), ConsoleColor.White);
                    log.writeLine(string.Format("alpha1 = {0}", alpha1));
                    log.writeLine(string.Format("beta1 = {0}", beta1));
                    log.writeLine(string.Format("x = {0}", X));
                    log.printArray(v1, string.Format("v1[]: ", v1.Count), ConsoleColor.White);
                    log.writeLine(string.Format("gamma0 = {0}", gamma0));
                    log.writeLine(string.Format("delta0 = {0}", delta0));
                    log.writeLine(string.Format("gamma1 = {0}", gamma1));
                    log.writeLine(string.Format("delta1 = {0}", delta1));
                    log.printArray(x, string.Format("x[]: ", x.Count), ConsoleColor.White);
                    progonCheck("Проверяем массив x:", ref x, Y, v0);
                    log.printArray(y, string.Format("y[]: ", y.Count), ConsoleColor.White);
                    progonCheck("Проверяем массив y:", ref y, X, v1);
                    log.printArray(X0, string.Format("X0[]: ", X0.Count), ConsoleColor.White);
                    log.printArray(Y0, string.Format("Y0[]: ", Y0.Count), ConsoleColor.White);
                    log.printArray(X1, string.Format("X1[]: ", X1.Count), ConsoleColor.White);
                    log.printArray(Y1, string.Format("Y1[]: ", Y1.Count), ConsoleColor.White);
                    //X0X1Y0Y1Check("Проверяем массивы X0, Y0, X1, Y1:", ref X0, ref X1, ref Y0, ref Y1, ref x, ref y);
                }
                log.writeLine(string.Format("Jstar = {0}", Jstar));
                screenLog.Add(string.Format("N = {1} Jstar = {0}", Jstar, N));
                #endregion
            }
            catch (Exception e)
            {
                log.viewError(e);
            }
            finally
            {
                watch.Stop();
                Console.ForegroundColor = ConsoleColor.Yellow;
                log.writeLine(string.Format("Затраченное время: {0}", watch.Elapsed.ToString(@"hh\:mm\:ss\.FFFFFFF")));
                Console.ResetColor();
                watch.Reset();
                log.sw_log.Close();
            }
        }

        #endregion
        
        #region[прогонка]
        /// <summary>
        /// Метод прогонки
        /// </summary>
        /// <param name="x">неизвестные</param>
        /// <param name="b">коэффициент при i - 1</param>
        /// <param name="c">коэффициент при i</param>
        /// <param name="d">коэффициент при i + 1</param>
        /// <param name="r">свободные коэффициенты</param>
        void progon(ref double[] x, double b, double c, double d, double[] r)
        {
            try
            {
                double[] alpha = new double[N];
                double[] beta = new double[N];
                alpha[1] = - d / c;
                beta[1] = - r[0] / c;
                for (int k = 2; k <= n -1; k++)
                {
                    alpha[k] = - 1 / (alpha[k - 1] + c);
                    beta[k] = - (r[k - 1] + beta[k - 1]) / (alpha[k - 1] + c);
                }
                x[3 * n] = - (beta[n-1] + r[n-1]) / (alpha[n-1] - c);
                for (int k = n-1; k >= 2; k--)
                    x[3 * k - 3] = alpha[k] * x[3 * k] + beta[k];
                
            }
            catch (Exception e)
            {
                log.viewError(e);
            }
        }

        /// <summary>
        /// Метод прогонки
        /// </summary>
        /// <param name="x">неизвестные</param>
        /// <param name="b">коэффициент при i - 1</param>
        /// <param name="c">коэффициент при i</param>
        /// <param name="d">коэффициент при i + 1</param>
        /// <param name="r">свободные коэффициенты</param>
        void progon(ref Dictionary<int, double> x, double b, double c, double d, Dictionary<int, double> r)
        {
            try
            {
                Dictionary<int, double> alpha = new Dictionary<int, double>();
                Dictionary<int, double> beta = new Dictionary<int, double>();
                x[3 * N] = 0;
                alpha[2] = -d / c;
                beta[2] = -r[1] / c;
                for (int k = 2; k <= n - 1; k++)
                {
                    alpha[k + 1] = -1 / (alpha[k] + c);
                    beta[k + 1] = -(r[k] + beta[k]) / (alpha[k] + c);
                }
                x[3 * n] = -(beta[n] + r[n]) / (alpha[n] - c);
                for (int k = n; k >= 2; k--)
                    x[3 * k - 3] = alpha[k] * x[3 * k] + beta[k];
                x[0] = 0;

            }
            catch (Exception e)
            {
                log.viewError(e);
            }
        }

        /// <summary>
        /// Проверка результатов прогонки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="v0"></param>
        void progonCheck(string caption, ref double[] x, double y, double[] v0)
        {
            log.writeLine(caption);
            for (int k = 0; k <= n; k++)
            {
                if (3 * k - 3 >= 0)
                    log.writeLine(string.Format("{0:F5} + 2 * {1:F5} * {2:F5} + {3:F5} = {4:F5} ({5:F5})", x[3 * k - 3], y, x[3 * k], x[3 * k + 3], x[3 * k - 3] + 2 * y * x[3 * k] + x[3 * k + 3], v0[k]));
                else
                    log.writeLine(string.Format("{0:F5} + 2 * {1:F5} * {2:F5} + {3:F5} = {4:F5} ({5:F5})", 0, y, x[3 * k], x[3 * k + 3], 2 * y * x[3 * k] + x[3 * k + 3], v0[k]));
            }
        }

        /// <summary>
        /// Проверка результатов прогонки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="v0"></param>
        void progonCheck(string caption, ref Dictionary<int, double> x, double y, Dictionary<int, double> v0)
        {
            log.writeLine(caption);
            for (int k = 1; k <= n; k++)
            {
                log.writeLine(string.Format("{6} -> {0:F5} + 2 * {1:F5} * {2:F5} + {3:F5} = {4:F5} ({5:F5}) <- {6}",
                    x[3 * k - 3], y, x[3 * k], x[3 * k + 3], x[3 * k - 3] + 2 * y * x[3 * k] + x[3 * k + 3], - v0[k], k)); //fix me на предпоследнем месте должен быть v0 и там в последнем элементе несоотвествие
            }
        }
        #endregion

        void X0X1Y0Y1Check(string caption, ref Dictionary<int, double> X0, ref Dictionary<int, double> X1, ref Dictionary<int, double> Y0, ref Dictionary<int, double> Y1,
            ref Dictionary<int, double> x, ref Dictionary<int, double> y)
        {
            log.writeLine(caption);

            Dictionary<int, double> thisX0 = new Dictionary<int, double>();
            Dictionary<int, double> thisY0 = new Dictionary<int, double>();
            Dictionary<int, double> thisX1 = new Dictionary<int, double>();
            Dictionary<int, double> thisY1 = new Dictionary<int, double>();
            log.writeLine("Пример формы вывода: k -> thisX0[k]  = x[3 * k - 3] - x[3 * k - 2] - x[3 * k - 1] + x[3 * k] = thisX0[k]  (X0[k]) <- k");
            for (int k = 1; k <= N; k++)
            {
                x[3 * k - 2] = 0;
                x[3 * k - 1] = 0;
                y[3 * k - 2] = 0;
                y[3 * k - 1] = 0;
                thisX0[k] = x[3 * k - 3] - x[3 * k - 2] - x[3 * k - 1] + x[3 * k];
                log.writeLine(string.Format("{0} -> X0[{0}] = {1} - {2} - {3} + {4} = {5} ({6}) <- {0}", k, x[3 * k - 3], x[3 * k - 2], x[3 * k - 1], x[3 * k], thisX0[k], X0[k]));

                thisY0[k] = x[3 * k - 3] - 3 * x[3 * k - 2] + 3 * x[3 * k - 1] - x[3 * k];
                log.writeLine(string.Format("{0} -> Y0[{0}] = {1} - {2} + {3} - {4} = {5} ({6}) <- {0}", k, x[3 * k - 3], x[3 * k - 2], x[3 * k - 1], x[3 * k], thisY0[k], Y0[k]));

                thisX1[k] = y[3 * k - 3] - y[3 * k - 2] - y[3 * k - 1] + y[3 * k];
                log.writeLine(string.Format("{0} -> X1[{0}] = {1} - {2} - {3} + {4} = {5} ({6}) <- {0}", k, y[3 * k - 3], y[3 * k - 2], y[3 * k - 1], y[3 * k], thisX1[k], X1[k]));

                thisY1[k] = y[3 * k - 3] - 3 * y[3 * k - 2] + 3 * y[3 * k - 1] - y[3 * k];
                log.writeLine(string.Format("{0} -> Y1[{0}] = {1} - {2} + {3} - {4} = {5} ({6}) <- {0}", k, y[3 * k - 3], y[3 * k - 2], y[3 * k - 1], y[3 * k], thisY1[k], Y1[k]));
            }
        }

        #region[логирует данные]
        ///// <summary>
        ///// Экранирует ошибки
        ///// </summary>
        //static void viewError(Exception e)
        //{
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.WriteLine("ОШИБКА:");
        //    Console.WriteLine("Сообщение: " + e.Message);
        //    Console.WriteLine("Место: " + e.StackTrace);

        //    DateTime now = DateTime.Now;
        //    string path = string.Format("logs/errors at {0}h {1}m {2}s {3}ms {4}-{5}-{6}.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString());
        //    sw_err = new StreamWriter(path, false, Encoding.UTF8, 1000);
        //    sw_err.WriteLine(e.Message);
        //    sw_err.WriteLine(e.StackTrace);
        //    sw_err.Close();

        //    Console.ReadKey();
        //}

        ///// <summary>
        ///// Выводит в консоль и записывает в лог
        ///// </summary>
        ///// <param name="s"></param>
        //static void writeLine(string s)
        //{
        //    Console.WriteLine(s);
        //    sw_log.WriteLine(s);
        //}

        ///// <summary>
        ///// Экранирует матрицу в консоль
        ///// </summary>
        ///// <param name="u"></param>
        //static void printMatrix(double[,] matr, string caption, ConsoleColor color)
        //{
        //    Console.ForegroundColor = color;
        //    writeLine(caption);
        //    for (int j = 0; j < matr.GetLength(1); j++)
        //    {
        //        writeLine(string.Format("{4} -> {0:F10}\t{1:F10}\t{2:F10}\t{3:F10} <- {4}", matr[0, j], matr[1, j], matr[2, j], matr[3, j], j));
        //    }
        //    Console.ResetColor();
        //}

        ///// <summary>
        ///// Экранирует массив в консоль
        ///// </summary>
        ///// <param name="arr"></param>
        ///// <param name="caption"></param>
        //static void printArray(double[] arr, string caption, ConsoleColor color)
        //{
        //    Console.ForegroundColor = color;
        //    writeLine("");
        //    writeLine(caption);
        //    for (int i = 0; i < arr.GetLength(0); i++)
        //    {
        //        writeLine(string.Format("{1} -> {0:F16} <- {1}", arr[i], i));
        //    }
        //    Console.ResetColor();
        //}

        ///// <summary>
        ///// Экранирует массив в консоль
        ///// </summary>
        ///// <param name="arr"></param>
        ///// <param name="caption"></param>
        //static void printArray(Dictionary<int, double> arr, string caption, ConsoleColor color)
        //{
        //    Console.ForegroundColor = color;
        //    writeLine("");
        //    writeLine(caption);
        //    foreach(int i in arr.Keys)
        //    {
        //        writeLine(string.Format("{1} -> {0:F16} <- {1}", arr[i], i));
        //    }
        //    Console.ResetColor();
        //}
        #endregion
    }
}