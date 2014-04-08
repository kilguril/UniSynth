using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2.Editor;
using UniSynth2.Editor.Utils;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorSpectrumWindow : SynthGraphEditorWindow
	{	
		private const float		MIN_GRAPH_WIDTH 			= 100.0f;
		private const float		MIN_GRAPH_HEIGHT 			= 90.0f; 
		
		private const float		GRAPH_MARGIN_HORIZONTAL		= 20.0f;
		private const float		GRAPH_MARGIN_VERTICAL		= 30.0f;
		
		private enum SampleCount : int
		{
			s64		= 64,
			s128	= 128,
			s256	= 256,
			s512	= 512,
			s1024	= 1024,
			s2048	= 2048,
			s4096	= 4096,
			s8192   = 8192
		}
		
		private SampleCount		m_sampleCount	= SampleCount.s1024;
		private FFTWindow		m_fftWindow		= FFTWindow.Hanning;
		
		private SynthPlayback	m_playbackObject;
	
		public SynthGraphEditorSpectrumWindow( int windowId, string windowTitle, Rect windowRect ) : base( windowId, windowTitle, windowRect )
		{
			SynthPlayback.OnSynthPlaybackStarted += HandlePlaybackStarted;
			SynthPlayback.OnSynthPlaybackEnded	 += HandlePlaybackEnded;
		}
	
		protected override void Window (int windowId)
		{
			EditorGUILayout.BeginHorizontal();		
			m_sampleCount = (SampleCount)SynthGraphEditorFactory.EnumFieldWithLabel( m_sampleCount, "Sample Count" );
			m_fftWindow   = (FFTWindow)SynthGraphEditorFactory.EnumFieldWithLabel( m_fftWindow, "Window Type" );
			EditorGUILayout.EndHorizontal();
			
			if ( ( m_windowRect.width >= MIN_GRAPH_WIDTH) && ( m_windowRect.height >= MIN_GRAPH_HEIGHT ) )
			{
				DrawGrid();
			
				if ( m_playbackObject == null )
				{
					SetRequiresContiniousUpdates( false );
				}
				else
				{
					DrawPlaybackSpectrum();	
				}
			}
		}	
		
		private void DrawGrid()
		{
			SynthGraphEditorUtils.BeginLines( Color.white );
			
			float x,y;
			Vector2 from, to;						
			
			x = GRAPH_MARGIN_HORIZONTAL;
			
			from.x = x;
			from.y = GRAPH_MARGIN_VERTICAL + SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT;			
			to.x   = x;
			to.y   = m_windowRect.height - GRAPH_MARGIN_VERTICAL;
			SynthGraphEditorUtils.DrawLine( from, to );			
			
			y = m_windowRect.height - GRAPH_MARGIN_VERTICAL;
			from.x 	= GRAPH_MARGIN_HORIZONTAL;
			from.y 	= y;
			to.x 	= m_windowRect.width - GRAPH_MARGIN_HORIZONTAL;
			to.y 	= y;
			SynthGraphEditorUtils.DrawLine( from, to );			
			
			SynthGraphEditorUtils.EndLines();		
		}
		
		private void DrawPlaybackSpectrum()
		{
			AudioSource source = m_playbackObject.Source;
		
			if ( source != null )
			{
				float x = GRAPH_MARGIN_HORIZONTAL;
				float y = m_windowRect.height - GRAPH_MARGIN_VERTICAL;
			
				Vector2 from, to;
				
				SynthGraphCoordMapper mapper = new SynthGraphCoordMapper( 0.0f, 1.0f, y, GRAPH_MARGIN_VERTICAL + SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT, false );
			
				float[] samples = new float[(int)m_sampleCount];
				source.GetSpectrumData( samples, 0, m_fftWindow );
				
				SynthGraphEditorUtils.BeginLines( Color.green );
				
				for ( int i = 0; i < samples.Length; i++ )
				{
					from.x = x;
					from.y = y;
					
					to.x   = x;
					to.y   = mapper.MapFromTo( samples[ i ] );
				
					SynthGraphEditorUtils.DrawLine( from, to );
							
					x++;
				}
				
				SynthGraphEditorUtils.EndLines();
			}
		}
	
		private void HandlePlaybackStarted( SynthPlayback playback )
		{
			m_playbackObject = playback;
			SetRequiresContiniousUpdates( true );
		}
		
		private void HandlePlaybackEnded( )
		{		
			m_playbackObject = null;
			SetRequiresContiniousUpdates( false );
		}
	}
}