using System;
using System.Diagnostics;
using System.IO;
using System.Net;

using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;
using XAP.Interface;

namespace XAP.Client
{
    public class ApiClient
    {
        private readonly DataContractSerializer _dcs = new DataContractSerializer(typeof(AlertInstance), new[] { typeof(AlertProperty) });

        private readonly Uri _endpoint;

        public ApiClient(string endpoint)
            : this(new Uri(endpoint))
        {
        }

        public ApiClient(Uri endpoint)
        {
            _endpoint = endpoint;
        }

        public ApiClient()
        {

        }

        /// <summary>
        /// Creates a new, empty alert instance with the minimum properties that XAP knows about.
        /// 
        /// Property Name               Description
        /// -------------               -----------
        /// DefinitionId                An identifier for the rule, this should not change between instances
        /// SourceAlertId	            An identifier for the instance, this should be unique per alert instance and the repeats of the alert
        /// Source          Required    The friendly name of the alert source (SCOM, CertScanner, etc)
        /// SourceSystem	            The machine that fired the alert
        /// SourceId	                The ID for the alert source
        /// SourceCluster	            AP Cluster
        /// SourceEnvironment	        AP Environment
        /// EscalationId    Required    Inventory Id from the service catalog items in the service inventory
        /// AdditionalEscalationIds	    A CSV list of additional owners’ Inventory ids
        /// FiredTime	    Required    The time the alert instance fired (or repeated)
        /// Priority	    Required    Priority level 1 - 5
        /// Name	                    The friendly name of the rule, this should not change between instances.
        /// Title	        Required    Title of the alert
        /// Description	    Required    Description of the alert
        /// ImpactStatement	            A XOC Approved impact statement
        /// KnowledgeArticleId	        A XOC Knowledge Article (KAxxxxx)
        /// Attachment	                A string of text that will be attached to the ticket
        /// AttachmentFilename	        The file name of the attachment
        /// </summary>
        /// <returns></returns>
        public AlertInstance GetNewAlertInstance()
        {
            AlertInstance instance = new AlertInstance();
            //todo: call in to XAP to try and populate this list, enabling validation.
            instance.AddProperty("Title", string.Empty);
            instance.AddProperty("DefinitionId", string.Empty);
            instance.AddProperty("SourceAlertId", string.Empty);
            instance.AddProperty("Source", string.Empty);
            instance.AddProperty("SourceSystem", string.Empty);
            instance.AddProperty("SourceId", string.Empty);
            instance.AddProperty("SourceCluster", string.Empty);
            instance.AddProperty("SourceEnvironment", string.Empty);
            instance.AddProperty("EscalationId", string.Empty);
            instance.AddProperty("AdditionalEscalationIds", string.Empty);
            instance.AddProperty("FiredTime", string.Empty);
            instance.AddProperty("Priority", string.Empty);
            instance.AddProperty("Name", string.Empty);
            instance.AddProperty("Description", string.Empty);
            instance.AddProperty("ImpactStatement", string.Empty);
            instance.AddProperty("KnowledgeArticleId", string.Empty);
            instance.AddProperty("Attachment", string.Empty);
            instance.AddProperty("AttachmentFilename", string.Empty);

            return instance;
        }

        /// <summary>
        /// Reconstructs an AlertInstance from an XML string.
        /// </summary>
        /// <param name="xml">An XML string that represents an AlertInstance</param>
        /// <returns>An <see cref="AlertInstance" /> representation of the XML string.</returns>
        public AlertInstance FromXml(string xml)
        {
            AlertInstance ai;
            if (AlertInstance.TryParse(xml, out ai))
            {
                return ai;
            }

            return null;
        }

        /// <summary>
        /// Reconstructs an AlertInstance from an XML string.
        /// </summary>
        /// <param name="xml">An XmlDocument that represents an AlertInstance</param>
        /// <returns>An <see cref="AlertInstance" /> representation of the XML string.</returns>
        public AlertInstance FromXml(XmlDocument xml)
        {
            return FromXml(xml.OuterXml);
        }

        /// <summary>
        /// Converts a <see cref="AlertInstance"/> to an XML string
        /// </summary>
        /// <param name="alert">The AlertInstance to convert to XML</param>
        /// <returns>An XML string representation of the AlertInstance</returns>
        public string ToXml(AlertInstance alert)
        {
            return alert.AsXml();
        }

        /// <summary>
        /// Sends the <see cref="AlertInstance"/> to XAP.
        /// </summary>
        /// <param name="alert">The AlertInstance to send</param>
        /// <returns>The XAP ID assigned to the AlertInstance.</returns>
        public Guid SendAlert(AlertInstance alert)
        {
            using (Stream alertStream = GenerateAlertXml(alert))
            {
                return SendAlert(alertStream);
            }
        }

        private Stream GenerateAlertXml(AlertInstance alert)
        {
            MemoryStream stream = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                IndentChars = "\t",
                Indent = true
            };

            XmlWriter xw = XmlWriter.Create(stream, settings);

            _dcs.WriteObject(xw, alert);
            xw.Flush();
            stream.Flush();
            stream.Position = 0;
            return stream;
        }

        private Guid SendAlert(Stream xmlStream)
        {
            AlertInstance aiResponse = null;
            using (WebClientEx wc = new WebClientEx())
            {

                try
                {
                    int retries = 0;
                    const int maxRetries = 5;

                    do
                    {
                        Thread.Sleep(retries * 100);
                        retries++;
                        try
                        {
                            using (var ms = new MemoryStream())
                            {
                                xmlStream.CopyTo(ms);
                                ms.Seek(0, SeekOrigin.Begin);

                                var response = wc.UploadData(_endpoint, "POST", ms.ToArray());

                                using (var ms1 = new MemoryStream(response))
                                {
                                    var sr = new StreamReader(ms1);

                                    var xmlStr = sr.ReadToEnd();

                                    aiResponse = FromXml(xmlStr);
                                    if (aiResponse != null)
                                    {
                                        return aiResponse.XapId;
                                    }
                                }
                            }
                        }
                        catch (AggregateException ex)
                        {
                            foreach (Exception e in ex.InnerExceptions)
                            {
                                Debug.WriteLine("Unable to communicate with XAP after {1} attempts. {0}",
                                    e.Message, retries);
                            }

                        }
                    } while (retries < maxRetries);

                    if (retries == maxRetries)
                    {
                        Debug.WriteLine("Error calling XAP. Error: {0} attempts were made to communicate with XAP",
                            retries);
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error calling XAP. Error: {0}", ex);
                    aiResponse = null;
                    throw;
                }

                if (aiResponse != null)
                {
                    return aiResponse.XapId;
                }
                else
                {
                    Debug.WriteLine("Unable to parse the response from XAP.");
                    return Guid.Empty;
                }
            }
        }
    }

    public class WebClientEx : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var wr = base.GetWebRequest(address);
            wr.ContentType = "text/xml";

            return wr;
        }
    }
}
