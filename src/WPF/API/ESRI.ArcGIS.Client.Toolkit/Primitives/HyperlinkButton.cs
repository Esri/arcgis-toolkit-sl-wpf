using System.Diagnostics;
using System.Windows.Documents;

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
    /// <summary>
    /// *FOR INTERNAL USE ONLY* The HyperlinkButton control.
    /// </summary>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class HyperlinkButton : Hyperlink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperlinkButton"/> class.
        /// </summary>
        public HyperlinkButton() : base()
        {
            this.RequestNavigate += HyperlinkButton_RequestNavigate;
        }

        private void HyperlinkButton_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}