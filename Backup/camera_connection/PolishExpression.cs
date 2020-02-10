namespace camera_connection
{
    using System;
    using System.Collections;

    public static class PolishExpression
    {
        public static double Evaluate(string expression, double[] variables)
        {
            string[] strArray = expression.Trim().Split(new char[] { ' ' });
            Stack stack = new Stack();
            foreach (string str in strArray)
            {
                if (char.IsDigit(str[0]))
                {
                    stack.Push(double.Parse(str));
                }
                else if (str[0] == '$')
                {
                    stack.Push(variables[int.Parse(str.Substring(1))]);
                }
                else
                {
                    double a = (double) stack.Pop();
                    switch (str)
                    {
                        case "+":
                            stack.Push(((double) stack.Pop()) + a);
                            break;

                        case "-":
                            stack.Push(((double) stack.Pop()) - a);
                            break;

                        case "*":
                            stack.Push(((double) stack.Pop()) * a);
                            break;

                        case "/":
                            stack.Push(((double) stack.Pop()) / a);
                            break;

                        case "sin":
                            stack.Push(Math.Sin(a));
                            break;

                        case "cos":
                            stack.Push(Math.Cos(a));
                            break;

                        case "ln":
                            stack.Push(Math.Log(a));
                            break;

                        case "exp":
                            stack.Push(Math.Exp(a));
                            break;

                        case "sqrt":
                            stack.Push(Math.Sqrt(a));
                            break;

                        default:
                            throw new ArgumentException("Unsupported function: " + str);
                    }
                }
            }
            if (stack.Count != 1)
            {
                throw new ArgumentException("Incorrect expression.");
            }
            return (double) stack.Pop();
        }
    }
}

