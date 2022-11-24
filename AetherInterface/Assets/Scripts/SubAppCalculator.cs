using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SubAppCalculator : MonoBehaviour
{
    private string history;
    private string input;
    private List<int> lastinput;//contains length of last input for DEL funtion
    private bool clear_input;
    private bool Calc_Error;
    public GameObject Calc_Display;
    // Use this for initialization
    void Start()
    {
        clear_input = false;
        Calc_Error = false;
        input = " ";
        lastinput = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        Calc_Display.transform.GetChild(1).GetComponent<Text>().text = input;
        Calc_Display.transform.GetChild(0).GetComponent<Text>().text = history;
    }
    public void GetButtonValue(string val)
    {
        lastinput.Add(val.Length);
        if (clear_input)//prepare for newline input
        {
            history = input;
            input = " ";
            clear_input = false;
        }
        input += val;
    }
    public void Getbutton_del(bool ce)
    {
        if (ce)
        {
            input = " ";
        }
        else
        {
            if (lastinput.Count>0)
            {
                input = input.Substring(0, input.Length - lastinput[lastinput.Count - 1]);
                lastinput.RemoveAt(lastinput.Count - 1);
            }
        }
    }
    public void CalculateString()
    {
        if (!input.Contains("Error"))
        {
            Calc_Error = false;
        }
        history = input;
        if (GoodSyntax(input + ' '))
        {
            string temp = operate(input + ' ');
            if (!Calc_Error)
            {
                if (temp.Contains("N"))
                {
                    temp = temp.Replace("N", "-");
                }
                input = temp.Trim();//possibly not needed, check results from 5/2/18
            }
            else
            {
                input = "Error";
            }
        }
        else
        {
            input = "Error";
        }
        clear_input = true;
        lastinput.Clear();//empty list
    }
    private float operation_handle(float operand1, string operation, float operand2)//apply the proper operation to the strings and return a 
    {
        float res = 0;
        if (float.IsInfinity(operand1)|| float.IsInfinity(operand2))
        {
            Calc_Error = true;
        }
        switch (operation)
        {
            case "*":
                res = operand1 * operand2;
                break;
            case "/":
                res = operand1 / operand2;
                if (operand2 == 0)
                {
                    Calc_Error = true;
                }
                break;
            case "+":
                res = operand1 + operand2;
                break;
            case "-":
                res = operand1 - operand2;
                break;
            case "s":
                res = Mathf.Sin(operand2);
                break;
            case "c":
                res = Mathf.Cos(operand2);
                break;
            case "t":
                res = Mathf.Tan(operand2);
                break;
            case "^":
                res = Mathf.Pow(operand1, operand2);
                break;
            case "L":
                res = Mathf.Log(operand2, operand1);
                break;
            default:
                res = 0;
                break;
        }
        if (res==float.NaN)
        {
            Calc_Error = true;
        }
        return res;
    }
    private bool contains_operation(string val)
    {
        if (val.Contains("*") || val.Contains("/") || val.Contains("+") || val.Contains("-") || val.Contains("sin") || val.Contains("cos") || val.Contains("tan") || val.Contains("^"))
        {
            return true;
        }
        return false;
    }
    private bool GoodSyntax(string value)
    {
        int leftp = 0, rightp = 0;
        short digiop = 0;//balance of digits and operations
        for (int i = 0; (i < value.Length && !Calc_Error); i++)
        {
            switch (value[i])
            {
                case '∞':
                    break;
                case ' ':
                    break;
                case '(':
                    leftp++;
                    break;
                case ')':
                    rightp++;
                    break;
                case '+':
                case '/':
                case '*':
                case '^'://pow
                    digiop--;//expects two operands
                    break;
                case '-':
                    if (value[i - 1] == '(' || i - 1 == 0)//permits negative values
                    {
                        value = value.Substring(0, i) + "N" + value.Substring(i + 1, (value.Length - i) - 1);//replace negative numbers
                    }
                    else
                    {
                        digiop--;//expects two operands
                    }
                    break;
                case 's'://sin
                case 'c'://cos
                case 't'://tan
                    //Convert three letter operation to single letter
                    value = value.Substring(0, i + 1) + value.Substring(i + 3, (value.Length - i) - 3);//cut out previous characters
                    break;
                case 'L':
                    value = value.Substring(0, i + 1) + value.Substring(i + 4, (value.Length - i) - 4);//cut out previous characters
                    digiop--;
                    break;
                default:
                    bool decimal_avi = true;
                    while (!Calc_Error && ((char.IsDigit(value[i]) || value[i] == '.') && i < value.Length))//check for only one decimal in a number
                    {
                        if (decimal_avi && value[i] == '.')//two decimal case:error
                        {
                            decimal_avi = false;
                        }
                        else if (value[i] == '.')//decimal, remove avaliablity
                        {
                            Calc_Error = true;
                        }
                        else//digit
                        {
                        }
                        i++;
                    }
                    if (!Calc_Error)//&&!(lastop=='s'|| lastop == 'c'|| lastop == 't'|| lastop == '^')
                    {
                        digiop++;//one operand avaliable
                    }
                    i--;//overshoots string because of post ++
                    break;
            }
            if (leftp < rightp)
            {
                Calc_Error = true;
            }
        }
        if (leftp != rightp || digiop != 1)
        {
            Calc_Error = true;
            if (digiop != 1)
            {
                Debug.Log("digit/operation  error " + digiop);
            }
            else
            {
                Debug.Log("parenthesis error:" + leftp + ',' + rightp);
            }
        }
        Debug.Log("completed syntax check with status:ERROR=" + Calc_Error);
        if (!Calc_Error)
        {
            input = value;
        }
        return !Calc_Error;
    }
    private string operate(string data)
    {
        string order = "^sctL*/+-";//order 
        while (!Calc_Error && data.Contains("("))//eliminate all parenthesis
        {
            int left = 0, right = 0;
            for (int i = data.IndexOf("("); (!Calc_Error && i < data.Length - 1); i++)//calculate the substring  //somthing here
            {
                right = i;
                if (data[i] == ')')
                {
                    i = data.Length;
                }
                else if (data[i] == '(')
                {
                    left = i;
                }
            }
            data = data.Substring(0, left) + operate(" " + data.Substring(left + 1, (right - left) - 1) + " ") + data.Substring(right + 1, data.Length - right - 1);//calculate parenthesis and replace in string
        }
        if (data.Contains("NaN"))
        {
            Calc_Error = true;
        }
        for (int o = 0; (!Calc_Error && o < order.Length); o++)//for each operation
        {
            if (data.Contains(order[o].ToString()))//contains an operation
            {
                while (!Calc_Error && data.Contains(order[o].ToString()))//while string contains that operation
                {
                    int opp = data.IndexOf(order[o]);
                    int oplength = 1;
                    int cursor, beg, end;
                    string replace = "";
                    for (cursor = opp - 1; cursor > 0 && (char.IsDigit(data[cursor]) || data[cursor] == '.' || data[cursor] == 'N'); cursor--)//find first operand
                    {
                    }
                    beg = cursor;
                    for (cursor = opp + 1; cursor < data.Length && (char.IsDigit(data[cursor]) || data[cursor] == '.' || data[cursor] == 'N'); cursor++)//find second operand
                    {
                    }
                    end = cursor;
                    replace = sparse(operation_handle(fparse(data.Substring(beg + 1, (opp - beg) - 1)), data.Substring(opp, oplength), fparse(data.Substring((opp + oplength), end - (opp + oplength)))));
                    data = data.Substring(0, beg + 1) + replace + data.Substring(end, data.Length - end);
                }
            }

        }
        return data.Trim();//remove white space errors
    }
    private float fparse(string data)//parse to float
    {
        float res = 0;
        bool stat = false;
        if (data.Contains("N"))//negative number
        {
            if (data.Contains("∞") || data.Contains("Infinity"))
            {
                res = float.NegativeInfinity;
            }
            else
            {
                data = data.Replace("N", "-");
                stat = float.TryParse(data, out res);
            }
        }
        else
        {
            if (data.Contains("∞") || data.Contains("Infinity"))
            {
                res = float.NegativeInfinity;
            }
            else
            {
                stat = float.TryParse(data, out res);
            }
        }
        if (!(stat || res < 0 || data == " " || data == "" || data.Contains("∞")))//<>
        {
            Calc_Error = true;
        }
        return res;
    }
    private string sparse(float data)//parse to string
    {
        string res = "";
        if (data < 0)//negative number
        {
            if (float.IsNegativeInfinity(data))
            {
                res="N∞";
            }
            else
            {
                data *= -1;
                res = "N" + data.ToString();
            }
        }
        else
        {
            if (float.IsInfinity(data))
            {
                res = "∞";
            }
            else
            {
                res = data.ToString();
            }
        }
        return res;
    }
}
