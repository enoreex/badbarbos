using UnityEditor;
using UnityEngine;

using System.Reflection;
using System.Linq;

namespace Badbarbos
{
    [CustomPropertyDrawer(typeof(FieldReferenceEntry))]
    public class FieldReferenceEntryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float lh = EditorGUIUtility.singleLineHeight;
            float spacing = 2f;
            Rect rect = new Rect(position.x, position.y, position.width, lh);

            var targetProp = property.FindPropertyRelative("TargetObject");
            EditorGUI.PropertyField(rect, targetProp);

            rect.y += lh + spacing;
            var compProp = property.FindPropertyRelative("Component");
            Component currentComp = compProp.objectReferenceValue as Component;
            GameObject go = targetProp.objectReferenceValue as GameObject;
            var comps = go != null ? go.GetComponents<Component>() : new Component[0];
            string[] compNames = comps.Select(c => c.GetType().Name).ToArray();
            int idx = System.Array.IndexOf(comps, currentComp);
            if (comps.Length > 0)
            {
                int sel = EditorGUI.Popup(rect, "Component", idx, compNames);
                if (sel != idx)
                {
                    compProp.objectReferenceValue = comps[sel];
                    property.FindPropertyRelative("FieldName").stringValue = string.Empty;
                    currentComp = comps[sel];
                }
            }
            else
            {
                EditorGUI.LabelField(rect, "Component", "None");
                compProp.objectReferenceValue = null;
                property.FindPropertyRelative("FieldName").stringValue = string.Empty;
                currentComp = null;
            }

            rect.y += lh + spacing;
            var fieldProp = property.FindPropertyRelative("FieldName");
            string[] members = currentComp != null
                ? currentComp.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                                .Select(m => m.Name).ToArray()
                : new string[0];
            int midx = System.Array.IndexOf(members, fieldProp.stringValue);
            if (members.Length > 0)
            {
                int msel = EditorGUI.Popup(rect, "Member", midx, members);
                if (msel != midx)
                    fieldProp.stringValue = members[msel];
            }
            else
            {
                EditorGUI.LabelField(rect, "Member", "None");
                fieldProp.stringValue = string.Empty;
            }

            rect.y += lh + spacing;
            string typeName = "None";
            if (currentComp != null && !string.IsNullOrEmpty(fieldProp.stringValue))
            {
                var t = currentComp.GetType();
                var fi = t.GetField(fieldProp.stringValue, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi != null) typeName = fi.FieldType.Name;
                else
                {
                    var pi = t.GetProperty(fieldProp.stringValue, BindingFlags.Public | BindingFlags.Instance);
                    typeName = pi != null ? pi.PropertyType.Name : "Unknown";
                }
            }
            EditorGUI.LabelField(rect, "Type", typeName);

            rect.y += lh + spacing;
            var smoothProp = property.FindPropertyRelative("Smooth");
            EditorGUI.PropertyField(rect, smoothProp);

            rect.y += lh + spacing;
            var speedProp = property.FindPropertyRelative("SmoothSpeed");
            EditorGUI.PropertyField(rect, speedProp);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lh = EditorGUIUtility.singleLineHeight;
            float spacing = 2f;
            return lh * 6 + spacing * 5;
        }
    }
}