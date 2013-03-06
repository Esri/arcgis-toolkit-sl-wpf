// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace ESRI.ArcGIS.Client.Toolkit
{
	public partial class ScaleLine
	{
		/// <summary>
		/// Unit used by the scale line control
		/// </summary>
		/// <remarks>The integer value of the enums corresponds to 1/10th of a millimeter</remarks>
		public enum ScaleLineUnit
		{
			/// <summary>
			/// Undefined
			/// </summary>
			Undefined = -1,
			/// <summary>
			/// Decimal degrees
			/// </summary>
			DecimalDegrees = 0,
			/// <summary>
			/// Inches
			/// </summary>
			Inches = 254,
			/// <summary>
			/// Feet
			/// </summary>
			Feet = 3048,
			/// <summary>
			/// Yards
			/// </summary>
			Yards = 9144,
			/// <summary>
			/// Miles
			/// </summary>
			Miles = 16093440,
			/// <summary>
			/// Nautical Miles
			/// </summary>
			NauticalMiles = 18520000,
			/// <summary>
			/// Millimeters
			/// </summary>
			Millimeters = 10,
			/// <summary>
			/// Centimeters
			/// </summary>
			Centimeters = 100,
			/// <summary>
			/// Decimeters
			/// </summary>
			Decimeters = 1000,
			/// <summary>
			/// Meters
			/// </summary>
			Meters = 10000,
			/// <summary>
			/// Kilometers
			/// </summary>
			Kilometers = 10000000
		}
	}

}
