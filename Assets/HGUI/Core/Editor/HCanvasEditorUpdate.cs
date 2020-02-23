using huqiang.Core.HGUI;
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HCanvasEditorUpdate
{
    static HCanvasEditorUpdate()
    {
        //var type = Types.GetType("UnityEditor.EditorAssemblies", "UnityEditor.dll");
        //var method = type.GetMethod("SubclassesOf", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null);
        //var e = method.Invoke(null, new object[] { typeof(EditorMonoBehaviour) }) as IEnumerable;
        //foreach (Type editorMonoBehaviourClass in e)
        //{
        //    method = editorMonoBehaviourClass.BaseType.GetMethod("OnEditorMonoBehaviour", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (method != null)
        //    {
        //        method.Invoke(System.Activator.CreateInstance(editorMonoBehaviourClass), new object[0]);
        //    }
        //}
        EditorApplication.update += Update;
        //EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
        //EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        //EditorApplication.projectWindowChanged += OnProjectWindowChanged;
        //EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        //EditorApplication.modifierKeysChanged += OnModifierKeysChanged;
        //EditorApplication.searchChanged += OnSearchChanged;
        //EditorApplication.playmodeStateChanged += () => {
        //    if (EditorApplication.isPaused)
        //    {
        //        OnPlaymodeStateChanged(PlayModeState.Paused);
        //    }
        //    if (EditorApplication.isPlaying)
        //    {
        //        OnPlaymodeStateChanged(PlayModeState.Playing);
        //    }
        //    if (EditorApplication.isPlayingOrWillChangePlaymode)
        //    {
        //        OnPlaymodeStateChanged(PlayModeState.PlayingOrWillChangePlayMode);
        //    }
        //};
    }
    static float time;
    static void Update()
    {
        time += Time.deltaTime;
        if(time>1)
        {
            time = 0;
            var objs = SceneAsset.FindObjectsOfType(typeof(UICompositeHelp));
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    var obj = objs[i] as UICompositeHelp;
                    if (obj != null)
                        obj.Refresh();
                }
            }
            objs = SceneAsset.FindObjectsOfType(typeof(HCanvas));
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    var obj = objs[i] as HCanvas;
                    if (obj != null)
                        obj.Refresh();
                }
            }
        }
    }

    public enum PlayModeState
    {
        Playing,
        Paused,
        Stop,
        PlayingOrWillChangePlayMode
    }
}