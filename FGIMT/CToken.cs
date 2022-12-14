using System;
using System.Collections.Generic;
using System.Text;

namespace FGIMT
{
    enum tokenType { ttKw = 1, ttO, ttId, ttConst };
    enum keyWords
    {
        kwProgram = 1, kwVar, kwBegin, kwIf, kwThen, kwElse, kwReadln, kwWriteln,
        kwWhile, kwDo, kwRepeat, kwUntil, kwFor, kwTo, kwDownto, kwPrint, kwEnd
    };
    enum operators
    {
        oAssign = 1, oEqual, oNotEqual, oLess, oLessOrEqual, oMore, oMoreOrEqual,
        oPlus, oMinus, oMult, oDiv, oDIV, oMod, oAnd, oOr, oXor, oBracketIn, 
        oBracketOut, oComma, oDot, oDotAndComma, oDoubleDots, oApostrophe, 
        oAmpersant
    };

    public struct Point
    {
        public int x;
        public int y;
    }

    class CToken
    {
        protected tokenType tt;
        public tokenType TokenType { get { return tt; } }

        public Point position;
        virtual public string info()
        {
            return "";
        }
    }


    class CKeyWordToken : CToken
    {
        private keyWords kw;
        public CKeyWordToken(keyWords value, int x, int y)
        {
            tt = (tokenType)1;
            kw = value;
            position.x = x;
            position.y = y;
        }

        public override string info()
        {
            return kw.ToString();
        }
    }

    class COperatorToken : CToken
    {
        private operators o;

        public COperatorToken(operators value, int x, int y)
        {
            tt = (tokenType)2;
            o = value;
            position.x = x;
            position.y = y;
        }

        public override string info()
        {
            return o.ToString();
        }
    }

    class CIdentToken : CToken
    {
        private string id;

        public CIdentToken(string value, int x, int y)
        {
            tt = (tokenType)3;
            id = value;
            position.x = x;
            position.y = y;
        }

        public override string info()
        {
            return id.ToString();
        }
    }

    class CConstToken : CToken
    {
        private string cnst;
        private float number;

        public CConstToken(string value, int x, int y)
        {
            tt = (tokenType)4;
            cnst = value;
            position.x = x;
            position.y = y;
        }

        public CConstToken(float value, int x, int y)
        {
            tt = (tokenType)4;
            number = value;
            position.x = x;
            position.y = y;
        }

        public override string info()
        {
            string s;

            if (cnst != null)
                s = cnst.ToString();
            else
                s = number.ToString();

            return s;
        }
    }


}
