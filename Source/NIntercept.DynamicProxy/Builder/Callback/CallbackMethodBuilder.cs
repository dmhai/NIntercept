﻿using NIntercept.Definition;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class CallbackMethodBuilder : ICallbackMethodBuilder
    {
        public virtual MethodBuilder CreateMethod(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, CallbackMethodDefinition methodCallbackDefinition, FieldBuilder[] fields)
        {
            MethodBuilder methodBuilder = DefineMethod(typeBuilder, methodCallbackDefinition);

            GenericTypeParameterBuilder[] genericTypeParameters = MethodBuilderHelper.DefineGenericParameters(methodBuilder, methodCallbackDefinition.GenericArguments);
            if (methodCallbackDefinition.ParameterDefinitions.Length > 0)
                DefineParameters(methodBuilder, methodCallbackDefinition);

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Nop);

            Type returnType = methodCallbackDefinition.ReturnType;

            // locals
            var target = methodCallbackDefinition.Target;
            Type targetType = target != null ? target.GetType() : typeof(object);
            LocalBuilder targetLocalBuilder = il.DeclareLocal(targetType); // target
            LocalBuilder resultLocalBuilder = null; // result
            if (returnType != typeof(void))
            {
                if (returnType.ContainsGenericParameters)
                    resultLocalBuilder = il.DeclareLocal(genericTypeParameters.First(p => p.Name == returnType.Name));
                else
                    resultLocalBuilder = il.DeclareLocal(returnType);
            }

            if (target != null)
            {
                // return target.Method(p1,p2 ...);
                EmitHelper.StoreFieldInLocal(il, fields.ElementAt(1), targetLocalBuilder);
                il.Emit(OpCodes.Ldloc, targetLocalBuilder);
            }
            else
            {
                // return base.Method(p1, p2...);
                il.Emit(OpCodes.Ldarg_0);
            }

            var parameterDefinitions = methodCallbackDefinition.ParameterDefinitions;
            for (int i = 0; i < parameterDefinitions.Length; i++)
                il.EmitLdarg(i + 1);

            CallMethodOnTarget(il, methodCallbackDefinition, genericTypeParameters, target);

            if (returnType != typeof(void))
            {
                il.Emit(OpCodes.Stloc, resultLocalBuilder);
                il.Emit(OpCodes.Ldloc, resultLocalBuilder);
            }

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        protected virtual MethodBuilder DefineMethod(TypeBuilder typeBuilder, CallbackMethodDefinition methodCallbackDefinition)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodCallbackDefinition.Name, methodCallbackDefinition.MethodAttributes);
            methodBuilder.SetReturnType(methodCallbackDefinition.ReturnType);
            return methodBuilder;
        }

        protected virtual void DefineParameters(MethodBuilder methodBuilder, CallbackMethodDefinition methodCallbackDefinition)
        {
            methodBuilder.SetParameters(methodCallbackDefinition.ParameterDefinitions.Select(p => p.ParameterType).ToArray());

            int index = 1;
            foreach (var parameterDefinition in methodCallbackDefinition.ParameterDefinitions)
                methodBuilder.DefineParameter(index++, parameterDefinition.Attributes, parameterDefinition.Name);
        }

        protected virtual void CallMethodOnTarget(ILGenerator il, CallbackMethodDefinition methodCallbackDefinition, GenericTypeParameterBuilder[] genericTypeParameters, object target)
        {
            OpCode code = OpCodes.Call;
            if (methodCallbackDefinition.TypeDefinition.IsInterface) code = OpCodes.Callvirt;
            MethodInfo method = methodCallbackDefinition.Method;
            if (method.IsGenericMethod)
                il.Emit(code, method.MakeGenericMethod(genericTypeParameters));
            else
                il.Emit(code, method);
        }
    }
}
