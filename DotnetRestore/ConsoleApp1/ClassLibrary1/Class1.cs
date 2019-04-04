using ClassLibrary2;
using System;

namespace ClassLibrary1
{
    public class Class1
    {
        public string Hoge()
        {
            return new Class2().Hoge();
        }
    }
}
