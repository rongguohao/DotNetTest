﻿
using System;

namespace DapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            string connectionString = "Database=sys;Data Source=localhost;User Id=root;Password=123456;CharSet=utf8;port=3306";

            //string sql = @"select a.*,b.* from student a

            //        left join course b on a.Id = b.StudentId";

            //var  dic = new Dictionary<int, Student>();
            //List<Student> students;
            //using (IDbConnection connection = new MySqlConnection(connectionString))
            //{

            //    students = connection.Query<Student,Course,Student>(sql,(s,c) => {


            //        Student stu;

            //        if (!dic.TryGetValue(s.Id, out stu))
            //        {
            //            dic.Add(s.Id, stu = s);
            //        }


            //        if (stu.Courses == null)
            //        {
            //            stu.Courses = new List<Course>();
            //        }

            //        if (c != null)
            //        {
            //            if (!stu.Courses.Any(x => x.Id == c.Id))
            //            {
            //                stu.Courses.Add(c);
            //            }
            //        }

            //        return s;

            //    }).AsList();
            //}

            //students = dic.Values.ToList();


            Console.WriteLine("结束");
            Console.ReadKey();
        }
    }



}
