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
		#region Product information
		public const string		PRODUCT_TITLE		= "UniSynth Editor";
		public const int		VERSION_MAJOR	    = 0;
		public const int		VERSION_MINOR		= 0;
		public const int		VERSION_REVISION	= 1;
		
		public static string GetVersionString()
		{
			return string.Format( "{0}.{1}.{2}", VERSION_MAJOR, VERSION_MINOR, VERSION_REVISION );
		}
		
		public static string GetProductName()
		{
			return "UniSynth Editor";
		}
		
		public static string GetFullProductName()
		{
			return string.Format( "{0} - {1}", GetProductName(), GetVersionString() );
		}
		
		
		#endregion
	
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
		
		// Playback control
		private SynthPlayback m_playback;
		
		public SynthGraphEditor()
		{
			m_editorWindowRect.x = 0.0f;
			m_editorWindowRect.y = 0.0f;
			m_editorWindowRect.width = position.width;
			m_editorWindowRect.height = position.height;
			
			title = GetProductName();
			
			m_playback = new SynthPlayback();
			
			CreateWindows();
			BuildConvultedViewHierarchy();
			RegisterEvents();
		}
		
		private void Update()
		{		
			m_playback.Update();
			
			if ( m_view != null )
			{
				if ( GetViewHierarchyRequiresUpdate( m_view ) )
				{
					Repaint();
				}
			}
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
		
		private bool GetViewHierarchyRequiresUpdate( ISynthGraphEditorView node )
		{
			return m_view.RequiresContiniousUpdates();
		}
		
		private void RegisterEvents()
		{
			m_toolboxWindow.OnToolSelected += HandleToolSelected;
			m_clipWindow.OnPlaybackRequested += HandlePlaybackRequested;
		}
		
		private void HandlePlaybackRequested()
		{
			m_playback.PlayClip( SynthGraphEditorState.ActiveClip );
		}

		private void HandleToolSelected (SynthGraphEditorToolboxWindow.ToolReturnValue tool)
		{
			switch( tool )
			{
				case SynthGraphEditorToolboxWindow.ToolReturnValue.ADD_WAVE_NODE:
					m_graphWindow.AddNode( new SynthNodeGeneratorWave() );
				break;
				
				case SynthGraphEditorToolboxWindow.ToolReturnValue.ADD_CONSTANT_NODE:
					m_graphWindow.AddNode( new SynthNodeConstant() );
				break;
				
				case SynthGraphEditorToolboxWindow.ToolReturnValue.ADD_OPERATION_NODE:
					m_graphWindow.AddNode( new SynthNodeOperation() );
				break;
			}
		}
		
	}
}
