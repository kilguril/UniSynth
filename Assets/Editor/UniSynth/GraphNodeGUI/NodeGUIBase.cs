using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2;

namespace UniSynth2.Editor
{
	public class NodeGUIBase
	{	
		protected GUIStyle		m_style;
	
		private bool			m_isDragged;
		private Vector2			m_dragOffset;
	
		protected Rect			m_rect;
		protected ISynthGraphNode	m_dataNode;	// Graph data node this GUI represents
	
		public NodeGUIBase( ISynthGraphNode dataNode )
		{
			m_dataNode = dataNode;
			
			m_isDragged  = false;
			m_dragOffset = Vector2.zero;
			
			SetRect( GetDefaultRect() );
		}
		
		public virtual Rect GetDefaultRect()
		{
			return new Rect( 0.0f, 0.0f, SynthGraphEditorFactory.NODE_WIDTH, SynthGraphEditorFactory.NODE_LINE_HEIGHT);
		}
		
		public virtual string GetTitle()
		{
			return "Node";
		}
		
		public void SetRect( Rect rect )
		{
			m_rect = rect;
		}
		
		public void SetRect( float x, float y, float w, float h )
		{
			m_rect.x 	  = x;
			m_rect.y 	  = y;
			m_rect.width  = w;
			m_rect.height = h;
		}
	
		public void Draw()
		{	
			GUI.Box( m_rect, "" );
			GUILayout.BeginArea( m_rect );
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( GetTitle() );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			DrawNode();
			GUILayout.EndArea();
			ProcessInput();
		}
		
		protected virtual void DrawNode()
		{
		
		}
		
		private void ProcessInput()
		{
			Event e = Event.current;
		
			switch( e.type )
			{
				case EventType.MouseDown:
					if ( m_rect.Contains ( e.mousePosition ) )
					{
						m_dragOffset = e.mousePosition - new Vector2( m_rect.x, m_rect.y );
						m_isDragged  = true;
					
						e.Use();
					}
				break;
				
				case EventType.MouseDrag:
					if ( m_isDragged )
					{
						Vector2 newPos = e.mousePosition - m_dragOffset;
						m_rect.x = newPos.x;
						m_rect.y = newPos.y;
																
						e.Use();
					}				
				break;
				
				case EventType.MouseUp:				
					m_isDragged = false;				
				break;
			}
		
		}
	}
}