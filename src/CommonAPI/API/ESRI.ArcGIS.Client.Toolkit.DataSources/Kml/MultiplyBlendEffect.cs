using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml
{
	/// <summary>
	/// *FOR INTERNAL USE ONLY*
	/// Used to blend KML icons or ground overlays with the specified iconcolor.
	/// </summary>
	/// <remarks>
	/// The effect uses the pixel shader compiled file : MultiplyBlend.cso
	/// The source file MultiplyBlend.fx is provided for info. 
	/// In case it needs to be compiled it can be done with command like: "C:\Program Files (x86)\Windows Kits\8.1\bin\x64\fxc" /T ps_2_0 /E main /Fo”$(ProjectDir)/Kml/MultiplyBlend.cso” “$(ProjectDir)/Kml/MultiplyBlend.fx”
	/// </remarks>
	/// <exclude/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class MultiplyBlendEffect : ShaderEffect
	{
		/// <summary>
		/// A reference to the pixel shader used.
		/// </summary>
		private static readonly PixelShader StaticPixelShader;

		/// <summary>
		/// Creates an instance of the shader from the included pixel shader.
		/// </summary>
		static MultiplyBlendEffect()
		{
			StaticPixelShader = new PixelShader {UriSource = new Uri("/ESRI.ArcGIS.Client.Toolkit.DataSources;component/Kml/MultiplyBlend.cso", UriKind.RelativeOrAbsolute)};
		}

		/// <summary>
		/// The blend color property
		/// </summary>
		public static readonly DependencyProperty BlendColorProperty = DependencyProperty.Register("BlendColor", typeof(Color), typeof(MultiplyBlendEffect), new PropertyMetadata(Colors.White, PixelShaderConstantCallback(0)));

		/// <summary>
		/// The input property
		/// </summary>
		public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(MultiplyBlendEffect), 0);

		/// <summary>
		/// Creates an instance and updates the shader's variables to the default values.
		/// </summary>
		public MultiplyBlendEffect()
		{
			PixelShader = StaticPixelShader;

			UpdateShaderValue(BlendColorProperty);
			UpdateShaderValue(InputProperty);
		}

		/// <summary>
		/// Gets or sets the blend color.
		/// </summary>
		public Color BlendColor
		{
			get { return (Color)GetValue(BlendColorProperty); }
			set { SetValue(BlendColorProperty, value); }
		}

		/// <summary>
		/// Gets or sets the input used in the shader.
		/// </summary>
		[BrowsableAttribute(false)]
		public Brush Input
		{
			get { return (Brush)GetValue(InputProperty); }
			set { SetValue(InputProperty, value); }
		}
	}
}
