using huqiang.Core.HGUI;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditor;
using UnityEditor;
using UnityEngine;

public class UIHierarchy : EditorWindow
{
	public static TreeViewNode currentNode;
	public static void ChangeRoot(Transform transform)
    {
		if (transform != null)
			Root = CreateNodeChild(transform);
	}
	static TreeViewNode Root;
	private int treeIndex = 0;
	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/UIHierarchy")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        UIHierarchy window = (UIHierarchy)EditorWindow.GetWindow(typeof(UIHierarchy));
        window.Show();
    }
	Vector2 pos;
	GUIStyle defStyle;
	GUIStyle selectStyle;
	void OnGUI()
    {
		defStyle = new GUIStyle();
		defStyle.normal.background = null;
		defStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
		selectStyle = new GUIStyle();
		selectStyle.normal.background = null;
		selectStyle.normal.textColor = Color.white;

		treeIndex = 0;
		var w = position.width;
		var h = position.height;
		//EditorGUILayout.BeginVertical();
		pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Width(w), GUILayout.Height(h));
        if (Root != null)
            DrawFileTree(Root, 0);
        //GUILayout.Label("ttt",GUILayout.Width(w),GUILayout.Height(h+200));
        EditorGUILayout.EndScrollView();
		//EditorGUILayout.EndVertical();
		var evt = Event.current;
		var contextRect = new Rect(0, 0, 1000, 1000);

		if (evt.type == UnityEngine.EventType.ContextClick)
		{
			var mousePos  = evt.mousePosition;
			if (contextRect.Contains(mousePos))
			{
				// Now create the menu, add items and show it
				var menu  = new GenericMenu();

				menu.AddItem(new GUIContent("MenuItem1"), false, Callback, "item 1");
				menu.AddItem(new GUIContent("MenuItem2"), false, Callback, "item 2");
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, Callback, "item 3");

				menu.ShowAsContext();

				evt.Use();
			}
		}
	}
	void Callback(System.Object obj)
	{
		Debug.Log("Selected: " + obj);
	}
	private void DrawFileTree(TreeViewNode node, int level)
	{
		if (node == null)
		{
			return;
		}
		GUILayout.BeginHorizontal();

		GUIStyle style;
		if (node == currentNode)
		{
			style = selectStyle;
		}
		else 
		{
			style = defStyle; 
		}
		GUILayout.Space(5 + 14 * level);
		treeIndex++;
		if ( node.child.Count>0)
		{
			if(node.expand)
            {
				
				if (GUILayout.Button( "▼ " + node.content, style))
				{
					currentNode = node;
					node.expand = false;
				}
			}
            else
            {
				if (GUILayout.Button("► "+ node.content, style))
				{
					currentNode = node;
					node.expand = true;
				}
			}
		}
		else
		{
            if (GUILayout.Button("  "+node.content, style))
            {
                currentNode = node;
            }
        }
		GUILayout.EndHorizontal();
		if (node == null || !node.expand || node.child.Count == 0)
		{
			return;
		}
		for (int i = 0; i < node.child.Count; i++)
		{
			DrawFileTree(node.child[i], level + 1);
		}
	}
	static TreeViewNode CreateNodeChild(Transform part)
	{
		TreeViewNode node = new TreeViewNode();
		node.context = part;
		node.content = part.name;
		int c = part.childCount;
		for (int i = 0; i < c; i++)
			node.Add(CreateNodeChild(part.GetChild(i)));
		return node;
	}
}
