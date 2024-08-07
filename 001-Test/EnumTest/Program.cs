﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Hao;
using System;
using System.Collections.Generic;

namespace EnumTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var users = new List<User>() { new User() { type = UserType.typeX } };

            //Enum d = (UserType)4;

            //Console.WriteLine(Enum.IsDefined(typeof(UserType), d)); //false

            //Console.WriteLine(d.GetHashCode());

            //d= (UserType)1;


            //Console.WriteLine(Enum.IsDefined(typeof(UserType), d)); //true


            //Color myColor = Color.Red | Color.White | Color.Blue;

            //Console.WriteLine(Enum.IsDefined(typeof(Color), myColor));  //false

            //Console.WriteLine(myColor.ToString());
            //Console.WriteLine(myColor.ToString().Replace(" ", ""));

            //Array arrays = Enum.GetValues(typeof(UserType));
            //Console.WriteLine(arrays.GetValue(0).ToString());

            //User s = new User() { type = UserType.typeX };
            //User ss = new User() { type = UserType.typeY };
            //var sss = s.type.ToString();
            //s.type = (UserType)4;
            //Console.WriteLine(string.Format("{0} {1}", s.type, ss.type));

            //UserType? sa = null;
            //Console.WriteLine(1);
            //Console.WriteLine(sa.GetValueOrDefault()); //getvalueordefault

            ////FieldInfo[] fields = typeof(VehicleClassDetail).GetFields();
            ////for (int i = 0; i < fields.Length; i++)
            ////{
            ////    if (i > 0)
            ////        Console.WriteLine(fields[i] + "--->" + (int)fields[i].GetValue(null));
            ////    else
            ////        Console.WriteLine(fields[i]);
            ////}


            //System.Array values = System.Enum.GetValues(typeof(VehicleClassDetail));
            //foreach (var value in values)
            //{
            //    Console.WriteLine(value + "--" + (int)value);//获取名称和值
            //}

            ////Type? ssss = null;

            ////int i = (int)ssss;

            //CarType result;
            //Console.WriteLine("CarType100");
            //Console.WriteLine(Enum.Parse<CarType>("100")); // 100
            //Console.WriteLine(Enum.TryParse("C1", out result)); //true
            //Console.WriteLine(Enum.TryParse("C5", out result)); //false
            //Console.WriteLine(Enum.TryParse("0", out result)); //true
            //Console.WriteLine(Enum.TryParse("10", out result)); //true

            //Console.WriteLine("-10" + Enum.TryParse("-10", out result)); //true

            //Console.WriteLine(Enum.IsDefined(typeof(CarType), "C1")); //true
            //Console.WriteLine(Enum.IsDefined(typeof(CarType), "0")); //false
            //Console.WriteLine(Enum.IsDefined(typeof(CarType), 0)); //true
            //Console.WriteLine(Enum.IsDefined(typeof(CarType), 10));//false

            //var car1 = Parse<CarType>("2");

            //ProctolType pType = (ProctolType)0;

            //Console.WriteLine(ProctolType.N1 == ProctolType.N2); //相等
            //Console.WriteLine(d is Enum);


            ////获取枚举所属类型
            //Type foo4 = Enum.GetUnderlyingType(typeof(CarType));

            ////获取所有枚举成员
            //Array foo5 = Enum.GetValues(typeof(CarType));

            ////获取所有枚举成员的字段名
            //string[] foo6 = Enum.GetNames(typeof(CarType));

            //var user = default(AdminType);
            //Console.WriteLine(user); //0
            //Console.WriteLine(Enum.IsDefined(typeof(AdminType), 0)); //false

            //Stopwatch watch = new Stopwatch();

            //Enum userEnum = UserType.typeY;

            //watch.Start();

            //for (int i = 0; i < 10000000; i++)
            //{
            //    var t = Convert.ToInt32(userEnum);
            //}
            //watch.Stop();

            //Console.WriteLine(watch.ElapsedMilliseconds);

            //watch.Restart();
            //var intTYpe = typeof(int);
            //for (int i = 0; i < 10000000; i++)
            //{
            //    if (userEnum.GetType().UnderlyingSystemType == intTYpe)
            //    {
            //        //var t = userEnum.GetHashCode();
            //    }
            //    var t = userEnum.GetHashCode();
            //}
            //watch.Stop();

            //Console.WriteLine(watch.ElapsedMilliseconds);
            //Contract.Assert(true, "");

            //Console.WriteLine(CarType.C1.GetHashCode() == UserType.typeX.GetHashCode());
            //Console.WriteLine(CarType.C3.GetHashCode());

            //Console.WriteLine(EqualityComparer<UserType>.Default.GetHashCode(UserType.typeY));

            TestCast();

            Console.ReadKey();
        }

        /// <summary>
        /// 获取枚举实例
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="member">成员名或值,范例:Enum1枚举有成员A=0,则传入"A"或"0"获取 Enum1.A</param>
        public static TEnum Parse<TEnum>(object member)
        {
            string value = member?.ToString().Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                if (typeof(TEnum).IsGenericType)
                    return default(TEnum);
                throw new ArgumentNullException(nameof(member));
            }
            return (TEnum)Enum.Parse(GetType(typeof(TEnum)), value, true);
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="type">类型</param>
        public static Type GetType(Type type)
        {
            var underType = Nullable.GetUnderlyingType(type);

            return underType ?? type;
        }


        /// <summary>
        /// 测试转化性能
        /// </summary>
        public static void TestCast()
        {
            var summary = BenchmarkRunner.Run<EnumPerformanceTest>();

            Console.ReadKey();
        }


        public class EnumPerformanceTest
        {
            public int count = 10000;

            public Enum @enum = UserType.typeY;


            [Benchmark]
            public void IntConvert()
            {
                for (int i = 0; i < count; i++)
                {
                    int value = Convert.ToInt32(@enum);
                }
            }

            [Benchmark]
            public void EnumConvert()
            {
                for (int i = 0; i < count; i++)
                {
                    var type = @enum.GetType();

                    if (!Enum.IsDefined(type, @enum)) throw new ArgumentException("@enum不属于该枚举", nameof(@enum));

                    var fieldName = @enum.ToString();

                    object result = Enum.Parse(type, fieldName);

                    int value = (int)result;
                }
            }


            [Benchmark]
            public void GetHashCode1()
            {
                for (int i = 0; i < count; i++)
                {
                    int value = @enum.ToInt();
                }
            }

            [Benchmark]
            public void GetHashCode2()
            {
                for (int i = 0; i < count; i++)
                {
                    int value = EqualityComparer<UserType>.Default.GetHashCode(UserType.typeY);
                }
            }
        }

    }

    public class User
    {
        public UserType type { get; set; }
    }

    public enum UserType
    {
        typeX,
        typeY
    }


    public enum CarType
    {
        C1,
        C2,
        C3 = 4
    }

    [Flags] //Flags属性表示此枚举为位域枚举
    enum Color
    {
        None = 0,
        Red = 1,
        White = 2,
        Blue = 4,
        Black = 8
    }
}
