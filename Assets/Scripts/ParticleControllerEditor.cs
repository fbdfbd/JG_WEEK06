#if UNITY_EDITOR
// ⚠️ 반드시 Editor 폴더 안에 위치시킬 것
// Assets/Editor/ParticleControllerEditor.cs

using UnityEditor;
using UnityEngine;
using GameParticle;

namespace GameParticle.Editor
{
    [CustomEditor(typeof(ParticleController))]
    public class ParticleControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 기존 인스펙터 그대로 표시
            DrawDefaultInspector();

            EditorGUILayout.Space(8);

            var controller = (ParticleController)target;

            // Play Mode에서만 버튼 활성화
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            if (GUILayout.Button("▶ Play", GUILayout.Height(30)))
                controller.Play();

            EditorGUI.EndDisabledGroup();

            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Play Mode에서만 동작합니다.", MessageType.None);
        }
    }
}
#endif