using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Mocky
{
    public class Mocker
    {
        private readonly Type klass;
        private Dictionary<string, MockMethod> ms;

        public Mocker(Type klass)
        {
            this.klass = klass;
            this.ms = new Dictionary<string, MockMethod>();
        }

        public MockMethod When(string name)
        {
            MockMethod m;
            if (!ms.TryGetValue(name, out m))
            {
                m = new MockMethod(klass, name);
                ms.Add(name, m);
            }
            return m;
        }

        public object Create()
        {
            Type t = BuildType();
            return Activator.CreateInstance(t, new object[1] { ms.Values.ToArray() });
        }

        private Type BuildType()
        {
            //define names in strings
            string theName = "Mock" + klass.Name;
            string ASM_NAME = theName;
            string MOD_NAME = theName;
            string TYP_NAME = theName;
            string DLL_NAME = theName + ".dll";
            
            //initial preparations
            AssemblyBuilder asmBuilder =
                AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName(ASM_NAME), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(MOD_NAME , DLL_NAME);
            TypeBuilder typeBuilder = modBuilder.DefineType(TYP_NAME);
            
            //implement constructor
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public, 
                CallingConventions.Standard, 
                new Type[1] { typeof(MockMethod[]) });
            FieldBuilder mockBase = typeBuilder.DefineField("mockBase", typeof(MockBase), FieldAttributes.Private);
            ImplementConstructor(ctorBuilder, mockBase);
            
            //implement all interface methods
            typeBuilder.AddInterfaceImplementation(klass);
            List<Type> interfaces = GetAllInterfaces(new List<Type>(){klass});
            foreach (Type inter in interfaces)
            {
                foreach (MethodInfo method in inter.GetMethods())
                {
                    MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                        method.Name, 
                        MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                        method.ReturnType, 
                        GetParameterTypes(method.GetParameters()));
                
                    ImplementMethod(methodBuilder, method.Name, mockBase, method.GetParameters(), method.ReturnType);
                }
            }
            
            //create, save, return
            Type dynamicType = typeBuilder.CreateTypeInfo().AsType();
            asmBuilder.Save(DLL_NAME);
            return dynamicType;
        }

        private static void ImplementConstructor(ConstructorBuilder ctorBuilder, FieldInfo arr)
        {
            ILGenerator ctorIL = ctorBuilder.GetILGenerator();
            
            ctorIL.Emit(OpCodes.Ldarg_0);
            ConstructorInfo objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            ctorIL.Emit(OpCodes.Call, objectCtor);
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ConstructorInfo ctorInfo = typeof(MockBase).GetConstructors()[0];
            ctorIL.Emit(OpCodes.Newobj, ctorInfo);
            ctorIL.Emit(OpCodes.Stfld, arr);
            ctorIL.Emit(OpCodes.Ret);
            
            /*
             .class public auto ansi beforefieldinit Mocky.Calculator
                   extends [mscorlib]System.Object
                   implements Mocky.ICalculator
            {
              .field private class Mocky.MockBase mockBase
              .method public hidebysig specialname rtspecialname 
                      instance void  .ctor(class Mocky.MockMethod[] arr) cil managed
              {
                // Code size       21 (0x15)
                .maxstack  8
                IL_0000:  ldarg.0
                IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
                IL_0006:  nop
                IL_0007:  nop
                IL_0008:  ldarg.0
                IL_0009:  ldarg.1
                IL_000a:  newobj     instance void Mocky.MockBase::.ctor(class Mocky.MockMethod[])
                IL_000f:  stfld      class Mocky.MockBase Mocky.Calculator::mockBase
                IL_0014:  ret
              } // end of method Calculator::.ctor
             */
        }

        private Type[] GetParameterTypes(ParameterInfo[] getParameters)
        {
            Type[] res = new Type[getParameters.Length];
            int idx = 0;
            foreach (ParameterInfo parameter in getParameters)
            {
                res[idx++] = parameter.ParameterType;
            }

            return res;
        }

        private List<Type> GetAllInterfaces(List<Type> interfaces)
        {
            List<Type> res = new List<Type>();
            res.AddRange(interfaces);
            foreach (Type inter in interfaces)
            {
                res.AddRange(GetAllInterfaces(inter.GetInterfaces().ToList()));
            }
            return res;
        }

        private void ImplementMethod(MethodBuilder addMethodBuilder, string methodName, FieldInfo mockBase, ParameterInfo[] parameterTypes, Type returnType)
        {

            ILGenerator methIL = addMethodBuilder.GetILGenerator();

            if (returnType == typeof(void))
            {
                methIL.Emit(OpCodes.Ret);
            }

            //load mockbase instance
            methIL.Emit(OpCodes.Ldarg_0);
            methIL.Emit(OpCodes.Ldfld, mockBase);
            methIL.Emit(OpCodes.Ldstr, methodName);
            
            //Object constructor
            methIL.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
            methIL.Emit(OpCodes.Newarr, typeof(object));
            methIL.Emit(OpCodes.Dup);
            methIL.Emit(OpCodes.Ldc_I4_0);
            
            //load every parameter to the stack
            for(int i = 0; i < parameterTypes.Length; i++)
            {
                methIL.Emit(OpCodes.Ldarg_S, i + 1);
                
                if(parameterTypes[i].ParameterType != typeof(string))
                    methIL.Emit(OpCodes.Box, parameterTypes[i].ParameterType);
                methIL.Emit(OpCodes.Stelem_Ref);
                
                if (i + 1 >= parameterTypes.Length) continue;
                methIL.Emit(OpCodes.Dup);
                methIL.Emit(OpCodes.Ldc_I4, i+1);
            }
            
            //call Invoke from MockBase
            MethodInfo methodInfo = typeof(MockBase).GetMethod("Invoke");
            methIL.Emit(OpCodes.Callvirt, methodInfo);
            
            //Return
            if (returnType == typeof(string))
            {
                methIL.Emit(OpCodes.Castclass, typeof(string));
            }
            else
            {
                methIL.Emit(OpCodes.Unbox_Any, returnType);
            }
            methIL.Emit(OpCodes.Ret);
        }
    }
}