using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DbGui.Models;

public static class DbReflection
{
    public static MethodInfo ChooseGenericTypeCastMethodByTypeConstraints(Type type)
    {
        try
        {
            if (type.IsValueType && !type.IsEnum)
                return typeof(Extensions).GetMethod("ToTypeWithStructConstraint").MakeGenericMethod(type);
            if (type.IsEnum)
                return typeof(Extensions).GetMethod("ToTypeEnumConstraint").MakeGenericMethod(type);

            return typeof(Extensions).GetMethod("ToTypeWithClassConstraint").MakeGenericMethod(type);
        }
        catch (Exception ex)
        {
            LogHelper.Log($"Could not make generic method with type {type.Name}, {type} Caused error.");
            throw;
        }
    }
    
    public static void TryCastToType(Type type, MethodInfo castGenericMethod, string? element)
    {
        try
        {
            castGenericMethod.Invoke(null, new object[] { element });
        }
        catch (Exception)
        {
            LogHelper.Log($"Element '{element}' can't be casted to type {type}.");
            throw;
        }
    }

    public static object? CastToType(Type type, string? value)
    {
        return ChooseGenericTypeCastMethodByTypeConstraints(type).Invoke(null, new object[] { value });
    }
    
    public static object? CreateNewObject(Dictionary<string, Type> csvTableStructure)
    {
        var myType = CompileResultType(csvTableStructure);
        return Activator.CreateInstance(myType);
    }
    
    public static Type CompileResultType(Dictionary<string, Type> csvTableStructure)
    {
        TypeBuilder tb = GetTypeBuilder("MyDinamicType");
        // Let it be here, I'm not sure if I have to define constructor 
        //ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        
        foreach (var (fieldName, fieldType) in csvTableStructure)
            CreateProperty(tb, fieldName, fieldType);

        Type objectType = tb.CreateType();
        return objectType;
    }
    
    private static TypeBuilder GetTypeBuilder(string typeSignature)
    {
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
    
    public static void FillObjectWithData(object? tableObject,
        Dictionary<string, Type> csvTableStructure,
        List<string?> csvTableStrokeData)
    {
        int index = 0;

        foreach (var (fieldName, fieldType) in csvTableStructure)
        {
            var propertyInfo = tableObject.GetType().GetProperty(fieldName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                if (fieldType != typeof(string))
                {
                    object? value = CastToType(fieldType, csvTableStrokeData[index]);
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
}