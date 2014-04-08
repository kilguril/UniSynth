using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2.Editor;
using UniSynth2.Editor.Utils;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorGraphConnection 
	{
		public enum ConnectionState
		{
			NO_CONNECTED,
			CONNECTING,
			CONNECTED
		}
		
		public ConnectionState State 
		{
			get { return m_state; }
		}
	
		public NodeGUIBase m_from;
		public NodeGUIBase m_to;
		public int			m_toSlot;
		
		private ConnectionState m_state;
		
		public void Draw()
		{
			Vector2 from	= Vector2.zero;
			Vector2 to		= Vector2.zero;
		
			switch( m_state )
			{
				case ConnectionState.NO_CONNECTED:
					return;
					
				case ConnectionState.CONNECTING:
					if ( m_from != null )
					{
						from = m_from.GetOutputHandle();
					}
					else
					{
						from = m_to.GetInputHandle( m_toSlot );
					}
					
					Event e = Event.current;
					
					if ( e == null )
					{
						to = from;
					}			
					else
					{		
						to = e.mousePosition;
					}
				break;
					
				case ConnectionState.CONNECTED:
					from = m_from.GetOutputHandle();
					to = m_to.GetInputHandle( m_toSlot );
				break;	
			}
			
			int i = 10;
			i++;
			int z = i;
			SynthGraphEditorUtils.DrawLine( from, to );
		}
		
		public void SetFrom( NodeGUIBase from )
		{
			m_from = from;
			UpdateState();
		}
		
		public void SetTo( NodeGUIBase to, int slot )
		{
			m_to = to;
			m_toSlot = slot;
			UpdateState();
		}
		
		public void Reset()
		{
			m_from = null;
			m_to = null;
			UpdateState();
		}
		
		private void UpdateState()
		{
			if ( m_from != null || m_to != null )
			{
				if ( m_from != null && m_to != null )
				{
					m_state = ConnectionState.CONNECTED;
				}
				else
				{
					m_state = ConnectionState.CONNECTING;
				}
			}
			else
			{
				m_state = ConnectionState.NO_CONNECTED;
			}
		}		
	}
}