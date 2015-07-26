using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    //Source: //https://msdn.microsoft.com/en-us/library/ff650316.aspx
    // Other ways implementing Singleton pattern //http://csharpindepth.com/Articles/General/Singleton.aspx#lock
    // You can go through saved PDF versions of the above links in Docs\Singleton folder.


    // Thread Safe 
    public class EmailSystemSingletonType1
    {
        private EmailSystemSingletonType1() { }
        private static volatile EmailSystemSingletonType1 _instance;
        private static object lockObject;
        public static EmailSystemSingletonType1 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new EmailSystemSingletonType1();
                            return _instance;
                        }
                        else
                            return _instance;
                    }
                }
                else
                    return _instance;
            }
        }
    }

    // Thread Safe - This is possible only in .NET. That with the help of CLR.
    public class EmailSystemSingletonType2
    {
        private EmailSystemSingletonType2() { }
        private static readonly EmailSystemSingletonType2 _instance = new EmailSystemSingletonType2();
        public static EmailSystemSingletonType2 Instance
        {
            get
            {
                return _instance;
            }
        }
    }

    //Simple Singleton design pattern implementation - *Not Thread Safe
    public class EmailSystemSingletonType3
    {
        private EmailSystemSingletonType3() { }
        private static EmailSystemSingletonType3 _instance;
        public EmailSystemSingletonType3 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmailSystemSingletonType3();
                    return _instance;
                }
                else
                    return _instance;
            }
        }
    }
}
