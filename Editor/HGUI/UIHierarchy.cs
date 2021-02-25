using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class UIHierarchy : EditorWindow
{
	public static UIElement SelectNode;
	public static void ChangeRoot(string guid, UIElement ui)
    {
		if (ui == null)
			return;
		Root = ui;
		SelectNode = null;
	}
	static void Save(string guid, UIElement root)
    {
		var di = HistoryManager.GetDataInfo(guid,true,typeof(HCanvas),typeof(HImage),typeof(TextBox),typeof(HText),
			typeof(HLine),typeof(HGraphics),typeof(UIElement));
        UnsafeDataWriter dw2 = new UnsafeDataWriter();
		var db = dw2.Write<HCanvas>(root, di);
		HistoryManager.AddRecord("ui", guid, db.ToBytes());
	}
	static UIElement Root;
	static string serach = "";
	private int treeIndex = 0;
	// Add menu named "My Window" to the Window menu
	[UnityEditor.MenuItem("HGUI/UIHierarchy")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        UIHierarchy window = (UIHierarchy)EditorWindow.GetWindow(typeof(UIHierarchy));
        window.Show();
    }

	static GUIStyle defStyle;
	static GUIStyle activeStyle;
	static GUIStyle selectStyle;
	static GUIStyle pointerStyle;
	static GUIStyle dockStyle;
	Vector2 pos;
	Vector2 mousePosition;
	bool mouseDown;
	bool Draing;
	List<UIElement> serachs = new List<UIElement>();
	UIElement DockNode;
	int dockPos;
	void OnGUI()
    {
		if(defStyle==null)
        {
			defStyle = new GUIStyle();
			defStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
			defStyle.fixedHeight = 16;
			activeStyle = new GUIStyle();
			activeStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);
			activeStyle.fixedHeight = 16;

			Texture2D light = new Texture2D(1, 1);
			light.SetPixel(0, 0, new Color32(80, 80, 80, 255));
			light.Apply();
			selectStyle = new GUIStyle();
			selectStyle.normal.background = light;
			selectStyle.normal.textColor = Color.white;
			selectStyle.fixedHeight = 16;

			Texture2D dark = new Texture2D(1, 1);
			dark.SetPixel(0, 0, new Color32(32, 32, 32, 255));
			dark.Apply();
			pointerStyle = new GUIStyle();
			pointerStyle.normal.background = dark;
			pointerStyle.fixedHeight = 16;

			Texture2D dock = new Texture2D(1, 1);
			dock.SetPixel(0, 0, new Color32(0, 186, 255, 128));
			dock.Apply();
			dockStyle = new GUIStyle();
			dockStyle.normal.background = dock;
			dockStyle.fixedHeight = 16;
		}
		
		treeIndex = 0;
		var w = position.width;
		var h = position.height;
		var evt = Event.current;
		wantsMouseMove = true;
		if (wantsMouseMove)
			if (mousePosition != evt.mousePosition)
				Repaint();
		mousePosition = evt.mousePosition;
		var contextRect = new Rect(0, 16, w, h - 16);
		mouseDown = false;
		if (evt.button == 1)
		{
			var mousePos = evt.mousePosition;
			if (contextRect.Contains(mousePos))
			{
				CreateMenu();
				evt.Use();
			}
		}else if(evt.button==0)
        {
			if (evt.type == EventType.MouseDown)
			{
				mouseDown = true;
				SelectNode = null;
			}
			else if (evt.type == EventType.MouseDrag)
			{
				Draing = true;
			}
			else if (evt.type == EventType.MouseUp)
			{
				Draing = false;
				if (DockNode != null)
				{
					ChangeTree();
				}
			}
		}
		DockNode = null;
		string str = serach;
		serach = GUILayout.TextField(serach);
		if(serach=="")
        {
			pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Width(w), GUILayout.Height(h));
			if (Root != null)
				DrawTree(Root, 0, Root.activeSelf);
			GUILayout.BeginHorizontal();
			GUILayout.Box("",GUI.skin.label);
			GUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();
        }
        else
        {
			if (serach != str)
			{
				serachs.Clear();
				string con = serach.Replace(" ","");
				Serach(serachs, Root, con.ToLower());
			}
			DrawSerach(serachs);
		}
		if (Draing)
		{
			if (SelectNode != null)
			{
				var t2d = Resources.Load<Texture2D>("Butterfly");
				//Cursor.SetCursor(t2d, new Vector2(64, 64), CursorMode.Auto);
				var rect = new Rect(mousePosition, new Vector2(24, 24));
				rect.y += 6;
				GUI.DrawTexture(rect, t2d);
			}
        }
	}
	void Callback(System.Object obj)
	{
		var m = obj as MethodInfo;
		if(m!=null)
        {
			if(SelectNode==null)
            {
				if(Root!=null)
                {
					var r = m.Invoke(null, new[] { Root });
					var ui = r as UIElement;
					if (ui != null)
					{
						//ui.SetParent(Root);
					}
				}
            }
            else
            {
				var r = m.Invoke(null, new[] { SelectNode});
				var ui = r as UIElement;
				if (ui != null)
				{
					//ui.SetParent(SelectNode);
				}
			}
        }else
        {
			string str = obj as string;
            switch (str)
            {
				case "Delete":
					if(SelectNode.parent!=null)
                    {
						SelectNode.SetParent(null);
						SelectNode = null;
                    }
					break;
            }
        }
	}
	private void DrawTree(UIElement node, int level, bool p)
	{
		if (node == null)
		{
			return;
		}
		GUILayout.BeginHorizontal();
		var ui = node;
		if(mouseDown)
        {
			float a = treeIndex * 16 + 16;
			a -= pos.y;
			if (mousePosition.x >= 0 & mousePosition.x <= position.width)
				if (mousePosition.y >= a & mousePosition.y < a + 16)
					SelectNode = node;
        }
		if (node == SelectNode)
		{
			selectStyle.fixedWidth = position.width;
			GUILayout.Box("", selectStyle);
			GUILayout.Space(-position.width);
        }
        else
        {
			float a = treeIndex * 16 + 16;
			a -= pos.y;
			if(mousePosition.x>=0&mousePosition.x<=position.width)
				if(mousePosition.y>=a&mousePosition.y<a+16)
                {
					pointerStyle.fixedWidth = position.width;
					GUILayout.Box("", pointerStyle);
					GUILayout.Space(-position.width);
				}
        }
		if (Draing & SelectNode != null)
		{
			float a = treeIndex * 16 + 16;
			float b = a - pos.y;
			if (mousePosition.x >= 0 & mousePosition.x <= position.width)
            {
				if (mousePosition.y >= b & mousePosition.y < b + 16)
				{
					DockNode = node;
					if (mousePosition.y < b + 5)
					{
						dockPos = 0;
						dockStyle.fixedHeight = 2;
						dockStyle.fixedWidth = position.width;
						GUI.Box(new Rect(0, a - 18, position.width, 5), "", dockStyle);
					}
					else if (mousePosition.y < b + 10)
					{
						dockPos = 1;
						dockStyle.fixedHeight = 16;
						pointerStyle.fixedWidth = position.width;
						GUI.Box(new Rect(0, a - 17, position.width, 16), "", dockStyle);
					}
					else
					{
						dockPos = 2;
						dockStyle.fixedHeight = 2;
						dockStyle.fixedWidth = position.width;
						GUI.Box(new Rect(0, a - 2, position.width, 5), "", dockStyle);
					}
				}
			}
		}
		GUIStyle style;
	    if(ui.activeSelf & p)
		{
			style = activeStyle;
		}
        else
        {
			p = false;
			style = defStyle;
		}
		
		GUILayout.Space(5 + 14 * level);
		treeIndex++;
		if ( node.child.Count>0)
		{
			if(node.expand)
            {
				style.fixedWidth = 16;
				if(GUILayout.Button("▼ ", style))
                {
					SelectNode = node;
					node.expand = false;
					UIInspector.ChangeUI(SelectNode);
				}
				style.fixedWidth = 0;
				if (GUILayout.Button(node.name, style))
				{
					SelectNode = node;
					UIInspector.ChangeUI(SelectNode);
				}
			}
            else
            {
				style.fixedWidth = 16;
				if (GUILayout.Button("► ", style))
                {
					SelectNode = node;
					node.expand = true;
					UIInspector.ChangeUI(SelectNode);
				}
				style.fixedWidth = 0;
				if (GUILayout.Button(node.name, style))
				{
					SelectNode = node;
					UIInspector.ChangeUI(SelectNode);
				}
			}
		}
		else
		{
			GUILayout.Space(16);
			if (GUILayout.Button(node.name, style))
            {
                SelectNode = node;
				UIInspector.ChangeUI(SelectNode);
			}
        }
		GUILayout.EndHorizontal();
		if (node == null || !node.expand || node.child.Count == 0)
		{
			return;
		}
		for (int i = 0; i < node.child.Count; i++)
		{
			DrawTree(node.child[i], level + 1, p);
		}
	}
	private void DrawSerach(List<UIElement> list)
    {
		for(int i=0;i<list.Count;i++)
        {
			GUILayout.BeginHorizontal();
			var node = list[i];
			if (node == SelectNode)
			{
				selectStyle.fixedWidth = position.width;
				GUILayout.Box("", selectStyle);
				GUILayout.Space(-position.width);
			}
			var ui = node;
			GUIStyle style;
			if(ui.activeSelf)
            {
				style = activeStyle;
			}
            else
            {
				style = defStyle;
            }
			GUILayout.Space(5);
			StringBuilder sb = new StringBuilder();
			var n = node;
			while (n.parent!=null)
            {
				sb.Insert(0,n.parent.name+"/");
				n = n.parent;
            }
			sb.Append(node.name);
			if (GUILayout.Button(sb.ToString(), style))
			{
				SelectNode = node;
			    while(node.parent!=null)
                {
					node.expand = true;
					node = node.parent;
                }
				node.expand = true;
				UIInspector.ChangeUI(SelectNode);
			}
			GUILayout.EndHorizontal();
		}
    }
	void Serach(List<UIElement>list ,UIElement node, string con)
    {
		if (node.name.ToLower().Contains(con))
			list.Add(node);
		for (int i = 0; i < node.child.Count; i++)
			Serach(list,node.child[i],con);
    }
    void CreateMenu()
    {
		var type = typeof(CompositeMenu);
		var methods = type.GetMethods();
		var menu = new GenericMenu();

		for (int i=0;i<methods.Length;i++)
        {
			var attr = methods[i].GetCustomAttributes(typeof(UnityEditor.MenuItem),true);
			if(attr!=null)
            {
				if(attr.Length>0)
                {
					string content = (attr[0] as UnityEditor.MenuItem).menuItem;
					string str =content.Replace("HGUI/","");
					menu.AddItem(new GUIContent(str), false, Callback, methods[i]);
				}
            }
        }
		menu.AddItem(new GUIContent("Delete"), false, Callback,"Delete");
		menu.ShowAsContext();
	}
	void ChangeTree()
    {
		if(SelectNode!=null)
        {
			if (SelectNode.parent == null)
				return;
			if (SelectNode == DockNode)
				return;
			if (DockNode.parent == null)
				dockPos = 1;
			switch(dockPos)
            {
				case 0:
					SelectNode.SetParent(DockNode.parent);
					int index = DockNode.GetSiblingIndex();
					SelectNode.SetSiblingIndex(DockNode.GetSiblingIndex());
					var p = DockNode.parent;
					var son = SelectNode;
					son.SetParent(p);
					son.SetSiblingIndex(index);
					break;
				case 1:
					SelectNode.SetParent(DockNode);
					break;
				case 2:
					SelectNode.SetParent(DockNode.parent);
					index = DockNode.GetSiblingIndex();
					SelectNode.SetSiblingIndex(index +1);
					p = DockNode.parent;
					son = SelectNode;
					son.SetParent(p);
					son.SetSiblingIndex(index+1);
					break;
            }
        }
    }
}
