using UnityEngine;
using System.Collections;

using UniSynth2.Editor;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorSplitView : ISynthGraphEditorView
	{
		public enum SplitMode
		{
			HORIZONTAL,
			VERTICAL
		}
		
		public ISynthGraphEditorView Left
		{
			get { return m_leftView; }
		}
		
		public ISynthGraphEditorView Right
		{
			get { return m_rightView; }
		}
		
		protected Rect 		m_viewRect;
		protected float		m_splitPosition;
		protected SplitMode m_splitMode;
		
		protected Rect		m_leftRect;
		protected Rect		m_rightRect;
		
		protected ISynthGraphEditorView m_leftView;
		protected ISynthGraphEditorView m_rightView;
		
		protected bool		m_isResizing;
		
		public SynthGraphEditorSplitView( Rect rect, float splitPoint, SplitMode mode, ISynthGraphEditorView leftView, ISynthGraphEditorView rightView )
		{
			m_isResizing = false;
		
			m_viewRect = rect;
			m_splitPosition = splitPoint;
			
			m_leftView = leftView;
			m_rightView = rightView;
			
			m_splitMode = mode;
			CalcChildren();
		}
		
		public Rect GetRect()
		{
			return m_viewRect;
		}
		
		public void SetRect( Rect rect )
		{
			m_viewRect = rect;
			CalcChildren();
		}
		
		public virtual void Draw()
		{
			ProcessEvents();
		
			if ( m_leftView != null )
			{
				m_leftView.Draw();
			}
			
			if ( m_rightView != null )
			{
				m_rightView.Draw();
			}			
		}
		
		protected void ProcessEvents()
		{
			Event e = Event.current;
			
			switch( e.type )
			{
				case EventType.MouseDown:
					if ( m_viewRect.Contains( e.mousePosition ) )
					{
						switch( m_splitMode )
						{
							case SplitMode.VERTICAL:
								if (  ( e.mousePosition.x > m_viewRect.x + ( m_viewRect.width * m_splitPosition ) - SynthGraphEditorFactory.SPLIT_VIEW_HANDLE_SIZE / 2.0f )
						    	&& ( e.mousePosition.x < m_viewRect.x + ( m_viewRect.width * m_splitPosition ) + SynthGraphEditorFactory.SPLIT_VIEW_HANDLE_SIZE / 2.0f ) )
					   		    {
					   		   		m_isResizing = true;
					   		   		e.Use();
					   		    }
							break;
							
							case SplitMode.HORIZONTAL:
								if (  ( e.mousePosition.y > m_viewRect.y + ( m_viewRect.height * m_splitPosition ) - SynthGraphEditorFactory.SPLIT_VIEW_HANDLE_SIZE / 2.0f )
						    	&& ( e.mousePosition.y < m_viewRect.y + ( m_viewRect.height * m_splitPosition ) + SynthGraphEditorFactory.SPLIT_VIEW_HANDLE_SIZE / 2.0f ) )
								{
									m_isResizing = true;
									e.Use();
								}							
							break;
						}
					}
				break;
				
				case EventType.MouseDrag:
					if ( m_isResizing )
					{					
						switch( m_splitMode )
						{
							case SplitMode.HORIZONTAL:
								m_splitPosition = ( m_leftRect.height + e.delta.y ) / m_viewRect.height;
								m_splitPosition = Mathf.Clamp( m_splitPosition, SynthGraphEditorFactory.SPLIT_VIEW_RANGE_MIN, SynthGraphEditorFactory.SPLIT_VIEW_RANGE_MAX );								
							break;
							
							case SplitMode.VERTICAL:
								m_splitPosition = ( m_leftRect.width + e.delta.x ) / m_viewRect.width;
								m_splitPosition = Mathf.Clamp( m_splitPosition, SynthGraphEditorFactory.SPLIT_VIEW_RANGE_MIN, SynthGraphEditorFactory.SPLIT_VIEW_RANGE_MAX );
							break;
						}
						
						CalcChildren();
						e.Use();
					}
				break;
				
				case EventType.MouseUp:
					m_isResizing = false;
				break;
			}
		}
		
		private void CalcChildren()
		{
			switch( m_splitMode )
			{
				case SplitMode.HORIZONTAL:
				{
					float hleft = m_viewRect.height * m_splitPosition;
					
					m_leftRect.x = m_viewRect.x;
					m_leftRect.y = m_viewRect.y;
					
					m_leftRect.width  = m_viewRect.width;
					m_leftRect.height = hleft;
					
					m_rightRect.x = m_viewRect.x;
					m_rightRect.y = m_viewRect.y + m_leftRect.height;
					
					m_rightRect.width  = m_viewRect.width;
					m_rightRect.height = m_viewRect.height - m_leftRect.height;
				}
				break;
				
				case SplitMode.VERTICAL:
				{
					float wleft = m_viewRect.width * m_splitPosition;
					
					m_leftRect.x = m_viewRect.x;
					m_leftRect.y = m_viewRect.y;
					
					m_leftRect.width  = wleft;
					m_leftRect.height = m_viewRect.height;
					
					m_rightRect.x = m_viewRect.x + m_leftRect.width;
					m_rightRect.y = m_viewRect.y;
					
					m_rightRect.width = m_viewRect.width - m_leftRect.width;
					m_rightRect.height = m_viewRect.height;
				}
				break;
			}
			
			if ( m_leftView != null )
			{
				m_leftView.SetRect( m_leftRect );
			}
			
			if ( m_rightView != null )
			{
				m_rightView.SetRect( m_rightRect );
			}
		}
	}
}