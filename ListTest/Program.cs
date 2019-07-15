﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ListTest
{
    class Program
    {
        static void Main(string[] args)
        {
            #region List分组
            List<string> lst = new List<string>() { "1", "2", "3", "4", "5", "6" };

            List<List<string>> listGroup = new List<List<string>>();
            int j = 3;
            for (int i = 0; i < lst.Count; i += 3)
            {
                Console.WriteLine(i);
                List<string> cList = new List<string>();
                cList = lst.Take(j).Skip(i).ToList();
                j += 3;
                listGroup.Add(cList);
            }
            #endregion

            #region SortDictionary
            var dic = JsonConvert.DeserializeObject<SortedDictionary<string, object>>("{\"a\":\"1\",\"c\":\"我\"}");
            SortedDictionary<string, object> keyValues = new SortedDictionary<string, object>(dic);
            keyValues.OrderBy(m => m.Key);//按照键排序
            #endregion


            #region Select&SelectMany
            string[] text = { "Albert was here", "Burke slept late", "Connor is happy" };
            var tokens = text.Select(s => s.Split(" "));
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (string[] line in tokens)
            {
                foreach (string token in line)
                {
                    Console.WriteLine("{0}", token);
                }
            }


            string[] text2 = { "Albert was here", "Burke slept late", "Connor is happy" };
            Console.ForegroundColor = ConsoleColor.Red;
            var tokens2 = text2.SelectMany(s => s.Split(' '));
            foreach (string token in tokens2)
            {
                Console.WriteLine("{0}", token);
            }
            #endregion

            Console.ReadKey();
        }
    }
}