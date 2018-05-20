using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNet.Core.SmartResult
{
    public class SmartResult : Attribute, IResultFilter
    {
        #region Mapper

        static MapperConfiguration mapperConfiguration;
        static IMapper mapper;

        #endregion

        #region Properties
        /// <summary>
        /// The Default returned Type. Usually configured for desktop
        /// </summary>
        public Type Default { get; set; }
        /// <summary>
        /// The returned Type for Mobile browsers
        /// </summary>
        public Type Mobile { get; set; }
        /// <summary>
        /// The returned Type for native devices
        /// </summary>
        public Type Native { get; set; }

        #endregion

        #region Methods 

        static IsMobile isMobile;
        static IsNative isNative;

        #endregion

        #region Processing

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (!CanProssess())
            {
                context.HttpContext.Response.Headers.Add("Result-Type", "Default");
                return;
            }

            var objectResult = context.Result as ObjectResult;
            var isMobileBrowser = isMobile != null && isMobile(context.HttpContext);
            var isNativeDevice = isNative != null && isNative(context.HttpContext);

            if (objectResult != null)
            {
                Type resultType = GetType(objectResult.Value.GetType());
                Type mobileType = Mobile != null ? GetType(Mobile) : null;
                Type nativeType = Native != null ? GetType(Native) : null;

                if (Mobile != null && isMobileBrowser && resultType != mobileType)
                {
                    objectResult.Value = mapper.Map(objectResult.Value, Default, Mobile);
                    context.HttpContext.Response.Headers.Add("Result-Type", "Mobile");
                }
                else if (Native != null && isNativeDevice && resultType != nativeType)
                {
                    objectResult.Value = mapper.Map(objectResult.Value, Default, Native);
                    context.HttpContext.Response.Headers.Add("Result-Type", "Native");
                }
                else
                {
                    context.HttpContext.Response.Headers.Add("Result-Type", "Default");
                }
            }
        }

        #endregion

        #region Configuration
        public static void Configure(SmartResultConfiguration configuration)
        {
            if (mapperConfiguration == null)
            {
                mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    configuration.Profiles.ForEach(p =>
                    {
                        cfg.AddProfile(p);
                    });
                });
                mapper = mapperConfiguration.CreateMapper();
            }

            isMobile = configuration.IsMobileBrowser();
            isNative = configuration.IsNativeDevice();
        }

        #endregion  

        private Type GetType(Type type)
        {
            Type objectType = type;

            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition()
                    == typeof(IList<>))
                {
                    objectType = type.GetGenericArguments()[0];
                    break;
                }
                else
                {
                    objectType = type.GetGenericArguments()[0];
                }
            }

            return objectType;
        }

        private bool CanProssess()
        {
            return Default != null && (Mobile != null || Native != null) && mapper != null;
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {

        }
    }
}