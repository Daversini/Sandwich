#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager))]
public class SoundManager_Inspector : Editor
{
    private AudioClipsData m_AudioClipsData;

    private void Awake()
    {
        m_AudioClipsData = Resources.Load<AudioClipsData>("Audio Clips Datas");
        EditorUtility.SetDirty(m_AudioClipsData);
    }

    public override void OnInspectorGUI()
    {
        DrawClipsList();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+", GUILayout.Width(30f)))
                QueueNewClip();
            if (GUILayout.Button("-", GUILayout.Width(30f)) && m_AudioClipsData.AudioClips.Count > 0)
                DequeueLastClip();
        }
            
    }

    private void DrawClipsList()
    {
        if (m_AudioClipsData.AudioClips != null)
        {
            for (int i = 0; i < m_AudioClipsData.AudioClips.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    m_AudioClipsData.AudioClips[i] = EditorGUILayout.ObjectField(m_AudioClipsData.AudioClips[i], typeof(AudioClip), false) as AudioClip;
                    m_AudioClipsData.AudioClipsID[i] = EditorGUILayout.TextField(m_AudioClipsData.AudioClipsID[i], GUILayout.Width(100f));
                }

            }
        }
    }

    private void QueueNewClip()
    {
        if (m_AudioClipsData.AudioClips == null)
        {
            m_AudioClipsData.AudioClips = new List<AudioClip>();
            m_AudioClipsData.AudioClipsID = new List<string>();
        }

        m_AudioClipsData.AudioClips.Add(null);
        m_AudioClipsData.AudioClipsID.Add(null);
    }

    private void DequeueLastClip()
    {
        m_AudioClipsData.AudioClips.RemoveAt(m_AudioClipsData.AudioClips.Count - 1);
        m_AudioClipsData.AudioClipsID.RemoveAt(m_AudioClipsData.AudioClipsID.Count - 1);
    }
}
#endif
