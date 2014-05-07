using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UniSynth2;
using UniSynth2.Editor;
using UniSynth2.Editor.Utils;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorGraphWindow : SynthGraphEditorWindow
	{
		private bool		m_dirty;
		
		private SoundClip	m_cachedClip;
	
		private Dictionary< ISynthGraphNode, NodeGUIBase > m_graphCache;
		private Dictionary< ISynthGraphNode, NodeGUIBase > m_floatingSelection;	// Unconnected nodes
		
		private SynthGraphEditorGraphConnection m_cursorConnection;
		private List<SynthGraphEditorGraphConnection> m_connectionCache;
	
		public SynthGraphEditorGraphWindow( int windowId, string windowTitle, Rect windowRect ) : base( windowId, windowTitle, windowRect )
		{
			m_graphCache		= new Dictionary<ISynthGraphNode, NodeGUIBase>();
			m_floatingSelection = new Dictionary<ISynthGraphNode, NodeGUIBase>();
			
			NodeGUIBase.OnNodeConnectorSelected += HandleNodeConnectorSelected;
			
			m_cursorConnection = new SynthGraphEditorGraphConnection();
			m_connectionCache = new List<SynthGraphEditorGraphConnection>();
			
			SetDirty( true );
		}

		void HandleNodeConnectorSelected (NodeGUIBase node, int connectorIndex, NodeGUIBase.NodeConnectorType connectorType)
		{
			switch( connectorType )
			{
				case NodeGUIBase.NodeConnectorType.DATA_IN:
					m_cursorConnection.SetTo( node, connectorIndex );
				break;
				
				case NodeGUIBase.NodeConnectorType.DATA_OUT:
					m_cursorConnection.SetFrom( node );
				break;
			}	
			
			switch( m_cursorConnection.State )
			{
				case SynthGraphEditorGraphConnection.ConnectionState.NO_CONNECTED:
					SetRequiresContiniousUpdates( false );
				break;
			
				case SynthGraphEditorGraphConnection.ConnectionState.CONNECTING:
					SetRequiresContiniousUpdates( true );
				break;
				
				case SynthGraphEditorGraphConnection.ConnectionState.CONNECTED:
					SetRequiresContiniousUpdates( false );
					
					if ( m_cursorConnection.m_to.AddConnection( m_cursorConnection ) )
					{
						SetDirty( true );
					}
					
					m_cursorConnection = new SynthGraphEditorGraphConnection();
				break;
				
			}
		}
	
		protected override void Window (int windowId)
		{
			SoundClip clip = SynthGraphEditorState.ActiveClip;
					
			if ( ( clip != m_cachedClip ) || ( m_dirty ) )
			{
				if ( clip != null )
				{
					RebuildNodeGUICache( clip );
					RebuildConnectionCache( clip );
				}
			}
			
			foreach( NodeGUIBase control in m_graphCache.Values )
			{
				control.Draw();
			}
			
			foreach( NodeGUIBase control in m_floatingSelection.Values )
			{
				control.Draw();
			}
			
			SynthGraphEditorUtils.BeginLines( SynthGraphEditorFactory.COLOR_LINES );			
			
			m_cursorConnection.Draw();			
			
			foreach( SynthGraphEditorGraphConnection connection in m_connectionCache )
			{
				connection.Draw();
			}
			
			SynthGraphEditorUtils.EndLines();
		}	
		
		public void AddNode( ISynthGraphNode node )
		{
			m_floatingSelection.Add( node, SynthGraphEditorFactory.GetGUIControl( node ) );
		}
		
		private void SetDirty( bool dirty )
		{
			m_dirty = dirty;
		}
		
		private void RebuildConnectionCache( SoundClip clip )
		{
			Log ("Rebuilding connection cache");
			m_connectionCache.Clear();
			
			CacheConnection( clip.Root );	
			
			Log ( string.Format("Cached {0} connections" , m_connectionCache.Count ) );		
		}
		
		private void CacheConnection( ISynthGraphNode node )
		{
			ISynthGraphNode[] inputs = node.GetSourceNodes();
			
			for( int i = 0; i < inputs.Length; i++ )
			{
				if ( inputs[ i ] != null )
				{
					SynthGraphEditorGraphConnection connection = new SynthGraphEditorGraphConnection();
					connection.SetFrom( GetGUIForNode( inputs[ i ] ) );
					connection.SetTo( GetGUIForNode( node ), i );
										
					m_connectionCache.Add( connection );
				
					CacheConnection( inputs[ i ] );
				}
			}
		}
		
		private NodeGUIBase GetGUIForNode( ISynthGraphNode node )
		{
			if ( m_graphCache.ContainsKey( node ) )
			{
				return m_graphCache[ node ];
			}
			
			// Should probably assert here, since if a node is connect it should be cached...
			if ( m_floatingSelection.ContainsKey( node ) )
			{
				return m_floatingSelection[ node ];
			}
			
			return null;
		}
		
		private void RebuildNodeGUICache( SoundClip clip )
		{
			Log ("Rebuilding node cache");
			
			Dictionary< ISynthGraphNode, NodeGUIBase > cache = new Dictionary<ISynthGraphNode, NodeGUIBase>( m_graphCache );
			m_graphCache.Clear();
		
			CacheNode( clip.Root, cache );	
			
			m_cachedClip = clip;
			SetDirty( false );
			
			Log (string.Format("Cached {0} nodes", m_graphCache.Count ) );
			
			// Move remaining nodes to floating selection
			foreach ( KeyValuePair< ISynthGraphNode, NodeGUIBase > kvp in cache )
			{
				m_floatingSelection.Add( kvp.Key, kvp.Value );
			}
			Log ( string.Format("Floating selection contains {0} nodes", m_floatingSelection.Count ) );
		}	
		
		private void CacheNode( ISynthGraphNode node, Dictionary<ISynthGraphNode, NodeGUIBase> oldCache )
		{
			// If node is part of hierarchy, remove it from floating selection
			if ( m_floatingSelection.ContainsKey( node ) )
			{
				m_graphCache.Add( node, m_floatingSelection[ node ] );
				m_floatingSelection.Remove( node );
			}
			else if ( oldCache.ContainsKey( node ) )
			{
				m_graphCache.Add( node, oldCache[ node ] );
				oldCache.Remove( node );
			}
			else
			{
				if ( !m_graphCache.ContainsKey( node ) )
				{
					m_graphCache.Add( node, SynthGraphEditorFactory.GetGUIControl( node ) );
				}
			}
			
			ISynthGraphNode[] inputs = node.GetSourceNodes();
			
			for( int i = 0; i < inputs.Length; i++ )
			{
				if ( inputs[ i ] != null )
				{
					CacheNode( inputs[ i ], oldCache );
				}
			}
		}
	}
}