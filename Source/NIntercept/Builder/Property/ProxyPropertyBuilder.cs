﻿using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyPropertyBuilder : IProxyPropertyBuilder
    {
        private static readonly IProxyMethodBuilder DefaultProxyMethodBuilder;
        private IProxyMethodBuilder proxyMethodBuilder;

        static ProxyPropertyBuilder()
        {
            DefaultProxyMethodBuilder = new ProxyMethodBuilder();
        }

        public virtual IProxyMethodBuilder ProxyMethodBuilder
        {
            get { return proxyMethodBuilder ?? DefaultProxyMethodBuilder; }
            set { proxyMethodBuilder = value; }
        }

        public virtual PropertyBuilder CreateProperty(ModuleScope moduleScope, TypeBuilder typeBuilder, PropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            Type returnType = propertyDefinition.PropertyType;
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyDefinition.Name, propertyDefinition.Attributes, returnType, new Type[] { returnType });

            PropertyGetMethodDefinition propertyGetMethodDefinition = propertyDefinition.PropertyGetMethodDefinition;
            if (propertyGetMethodDefinition != null)
            {
                MethodBuilder getMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleScope, typeBuilder, propertyGetMethodDefinition, propertyDefinition.Property, fields);
                // copy attributes to method
                InterceptorAttributeHelper.AddInterceptorAttributes(getMethodBuilder, propertyDefinition.PropertyGetInterceptorAttributes);
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }

            PropertySetMethodDefinition propertySetMethodDefinition = propertyDefinition.PropertySetMethodDefinition;
            if (propertySetMethodDefinition != null)
            {
                MethodBuilder setMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleScope, typeBuilder, propertySetMethodDefinition, propertyDefinition.Property, fields);
                InterceptorAttributeHelper.AddInterceptorAttributes(setMethodBuilder, propertyDefinition.PropertySetInterceptorAttributes);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            return propertyBuilder;
        }
    }
}
