using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml
{
    /// <summary>
    /// Stores an extended data definition for a KML feature.
    /// </summary>
    public class KmlExtendedData
    {
        /// <summary>
        /// The name of the extended data variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The optional display name for the extended data variable.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The value of the extended data variable.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>Returns the value of the object instead of the type information.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
