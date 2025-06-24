using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace Badbarbos
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using ICENet.Traffic;

    [Serializable]
    public class FieldReferenceEntry
    {
        public GameObject TargetObject;

        public Component Component;

        public string FieldName;

        public bool Smooth;

        public float SmoothSpeed = 1f;
    }

    public class EntityFieldCollector : MonoBehaviour
    {
        public List<FieldReferenceEntry> Entries = new List<FieldReferenceEntry>();

        Dictionary<FieldReferenceEntry, Coroutine> _coros = new Dictionary<FieldReferenceEntry, Coroutine>();

        public void SerializeFields(ICENet.Traffic.Buffer buffer)
        {
            buffer.Write((ushort)Entries.Count);
            foreach (var e in Entries)
            {
                var t = GetMemberType(e);
                var v = GetMemberValue(e);
                if (t == typeof(int)) buffer.Write((int)v);
                else if (t == typeof(float)) buffer.Write((float)v);
                else if (t == typeof(string)) buffer.Write((string)v);
                else if (t == typeof(bool)) buffer.Write(Convert.ToByte(v));
                else if (t == typeof(Vector3)) { buffer.Write(((Vector3)v).x); buffer.Write(((Vector3)v).y); buffer.Write(((Vector3)v).z); }
            }
        }

        public void DeserializeFields(ICENet.Traffic.Buffer buffer)
        {          
            ushort count = buffer.ReadUInt16();
            int len = Mathf.Min(count, (ushort)Entries.Count);
            for (int i = 0; i < len; i++)
            {
                var e = Entries[i];
                var comp = e.Component;
                var t = GetMemberType(e);
                object val = null;
                if (t == typeof(int)) val = buffer.ReadInt32();
                else if (t == typeof(float)) val = buffer.ReadSingle();
                else if (t == typeof(string)) val = buffer.ReadString();
                else if (t == typeof(bool)) val = Convert.ToBoolean(buffer.ReadByte());
                else if (t == typeof(Vector3))
                {
                    val = new Vector3(
                        buffer.ReadSingle(),
                        buffer.ReadSingle(),
                        buffer.ReadSingle()
                    );
                }

                if (e.Smooth && (t == typeof(float) || t.IsValueType))
                {
                    if (_coros.TryGetValue(e, out var c)) StopCoroutine(c);
                    _coros[e] = StartCoroutine(SmoothApply(e, val));
                }
                else
                {
                    ApplyValue(e, val);
                }
            }
        }

        IEnumerator SmoothApply(FieldReferenceEntry e, object target)
        {
            var comp = e.Component;
            var t = GetMemberType(e);
            var fi = comp.GetType().GetField(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var pi = fi == null ? comp.GetType().GetProperty(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) : null;

            if (t == typeof(float))
            {
                float from = (float)GetMemberValue(e);
                float to = (float)target;
                float d = 0;
                while (d < 1f)
                {
                    d += Time.deltaTime * e.SmoothSpeed;
                    float cur = Mathf.Lerp(from, to, d);
                    if (fi != null) fi.SetValue(comp, cur);
                    else pi.SetValue(comp, cur);
                    yield return null;
                }
            }
            else if (t == typeof(Vector3))
            {
                Vector3 from = (Vector3)GetMemberValue(e);
                Vector3 to = (Vector3)target;
                float d = 0;
                while (d < 1f)
                {
                    d += Time.deltaTime * e.SmoothSpeed;
                    Vector3 cur = Vector3.Lerp(from, to, d);
                    if (fi != null) fi.SetValue(comp, cur);
                    else pi.SetValue(comp, cur);
                    yield return null;
                }
            }
            else if (t == typeof(Vector2))
            {
                Vector2 from = (Vector2)GetMemberValue(e);
                Vector2 to = (Vector2)target;
                float d = 0;
                while (d < 1f)
                {
                    d += Time.deltaTime * e.SmoothSpeed;
                    Vector2 cur = Vector2.Lerp(from, to, d);
                    if (fi != null) fi.SetValue(comp, cur);
                    else pi.SetValue(comp, cur);
                    yield return null;
                }
            }
            else if (t == typeof(Vector4))
            {
                Vector4 from = (Vector4)GetMemberValue(e);
                Vector4 to = (Vector4)target;
                float d = 0;
                while (d < 1f)
                {
                    d += Time.deltaTime * e.SmoothSpeed;
                    Vector4 cur = Vector4.Lerp(from, to, d);
                    if (fi != null) fi.SetValue(comp, cur);
                    else pi.SetValue(comp, cur);
                    yield return null;
                }
            }
            else
            {
                ApplyValue(e, target);
            }
            _coros.Remove(e);
        }

        void ApplyValue(FieldReferenceEntry e, object v)
        {
            var comp = e.Component;
            var f = comp.GetType().GetField(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null) f.SetValue(comp, v);
            else
            {
                var p = comp.GetType().GetProperty(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (p != null && p.CanWrite) p.SetValue(comp, v);
            }
        }

        Type GetMemberType(FieldReferenceEntry e)
        {
            if (e == null || e.Component == null || string.IsNullOrEmpty(e.FieldName)) return null;
            var t = e.Component.GetType();
            var fi = t.GetField(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null) return fi.FieldType;
            var pi = t.GetProperty(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi != null) return pi.PropertyType;
            return null;
        }

        object GetMemberValue(FieldReferenceEntry e)
        {
            if (e == null || e.Component == null || string.IsNullOrEmpty(e.FieldName)) return null;
            var t = e.Component.GetType();
            var fi = t.GetField(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null) return fi.GetValue(e.Component);
            var pi = t.GetProperty(e.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi != null) return pi.GetValue(e.Component);
            return null;
        }
    }
}