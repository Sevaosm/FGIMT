using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FGIMT
{
    class Lexer
    {
        public StreamReader fstream;
        public Point curPos;
        public char c_sup;

        public Lexer()
        {
            fstream = new StreamReader(Connection.str);
            curPos = new Point();
            curPos.x = 0;
            curPos.y = 1;
            c_sup = ' ';
        }

        public int getNextLiter()
        {
            int c = fstream.Read();
            curPos.x++;
            if (c == 10)
            {
                curPos.x = 0;  // позиция неверно считается !!!!!!!!!!!!!!!!!!!!!!!
                curPos.y++;
            }
            return c;
        }

        public CToken getNextToken()
        {
            Dictionary<string, int> kWords = new Dictionary<string, int>()
            {
                { "Program", 1 },
                { "var", 2 },
                { "begin", 3 },
                { "if", 4 },
                { "then", 5 },
                { "else", 6 },
                { "readln", 7 },
                { "writeln", 8 },
                { "while", 9 },
                { "do", 10 },
                { "repeat", 11 },
                { "until", 12 },
                { "for", 13 },
                { "to", 14 },
                { "downto", 15 },
                { "print", 16 },
                { "end", 17 },
            };

            Dictionary<string, int> operators = new Dictionary<string, int>()
            {
                { ":=", 1 },
                { "=", 2 },
                { "<>", 3 },
                { "<", 4 },
                { "<=", 5 },
                { ">", 6 },
                { ">=", 7 },
                { "+", 8 },
                { "-", 9 },
                { "*", 10 },
                { "/", 11 },
                { "div", 12 },
                { "%", 13 },
                { "and", 14 },
                { "or", 15 },
                { "xor", 16 },
                { "(", 17 },
                { ")", 18 },
                { ",", 19 },
                { ".", 20 },
                { ";", 21 },
                { ":", 22 },
                { "'", 23 },
                { "&", 24 },
            };

            int c;
            bool kavychka = false;
            string s;
            float number;


            if (c_sup != ' ')
                s = "" + c_sup;
            else
                s = "";
            while (true)
            {
                if (c_sup == ' ') /* буфер пуст */
                {
                    c = getNextLiter();

                    if (c >= 65 && c <= 90 || c >= 97 && c <= 122 || c >= 48 && c <= 57 ||  /* буква или цифра */
                       (c >= 33 && c <= 47 || c >= 58 && c <= 64) && kavychka == true ||    /* операторный символ в кавычках (т.е. в константе) */
                        c == 34  /* кавычка */)
                    {
                        s += (char)c;
                        if (c == 34)  // нужно для проверки вхождения операторов в строку (константу)
                            kavychka = !kavychka;
                    }

                    else if (c >= 32 && c <= 47 || c >= 58 && c <= 64 /* операторы вне кавычек */ || c == 13)
                    {
                        if (c == 32 && s == "")
                            continue; // случай, когда после оператора получен пробел, его нельзя сохранять как токен
                        bool is_num = true;  // число ли
                        bool is_const_string = false;  // константа ли
                        for (int i = 0; i < s.Length; i++) // проход по s для определения типа токена
                        {
                            if (s[i] >= 65 && s[i] <= 90 || s[i] >= 97 && s[i] <= 122)
                                is_num = false; // если есть буква, то это - константа (не число)
                            else if (s[i] == 34)
                                is_const_string = true; // если есть кавычка, то это строковая константа
                        }

                        if (is_num == false) // кл. слово, ид или строковая константа
                        {
                            if (c != 13)
                                c_sup = (char)c; // сохранили оператор или его часть
                            if (kWords.ContainsKey(s)) // проверка на ключ. сл.
                            {
                                return new CKeyWordToken((keyWords)kWords[s], curPos.x-1, curPos.y);
                            }
                            else if (is_const_string == false) // проверка на ИД (нет кавычек)
                            {
                                return new CIdentToken(s, curPos.x - 1, curPos.y);
                            }
                            else // строковая константа (есть кавычки)
                            {
                                return new CConstToken(s, curPos.x - 1, curPos.y);
                            }
                        }
                        else if (is_num == true && s != "") // константа (число)
                        {
                            if (c == 46) // случай, когда вещ. число
                            {
                                s += (char)c;
                                continue;
                            }

                            if (c != 13)
                                c_sup = (char)c; // сохранили оператор или его часть
                            return new CConstToken(float.Parse(s, System.Globalization.CultureInfo.InvariantCulture.NumberFormat), curPos.x - 1, curPos.y);
                        }
                        else  // в с хранится оператор или его часть, нельзя его запомнить и брейкануть, так как будет пустой вызов getNextToken()
                        {
                            if (c != 13)
                            {
                                c_sup = (char)c; // сохранили оператор или его часть
                                s += c_sup;
                            }
                            continue;
                        }

                        //c_sup = (char)c; // сохранили оператор или его часть
                        //break;
                    }
                }
                else
                {
                    if (s[0] == ':')
                    {
                        c = getNextLiter();
                        if ((char)c == '=') // случай :=
                        {
                            s += (char)c;
                            c_sup = ' '; // следующая литера нас устроила, поэтому возврат к норм. проходу
                        }
                        else // какой-то символ кроме присваивания
                        {
                            c_sup = (char)c; // запоминаем какой-то символ, чтобы дальше собирать лексему
                        }
                    }
                    else if (s[0] == '<')
                    {
                        c = getNextLiter();
                        if ((char)c == '=') // случай <=
                        {
                            s += (char)c;
                            c_sup = ' '; // следующая литера нас устроила, поэтому возврат к норм. проходу
                        }
                        else if ((char)c == '>') // случай <>
                        {
                            s += (char)c;
                            c_sup = ' '; // следующая литера нас устроила, поэтому возврат к норм. проходу
                        }
                        else
                        {
                            c_sup = (char)c; // запоминаем какой-то символ, чтобы дальше собирать лексему
                        }
                    }
                    else if (s[0] == '>')
                    {
                        c = getNextLiter();
                        if ((char)c == '=') // случай >=
                        {
                            s += (char)c;
                            c_sup = ' '; // следующая литера нас устроила, поэтому возврат к норм. проходу
                        }
                        else
                        {
                            c_sup = (char)c; // запоминаем какой-то символ, чтобы дальше собирать лексему
                        }
                    }
                    else
                    {
                        c_sup = ' '; // следующая литера нам неинтересна сейчас, поэтому возврат к норм. проходу
                    }
                    return new COperatorToken((operators)operators[s], curPos.x, curPos.y);

                }
            }
        }
    }
}
