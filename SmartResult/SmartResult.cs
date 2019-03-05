using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNet.Core.SmartResult
{
    public class SmartResult : Attribute, IResultFilter
    {
        private static SmartResultConfiguration _configuration;

        #region Mapper

        static MapperConfiguration mapperConfiguration;
        static IMapper mapper;

        #endregion

        #region Properties

        /// <summary>
        /// The Default returned Type. Usually configured for desktop
        /// </summary>
        private Type _desktop;

        /// <summary>
        /// The returned Type for Mobile browsers
        /// </summary>
        private Type _mobile;

        /// <summary>
        /// The returned Type for native devices
        /// </summary>
        private Type _native;

        #endregion

        #region Methods 

        static IsMobile isMobile;
        static IsNative isNative;

        #endregion

        #region Processing

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (!CanProcess(objectResult.Value.GetType()))
                {
                    context.HttpContext.Response.Headers.Add("Result-Type", "Default");
                    return;
                }

                var isMobileBrowser = isMobile != null && isMobile(context.HttpContext);
                var isNativeDevice = isNative != null && isNative(context.HttpContext);


                Type resultType = GetType(objectResult.Value.GetType());
                Type mobileType = _mobile != null ? GetType(_mobile) : null;
                Type nativeType = _native != null ? GetType(_native) : null;

                if (_mobile != null && isMobileBrowser && resultType != mobileType)
                {
                    objectResult.Value = mapper.Map(objectResult.Value, _desktop, _mobile);
                    context.HttpContext.Response.Headers.Add("Result-Type", "Mobile");
                }
                else if (_native != null && isNativeDevice && resultType != nativeType)
                {
                    objectResult.Value = mapper.Map(objectResult.Value, _desktop, _native);
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
            _configuration = configuration;

            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                configuration.Profiles.ForEach(p =>
                {
                    cfg.AddProfile(p.Profile);
                });
            });
            mapper = mapperConfiguration.CreateMapper();

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

        private Type CreateType(Type defaultType, Type type)
        {
            Type objectType = type;

            foreach (Type interfaceType in defaultType.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition()
                    == typeof(IList<>))
                {
                    objectType = typeof(List<>).MakeGenericType(type);
                    break;
                }
            }

            return objectType;
        }

        private bool CanProcess(Type type)
        {
            var baseType = GetType(type);

            var config = _configuration.Profiles.FirstOrDefault(p => p.Desktop == baseType);

            if (config == null)
                return false;

            var canProcess = config.Desktop != null && (config.Mobile != null || config.Native != null) && mapper != null;

            if (canProcess)
            {
                _desktop = type;

                if (config.Mobile != null)
                {
                    _mobile = CreateType(type, config.Mobile);
                }

                if (config.Native != null)
                {
                    _native = CreateType(type, config.Native);
                }
            }

            return canProcess;
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {

        }
    }
}