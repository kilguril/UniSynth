using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2;
using UniSynth2.Editor.Windows;

namespace UniSynth2.Editor
{
	public class NodeGUIBase
	{	
		public enum NodeConnectorType
		{
			DATA_IN,
			DATA_OUT
		}
		
		public ISynthGraphNode DataNode
		{
			get { return m_dataNode; }
		}
		
		public delegate void NodeSelected( NodeGUIBase node );
		public static event NodeSelected OnNodeSelected;
		
		public delegate void NodeConnectorSelected( NodeGUIBase node, int connectorIndex, NodeConnectorType connectorType );
		public static event NodeConnectorSelected OnNodeConnectorSelected;
	
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
		
		// Validate connection / prevent loopbacks		
		// If valid, apply
		// Return result
		public bool AddConnection( SynthGraphEditorGraphConnection connection )
		{
			if ( ValidateConnection( connection ) )
			{
				DataNode.SetSourceNode( connection.m_from.DataNode, connection.m_toSlot );
				return true;
			}
		
			return false;
		}
		
		private bool ValidateConnection( SynthGraphEditorGraphConnection connection )
		{
			if ( connection.m_from.DataNode == DataNode )
			{
				return false;	// Loopback
			}
			
			if ( connection.m_toSlot >= DataNode.GetSourceCount() )
			{
				return false;	// Out of input range
			}
			
			return true;
		}
		
		public Vector2 GetInputHandle( int index )
		{
			int primaryInputs = m_dataNode.GetSourceCount() - GetSecondaryInputCount();
		
			Vector2 output;			
			
			if ( index < primaryInputs )
			{
				float slotSize = ( ( m_rect.width ) / primaryInputs );
				output.x = m_rect.x + ( slotSize * index ) + ( slotSize / 2.0f );
				output.y = m_rect.y - ( SynthGraphEditorFactory.NODE_HEADER_HEIGHT / 2.0f );
			}
			else
			{
				int secondaryIndex = index - primaryInputs;
			
				output.x = m_rect.x - SynthGraphEditorFactory.NODE_CONNECTOR_SIZE;
				output.y = m_rect.y + SynthGraphEditorFactory.NODE_HEADER_HEIGHT + SynthGraphEditorFactory.NODE_HEADER_HEIGHT + ( SynthGraphEditorFactory.NODE_CONNECTOR_SIZE / 2.0f ) + ( SynthGraphEditorFactory.NODE_LINE_HEIGHT * 2.0f * secondaryIndex );
			}
			
			return output;
		}
		
		public Vector2 GetOutputHandle()
		{
			Vector2 output;
			
			output.x = m_rect.x + ( m_rect.width / 2.0f );
			output.y = m_rect.y + m_rect.height + ( SynthGraphEditorFactory.SPLIT_VIEW_HANDLE_SIZE / 2.0f );
			
			return output;
		}
		
		public virtual Rect GetDefaultRect()
		{
			return new Rect( 0.0f, 0.0f, SynthGraphEditorFactory.NODE_WIDTH, SynthGraphEditorFactory.NODE_HEADER_HEIGHT);
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
			
			Rect headerRect = m_rect;
			headerRect.y += 3.0f;
			
			SynthGraphEditorFactory.HorizontalCenteredLabel( headerRect, GetTitle() );
			
			Rect contentRect 	= m_rect;
			contentRect.y 		+= SynthGraphEditorFactory.NODE_HEADER_HEIGHT;
			contentRect.height	-= SynthGraphEditorFactory.NODE_HEADER_HEIGHT;
			contentRect.x		+= SynthGraphEditorFactory.NODE_PADDING;
			contentRect.width	-= SynthGraphEditorFactory.NODE_PADDING * 2.0f;
			
			GUILayout.BeginArea( contentRect );		
			DrawNode();
			GUILayout.EndArea();
			
			if ( m_dataNode != null )
			{				
				int secondaryInputs = GetSecondaryInputCount();
				int primaryInputs   = m_dataNode.GetSourceCount() - secondaryInputs;
			
				if ( ShouldDrawInputHandles() )
				{
					Rect[] controls = new Rect[ primaryInputs ];
				
					for ( int i = 0; i < controls.Length; i++ )
					{
						Vector2 handle = GetInputHandle( i );
						
						controls[ i ].x = handle.x - SynthGraphEditorFactory.NODE_CONNECTOR_SIZE / 2.0f;
						controls[ i ].y = handle.y - SynthGraphEditorFactory.NODE_CONNECTOR_SIZE / 2.0f;
						controls[ i ].width  = SynthGraphEditorFactory.NODE_CONNECTOR_SIZE;
						controls[ i ].height = SynthGraphEditorFactory.NODE_CONNECTOR_SIZE;
					}
			
					int conn = SynthGraphEditorFactory.ConnectorGroup( controls );
					
					if ( conn > SynthGraphEditorFactory.BUTTON_GROUP_NO_INPUT )
					{
						RaiseNodeConnectorSelected( this, conn, NodeConnectorType.DATA_IN );
					}
				}
				
				if ( ShouldDrawOutputHandles() )
				{
					Rect[] controls = new Rect[ 1 ];
					
					Vector2 handle = GetOutputHandle();
					
					controls[ 0 ].x = handle.x - SynthGraphEditorFactory.NODE_CONNECTOR_SIZE / 2.0f;
					controls[ 0 ].y = handle.y - SynthGraphEditorFactory.NODE_CONNECTOR_SIZE / 2.0f;
					controls[ 0 ].width  = SynthGraphEditorFactory.NODE_CONNECTOR_SIZE;
					controls[ 0 ].height = SynthGraphEditorFactory.NODE_CONNECTOR_SIZE;
				
					int conn = SynthGraphEditorFactory.ConnectorGroup( controls );
					
					if ( conn > SynthGraphEditorFactory.BUTTON_GROUP_NO_INPUT )
					{
						RaiseNodeConnectorSelected( this, conn, NodeConnectorType.DATA_OUT );
					}
				}
				
				if ( secondaryInputs > 0 )
				{
					Rect[] controls = new Rect[ secondaryInputs ];	
									
					for ( int i = 0; i < controls.Length; i++ )
					{
						Vector2 handle = GetInputHandle( i + primaryInputs );
						
						controls[ i ].x = handle.x - SynthGraphEditorFactory.NODE_CONNECTOR_SIZE / 2.0f;
						controls[ i ].y = handle.y - SynthGraphEditorFactory.NODE_CONNECTOR_SIZE / 2.0f;
						controls[ i ].width  = SynthGraphEditorFactory.NODE_CONNECTOR_SIZE;
						controls[ i ].height = SynthGraphEditorFactory.NODE_CONNECTOR_SIZE;
					}
					
					int secondary = SynthGraphEditorFactory.ConnectorGroup( controls );
					
					if ( secondary > SynthGraphEditorFactory.BUTTON_GROUP_NO_INPUT )
					{
						RaiseNodeConnectorSelected( this, secondary + primaryInputs, NodeConnectorType.DATA_IN );
					}
				}
			}
			
			ProcessInput();
		}
		
		public void Select()
		{
			if ( OnNodeSelected != null )
			{
				OnNodeSelected( this );
			}
		}

		protected virtual void DrawNode()
		{

		}

		protected virtual bool ShouldDrawInputHandles()
		{
			return true;
		}
		
		protected virtual bool ShouldDrawOutputHandles()
		{
			return true;
		}

		protected virtual int GetSecondaryInputCount()
		{
			return 0;
		}

		private void ProcessInput()
		{
			Event e = Event.current;
		
			switch( e.type )
			{
				case EventType.ContextClick:
					if ( m_rect.Contains ( e.mousePosition ) )
					{
						Select();
						e.Use();
					}
				break;
			
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
		
		private static void RaiseNodeConnectorSelected( NodeGUIBase node, int connectorIndex, NodeConnectorType connectorType )
		{
			if ( OnNodeConnectorSelected != null )
			{
				OnNodeConnectorSelected( node, connectorIndex, connectorType );
			}
		}
	}
}