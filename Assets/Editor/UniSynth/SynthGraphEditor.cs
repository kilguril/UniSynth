using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using UniSynth2.Editor.Windows;

namespace UniSynth2.Editor
{
	public class SynthGraphEditor : EditorWindow 
	{	
		// New clip default values
		public const string		DEFAULT_CLIP_NAME	= "Unnamed Clip";
		public const float		DEFAULT_CLIP_LENGTH = 1.0f;
		public const int		DEFAULT_CLIP_SAMPLE	= 8000;
		public const bool		DEFAULT_CLIP_STEREO = false;
		public const bool		DEFAULT_CLIP_3D		= false;
	
		// Editor primary view
		private ISynthGraphEditorView  m_view;
		
		// Sub windows
		private SynthGraphEditorToolboxWindow  m_toolboxWindow;
		private SynthGraphEditorClipWindow 	   m_clipWindow;
		private SynthGraphEditorGraphWindow	   m_graphWindow;
		private SynthGraphEditorSpectrumWindow m_spectrumWindow;
		private SynthGraphEditorWaveformWindow m_waveformWindow;
		
		// Editor window cached rect to detect screen changes
		private Rect m_editorWindowRect;
		
		public SynthGraphEditor()
		{
			m_editorWindowRect.x = 0.0f;
			m_editorWindowRect.y = 0.0f;
			m_editorWindowRect.width = position.width;
			m_editorWindowRect.height = position.height;
			
			title = "UniSynth Editor";
			
			CreateWindows();
			BuildConvultedViewHierarchy();
			RegisterEvents();
		}
		
		private void OnGUI()
		{
			SizeChanged();
		
			BeginWindows();	
			m_view.Draw();
			EndWindows();
		}	
		
		private void SizeChanged()
		{
			if ( ( position.width != m_editorWindowRect.width ) || ( position.height != m_editorWindowRect.height ) )
			{
				m_editorWindowRect.width = position.width;
				m_editorWindowRect.height = position.height;
				
				m_view.SetRect( m_editorWindowRect );
			}
		}
		
		private void CreateWindows()
		{
			m_clipWindow 		= new SynthGraphEditorClipWindow	  ( 0, "SoundClip", new Rect() );
			m_toolboxWindow 	= new SynthGraphEditorToolboxWindow   ( 1, "Tools", new Rect() );
			m_graphWindow 		= new SynthGraphEditorGraphWindow     ( 2, "", new Rect() );
			m_waveformWindow 	= new SynthGraphEditorWaveformWindow  ( 3, "Waveform", new Rect() );
			m_spectrumWindow 	= new SynthGraphEditorSpectrumWindow  ( 4, "Spectrum", new Rect() )						;
		}
		
		// Hey, it's a honest method name...
		private void BuildConvultedViewHierarchy()
		{
			m_view = new SynthGraphEditorSplitView
			(
				m_editorWindowRect,
				0.25f,
				SynthGraphEditorSplitView.SplitMode.VERTICAL,
				new SynthGraphEditorSplitView(
					new Rect(),
					0.35f,
					SynthGraphEditorSplitView.SplitMode.HORIZONTAL,
					m_clipWindow,
					m_toolboxWindow
				),
				new SynthGraphEditorSplitView(
					new Rect(),
					0.75f,
					SynthGraphEditorSplitView.SplitMode.HORIZONTAL,
					m_graphWindow,
					new SynthGraphEditorSplitView(
						new Rect(),
						0.5f,
						SynthGraphEditorSplitView.SplitMode.VERTICAL,
						m_waveformWindow,
						m_spectrumWindow
					)
				)
			);
		}
		
		private T GetView<T>() where T : ISynthGraphEditorView
		{
			return GetView<T>( m_view );
		}
		
		private T GetView<T>( ISynthGraphEditorView node ) where T : ISynthGraphEditorView
		{
			if ( node is T )
			{
				return (T)node;
			}
			
			if ( node is SynthGraphEditorSplitView )
			{
				T left  = GetView<T>( (node as SynthGraphEditorSplitView ).Left );
				
				if ( !EqualityComparer< T >.Default.Equals( left, default(T) ) )
				{
					return left;
				}
				
				T right = GetView<T>( (node as SynthGraphEditorSplitView ).Right );
				
				if ( !EqualityComparer< T >.Default.Equals( right, default(T) ) )
				{
					return right;
				}
			}
			
			return default(T);
		}
		
		private void RegisterEvents()
		{
			m_toolboxWindow.OnToolSelected += HandleToolSelected;
			
		}

		void HandleToolSelected (SynthGraphEditorToolboxWindow.ToolReturnValue tool)
		{
			switch( tool )
			{
				case SynthGraphEditorToolboxWindow.ToolReturnValue.ADD_WAVE_NODE:
					m_graphWindow.AddNode( new SynthNodeOutputMono() );
				break;
			}
		}
		
	}
}
