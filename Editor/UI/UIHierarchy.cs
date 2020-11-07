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

	void OnGUI()
    {
		treeIndex = 0;
		if (Root != null)
			DrawFileTree(Root, 0);
	}
	private void DrawFileTree(TreeViewNode node, int level)
	{
		if (node == null)
		{
			return;
		}
		GUIStyle style = new GUIStyle();
		style.normal.background = null;
		style.normal.textColor = new Color(0.7f,0.7f,0.7f,1);
		if (node == currentNode)
		{
			style.normal.textColor = Color.red;
		}

		Rect rect = new Rect(5 + 20 * level, 5 + 16 * treeIndex, node.content.Length * 25, 20);
		treeIndex++;

		if ( node.child.Count>0)
		{
			//node.expand = EditorGUI.Foldout(rect, node.expand, node.content, true);
			if(node.expand)
            {
				if (GUI.Button(rect, "▼ " + node.content, style))
				{
					currentNode = node;
					node.expand = false;
					Debug.Log(node.content);
				}
			}
            else
            {
				if (GUI.Button(rect, "► "+ node.content, style))
				{
					currentNode = node;
					node.expand = true;
					Debug.Log(node.content);
				}
			}
		
		}
		else
		{
            //node.content  = GUI.TextArea(rect, node.content, style);
            if (GUI.Button(rect, node.content, style))
            {
                Debug.Log(node.content);
                currentNode = node;
            }
            //EditorGUI.Foldout(rect, true, node.content, true);
        }

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
