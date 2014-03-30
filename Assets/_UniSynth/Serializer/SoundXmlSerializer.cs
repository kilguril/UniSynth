using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace UniSynth
{
	public class SoundXmlSerializer
	{
		private const string					DEFAULT_CLIP_NAME			= "Undefined";
		private const float						DEFAULT_CLIP_LENGTH			= 1.0f;
		private const int						DEFAULT_CLIP_SAMPLERATE		= 1;
		private const bool						DEFAULT_CLIP_STEREO			= false;
		private const bool						DEFAULT_CLIP_3D				= false;
	
		private static WaveShapes.WaveGenerator	DEFAULT_WAVE_SHAPE			= WaveShapes.WaveSquare;
	
		private const float						DEFAULT_KEYFRAME_TIME 		= 0.0f;
		private const float						DEFAULT_KEYFRAME_FREQUENCY  = 1.0f;
		private const float						DEFAULT_KEYFRAME_GAIN		= 0.0f;
	
		// Deserialize from XML
		public static SoundClip FromXML( XmlDocument xml )
		{
			XmlNode clipNode = xml.SelectSingleNode("Clip");
		
			string			   clipName				= ParseString( clipNode, "name", DEFAULT_CLIP_NAME );
			float			   clipLength			= ParseFloat( clipNode, "length", DEFAULT_CLIP_LENGTH );
			int				   clipSampleRate		= ParseInt( clipNode, "sampleRate", DEFAULT_CLIP_SAMPLERATE );
			bool			   clipStereo			= ParseBool( clipNode, "stereo", DEFAULT_CLIP_STEREO );
			bool			   clip3D				= ParseBool( clipNode, "is3D", DEFAULT_CLIP_3D );
			List< ISoundPass > clipSoundPasses		= new List< ISoundPass >();
			
			Dictionary< string, ObservableProperty< float > > paramList	= new Dictionary< string, ObservableProperty< float > >();
			
			foreach( XmlNode paramNode in clipNode.SelectNodes( "Param" ) )
			{
				string paramName = "";
				float  paramVal	 = 0.0f;
				
				if ( !TryParseString( paramNode, "name", out paramName ) )
				{
					continue;
				}
				
				if ( !TryParseFloat( paramNode, "value", out paramVal ) )
				{
					continue;
				}
				
				paramList.Add( paramName, new ObservableProperty< float >( paramVal ) );
			}
						
			foreach( XmlNode waveNode in clipNode.SelectNodes("Wave") )
			{
				clipSoundPasses.Add( ParseWave( waveNode, clipSampleRate, Mathf.FloorToInt( clipSampleRate * clipLength ), paramList ) );
			}
		
			return new SoundClip( clipName, clipLength, clipSampleRate, clipStereo, clip3D, clipSoundPasses.ToArray(), paramList );
		}
		
		// Serialize to XML
		public static XmlDocument ToXML( SoundClip clip )
		{
			throw new NotImplementedException();
		}
		
		private static PassWave ParseWave( XmlNode node, int sampleRate, int sampleLength, Dictionary< string, ObservableProperty< float > > paramList )
		{
			XmlNodeList	keyframeNodes = node.SelectNodes( "Key" );
			WaveKeyframe[] keyframes = new WaveKeyframe[ keyframeNodes.Count + 1 ];
			
			keyframes[ 0 ] = ParseWaveKeyframe( node );
			
			for ( int i = 0; i < keyframeNodes.Count; i++ )
			{
				keyframes[ i + 1 ] = ParseWaveKeyframe( keyframeNodes[ i ] );
			}
			
			// Parse bindings
			XmlNodeList bindingNodes = node.SelectNodes( "Bind" );
			
			foreach( XmlNode bindingNode in bindingNodes )
			{
				int 	targetKey;
				string  sourceKey;
				
				if ( !TryParseString( bindingNode, "source", out sourceKey ) )
				{
					continue;
				}
				
				if ( !paramList.ContainsKey( sourceKey ) )
				{
					continue;
				}
				
				if ( !TryParseInt( bindingNode, "target", out targetKey ) )
				{
					continue;
				}
			
				// Parse binding
				WaveKeyframeDataBinding binding = ParseWaveKeyframeDataBinding( bindingNode );
				
				if ( binding == null )
				{
					continue;
				}
				
				binding.SetSource( paramList[ sourceKey ] );
				binding.SetTarget( keyframes[ targetKey ] );

				keyframes[ targetKey ].AddDataBinding( binding );
			}
			
			return new PassWave( sampleLength, sampleRate, ParseWaveShape( node ), keyframes );
		}
		
		private static WaveKeyframe ParseWaveKeyframe( XmlNode node )
		{
			float time, frequency, gain;
			
			time 	  = ParseFloat( node, "time", DEFAULT_KEYFRAME_TIME );
			frequency = ParseFloat( node, "frequency", DEFAULT_KEYFRAME_FREQUENCY );
			gain 	  = ParseFloat( node, "gain", DEFAULT_KEYFRAME_GAIN );
						
			return new WaveKeyframe( time, frequency, gain );
		}
		
		private static WaveKeyframeDataBinding ParseWaveKeyframeDataBinding( XmlNode node )
		{
			// <Bind source="pChain" target="0" property="frequency" method="range" rangeSource="0,10" rangeTarget="880,1200" />	
			try
			{
				WaveKeyframeDataBinding.BoundProperty boundProperty;
				DataBinding.BindingMode   bindingMode;
			
				switch( node.Attributes[ "property"].InnerText )
				{
					case "time":
						boundProperty = WaveKeyframeDataBinding.BoundProperty.TIME;
						break;
						
					case "frequency":
						boundProperty = WaveKeyframeDataBinding.BoundProperty.FREQUENCY;
						break;
						
					case "gain":
						boundProperty = WaveKeyframeDataBinding.BoundProperty.GAIN;
						break;
						
					default:
						return null;
				}
				
				switch( node.Attributes[ "method" ].InnerText )
				{
					case "range":
						bindingMode = DataBinding.BindingMode.MAP_RANGE;
						break;
						
					case "value":
						bindingMode = DataBinding.BindingMode.VALUE;
						break;
					
					default:
						return null;
				}
				
				float sourceFrom = 0.0f;
				float sourceTo	 = 0.0f;
				
				float targetFrom = 0.0f;
				float targetTo	 = 0.0f;
				
				TryParseFloatRange( node, "rangeSource", out sourceFrom, out sourceTo );
				
				if ( bindingMode == WaveKeyframeDataBinding.BindingMode.MAP_RANGE )
				{
					TryParseFloatRange( node, "rangeTarget", out targetFrom, out targetTo );
				}
			
				return new WaveKeyframeDataBinding( boundProperty, bindingMode, sourceFrom, sourceTo, targetFrom, targetTo );
			}
			catch
			{
				// This WHOLE thing needs to be refactored... for the time being return null on error
				return null;
			}
		}
		
		private static WaveShapes.WaveGenerator ParseWaveShape( XmlNode node )
		{
			try
			{
				string shapeIdentifier = node.Attributes[ "shape" ].InnerText;
				
				switch( shapeIdentifier )
				{
					case "Square":
						return WaveShapes.WaveSquare;
					
					case "Triangle":
						return WaveShapes.WaveTriangle;
					
					case "Saw":
						return WaveShapes.WaveSaw;
					
					case "Sine":
						return WaveShapes.WaveSine;
					
					case "SineLP":
						return WaveShapes.WaveSineLowP;
					
					case "SineHP":
						return WaveShapes.WaveSineHighP;
					
					default:
						return DEFAULT_WAVE_SHAPE;
				}
			}
			catch
			{
				return DEFAULT_WAVE_SHAPE;
			}
		}
		
		private static bool TryParseString ( XmlNode node, string attributeName, out string result )
		{
			result = "";
		
			try 
			{
				result = node.Attributes[ attributeName ].InnerText;
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		private static string ParseString( XmlNode node, string attributeName, string defaultValue )
		{
			try
			{
				return node.Attributes[ attributeName ].InnerText;
			}
			catch
			{
				return defaultValue;
			}
		}
		
		private static bool TryParseFloat( XmlNode node, string attributeName, out float result )
		{
			result = 0.0f;
		
			try 
			{
				result = float.Parse( node.Attributes[ attributeName ].InnerXml );
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		private static float ParseFloat( XmlNode node, string attributeName, float defaultValue )
		{
			try
			{
				return float.Parse( node.Attributes[ attributeName ].InnerText );
			}
			catch
			{
				return defaultValue;
			}
		}
		
		private static bool TryParseFloatRange( XmlNode node, string attributeName, out float from, out float to )
		{
			from = 0.0f;
			to 	 = 0.0f;
		
			try 
			{
				string[] splitVals = node.Attributes[ attributeName ].InnerText.Split(',');
				
				from = float.Parse( splitVals[ 0 ] );
				
				if ( splitVals.Length < 2 )
				{
					to = from;
				}
				else
				{
					to = float.Parse( splitVals[ 1 ] );
				}
				
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		private static bool TryParseInt( XmlNode node, string attributeName, out int result )
		{
			result = 0;
			
			try 
			{
				result = int.Parse( node.Attributes[ attributeName ].InnerXml );
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		private static int ParseInt( XmlNode node, string attributeName, int defaultValue )
		{
			try
			{
				return int.Parse( node.Attributes[ attributeName ].InnerText );
			}
			catch
			{
				return defaultValue;
			}
		}
		
		private static bool ParseBool( XmlNode node, string attributeName, bool defaultValue )
		{
			try
			{
				return bool.Parse( node.Attributes[ attributeName ].InnerText );
			}
			catch
			{
				return defaultValue;
			}
		}
	}
}
