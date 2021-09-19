using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AffenECS
{
    public static class SerializationUtils
    {
        private static readonly Dictionary<Type, FieldInfo[]> ComponentTypeFieldInfos = new Dictionary<Type, FieldInfo[]>();
        private static readonly Dictionary<Type, ushort> SerializedTypesByType = new Dictionary<Type, ushort>();
        private static readonly Dictionary<ushort, Type> SerializedTypesByIndex = new Dictionary<ushort, Type>();
        
        static SerializationUtils()
        {
            var ecsComponentTypes = EcsTypes.ComponentTypes.OrderBy(x => x.FullName);

            ushort index = 0;
            foreach (Type ecsComponentType in ecsComponentTypes)
            {
                SerializedTypesByType.Add(ecsComponentType, index);
                SerializedTypesByIndex.Add(index, ecsComponentType);
                index++;
            }
        }
        
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
                    var chars = ((string) value).ToCharArray();
                    bw.Write(chars.Length);
                    bw.Write(chars);
                }
                else if (type == typeof(Vector2))
                {
                    var typedValue = (Vector2) value;
                    bw.Write(typedValue.x);
                    bw.Write(typedValue.y);
                }
                else if (type == typeof(Vector3))
                {
                    var typedValue = (Vector3) value;
                    bw.Write(typedValue.x);
                    bw.Write(typedValue.y);
                    bw.Write(typedValue.z);
                }
                else if (type == typeof(Vector4))
                {
                    var typedValue = (Vector4) value;
                    bw.Write(typedValue.x);
                    bw.Write(typedValue.y);
                    bw.Write(typedValue.z);
                    bw.Write(typedValue.w);
                }
                else if (type == typeof(Quaternion))
                {
                    var typedValue = (Quaternion) value;
                    bw.Write(typedValue.x);
                    bw.Write(typedValue.y);
                    bw.Write(typedValue.z);
                    bw.Write(typedValue.w);
                }
                else if (type == typeof(Color))
                {
                    var typedValue = (Color) value;
                    bw.Write(typedValue.r);
                    bw.Write(typedValue.g);
                    bw.Write(typedValue.b);
                    bw.Write(typedValue.a);
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
                object value = null;
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
                    int length = br.ReadInt32();
                    value = new string(br.ReadChars(length));
                }
                else if (type == typeof(Vector2))
                {
                    value = new Vector2(br.ReadSingle(), br.ReadSingle());
                }
                else if (type == typeof(Vector3))
                {
                    value = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }
                else if (type == typeof(Vector4))
                {
                    value = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }
                else if (type == typeof(Quaternion))
                {
                    value = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }
                else if (type == typeof(Color))
                {
                    value = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }
                
                fieldInfo.SetValue(component, value);
            }
            
            ms.Dispose();
            br.Dispose();
        }
        
        private static ushort SerializeType(this EcsComponent ecsComponent)
        {
            return SerializedTypesByType[ecsComponent.GetType()];
        }

        private static Type DeserializeType(this ushort ecsComponentSerializedTypeIndex)
        {
            return SerializedTypesByIndex[ecsComponentSerializedTypeIndex];
        }

        private static FieldInfo[] GetFieldInfos(Type componentType)
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