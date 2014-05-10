using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FQW
{
    class ParserException : ApplicationException
    {
        public ParserException(string str) : base(str) { }

        public override string ToString()
        {
            return Message;
        }
    }
    class Parser
    {
        /// <summary>
        /// перечисляем типы лексем
        /// </summary>
        enum Types { NONE, DELIMITER, VARIABLE, FUNCTION, NUMBER};
        /// <summary>
        /// перечисляем типы ошибок
        /// </summary>
        public enum Errors { SYNTAX, UNBALPARENTS, NOEXP, DIVBYZERO, OTHER, NOERR, BREAK };
        /// <summary>
        /// ссылка на строку выражения
        /// </summary>
        string exp;  
        /// <summary>
        /// текущий индекс в выражении 
        /// </summary>
        int expIdx; 
        /// <summary>
        /// текущая лексема 
        /// </summary>
        string token; 
        /// <summary>
        /// тип лексемы 
        /// </summary>
        Types tokType; 
        /// <summary>
        /// переменная х
        /// </summary>
        double varX; 
        /// <summary>
        /// тип ошибки
        /// </summary>
        public Errors err; 
        /// <summary>
        /// массив для переменных
        /// </summary>
        public double[] vars = new double[26]; 
        
        List<double> forTG = new List<double>();

        // инициализируем переменные с нулевыми значениями
        public Parser() 
        {
            for (int i = 0; i < vars.Length; i++)
                vars[i] = 0.0;
        }

        /// <summary>
        /// входная точка анализатора
        /// </summary>
        /// <param name="expstr"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        public double Evaluate(string expstr, double var)
        {
            double result;
            varX = var;

            exp = expstr;
            expIdx = 0;

            try
            {
                GetToken();
                if (token == "")
                {
                    err = Errors.NOEXP;
                    //SyntaxErr(err); // выражение отсутствует
                    return 0.0;
                }

                //EvalExp1(out result);
                EvalExp2(out result);
                forTG.Add(result);

                if (token != "") // последняя лексема должна быть null-значением
                {
                    err = Errors.SYNTAX;
                    //SyntaxErr(err);
                }
                return result;
            }
            catch (ParserException exc)
            {
                // при необходимости обработку других ошибок добавлять сюда
                //System.Windows.Forms.MessageBox.Show(Form,exc);/*Console.WriteLine(exc);*/
                //MessageBox.Show(exc.Message);
                err = Errors.OTHER;
                return 0.0;
            }
            catch (Exception)
            {
                //MessageBox.Show("Ошибка");
                err = Errors.OTHER;
                return 0.0;
            }
        }
        
        // обрабатываем присвоение
        /*void EvalExp1(out double result)
        {
            int varIdx;
            Types ttokType;
            string temptoken;

            if (tokType == Types.VARIABLE)
            {
                temptoken = String.Copy(token); // сохраняем старую лексему 
                ttokType = tokType;
                varIdx = Char.ToUpper(token[0]) - 'A'; // вычислем индекс переменной
                GetToken();
                if (token != "=")
                {
                    PutBack(); // возвращаем текущую лексему в поток и восстанавливаем старую, поскольку отсутствует присвоение
                    token = String.Copy(temptoken);
                    tokType = ttokType;
                }
                else
                {
                    GetToken(); // получаем следующую часть выражения exp
                    EvalExp2(out result);
                    vars[varIdx] = result;
                    return;
                }
            }
            EvalExp2(out result);
        }*/

         
        /// <summary>
        /// Сложение или вычитание двух членов выражения
        /// </summary>
        /// <param name="result"></param>
        void EvalExp2(out double result)
        {
            string op;
            double partialResult;

            EvalExp3(out result);
            while ((op = token) == "+" || op == "-")
            {
                GetToken();
                EvalExp3(out partialResult);
                switch (op)
                {
                    case "-":
                        result = result - partialResult;
                        break;
                    case "+":
                        result = result + partialResult;
                        break;
                }
            }
        }

        /// <summary>
        /// умножение или деление двух множителей
        /// </summary>
        /// <param name="result"></param>
        void EvalExp3(out double result)
        {
            string op;
            double partialResult = 0.0;

            EvalExp4(out result);
            while ((op = token) == "*" || op == "/" || op == "%")
            {
                GetToken();
                EvalExp4(out partialResult);
                switch (op)
                {
                    case "*":
                        result = result * partialResult;
                        break;
                    case "/":
                        if (partialResult == 0.0)
                        {
                            err = Errors.DIVBYZERO;
                            //SyntaxErr(err);
                        }
                        result = result / partialResult;
                        break;
                    case "%":
                        if (partialResult == 0.0)
                        {
                            err = Errors.DIVBYZERO;
                            //SyntaxErr(err);
                        }
                        result = (int)result % (int)partialResult;
                        break;
                }
            }
        }

        /// <summary>
        /// возведение в степень
        /// </summary>
        /// <param name="result"></param>
        void EvalExp4(out double result)
        {
            double partialResult, ex;
            int t;

            EvalExp5(out result);
            if (token == "^")
            {
                GetToken();
                EvalExp4(out partialResult);
                ex = result;
                if (partialResult == 0.0)
                {
                    result = 1.0;
                    return;
                }
                for (t = (int)partialResult - 1; t > 0; t--)
                    result = result * (double)ex;
            }
        }

        /// <summary>
        /// выполнение операции унарного + или -
        /// </summary>
        /// <param name="result"></param>
        void EvalExp5(out double result)
        {
            string op;

            token = token.ToLower();
            op = "";
            if (
                   (
                       (tokType == Types.DELIMITER) && token == "+" || token == "-"
                   )

                   ||

                   (
                       (tokType == Types.FUNCTION) && (token == "sin" || token == "cos" || token == "tg" 
                       || token == "sqrt" || token == "arcsin" || token == "arccos" || token == "arctg"
                       || token == "abs" || token == "sinh" || token == "cosh" || token == "tgh"
                       || token == "log" || token == "ln" || token == "trunc" || token == "exp" || token == "e" || token == "pi"
                       )
                   )
                )
            {
                op = token;
                GetToken();
            }
            //else if (token == "log") // обработка логарифма
            //    {
            //        op = token; // op = log
            //        GetToken(); // должна быть скобка
            //        EvalExp6(out result);

            //        else
            //        {
            //            VarLog;
            //            GetToken();
            //            NewBase
            //        }
            //        result = Math.Log(VarLog,NewBase);
            //        GetToken();
            //    }
            if (tokType != Types.VARIABLE)
            {
                EvalExp6(out result);
                if (op == "-")
                    result = -result;
                if (op == "sin")
                    result = Math.Sin(result);
                if (op == "cos")
                    result = Math.Cos(result);
                if (op == "tg")
                    result = Math.Tan(result);
                if (op == "sqrt")
                    result = Math.Sqrt(result);
                if (op == "arccos")
                    result = Math.Acos(result);
                if (op == "arcsin")
                    result = Math.Asin(result);
                if (op == "arctg")
                    result = Math.Atan(result);
                if (op == "cosh")
                    result = Math.Cosh(result);
                if (op == "sinh")
                    result = Math.Sinh(result);
                if (op == "tgh")
                    result = Math.Tanh(result);
                if (op == "log")
                {
                    double varLog = result;
                    EvalExp6(out result);
                    int newBase = (int) result;
                    result = Math.Log(varLog,newBase);
                }
                if (op == "ln")
                    result = Math.Log10(result);
                if (op == "trunc")
                    result = Math.Truncate(result);
                if (op == "abs")
                    result = Math.Abs(result);
                if (op == "exp")
                    result = Math.Exp(result);
            }
            else
            {
                if (token == "pi")
                {
                    if (op == "-")
                        result = - Math.PI;
                    else
                        result = Math.PI;
                    GetToken();
                }
                else if (token == "e")
                {
                    if (op == "-")
                        result = - Math.E;
                    else
                        result = Math.E;
                    GetToken();
                }
                else if (token == "λ" 
                      || token == "γ" 
                      || token == "τ" 
                      || token == "ε" 
                      || token == "ω" 
                      || token == "ξ" 
                      || token == "ϕ" 
                      || token == "ρ" 
                      || token == "σ" 
                      || token == "θ" 
                      || token == "α" 
                      || token == "β" 
                      || token == "δ" 
                      || token == "Δ" 
                      || token == "ξ" 
                      || token == "t" 
                      || token == "x" 
                      || token == "y" 
                      || token == "z")
                    
                {
                    result = varX;
                    GetToken();
                }
                else
                {
                    err = Errors.SYNTAX;
                    result = 0.0;
                    GetToken();
                }
            }
        }

        /// <summary>
        /// обработка выражения в круглых скобках
        /// </summary>
        /// <param name="result"></param>
        void EvalExp6(out double result)
        {
            if ((token == "("))
            {
                GetToken();
                EvalExp2(out result);
                if (token != ")")
                {
                    err = Errors.UNBALPARENTS;
                    //SyntaxErr(err);
                }
                GetToken();
            }
            else
                Atom(out result);
        }

        // получаем значение числа
        void Atom(out double result)
        {
            switch (tokType)
            {
                case Types.NUMBER:
                    try
                    {
                        //if (token.ToLower() == "pi")
                        //    result = Math.PI;
                        result = Double.Parse(token);
                    }
                    catch (FormatException)
                    {
                        result = 0.0;
                        err = Errors.SYNTAX;
                        //SyntaxErr(err);
                    }
                    GetToken();
                    return;
                /*case Types.VARIABLE:
                    result = FindVar(token); // или minValue присваивать?
                    GetToken();
                    return;*/
                default:
                    result = 0.0;
                    err = Errors.SYNTAX;
                    //SyntaxErr(err);
                    break;
            }
        }

        // возвращаем значение переменной
        /*double FindVar(string vname)
        {
            if (!Char.IsLetter(vname[0]))
            {
                SyntaxErr(Errors.SYNTAX);
                return 0.0;
            }
            return vars[Char.ToUpper(vname[0]) - 'A'];
        }

        // возвращаем лексему в поток
        void PutBack()
        {
            for (int i = 0; i < token.Length; i++)
                expIdx--;
        }*/

        /// <summary>
        /// обрабатываем синтаксическую ошибку
        /// </summary>
        /// <param name="error"></param>
        void SyntaxErr(Errors error)
        {
            string[] err = 
            {
                "Синтаксическая ошибка",
                "Дисбаланс скобок",
                "Выражение отсутствует",
                "Деление на нуль"
            };

            throw new ParserException(err[(int)error]);
        }

        /// <summary>
        /// получаем следующую лексему
        /// </summary>
        void GetToken()
        {
            tokType = Types.NONE;
            token = "";

            if (expIdx == exp.Length) 
                return; // конец выражения

            // пропускаем пробелы
            while (expIdx < exp.Length && Char.IsWhiteSpace(exp[expIdx])) ++expIdx;

            // хвостовой пробел завершает выражение
            if (expIdx == exp.Length) 
                return;

            //Stck.push(exp[expIdx]);// не правильно
            //MessageBox.Show(Convert.ToString(Stck.getNum()));

            if (IsDelim(exp[expIdx])) // это оператор?
            {
                token += exp[expIdx];
                expIdx++;
                tokType = Types.DELIMITER;
            }
            else if (Char.IsLetter(exp[expIdx])) // это переменная или функция?
            {
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length)
                        break;
                }
                token = token.ToLower();
                if (token != "sin" && token != "cos" && token != "tg" &&
                    token != "sqrt" && token != "abs" && token != "arcsin" && token != "arccos" && token != "arctg" &&
                    token != "sinh" && token != "cosh" && token != "tgh" && token != "exp" &&
                    token != "trunc" && token != "log" && token != "ln" && token != "abs")
                {
                    tokType = Types.VARIABLE;
                }
                else
                    tokType = Types.FUNCTION;
            }
            else if (Char.IsDigit(exp[expIdx]))// || (Char.ToLower(exp[expIdx]) == 'p') || (Char.ToLower(exp[expIdx]) == 'i')) // это число?
            {
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length)
                        break;
                }
                tokType = Types.NUMBER;
            }
        }

        /// <summary>
        /// метод возвращает значение true, если символ с является разделителем
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool IsDelim(char c)
        {
            if ((" +-/*%^=()".IndexOf(c) != -1))
                return true;
            return false;
        }
    }
}
