﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    /// <summary>
    /// Class for emitting a Mod subclass with given SettignsMenuDef
    /// </summary>
    internal static class ModEmitter
    {
        internal static Type EmitMod(SettingsMenuDef menu)
        {
            ModContentPack content = menu.modContentPack;
            if (content == null)
            {
                Verse.Log.Error("[XML Extensions] SettingsMenuDef " + menu.defName + " ModContentPack is null");
                return null;
            }
            string modName = "XmlExtensions_Mod_" + menu.defName;
            var asmName = new AssemblyName(modName);
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var moduleBuilder = asmBuilder.DefineDynamicModule(modName + "Module");
            // Define the Mod subclass
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                modName,
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(Mod)
            );
            // Define constructor
            ConstructorInfo baseCtor = typeof(Mod).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(ModContentPack) }, null);
            if (baseCtor == null)
                throw new Exception("Base constructor not found!");
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(ModContentPack) }
            );
            ILGenerator ilCtor = ctorBuilder.GetILGenerator();
            ilCtor.Emit(OpCodes.Ldarg_0); // this
            ilCtor.Emit(OpCodes.Ldarg_1); // content
            ilCtor.Emit(OpCodes.Call, baseCtor); // base(content)
            ilCtor.Emit(OpCodes.Ret);

            // Override DoSettingsWindowContents(Rect)
            MethodInfo baseDrawMethod = typeof(Mod).GetMethod("DoSettingsWindowContents", BindingFlags.Public | BindingFlags.Instance);
            MethodBuilder drawMethod = typeBuilder.DefineMethod(
                "DoSettingsWindowContents",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                new[] { typeof(Rect) }
            );
            FieldBuilder menuField = typeBuilder.DefineField(
                "menuDef",
                typeof(SettingsMenuDef),
                FieldAttributes.Public | FieldAttributes.Static
            );
            MethodInfo findWindowStack = typeof(Find).GetProperty("WindowStack").GetGetMethod();
            MethodInfo addWindowMethod = typeof(WindowStack).GetMethod("Add", new[] { typeof(Window) });
            ConstructorInfo ctor = typeof(ModSettingsWindow).GetConstructor(
                new[] { typeof(SettingsMenuDef), typeof(bool) }
            );

            // Create Find.WindowStack.Add(new XmlExtensions_MenuModSettings(menu))
            ILGenerator ilDraw = drawMethod.GetILGenerator();
            ilDraw.Emit(OpCodes.Call, findWindowStack);      // -> push Find.WindowStack
            ilDraw.Emit(OpCodes.Ldsfld, menuField);          // -> push SettingsMenuDef (menuDef)
            ilDraw.Emit(OpCodes.Ldc_I4_0);                   // -> push 'false' (bool is nested = false)
            ilDraw.Emit(OpCodes.Newobj, ctor);               // -> call ctor(SettingsMenuDef, bool)
            ilDraw.Emit(OpCodes.Callvirt, addWindowMethod);  // -> call WindowStack.Add(Window)
            ilDraw.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(drawMethod, baseDrawMethod);

            // Override SettingsCategory()
            MethodInfo baseCategoryMethod = typeof(Mod).GetMethod("SettingsCategory", BindingFlags.Public | BindingFlags.Instance);
            MethodBuilder catMethod = typeBuilder.DefineMethod(
                "SettingsCategory",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(string),
                Type.EmptyTypes
            );

            ILGenerator ilCat = catMethod.GetILGenerator();
            ilCat.Emit(OpCodes.Ldstr, Helpers.TryTranslate(menu.label, menu.tKey));
            ilCat.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(catMethod, baseCategoryMethod);

            Type emittedType = typeBuilder.CreateType();
            FieldInfo menuInfo = emittedType.GetField("menuDef", BindingFlags.Public | BindingFlags.Static);
            menuInfo.SetValue(null, menu);
            return emittedType;
        }
    }
}