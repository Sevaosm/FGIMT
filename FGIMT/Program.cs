using System;
using System.IO;
using System.Collections.Generic;


namespace FGIMT
{
    class Program
    {
        static void Main(string[] args)
        {
            Syntax syntaxAnalys = new Syntax();
            syntaxAnalys.syntaxCheck();

            //CToken token = new CToken();

            //Lexer lexer = new Lexer();

            //while (true)
            //{
            //    token = lexer.getNextToken();
            //    Console.WriteLine("Y: " + token.position.y + "; X: " + token.position.x + ";    " + token.info());
            //}



        }
    }
}
