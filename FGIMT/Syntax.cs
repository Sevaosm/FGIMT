using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FGIMT
{
    class Syntax
    {
        public Lexer lexerObj = new Lexer();
        public CToken curToken;
        public CToken nextToken;
        public int countBracket = 0;

        // блок Program
        public void blockProgram()
        {
            // структура: Program <ident>;

            // проверка Program
            curToken = lexerObj.getNextToken();
            if (curToken.info() != keyWords.kwProgram.ToString())
                throw new Exception("Нет объявления ключевого слова 'Program' ");

            // проверка <название программы>
            curToken = lexerObj.getNextToken();
            if (curToken.TokenType != tokenType.ttId)
                throw new Exception("Неверное объявление идентификатора");


            // проверка <точка с запятой>
            curToken = lexerObj.getNextToken();
            if (curToken.info() != operators.oDotAndComma.ToString())
                throw new Exception("Нет объявления оператора ';' ");
        }

        // блок var
        public void blockVarMain()
        {
            // проверяем на правильность перечисление переменных
            blockVarRec();

            // проверяем идентификатор типа
            curToken = lexerObj.getNextToken();
            if (curToken.TokenType != tokenType.ttId)
                throw new Exception("Неверное объявление типа");

            // проверка на двоеточие
            curToken = lexerObj.getNextToken();
            if (curToken.info() != operators.oDotAndComma.ToString())  // проверка на точку с запятой
                throw new Exception("Нет объявления оператора ';' ");

            // проверка следующего токена (либо очередное объявление переменных, либо начало основного кода begin)
            nextToken = lexerObj.getNextToken();
            if (nextToken.TokenType == tokenType.ttId)  // проверка идентификатора - переменной
            {
                // далле перечисляем переменные с другим типом, в токене лежит 1 ident (можно завести буфер с хранением этой переменной и потом доставать)!!!!!!!!!!!!!!!!!!!!
                blockVarMain();
            }
            else
            {
                // если это не идентификатор, то падаем в блок основной проги
                return;
            }
        }

        // блок var
        public void blockVarRec() // проверка <val> {, <val>} :
        {
            // проверка идентификатора
            if (nextToken != null)
            {
                curToken = nextToken;
                nextToken = null;
            }
            else
                curToken = lexerObj.getNextToken();

            if (curToken.TokenType != tokenType.ttId)
                throw new Exception("Неверное объявление идентификатора");

            curToken = lexerObj.getNextToken();
            if (curToken.info() == operators.oComma.ToString())
            {
                // продолжение перечисления переменных
                blockVarRec();
            }
            else if (curToken.info() == operators.oDoubleDots.ToString())
            {
                // перечисление переменных окончено, дальше <type>
                return;
            }
            else
            {
                // после перечисления переменных нет ;
                throw new Exception("Нет объявления оператора ';' ");
            }

        }

        // блок var
        public void blockVar() // возможность рекурсии 
        {
            // структура: var <val> {, <val>} : <typeOfData>; { <val> {, <val>} : <typeOfData>; }

            // проверка var
            curToken = lexerObj.getNextToken();
            if (curToken.info() != keyWords.kwVar.ToString())
                throw new Exception("Нет объявления ключевого слова 'var' ");

            blockVarMain();
        }


        // <слагаемое> ::= <множитель> {<операции умножения> <множитель>}
        public void blockMainCodeOperatorExpressionMult()
        {
            // проверка на '(' и ')'
            curToken = lexerObj.getNextToken();
            if (curToken.info() == operators.oBracketIn.ToString())
            {
                // прибавляем скобку
                countBracket++;

                blockMainCodeOperatorExpressionAdd();
                if (curToken.info() != operators.oBracketOut.ToString())
                    throw new Exception("Нет объявления оператора ')' ");
                else // вычитаем скобку
                    countBracket--;
            }
            else if (curToken.TokenType != tokenType.ttId && curToken.TokenType != tokenType.ttConst)
                throw new Exception("Нет объявления операнда выражения ");

            // проверка операции
            curToken = lexerObj.getNextToken();
            if (curToken.info() == operators.oMult.ToString() || curToken.info() == operators.oDiv.ToString() ||
                curToken.info() == operators.oMod.ToString() || curToken.info() == operators.oDIV.ToString())
                blockMainCodeOperatorExpressionMult();
            else return;
        }


        // <выражение> ::= <слагаемое> {<операции сложения> <слагаемое>}
        public void blockMainCodeOperatorExpressionAdd()
        {
            // <слагаемое>
            blockMainCodeOperatorExpressionMult();

            // проверка операции
            if (curToken.info() == operators.oPlus.ToString() || curToken.info() == operators.oMinus.ToString())
                blockMainCodeOperatorExpressionAdd();
            else if (curToken.info() == operators.oBracketOut.ToString())
                return;
            else if (curToken.info() == operators.oDotAndComma.ToString())
            {
                if (countBracket == 0)
                    return;
                else
                    throw new Exception("В строке не соблюдено соответствие скобок '(' ')' ");
            }
            else if (curToken.info() == operators.oEqual.ToString() || curToken.info() == operators.oNotEqual.ToString() ||
                     curToken.info() == operators.oLess.ToString() || curToken.info() == operators.oLessOrEqual.ToString() ||
                     curToken.info() == operators.oMore.ToString() || curToken.info() == operators.oMoreOrEqual.ToString())
                return;
            else
                throw new Exception("Нет объявления оператора '+' или '-' ");
        }


        // присваивание
        public void blockMainCodeOperatorAssign()
        {
            nextToken = null;
            curToken = lexerObj.getNextToken();
            if (curToken.info() != operators.oAssign.ToString())  // проверка на присваивание
                throw new Exception("Нет объявления оператора ':=' ");
            else
                blockMainCodeOperatorExpressionAdd(); // <выражение>
        }


        // <логич. выражение> ::= <простое логич. выражение> ::= (<логич. множитель> <операция сравнения> <логич. множитель>)
        public void blockMainCodeOperatorLogicExpression()
        {
            // проверка открывающей скобки
            curToken = lexerObj.getNextToken();
            if (curToken.info() != operators.oBracketIn.ToString())
                throw new Exception("Нет объявления оператора '(' ");

            // проверка левого выражения
            blockMainCodeOperatorExpressionAdd();

            // проверка правого выражения
            blockMainCodeOperatorExpressionAdd();

        }

        // <усл. оператор> ::= if <логич. выражение> then <оператор> | if <логич. выражение> then <оператор> else <оператор>
        public void blockMainCodeOperatorLogic()
        {
            curToken = nextToken;
            nextToken = null;

            // <логич. выражение> ::= <простое логич. выражение>
            blockMainCodeOperatorLogicExpression();

            // проверка then
            curToken = lexerObj.getNextToken();
            if (curToken.info() != keyWords.kwThen.ToString())
                throw new Exception("Нет объявления ключевого слова 'then' ");

            nextToken = lexerObj.getNextToken();

            // оператор 
            blockMainCode();

            // проверка else
            curToken = lexerObj.getNextToken();
            if (curToken.info() == keyWords.kwElse.ToString())
            {
                nextToken = lexerObj.getNextToken();

                // оператор 
                blockMainCode();
            }
            else
                return;
        }


        // блок основной программы (оператор)
        public void blockMainCodeOperator()
        {
            nextToken = lexerObj.getNextToken();
            if (nextToken.TokenType == tokenType.ttId) // <основной> -> <присваивание> ::= <переменная> := <выражение>
            {
                blockMainCodeOperatorAssign();
            }
            else if (nextToken.info() == keyWords.kwBegin.ToString()) // <производный> -> <составной>
            {
                blockMainCode();
            }
            else if (nextToken.info() == keyWords.kwEnd.ToString())
            {
                return;
            }
            else if (nextToken.info() == keyWords.kwIf.ToString()) // <производный> -> <усл. оператор> ::= if <логич. выражение> then <оператор> | if <логич. выражение> then <оператор> else <оператор>
            {
                blockMainCodeOperatorLogic();
            }
            else if (nextToken.info() == keyWords.kwPrint.ToString()) // <вывод> ::= print (<выражение>);
            {
                blockMainCodeOperatorExpressionAdd();
            }

            blockMainCodeOperator();
        }


        // блок основной программы <составной>
        public void blockMainCode()
        {
            // проверка begin
            if (nextToken.info() == keyWords.kwBegin.ToString())
            {
                curToken = nextToken; // в curToken лежит 'begin' или 'print'
                nextToken = null;
            }
            else
            {
                throw new Exception("Нет объявления ключевого слова 'begin' ");
            }

            // <оператор>
            blockMainCodeOperator();


            // проверка end
            if (nextToken.info() == keyWords.kwEnd.ToString())
            {
                curToken = lexerObj.getNextToken(); // в curToken лежит 'begin' или 'print'
                nextToken = null;

                // конец программы
                if (curToken.info() == operators.oDot.ToString())
                    return;

                if (curToken.info() != operators.oDotAndComma.ToString())
                    throw new Exception("Нет объявления оператора ';' ");


            }
        }

        public void syntaxCheck()
        {
            // block Program
            blockProgram();

            // block var
            blockVar();

            // block of main code (begin .. end.)
            blockMainCode();

        }
    }
}
