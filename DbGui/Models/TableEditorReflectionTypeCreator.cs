using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DbGui.Models;

public static class TableEditorReflectionTypeCreator
{
    public static void FillObjectWithDataMetaData(object? tableObject,
        Dictionary<string, Type> csvTableStructure,
        List<string> csvTableStrokeData)
    {
        int index = 0;

        foreach (var (fieldName, fieldType) in csvTableStructure)
        {
            var propertyInfo = tableObject.GetType().GetProperty(fieldName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                if (fieldType != typeof(string))
                {
                    object? value = DbReflection.CastToType(fieldType, csvTableStrokeData[index]);
                    // So cast returns string casted to T?, then results upcasts to object?
                    // How does SetValue sets object? value to T? property ?
                    propertyInfo.SetValue(tableObject, value, null);
                }
                else
                {
                    propertyInfo.SetValue(tableObject, csvTableStrokeData[index], null);
                }

                index++;
            }
        }
    }
    
    
    
    public static object? CreateNewObject(Dictionary<string, Type> csvTableStructure)
        {
            var myType = CompileResultType(csvTableStructure);
            return Activator.CreateInstance(myType);
        }
        public static Type CompileResultType(Dictionary<string, Type> csvTableStructure)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            
            foreach (var (fieldName, fieldType) in csvTableStructure)
                CreateProperty(tb, fieldName, fieldType);

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MyDynamicType";
            
            var assemblyName = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, 
                AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            var typeBuilder = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public,
                    null);
            
            return typeBuilder;
        }

        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, 
                propertyType, null);
            DefineGetterAndSetter(typeBuilder, propertyName, propertyType, fieldBuilder, propertyBuilder);
        }

        private static void DefineGetterAndSetter(TypeBuilder tb, string propertyName, Type propertyType,
            FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName,
                MethodAttributes.Public,
                propertyType,
                Type.EmptyTypes);

            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
}