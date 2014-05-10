using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveTask
{
    class WaveLogic
    {
        //N, λ, μ, γ, τ, ε, ω, ξ, ϕ, ρ, σ, θ, α, β, δ, Δ
        #region[переменные]
        /// <summary>
        /// Логирование
        /// </summary>
        FQW.Logging log;
        /// <summary>
        /// Log сохранения, вызванного пользователем
        /// </summary>
        public List<string> screenLog = new List<string>();
        /// <summary>
        /// Включить логирование
        /// </summary>
        bool logging = false;
        /// <summary>
        /// Минимум функционала
        /// </summary>
        public double Jstar = 0;
        /// <summary>
        /// Количество узлов
        /// </summary>
        int N = 0;
        /// <summary>
        /// γ
        /// </summary>
        double gamma = 0;
        /// <summary>
        /// δ
        /// </summary>
        double delta = 0;
        /// <summary>
        /// λ
        /// </summary>
        double lambda = 0;
        /// <summary>
        /// μ
        /// </summary>
        double mu = 0;
        /// <summary>
        /// ν
        /// </summary>
        double nu;
        /// <summary>
        /// υ ипсилон больше похоже на то, что в статье
        /// </summary>
        double eta;
        /// <summary>
        /// τ
        /// </summary>
        double tau;
        double[] tauArray;
        /// <summary>
        /// ε
        /// </summary>
        double epsilon = 0;
        /// <summary>
        /// ω
        /// </summary>
        double omega = - 10;
        /// <summary>
        /// ξ
        /// </summary>
        double xi = 0;
        /// <summary>
        /// ρ0
        /// </summary>
        string ro0 = "";
        /// <summary>
        /// (ρ0(0))'
        /// </summary>
        string ro0_0_D = "";
        /// <summary>
        /// (ρ0(0))''
        /// </summary>
        string ro0_0_DD = "";
        /// <summary>
        /// ρ1
        /// </summary>
        string ro1 = "";
        /// <summary>
        /// (ρ1(0))'
        /// </summary>
        string ro1_0_D = "";
        /// <summary>
        /// (ρ1(0))''
        /// </summary>
        string ro1_0_DD = "";
        /// <summary>
        /// ψ
        /// </summary>
        string psi = "";
        /// <summary>
        /// ϕ
        /// </summary>
        string phi = "";
        /// <summary>
        /// ψ
        /// </summary>
        double[] psiArray;
        /// <summary>
        /// ϕ
        /// </summary>
        double[] phiArray;
        /// <summary>
        /// θ
        /// </summary>
        double teta = 0;
        /// <summary>
        /// α
        /// </summary>
        double alpha = 0;
        /// <summary>
        /// β
        /// </summary>
        double beta = 0;
        /// <summary>
        /// h
        /// </summary>
        double h;
        double[] hArray;
        
        /// <summary>
        /// трехдиагональная матрица u
        /// </summary>
        double[,] u;

        int n = 0;
        double a = 0;
        double b = 0;
        double X = 0;
        double Y = 0;
        double[,] A;
        /// <summary>
        /// Симметрическая матрица В
        /// </summary>
        double[,] B;
        double[] Cheb;
        Dictionary<int, double> x;
        Dictionary<int, double> y;
        Dictionary<int, double> Xk;
        Dictionary<int, double> Yk;
        Dictionary<int, double> v;
        Dictionary<int, double> w;
        Dictionary<int, double> z;
        Dictionary<int, double> Z;
        Dictionary<int, double> muArray;
        Dictionary<int, double> nuArray;
        Dictionary<int, double> etaArray;

        FQW.Parser parser = new FQW.Parser();
        #endregion

        public WaveLogic()
        {
            log = new FQW.Logging();
        }

        public WaveLogic(int N, bool logging)
        {
            log = new FQW.Logging();
            setStartParameters(N, logging, 2);
        }
        /// <summary>
        /// Задает начальные параметры
        /// </summary>
        /// <param name="N">Количество узлов</param>
        /// <param name="logging">Требуется ли логирование</param>
        /// <param name="numberOfTask">Номер задачи</param>
        public void setStartParameters(int N, bool logging, int numberOfTask)
        {
            try
            {
                this.N = N;
                this.logging = logging;
                //do {
                //    lambda = log.readDouble("Введите lambda:");
                //}
                //while (lambda > 1 || lambda < 0);
                //do { 
                //    gamma = log.readDouble("Введите gamma:");
                //} 
                //while (gamma == 0);
                //do { 
                //    tau = log.readDouble("Введите tau:");
                //} 
                //while (tau <= 0);
                if (numberOfTask == 1)
                {
                    #region[задача 1]
                    lambda = 0.9;
                    gamma = 1;
                    tau = 0.1;

                    #region[вычисление переменных]
                    a = 1;
                    b = 1;
                    ro0 = string.Format("(sqrt({0}) * t)^3 + (sqrt({0}) * t)^4", b);
                    ro1 = string.Format("(sqrt({0}) * t + sqrt({1}))^3 + (sqrt({0}) * t - sqrt({1}))^4", b, a);
                    phi = string.Format("(sqrt({0}) * ξ)^3 + (sqrt({0}) * ξ)^4", a);
                    psi = string.Format("0");
                    u = new double[3, 2 * N + 1];
                    tauArray = new double[3];
                    hArray = new double[2 * N + 1];
                    phiArray = new double[2 * N + 1];
                    psiArray = new double[2 * N + 1];
                    x = new Dictionary<int, double>();
                    y = new Dictionary<int, double>();
                    z = new Dictionary<int, double>();
                    Z = new Dictionary<int, double>();
                    muArray = new Dictionary<int, double>();
                    nuArray = new Dictionary<int, double>();
                    v = new Dictionary<int, double>();
                    Xk = new Dictionary<int, double>();
                    mu = 1 - lambda;
                    omega = lambda - mu;
                    n = N - 1;
                    h = 1.0 / (2 * N);
                    teta = gamma * gamma * tau * tau / (h * h);
                    nu = h / (tau * tau * tau);

                    for (int i = 0; i <= 2; i++)
                        tauArray[i] = i * tau;
                    for (int j = 0; j <= 2 * N; j++)
                        hArray[j] = j * h;

                    for (int k = 0; k < psiArray.Length; k++)
                        psiArray[k] = parser.Evaluate(psi, hArray[k]);

                    #endregion
                    #region[вычисление массивов]

                    for (int i = 0; i < u.GetLength(0); i++)
                    {
                        u[i, 0] = parser.Evaluate(ro0, i * tau);
                        u[i, 2 * N] = parser.Evaluate(ro1, i * tau);
                    }
                    for (int j = 0; j < u.GetLength(1); j++)
                    {
                        u[0, j] = parser.Evaluate(phi, j * h);
                    }
                    for (int k = 1; k <= N; k++)
                    {
                        Z.Add(k, u[0, 2 * k - 2] - 2 * u[0, 2 * k - 1] + u[0, 2 * k]);
                    }
                    for (int k = 1; k <= N - 1; k++)
                        v.Add(k, -mu * mu * teta * Z[k] - lambda * lambda * teta * Z[k + 1]);
                    double l2m2 = lambda * lambda * mu * mu;
                    progon(ref y, l2m2, -(lambda * lambda + mu * mu + 2 * l2m2), l2m2, v);//4.4
                    for (int k = 1; k <= N - 1; k++)//4.3
                        x.Add(2 * k, 2 * tau * psiArray[2 * k] + 2 * y[2 * k]);
                    double oneMinusLambdaMuTeta = (1 - lambda * mu) * teta;
                    x.Add(0, u[2, 0] - u[0, 0]);
                    x.Add(2 * N, u[2, 2 * N] - u[0, 2 * N]);
                    for (int k = 1; k <= N; k++)//4.2
                        x.Add(2 * k - 1, (
                                            (teta * Z[k] - mu * y[2 * k - 2] - lambda * y[2 * k]) / (2 * oneMinusLambdaMuTeta)
                                            + 0.5 * (x[2 * k - 2] + x[2 * k])
                                         )
                             );
                    for (int k = 1; k <= N; k++)//4.1
                        y.Add(2 * k - 1, 2 * tau * psiArray[2 * k - 1] - x[2 * k - 1] + lambda * y[2 * k - 2] + mu * y[2 * k]);

                    for (int j = 1; j <= 2 * N - 1; j++)
                    {
                        u[1, j] = psiArray[j] + 0.5 * (x[j] - y[j]);
                        u[2, j] = psiArray[j] + x[j];
                    }

                    for (int k = 1; k <= N; k++)
                        Xk.Add(k, x[2 * k - 2] - 2 * x[2 * k - 1] + x[2 * k]);
                    //Xk.Add(k, (mu * y[2 * k - 2] + lambda * y[2 * k] - teta * Z[k]) / (teta * (1 - lambda * mu)));
                    #endregion
                    #region[логирование]
                    if (logging)
                    {
                        log.writeLine(string.Format("lambda = {0}", lambda));
                        log.writeLine(string.Format("mu = {0}", mu));
                        log.writeLine(string.Format("omega = {0}", omega));
                        log.writeLine(string.Format("gamma = {0}", gamma));
                        log.writeLine(string.Format("tau = {0}", tau));
                        log.writeLine(string.Format("ro0 = {0}", ro0));
                        log.writeLine(string.Format("ro1 = {0}", ro1));
                        log.writeLine(string.Format("phi = {0}", phi));
                        log.writeLine(string.Format("psi = {0}", psi));

                        log.writeLine(string.Format("h = {0}", h));
                        log.writeLine(string.Format("teta = {0}", teta));
                        log.printArray(psiArray, string.Format("psiArray[{0}]: ", psiArray.Length), ConsoleColor.White);
                        log.printArray(phiArray, string.Format("phiArray[{0}]: ", phiArray.Length), ConsoleColor.White);
                        log.printArray(Z, string.Format("Z[{0}]: ", Z.Count), ConsoleColor.White);
                        log.printArray(v, string.Format("v[{0}]: ", v.Count), ConsoleColor.White);
                        log.printArray(x, string.Format("x[{0}]: ", x.Count), ConsoleColor.White);
                        log.printArray(y, string.Format("y[{0}]: ", y.Count), ConsoleColor.White);
                        progonCheck("Проверяем массив y:", ref y, X, v);
                        log.printMatrix(u, 2, string.Format("u[{0},{1}]: ", u.GetLength(0), u.GetLength(1)), ConsoleColor.White);
                    }
                    #endregion
                    #endregion
                }
                else if (numberOfTask == 2)
                {
                    #region[задача 2]
                    lambda = 0.9;
                    gamma = 1;
                    tau = 0.1;

                    #region[вычисление переменных]
                    a = 1;
                    b = 1;
                    ro0 = string.Format("(sqrt({0}) * t)^3 + (sqrt({0}) * t)^4", b);
                    ro0_0_D = string.Format("0");
                    ro0_0_DD = string.Format("0");
                    ro1 = string.Format("(sqrt({0}) * t + sqrt({1}))^3 + (sqrt({0}) * t - sqrt({1}))^4", b, a);
                    ro1_0_D = string.Format("0");
                    ro1_0_DD = string.Format("0");
                    phi = string.Format("(sqrt({0}) * ξ)^3 + (sqrt({0}) * ξ)^4", a);
                    psi = string.Format("0");

                    tauArray = new double[3];
                    u = new double[3, 2 * N + 1];
                    hArray = new double[2 * N + 1];
                    phiArray = new double[2 * N + 1];
                    psiArray = new double[2 * N + 1];
                    v = new Dictionary<int, double>();
                    w = new Dictionary<int, double>();
                    x = new Dictionary<int, double>();
                    y = new Dictionary<int, double>();
                    z = new Dictionary<int, double>();
                    Z = new Dictionary<int, double>();
                    Xk = new Dictionary<int, double>();
                    Yk = new Dictionary<int, double>();
                    muArray = new Dictionary<int, double>();
                    nuArray = new Dictionary<int, double>();
                    mu = 1 - lambda;
                    omega = lambda - mu;
                    n = N - 1;
                    h = 1.0 / (2 * N);
                    teta = gamma * gamma * tau * tau / (h * h);
                    nu = h / (tau * tau * tau);

                    for (int i = 0; i <= 2; i++)
                        tauArray[i] = i * tau;
                    for (int j = 0; j <= 2 * N; j++)
                        hArray[j] = j * h;

                    for (int k = 0; k < psiArray.Length; k++)
                        psiArray[k] = parser.Evaluate(psi, hArray[k]);

                    #endregion
                    #region[вычисление массивов]
                    progon(ref x, 1, 2 * Y, 1, v);
                    x.Add(2 * N, (v[N] - x[2 * n]) / Y);
                    for (int k = 2; k <= N; k++)
                        Xk.Add(k, gamma * (2 * z[k] - x[2 * k - 2] - x[2 * k]));

                    progon(ref y, 1, 2 * X, 1, etaArray);
                    y.Add(2 * N, (etaArray[N] - y[2 * n]) / X);
                    for (int k = 2; k <= N; k++)
                        Yk.Add(k, delta * (2 * w[k] - y[2 * k - 2] - y[2 * k]));
                    #endregion
                    #region[логирование]

                    #endregion
                    #endregion
                }
                logic(numberOfTask);
            }
            catch (Exception e) { log.viewError(e); }
        }


        double ro_0(double t)
        {
            return parser.Evaluate(ro0, t) - parser.Evaluate(ro0, 0);
        }

        double ro_1(double t)
        {
            return parser.Evaluate(ro1, t) - parser.Evaluate(ro1, 0);
        }

        double phi_(double xi)
        {
            return - (a / (6 * b)) * xi * (1 - xi) * (parser.Evaluate(ro0_0_DD, 0) * (2 - xi) + parser.Evaluate(ro1_0_DD, 0) * (1 + xi));
        }

        double psi_(double xi)
        {
            return parser.Evaluate(ro0_0_D, 0) * (1 - xi) + parser.Evaluate(ro1_0_D, xi) * xi;
        }

        #region[прогонка]
        /// <summary>
        /// Метод прогонки
        /// </summary>
        /// <param name="x">неизвестные</param>
        /// <param name="b">коэффициент при i - 1</param>
        /// <param name="c">коэффициент при i</param>
        /// <param name="d">коэффициент при i + 1</param>
        /// <param name="r">свободные коэффициенты</param>
        void progon(ref Dictionary<int, double> y, double b, double c, double d, Dictionary<int, double> r)
        {
            try
            {
                Dictionary<int, double> alpha = new Dictionary<int, double>();
                Dictionary<int, double> beta = new Dictionary<int, double>();
                y[2 * N] = 0;
                alpha[2] = -d / c;
                beta[2] = -r[1] / c;
                for (int k = 2; k <= n - 1; k++)
                {
                    alpha[k + 1] = -1 / (alpha[k] + c);
                    beta[k + 1] = -(r[k] + beta[k]) / (alpha[k] + c);
                }
                y[2 * n] = -(beta[n] + r[n]) / (alpha[n] - c);
                for (int k = n; k >= 2; k--)
                    y[2 * k - 2] = alpha[k] * y[2 * k] + beta[k];
                y[0] = 0;

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
        void progonCheck(string caption, ref Dictionary<int, double> x, double y, Dictionary<int, double> v0)
        {
            log.writeLine(caption);
            for (int k = 1; k <= n; k++)
            {
                log.writeLine(string.Format("{6} -> {0:F5} + 2 * {1:F5} * {2:F5} + {3:F5} = {4:F5} ({5:F5}) <- {6}",
                    x[2 * k - 2], y, x[2 * k], x[2 * k + 2], x[2 * k - 2] + 2 * y * x[2 * k] + x[2 * k + 2], - v0[k], k)); 
            }
        }
        #endregion

        /// <summary>
        /// Вычисление функционала
        /// </summary>
        /// <param name="numberOfTask">Номер задачи</param>
        void logic(int numberOfTask)
        {
            if (numberOfTask == 1)
            {
                Jstar = 0;
                for (int m = 1; m <= N; m++)
                {
                    Jstar = Math.Pow(lambda * y[2 * m - 2] + mu * y[2 * m] - teta * Z[m], 2)
                        + Math.Pow(mu * y[2 * m - 2] + lambda * y[2 * m] - teta * Xk[m] - teta * Z[m], 2)
                        + Math.Pow(y[2 * m] - lambda * teta * Xk[m] - teta * Z[m], 2)
                        + Math.Pow(y[2 * m - 2] - mu * teta * Xk[m] - teta * Z[m], 2); // 3.1
                }
                Jstar *= nu;
                log.writeLine(string.Format("3.1: N = {1} Jstar = {0}", Jstar, N));
                Jstar = 0; // ниже где-то ошибка
                Jstar = lambda * lambda * (1 + mu * mu) * y[0] * y[0] + mu * mu * (1 + lambda * lambda) * y[2 * N] * y[2 * N]
                    - lambda * lambda * mu * mu * (y[0] * y[2] + y[2 * N - 2] * y[2 * N])
                    - 2 * lambda * lambda * y[0] * Z[1]
                    - 2 * mu * mu * y[2 * N] * teta * Z[N];
                for (int k = 1; k <= N - 1; k++)
                    Jstar -= y[2 * k] * (mu * mu * teta * Z[k] + lambda * lambda * teta * Z[k + 1]);
                for (int k = 1; k <= N - 1; k++)
                    Jstar += (lambda * lambda + mu * mu) * teta * teta * Z[k] * Z[k];
                Jstar *= (2 * nu) / (1 - lambda * mu);//6.1

                log.writeLine(string.Format("6.1: N = {1} Jstar = {0}", Jstar, N));
                screenLog.Add(string.Format("N = {1} Jstar = {0}", Jstar, N));
            }
            else if (numberOfTask == 2)
            { }
        }
    }
}
#region[точное решение одной задачи оптимизации, порожденной простейшим волновым уравнением Родионова Н.В. Не используем, потому что многочлены Чебышева]
/*
                u = new double[2, 2 * N + 1];
                phiArray = new double[2 * N + 1];
                psiArray = new double[2 * N + 1];
                x = new Dictionary<int, double>();
                y = new Dictionary<int, double>();
                z = new Dictionary<int, double>();
                Z = new Dictionary<int, double>();
                muArray = new Dictionary<int, double>();
                nuArray = new Dictionary<int, double>();
                v = new Dictionary<int, double>();
                lambda = (1 + omega) / 2;
                mu = (1 - omega) / 2;
                n = N - 1;
                h = 1.0 / (2 * N);
                teta = gamma * gamma * tau * tau / (h * h);
                nu = h / (tau * tau * tau);

                #endregion
                #region[вычисление массивов]

                for (int i = 0; i < u.GetLength(0); i++)
                {
                    u[i, 0] = parser.Evaluate(ro0, i * tau);
                    u[i, 2 * N] = parser.Evaluate(ro1, i * tau);
                }
                for (int j = 0; j < u.GetLength(1); j++)
                {
                    psiArray[j] = parser.Evaluate(psi, j * h);
                    phiArray[j] = parser.Evaluate(phi, j * h);
                }
                for (int k = 1; k <= N; k++)
                {
                    Z.Add(k, phiArray[2 * k - 2] - 2 * phiArray[2 * k - 1]  + phiArray[2 * k]);
                    z.Add(k, teta * Z[k]);
                    muArray.Add(k, - z[k] / (lambda * lambda));
                    nuArray.Add(k, -z[k] / (mu * mu));
                }
                for (int k = 1; k <= n; k++)
                    v.Add(k, muArray[k] + nuArray[k + 1]);
                X = -1 - 1.0 / (2 * lambda * lambda) - 1.0 / (2 * mu * mu);
                if (X > -5)
                    log.beautyWriteLine(string.Format("X вышел за ограничение: {0} > -5", X), ConsoleColor.Red);
                alpha = -1 - 1.0 / (lambda * lambda);
                beta = -1 - 1.0 / (mu * mu);
                if (alpha + beta != 2 * X)
                    log.beautyWriteLine(string.Format("{0} + {1} != 2 * X: {2} != {3}", log.greek["alpha"], log.greek["beta"], alpha + beta, 2 * X), ConsoleColor.Red);
                if (alpha * beta <= 1)
                    log.beautyWriteLine(string.Format("{0} * {1} <= 1: {2} <= 1", log.greek["alpha"], log.greek["beta"], alpha * beta), ConsoleColor.Red);
                
                progon(ref y, 1, 2 * X, 1, v); //(7)
                for (int k = 1; k <= n; k++)
                    x.Add(2 * k, 2 * tau * psiArray[2 * k] + 2 * y[2 * k]); //(6)

                x.Add(0, 0);// начальное значение откуда взыть?
                x.Add(2 * N, 0); // конечное значение откуда взять?

                double oneMinusLambdaMu = 1 - lambda * mu;
                for (int k = 1; k <= N; k++)
                    x.Add(2 * k - 1, (
                                        z[k] - mu * y[2 * k - 2] - lambda * y[2 * k] 
                                        + oneMinusLambdaMu * teta * (x[2 * k - 2] + x[2 * k])
                                     ) 
                                     / (2 * (oneMinusLambdaMu * teta)));//(5)
                for (int k = 1; k <= N; k++)
                    y.Add(2 * k - 1, x[2 * k - 1] - lambda * y[2 * k - 2] - mu * y[2 * k] - 2 * tau * psiArray[2 * k - 1]);//(4)

                //у u размерность 2, т.е. нулевая строка (в конспекте - 1) и первая строка (в конспекте - 2)
                for (int j = 0; j < 2 * N; j++)
                    u[1, j] = x[j] + phiArray[j];
                for (int j = 0; j < 2 * N; j++)
                    u[0, j] = (u[1, j] + phiArray[j] - y[j]) * 0.5;
 */
                #endregion