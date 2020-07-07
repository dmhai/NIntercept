﻿using NIntercept;
using NIntercept.Builder;
using NIntercept.Definition;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeGenerationSample
{
    public class CleanPropertyMethodBuilder : InterceptableMethodBuilder
    {
        private NotifyPropertyChangedFeature notifyPropertyChangedFeature;

        public CleanPropertyMethodBuilder()
        {
            notifyPropertyChangedFeature = new NotifyPropertyChangedFeature();
        }

        public override MethodBuilder CreateMethod(ProxyScope proxyScope, MethodDefinition methodDefinition, MemberInfo member, FieldBuilder memberField, FieldBuilder callerMethodField)
        {
            // create clean properties (invocation and callback method not added)
            if (methodDefinition.MethodDefinitionType == MethodDefinitionType.Getter)
            {
                MethodBuilder methodBuilder = DefineMethodAndParameters(proxyScope, methodDefinition);

                var il = methodBuilder.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, methodDefinition.Method);
                il.Emit(OpCodes.Ret);

                return methodBuilder;
            }
            if (methodDefinition.MethodDefinitionType == MethodDefinitionType.Setter)
            {
                // Setter raise OnPropertyChanged
                MethodBuilder methodBuilder = DefineMethodAndParameters(proxyScope, methodDefinition);

                var il = methodBuilder.GetILGenerator();

                notifyPropertyChangedFeature.CheckEquals(proxyScope, il, methodDefinition.Method);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, methodDefinition.Method);

                notifyPropertyChangedFeature.InvokeOnPropertyChanged(proxyScope, il, methodDefinition.Method);

                il.Emit(OpCodes.Ret);

                return methodBuilder;
            }
            return base.CreateMethod(proxyScope, methodDefinition, member, memberField, callerMethodField);
        }


        private MethodBuilder DefineMethodAndParameters(ProxyScope proxyScope, MethodDefinition methodDefinition)
        {
            MethodBuilder methodBuilder = proxyScope.DefineMethod(methodDefinition.Name, methodDefinition.MethodAttributes);
            methodBuilder.SetReturnType(methodDefinition.ReturnType);

            // parameters
            methodBuilder.DefineGenericParameters(methodDefinition.GenericArguments);
            if (methodDefinition.ParameterDefinitions.Length > 0)
                DefineParameters(methodBuilder, methodDefinition);
            return methodBuilder;
        }

    }
}
