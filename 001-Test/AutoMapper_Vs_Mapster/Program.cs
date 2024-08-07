﻿using AutoMapper;
using BenchmarkDotNet.Attributes;
using Force.DeepCloner;
using Mapster;
using Moq.AutoMock;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AutoMapper_Vs_Mapster
{
    internal class Program
    {
        //运行  dotnet run --project .\AutoMapper_Vs_Mapster.csproj -c Release

        //原文 https://www.cnblogs.com/hippieZhou/p/15590457.html

        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<PerformanceTest>();


            //var dto1 = new EntityClone { NickName = "2222", Dto2 = new EntityDtoClone { NickName = "1111" } };

            ////mapster
            //var dto2 = dto1.Adapt<EntityClone>(); //Mapster 默认递归映射对象是深拷贝，如果不想使用深拷贝，可以通过调用 ShallowCopyForSameType 方法设置为浅拷贝：https://www.cnblogs.com/staneee/p/14913759.html

            //Console.WriteLine(object.ReferenceEquals(dto1.NickName, dto2.NickName)); // true 

            //Console.WriteLine(object.ReferenceEquals(dto1.Dto2, dto2.Dto2)); // false


            ////deepcloner
            //var dto3 = dto1.DeepClone(); //深克隆

            //Console.WriteLine(object.ReferenceEquals(dto1.NickName, dto3.NickName)); //  true 

            //Console.WriteLine(object.ReferenceEquals(dto1.Dto2, dto3.Dto2)); // false


            //var dto5 = dto1.ShallowClone(); //浅克隆

            //Console.WriteLine(object.ReferenceEquals(dto1.NickName, dto5.NickName)); //  true 

            //Console.WriteLine(object.ReferenceEquals(dto1.Dto2, dto5.Dto2)); // true


            ////json序列化
            //var dto4 = JsonConvert.DeserializeObject<EntityClone>(JsonConvert.SerializeObject(dto1));

            //Console.WriteLine(object.ReferenceEquals(dto1.NickName, dto4.NickName)); //  false 

            //Console.WriteLine(object.ReferenceEquals(dto1.Dto2, dto4.Dto2)); // false

            ////autoMapper

            //var configuration = new MapperConfiguration(cfg =>
            //{
            //});

            //var mapper = configuration.CreateMapper();

            //var dto6 = mapper.Map<EntityClone>(dto1);  //automapper 对象 浅克隆

            //Console.WriteLine(object.ReferenceEquals(dto1.NickName, dto6.NickName)); //  true 

            //Console.WriteLine(object.ReferenceEquals(dto1.Dto2, dto6.Dto2)); // true

            MapsterTest.Test();

            Console.ReadKey();
        }
    }


    public enum MyEnum
    {
        [Description("进行中")]
        Doing,
        [Description("完成")]
        Done
    }
    public class EntityClone
    {
        public int Id { get; set; }
        public Guid Oid { get; set; }
        public string NickName { get; set; }
        public bool Created { get; set; }
        public MyEnum State { get; set; }

        public EntityDtoClone Dto2 { get; set; }
    }
    public class EntityDtoClone
    {
        public int Id { get; set; }
        public Guid Oid { get; set; }
        public string NickName { get; set; }
        public bool Created { get; set; }
        public MyEnum Enum { get; set; }
        public string EnumString { get; set; }
    }


    public class Entity
    {
        public int Id { get; set; }
        public Guid Oid { get; set; }
        public string NickName { get; set; }
        public bool Created { get; set; }
        public MyEnum State { get; set; }

        //public EntityDto Dto2 { get; set; }
    }

    public class EntityDto
    {
        public int Id { get; set; }
        public Guid Oid { get; set; }
        public string NickName { get; set; }
        public bool Created { get; set; }
        public MyEnum Enum { get; set; }
        public string EnumString { get; set; }
    }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Entity, EntityDto>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Oid, opt => opt.MapFrom(src => src.Oid))
                 .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.NickName))
                 .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                 .ForMember(dest => dest.Enum, opt => opt.MapFrom(src => src.State))
                 .ForMember(dest => dest.EnumString, opt => opt.MapFrom(src => src.State.GetDescription()));
        }
    }

    public class MapsterProfile : TypeAdapterConfig
    {
        public MapsterProfile()
        {
            ForType<Entity, EntityDto>()
                 .Map(dest => dest.Id, src => src.Id)
                 .Map(dest => dest.Oid, src => src.Oid)
                 .Map(dest => dest.NickName, src => src.NickName)
                 .Map(dest => dest.Created, src => src.Created)
                 .Map(dest => dest.Enum, src => src.State)
                 .Map(dest => dest.EnumString, src => src.State.GetDescription());
        }
    }

    public class PerformanceTest
    {
        private IReadOnlyList<Entity> _entities;
        private readonly AutoMapper.IMapper _autoMapper;
        private readonly MapsterMapper.Mapper _mapsterMapper;

        public PerformanceTest()
        {
            var mocker = new AutoMocker();
            _entities = Enumerable.Range(0, 1000000).Select(x => mocker.CreateInstance<Entity>()).ToList();

            var configuration = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _autoMapper = configuration.CreateMapper();
            _mapsterMapper = new MapsterMapper.Mapper(new MapsterProfile());
        }

        [Benchmark]
        public void Constructor()
        {
            var dtos = _entities.Select(x => new EntityDto
            {
                Id = x.Id,
                Oid = x.Oid,
                NickName = x.NickName,
                Created = x.Created,
                Enum = x.State,
                EnumString = x.State.GetDescription(),
            });
        }

        [Benchmark]
        public void AutoMapper()
        {
            var dtos = _autoMapper.Map<IEnumerable<EntityDto>>(_entities);

        }
        [Benchmark]
        public void Mapster()
        {
            var dtos = _mapsterMapper.Map<IEnumerable<EntityDto>>(_entities);
        }
    }

    public static class EnumDescr
    {
        public static string GetDescription(this Enum enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);  //获取描述属性
            if (objs == null || objs.Length == 0)  //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }
    }
}
