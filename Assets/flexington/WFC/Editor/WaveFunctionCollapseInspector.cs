using UnityEngine;
using UnityEditor;

namespace flexington.WFC
{
    [CustomEditor(typeof(WaveFunctionCollapseComponent))]
    public class WaveFunctionCollapseInspector : Editor
    {
        public WaveFunctionCollapseComponent Target => (WaveFunctionCollapseComponent)target;

        private Event _event;

        private void Awake()
        {
            if (_event == null) _event = Event.current;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Generate and Simulate"))
            {
                Target.Start();
            }
            if (GUILayout.Button("Step"))
            {
                Target.Editor_Step();
            }
            if (GUILayout.Button("Reset"))
            {
                Target.Reset();
            }
            EditorGUILayout.EndVertical();
        }
    }
}