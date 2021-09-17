using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AffenECS
{
    public static class NetworkUtils
    {
        private static readonly Dictionary<Type, FieldInfo[]> ComponentTypeFieldInfos = new Dictionary<Type, FieldInfo[]>(); 
        
        public static byte[] Serialize(this EcsComponent component)
        {
            var fieldInfos = GetFieldInfos(component.GetType());
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            bw.Write(component.SerializeType());
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                object value = fieldInfo.GetValue(component);
                Type type = fieldInfo.FieldType;
                if (type == typeof(bool))
                {
                    bw.Write((bool) value);
                }
                else if (type == typeof(byte))
                {
                    bw.Write((byte) value);
                }
                else if (type == typeof(sbyte))
                {
                    bw.Write((sbyte) value);
                }
                else if (type == typeof(byte[]))
                {
                    bw.Write(((byte[]) value).Length);
                    bw.Write((byte[]) value);
                }
                else if (type == typeof(char))
                {
                    bw.Write((char) value);
                }
                else if (type == typeof(char[]))
                {
                    bw.Write(((char[]) value).Length);
                    bw.Write((char[]) value);
                }
                else if (type == typeof(double))
                {
                    bw.Write((double) value);
                }
                else if (type == typeof(decimal))
                {
                    bw.Write((decimal) value);
                }
                else if (type == typeof(short))
                {
                    bw.Write((short) value);
                }
                else if (type == typeof(ushort))
                {
                    bw.Write((ushort) value);
                }
                else if (type == typeof(int))
                {
                    bw.Write((int) value);
                }
                else if (type == typeof(uint))
                {
                    bw.Write((uint) value);
                }
                else if (type == typeof(long))
                {
                    bw.Write((long) value);
                }
                else if (type == typeof(ulong))
                {
                    bw.Write((ulong) value);
                }
                else if (type == typeof(float))
                {
                    bw.Write((float) value);
                }
                else if (type == typeof(string))
                {
                    // bw.Write(((string) value).Length);
                    bw.Write((string) value);
                }
            }

            var array = ms.ToArray();
            
            ms.Dispose();
            bw.Dispose();

            return array;
        }

        public static void Deserialize(this byte[] bytes, EcsComponent component)
        {
            var ms = new MemoryStream(bytes);
            var br = new BinaryReader(ms);

            Type componentType = br.ReadUInt16().DeserializeType();
            if (componentType != component.GetType())
            {
                throw new ArgumentException("Invalid component Type: target is " + component.GetType() + ", but binary message type is " + componentType);
            }
            
            var fieldInfos = GetFieldInfos(componentType);

            foreach (var fieldInfo in fieldInfos)
            {
                object value;
                Type type = fieldInfo.FieldType;
                if (type == typeof(bool))
                {
                    value = br.ReadBoolean();
                }
                else if (type == typeof(byte))
                {
                    value = br.ReadByte();
                }
                else if (type == typeof(sbyte))
                {
                    value = br.ReadSByte();
                }
                else if (type == typeof(byte[]))
                {
                    int length = br.ReadInt32();
                    value = br.ReadBytes(length);
                }
                else if (type == typeof(char))
                {
                    value = br.ReadChar();
                }
                else if (type == typeof(char[]))
                {
                    int length = br.ReadInt32();
                    value = br.ReadChars(length);
                }
                else if (type == typeof(double))
                {
                    value = br.ReadDouble();
                }
                else if (type == typeof(decimal))
                {
                    value = br.ReadDecimal();
                }
                else if (type == typeof(short))
                {
                    value = br.ReadInt16();
                }
                else if (type == typeof(ushort))
                {
                    value = br.ReadUInt16();
                }
                else if (type == typeof(int))
                {
                    value = br.ReadInt32();
                }
                else if (type == typeof(uint))
                {
                    value = br.ReadUInt32();
                }
                else if (type == typeof(long))
                {
                    value = br.ReadInt64();
                }
                else if (type == typeof(ulong))
                {
                    value = br.ReadUInt64();
                }
                else if (type == typeof(float))
                {
                    value = br.ReadSingle();
                }
                else if (type == typeof(string))
                {
                    // int length = br.ReadInt32();
                    value = br.ReadString();
                }
                else
                {
                    value = null;
                }
                
                fieldInfo.SetValue(component, value);
            }
        }
    
        public static FieldInfo[] GetFieldInfos(Type componentType)
        {
            if (!ComponentTypeFieldInfos.TryGetValue(componentType, out var fieldInfos))
            {
                ComponentTypeFieldInfos[componentType] = componentType.GetFields().Where(x => x.DeclaringType == componentType).ToArray();
                fieldInfos = ComponentTypeFieldInfos[componentType];
            }

            return fieldInfos;
        }
    }
}