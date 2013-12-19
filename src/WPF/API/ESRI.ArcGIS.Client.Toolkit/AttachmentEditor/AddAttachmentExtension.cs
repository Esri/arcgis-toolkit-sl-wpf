// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Tasks;

namespace ESRI.ArcGIS.Client.Toolkit
{
    /// <summary>
    /// Workaround to the FeatureLayer.AddAttachment bug introduced with ArcGIS Runtime SDK 10.2 for WPF 
    /// To use this version instead of the standard version, change your code from featureLayer.AddAttachment(&lt;args&gt;)
    /// by AddAttachmentExtension.AddAttachment(featureLayer, &lt;args&gt;).
    /// </summary>
    public static class AddAttachmentExtension
    {
        /// <summary>
        /// Adds an attachment to a feature.
        /// </summary>
        /// <param name="featureLayer">The feature layer containing the feature to add an attachment to.</param>
        /// <param name="g">The feature to add an attachment to.</param>
        /// <param name="file">The file stream used for the attachment.</param>
        /// <param name="filename">Name of the attachment.</param>
        /// <param name="callback">The method to call when completed.</param>
        /// <param name="errorCallback">The method to call if an error occurs.</param>
        /// <remarks>Filename extension will be used to determine the MimeType associated with the attachment.</remarks>
        public static void AddAttachment(FeatureLayer featureLayer, Graphic g, Stream file, string filename, Action<AttachmentResult> callback, Action<Exception> errorCallback)
        {
            if (!IsAddAttachmentAllowed(featureLayer, g))
                throw new Exception("NotAllowedToAddFeatureAttachment");
            AddAttachment(featureLayer, g, file, filename, null, callback, errorCallback);
        }

        /// <summary>
        /// Adds an attachment to a feature.
        /// </summary>
        /// <param name="featureLayer">The feature layer containing the feature to add an attachment to.</param>
        /// <param name="g">The feature to add an attachment to.</param>
        /// <param name="file">The file stream used for the attachment.</param>
        /// <param name="filename">Name of the attachment.</param>
        /// <param name="contentType">MimeType of the content.</param>
        /// <param name="callback">The method to call when completed.</param>
        /// <param name="errorCallback">The method to call if an error occurs.</param>
        /// <remarks>Filename extension will be used to determine the MimeType associated with the attachment.</remarks>
        public static void AddAttachment(FeatureLayer featureLayer, Graphic g, Stream file, string filename, string contentType, Action<AttachmentResult> callback, Action<Exception> errorCallback)
        {
            if (!IsAddAttachmentAllowed(featureLayer, g))
                throw new Exception("NotAllowedToAddFeatureAttachment");
            if (!g.Attributes.ContainsKey(featureLayer.LayerInfo.ObjectIdField))
                throw new ArgumentOutOfRangeException("Graphic.Attributes[ObjectID]", "RequiredParameterIsNullOrEmpty");
            string featureID = g.Attributes[featureLayer.LayerInfo.ObjectIdField].ToString();
            AddAttachment(featureLayer, featureID, file, filename, contentType, callback, errorCallback);
        }

        /// <summary>
        /// Adds an attachment to a feature.
        /// </summary>
        /// <param name="featureLayer">The feature layer containing the feature to add an attachment to.</param>
        /// <param name="featureID">The ID of the feature to add an attachment to.</param>
        /// <param name="file">The file stream used for the attachment.</param>
        /// <param name="filename">Name of the attachment.</param>
        /// <param name="callback">The method to call when completed.</param>
        /// <param name="errorCallback">The method to call if an error occurs.</param>
        /// <remarks>Filename extension will be used to determine the MimeType associated with the attachment.</remarks>
        public static void AddAttachment(FeatureLayer featureLayer, string featureID, Stream file, string filename, Action<AttachmentResult> callback, Action<Exception> errorCallback)
        {
            AddAttachment(featureLayer, featureID, file, filename, null, callback, errorCallback);
        }

        /// <summary>
        /// Adds an attachment to a feature.
        /// </summary>
        /// <param name="featureLayer">The feature layer containing the feature to add an attachment to.</param>
        /// <param name="featureID">The ID of the feature to add an attachment to.</param>
        /// <param name="file">The file stream used for the attachment.</param>
        /// <param name="filename">Name of the attachment.</param>
        /// <param name="contentType">MimeType of the content.</param>
        /// <param name="callback">The method to call when completed.</param>
        /// <param name="errorCallback">The method to call if an error occurs.</param>
        /// <remarks>Filename extension will be used to determine the MimeType associated with the attachment.</remarks>
        public static void AddAttachment(FeatureLayer featureLayer, string featureID, Stream file, string filename, string contentType, Action<AttachmentResult> callback, Action<Exception> errorCallback)
        {
            if (string.IsNullOrEmpty(featureID))
                throw new ArgumentNullException("featureID");
            if (file == null)
                throw new ArgumentNullException("file");

            string url;
            try
            {
                if (!featureLayer.IsInitialized || featureLayer.LayerInfo == null)
                    throw new InvalidOperationException("Layer_NotInitialized");
                if (!featureLayer.LayerInfo.HasAttachments)
                    throw new NotSupportedException("FeatureLayer_AttachmentsNotSupported");
                if ((!featureLayer.LayerInfo.IsUpdateAllowed && !featureLayer.LayerInfo.IsAddAllowed) || string.IsNullOrEmpty(featureLayer.Url))
                    throw new NotSupportedException("FeatureLayer_AddAttachmentsNotSupported");
                if (!file.CanRead)
                    throw new InvalidOperationException("FeatureLayer_AttachmentReadAccessFailed");
                url = string.Format("{0}/{1}/addAttachment", featureLayer.Url, featureID);
            }
            catch (Exception ex)
            {
                if (errorCallback != null && featureLayer.Dispatcher != null)
                    featureLayer.Dispatcher.BeginInvoke(errorCallback, ex);
                return;
            }

            var parameters = new Dictionary<string, string> { { "f", "json" } };
            if (!string.IsNullOrEmpty(featureLayer.Token))
                parameters.Add("token", featureLayer.Token);
            if (!string.IsNullOrEmpty(featureLayer.GdbVersion))
            {
                //gdbVersion 10.1 Pre-Release
                parameters.Add("gdbVersion", featureLayer.GdbVersion);
            }

            var webClient = new ArcGISWebClient { ProxyUrl = featureLayer.ProxyUrl };
            webClient.Credentials = featureLayer.Credentials;
            webClient.ClientCertificate = featureLayer.ClientCertificate;

            webClient.PostMultipartCompleted += (sender, res) =>
            {
                Exception error = res.Error;
                if (error == null)
                {
                    string json = null;
                    if (res.Result != null)
                    {
                        using (var reader = new StreamReader(res.Result))
                        {
                            json = reader.ReadToEnd();
                        }
                    }
                    error = ServiceException.FromJson(json);
                    if (error != null)
                    {
                        if (errorCallback != null && featureLayer.Dispatcher != null)
                        {
                            featureLayer.Dispatcher.BeginInvoke(errorCallback, new Exception(error.Message, error.InnerException));
                        }
                    }
                    else if (callback != null)
                    {
                        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(AddAttachmentResults));
                        MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                        AddAttachmentResults value = js.ReadObject(ms) as AddAttachmentResults;
                        AttachmentResult result = value.Result;
                        if (featureLayer.Dispatcher != null)
                            featureLayer.Dispatcher.BeginInvoke(callback, result);
                    }
                }
                else
                {
                    if (errorCallback != null && featureLayer.Dispatcher != null)
                        featureLayer.Dispatcher.BeginInvoke(errorCallback, error);
                }

            };

            var streamContent = new ArcGISWebClient.StreamContent
            {
                ContentType = string.IsNullOrEmpty(contentType) ? GetContentType(filename) : contentType,
                Filename = filename,
                Name = "attachment",
                Stream = file
            };
            webClient.PostMultipartAsync(new Uri(url), parameters, new[] { streamContent });
        }

        private static bool IsAddAttachmentAllowed(FeatureLayer featureLayer, Graphic graphic)
        {
            if (featureLayer.IsReadOnly)
                return false;
            if (!featureLayer.LayerInfo.IsUpdateAllowed && !featureLayer.LayerInfo.IsAddAllowed)
                return false;

            if (!featureLayer.LayerInfo.HasAttachments)
                return false;

            if (!IsOwnershipBasedAccessControl(featureLayer.LayerInfo))
                return true; // Editor tracking or OwnershipBasedAccess control not set

            if (featureLayer.LayerInfo.OwnershipBasedAccessControl.AllowOthersToUpdate)
                return true; // OwnershipBasedAccess control set, but update authorized for everybody

            // Only the creator can update the graphic
            string creator = GetCreator(featureLayer, graphic);
            if (creator == null)
                return false;
            else if (creator.Trim().Length == 0) //anonymous creator in 10.1 Final is empty
                return true; // features from anonymous creator is open for update

            return string.Equals(creator, QualifiedEditUserName(featureLayer), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOwnershipBasedAccessControl(FeatureLayerInfo layerInfo)
        {
            return layerInfo != null && layerInfo.OwnershipBasedAccessControl != null
                && layerInfo.EditFieldsInfo != null && !string.IsNullOrEmpty(layerInfo.EditFieldsInfo.CreatorField)
                && (!layerInfo.OwnershipBasedAccessControl.AllowOthersToDelete || !layerInfo.OwnershipBasedAccessControl.AllowOthersToUpdate);
        }

        private static string GetCreator(FeatureLayer featureLayer, Graphic g)
        {
            string field = featureLayer.LayerInfo.EditFieldsInfo == null ? null : featureLayer.LayerInfo.EditFieldsInfo.CreatorField;
            if (string.IsNullOrEmpty(field))
                return null;
            object att = g.Attributes[field];
            return att == null ? null : att.ToString();
        }

        private static string QualifiedEditUserName(FeatureLayer featureLayer)
        {
            // when not logged, the server set the creator to the "anonymous" string,
            // then only non logged users can modify/delete the feature
            string user = featureLayer.EditUserName;
            if (!string.IsNullOrEmpty(featureLayer.LayerInfo.EditFieldsInfo.Realm))
                user += "@" + featureLayer.LayerInfo.EditFieldsInfo.Realm;
            return user;
        }

        private static string GetContentType(string filename)
        {
            string ext = null;
            if (!string.IsNullOrEmpty(filename))
            {
                int index = filename.LastIndexOf(".", StringComparison.Ordinal);
                if (index != -1)
                {
                    ext = filename.Substring(index + 1);
                }
            }
            return MIME_MAP(ext ?? "");
        }

        private static string MIME_MAP(string fileExt)
        {
            switch (fileExt.ToLower())
            {
                case "avi": return "video/x-msvideo";
                case "csv": return "text/csv";
                case "doc": return "application/msword";
                case "docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "f4v": return "video/mp4";
                case "flv": return "video/x-flv";
                case "gif": return "image/gif";
                case "htm":
                case "html": return "text/html";
                case "jpeg":
                case "jpg": return "image/jpeg";
                case "mov": return "video/quicktime";
                case "mpeg":
                case "mpg": return "video/mpeg";
                case "pdf": return "application/pdf";
                case "png": return "image/png";
                case "ppt": return "application/powerpoint";
                case "pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "swf": return "application/x-shockwave-flash";
                case "tif":
                case "tiff": return "image/tiff";
                case "txt": return "text/plain";
                case "xls": return "application/vnd.ms-excel";
                case "xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "xml": return "text/xml";
                case "z": return "application/x-compress";
                case "zip": return "application/zip";
                default: return "application/octet-stream";
            }
        }
    }

    [DataContract]
    internal sealed class AddAttachmentResults
    {
        [DataMember(Name = "addAttachmentResult")]
        public AttachmentResult Result { get; internal set; }
    }
}
