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
            FieldBuilder arr = typeBuilder.DefineField("arr", typeof(MockBase), FieldAttributes.Private);
            ImplementConstructor(ctorBuilder, arr);
            
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
                
                    ImplementMethod(methodBuilder, method.Name, arr);
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

        private void ImplementMethod(MethodBuilder addMethodBuilder, string methodName, FieldInfo arr)
        {

            ILGenerator methIL = addMethodBuilder.GetILGenerator();
            
            methIL.Emit(OpCodes.Ldarg_0);
            methIL.Emit(OpCodes.Ldfld, arr);
            methIL.Emit(OpCodes.Ldstr, methodName);
            
            methIL.Emit(OpCodes.Ldc_I4_2);
            methIL.Emit(OpCodes.Newarr, typeof(object));
            methIL.Emit(OpCodes.Dup);
            methIL.Emit(OpCodes.Ldc_I4_0);
            methIL.Emit(OpCodes.Ldarg_1);
            methIL.Emit(OpCodes.Box, typeof(int));
            methIL.Emit(OpCodes.Stelem_Ref);
            methIL.Emit(OpCodes.Dup);
            methIL.Emit(OpCodes.Ldc_I4_1);
            methIL.Emit(OpCodes.Ldarg_2);
            methIL.Emit(OpCodes.Box, typeof(int));
            methIL.Emit(OpCodes.Stelem_Ref);
            MethodInfo methodInfo = typeof(MockBase).GetMethod("Invoke");
            
            methIL.Emit(OpCodes.Callvirt, methodInfo);
            methIL.Emit(OpCodes.Unbox_Any, typeof(int));
            methIL.Emit(OpCodes.Ret);

            /*
              .method public hidebysig newslot virtual final 
                      instance int32  Add(int32 a,
                                          int32 b) cil managed
              {
                // Code size       51 (0x33)
                .maxstack  6
                .locals init (int32 V_0)
                IL_0000:  nop
                IL_0001:  ldarg.0
                IL_0002:  ldfld      class Mocky.MockBase Mocky.Calculator::mockBase
                IL_0007:  ldstr      "Add"
                
                IL_000c:  ldc.i4.2
                IL_000d:  newarr     [mscorlib]System.Object
                IL_0012:  dup
                IL_0013:  ldc.i4.0
                IL_0014:  ldarg.1
                IL_0015:  box        [mscorlib]System.Int32
                IL_001a:  stelem.ref
                IL_001b:  dup
                IL_001c:  ldc.i4.1
                IL_001d:  ldarg.2
                IL_001e:  box        [mscorlib]System.Int32
                IL_0023:  stelem.ref
                
                IL_0024:  callvirt   instance object Mocky.MockBase::Invoke(string,
                                                                            object[])
                IL_0029:  unbox.any  [mscorlib]System.Int32
                IL_002e:  stloc.0
                IL_002f:  br.s       IL_0031
            
                IL_0031:  ldloc.0
                IL_0032:  ret
              } // end of method Calculator::Add
             */
        }
    }
}