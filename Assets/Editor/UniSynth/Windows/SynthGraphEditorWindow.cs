using UnityEngine;
using System.Collections;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorWindow : ISynthGraphEditorView
	{
		protected int  		m_windowId;
		protected Rect 		m_windowRect;
		protected string 	m_windowTitle;
		
		protected bool		m_requiresContiniousUpdate = false;
		
		public SynthGraphEditorWindow( int windowId, string windowTitle, Rect windowRect )
		{
			m_windowId    = windowId;
			m_windowRect  = windowRect;
			m_windowTitle = windowTitle;
		}
		
		public Rect GetRect()
		{
			return m_windowRect;
		}
		
		public void SetRect( Rect rect )
		{
			m_windowRect = rect;
		}
		
		public void SetTitle( string title )
		{
			m_windowTitle = title;
		}
		
		public virtual void Draw()
		{
			m_windowRect = GUI.Window( 
               m_windowId,
               m_windowRect,
               Window,
               m_windowTitle
       		);
       		
       		ProcessEvents();
		}
		
		public bool RequiresContiniousUpdates ()
		{
			return m_requiresContiniousUpdate;
		}
		
		protected void SetRequiresContiniousUpdates( bool status )
		{
			m_requiresContiniousUpdate = status;
		}
		
		protected virtual void Window( int windowId )
		{
		
		}
		
		protected void ProcessEvents()
		{
			
		}
		
		protected void Log( string s )
		{
			Debug.Log ( string.Format("[{0:00.00}][{1}] {2}", Time.realtimeSinceStartup, this, s ) );
		}
		
		
	
	}
}
