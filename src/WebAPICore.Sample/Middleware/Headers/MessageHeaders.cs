using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace  WebAPICore.Sample.Middleware.Headers
{
    public class MessageHeaders
    {
        //TA defined headers
        public static readonly string MESSAGE_ID = "com-id";
        public static readonly string CUSTOMER_ID = "com-unique-customer-id";
        public static readonly string CORRELATION_ID = "com-correlation-id";
        public static readonly string SERVER_NAME = "com-server-name";
        public static readonly string SERVICE_NAME = "com-service-name";
        public static readonly string SESSION_ID = "com-session-id";
        public static readonly string THREAD_ID = "com-thread-id";
        public static readonly string THREAD_CORRELATION_ID = "com-thread-correlation-id";
        public static readonly string REQUEST_IP = "com-request-ip";
        public static readonly string MIME_TYPE = "com-mimetype";
        public static readonly string ORIGINATION_SYSTEM = "com-origination-system";
        public static readonly string DESTINATION_SYSTEM = "com-destination-system";
        public static readonly string MESSAGE_CATEGORY = "com-message-category";
        public static readonly string TIMESTAMP = "com-utc-timestamp";
        public static readonly string VERSION = "com-version";

        public static readonly string CUSTOM_PREFIX = "com-header-";

        public static string classVersion;

        //fields
        private SourceSystem? originationSystem = null;
        private SourceSystem? destinationSystem = null;
        private MessageCategory? messageCategory = null;
        private long? timestamp;


        /**
        * Default constructor. Sets current  timestamp and version.
        */

        public MessageHeaders()
        {
            this.Version = GetClassVersion();
            this.timestamp = DateTime.UtcNow.Ticks;
        }

        /**
          * Creates a header with the required fields only.
          * @param messageId Unique mesahe ID
          * @param correlationId Message Correlation ID
          */

        public MessageHeaders(string messageId, string correlationId) : this()
        {
            this.MessageId = messageId;
            this.CorrelationId = correlationId ?? messageId;
        }

        /**
          * Parameterized constructor. Test only.
          */

        public MessageHeaders(string messageId, string correlationId, string serverName, string serviceName,
            SourceSystem origin, SourceSystem destination, string requestIp,
            string sessionId, string threadId, string threadCorrelationId, string uniqueCustomerId,
            MessageCategory category, string mimeType) : this(messageId, correlationId)
        {

            this.UniqueCustomerId = uniqueCustomerId;
            this.ServerName = serverName;
            this.ServiceName = serviceName;
            this.RequestIp = requestIp;
            this.SessionId = sessionId;
            this.ThreadId = threadId;
            this.ThreadCorrelationId = threadCorrelationId;
            this.messageCategory = category;
            this.originationSystem = origin;
            this.destinationSystem = destination;
            this.MimeType = mimeType;
        }

        /**
         * Constructor that populates header values from the HTTP Request.
         * @param request HTTP request to extract header values from.
         */

        public MessageHeaders(NameValueCollection headers)
        {
            if (headers == null)
            {
                return;
            }
            SetHeaders(ConvertToDictionary(headers));
        }

        /**
         * Creates a new message headers object and populates it with
         * the values from the <code>headers</code> map.
         * @param headers A map with current header values.
         */
        public MessageHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                return;
            }

            SetHeaders(headers);
        }

        /**
          * Checks if the current set of message headers is valid.
          *
          * Validates that requiered fields (<code>messageId</code>,
          * <code>correlationid</code>, <code>timestamp</code>,
          * and <code>version</code>)  are not empty, and that version matches {@link #classVersion}.
          * @return <code>true</code> if message headers are valid; otherwise <code>false</code>
          */

        public bool IsValid()
        {

            return !string.IsNullOrEmpty(MessageId)
                   && !string.IsNullOrEmpty(CorrelationId)
                   && timestamp != null && timestamp > 0
                   && Version != null && Version.Equals(GetClassVersion());
        }

        /// <summary>
        /// Set Headers
        /// </summary>
        /// <param name="headers"></param>
        private void SetHeaders(Dictionary<string, string> headers)
        {
            foreach (var kvp in headers)
            {
                if (kvp.Value != null)
                {
                    SetHeaderValue(kvp.Key, kvp.Value);
                }
            }
        }

        /**
         * Sets the specified header's <code>value</code>.
         *
         * IMPORTANT: ignores unknown headers.
         *
         * @param name The name of the header.
         * @param value The value to set.
         */
        private void SetHeaderValue(string name, string value)
        {
            if (name.StartsWith(CUSTOM_PREFIX))
            {
                CustomValues.Add(name.Substring(CUSTOM_PREFIX.Length), value);
            }
            else if (MESSAGE_ID.Equals(name))
            {
                this.MessageId = value;
            }
            else if (CUSTOMER_ID.Equals(name))
            {
                this.UniqueCustomerId = value;
            }
            else if (CORRELATION_ID.Equals(name))
            {
                this.CorrelationId = value;
            }
            else if (SERVER_NAME.Equals(name))
            {
                this.ServerName = value;
            }
            else if (SERVICE_NAME.Equals(name))
            {
                this.ServiceName = value;
            }
            else if (THREAD_ID.Equals(name))
            {
                this.ThreadId = value;
            }
            else if (THREAD_CORRELATION_ID.Equals(name))
            {
                this.ThreadCorrelationId = value;
            }
            else if (REQUEST_IP.Equals(name))
            {
                this.RequestIp = value;
            }
            else if (SESSION_ID.Equals(name))
            {
                this.SessionId = value;
            }
            else if (MIME_TYPE.Equals(name))
            {
                this.MimeType = value;
            }
            else if (MESSAGE_CATEGORY.Equals(name))
            {
                this.messageCategory = (MessageCategory)Enum.Parse(typeof(MessageCategory), value);
            }
            else if (ORIGINATION_SYSTEM.Equals(name))
            {
                this.originationSystem = LookupSourceSystem(value);
            }
            else if (DESTINATION_SYSTEM.Equals(name))
            {
                this.destinationSystem = LookupSourceSystem(value);
            }
            else if (TIMESTAMP.Equals(name))
            {
                try
                {
                    this.timestamp = Convert.ToInt64(value);

                }
                catch (FormatException)
                {
                    // eat it for now
                }
            }
            else if (VERSION.Equals(name))
            {
                this.Version = value;
            }
            else
            {
                DefaultValues.Add(name, value);
            }
        }

        /**
         * Returns a map representation of this object.
         * @return a map of key and value pairs for each message header.
         */
        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            AddValue(headers, MESSAGE_ID, MessageId);
            AddValue(headers, CUSTOMER_ID, UniqueCustomerId);
            AddValue(headers, CORRELATION_ID, CorrelationId);
            AddValue(headers, SERVER_NAME, ServerName);
            AddValue(headers, SERVICE_NAME, ServiceName);
            AddValue(headers, THREAD_ID, ThreadId);
            AddValue(headers, THREAD_CORRELATION_ID, ThreadCorrelationId);
            AddValue(headers, REQUEST_IP, RequestIp);
            AddValue(headers, SESSION_ID, SessionId);
            AddValue(headers, MIME_TYPE, MimeType);
            if (messageCategory != null)
            {
                AddValue(headers, MESSAGE_CATEGORY, messageCategory.ToString());
            }
            if (originationSystem != null)
            {
                AddValue(headers, ORIGINATION_SYSTEM, originationSystem.ToString());
            }
            if (destinationSystem != null)
            {
                AddValue(headers, DESTINATION_SYSTEM, destinationSystem.ToString());
            }
            AddValue(headers, TIMESTAMP, timestamp?.ToString());
            AddValue(headers, VERSION, Version);

            // process custom key/value pairs
            foreach (KeyValuePair<string, string> entry in CustomValues)
            {
                AddValue(headers, CUSTOM_PREFIX + entry.Key, entry.Value);
            }
            // process default key/value pairs
            foreach (KeyValuePair<string, string> entry in DefaultValues)
            {
                AddValue(headers, entry.Key, entry.Value);
            }
            return headers;
        }


        /**
          * Adds a specified key/value  to the <code>map</code> only if
          * the <code>value</code> is not <code>null</code> or empty.
          *
          * @param map   The map to add values to.
          * @param key   The key with which the specified value is to be associated
          * @param value  The value to be associated with the specified key
          */
        private void AddValue(Dictionary<string, string> dictionary, string key, string value)
        {
            if (value != null && !string.IsNullOrEmpty(value.Trim()))
            {
                dictionary.Add(key, value);
            }
        }


        public string ServerName { get; set; }

        public string ServiceName { get; set; }

        public string RequestIp { get; set; }

        public string SessionId { get; set; }

        public string UniqueCustomerId { get; set; }

        public string CorrelationId { get; set; }

        public string Version { get; set; }

        public long? Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public Dictionary<string, string> CustomValues { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> DefaultValues { get; set; } = new Dictionary<string, string>();

        public string ThreadId { get; set; }

        public string ThreadCorrelationId { get; set; }

        public MessageCategory? GetMessageCategory()
        {
            return messageCategory;
        }

        public void SetMessageCategory(MessageCategory value)
        {
            this.messageCategory = value;
        }

        public string MessageId { get; set; }

        public string MimeType { get; set; }


        public SourceSystem? GetOriginationSystem()
        {
            return originationSystem;
        }

        public void SetOriginationSystem(SourceSystem value)
        {
            this.originationSystem = value;
        }

        public void SetOriginationSystem(int value)
        {
            this.originationSystem = LookupSourceSystem(value);
        }

        public SourceSystem? GetDestinationSystem()
        {
            return destinationSystem;
        }

        public void SetDestinationSystem(SourceSystem value)
        {
            this.destinationSystem = value;
        }

        public void SetDestinationSystem(int value)
        {
            this.destinationSystem = LookupSourceSystem(value);
        }

        /**
         * Returns class version.
         * The version is extracteding from the package metada 
         * The metadata version is generated by maven at build time.
         * @return current class version.
         */
        public static string GetClassVersion()
        {
            if (classVersion == null)
            {
                //return Assembly.GetEntryAssembly().GetName().Version.ToString();
                return "test";
            }
            return classVersion;
        }

        private SourceSystem? LookupSourceSystem(int value)
        {
            if (Enum.IsDefined(typeof(SourceSystem), value))
            {
                return (SourceSystem)value;
            }
            else
            {
                return null;
            }
        }

        private SourceSystem? LookupSourceSystem(string value)
        {
            return LookupSourceSystem(Convert.ToInt16(value));
        }


        private Dictionary<string, string> ConvertToDictionary(NameValueCollection nvc)
        {
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }


        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;

            MessageHeaders that = (MessageHeaders)o;

            if (!MessageId?.Equals(that.MessageId) ?? that.MessageId != null) return false;
            if (!CorrelationId?.Equals(that.CorrelationId) ?? that.CorrelationId != null)
                return false;
            if (!timestamp?.Equals(that.timestamp) ?? that.MessageId != null) return false;
            if (!Version?.Equals(that.Version) ?? that.Version != null) return false;
            return true;

        }

        public override int GetHashCode()
        {
            int result = MessageId?.GetHashCode() ?? 0;
            result = 31 * result + (CorrelationId?.GetHashCode() ?? 0);
            result = 31 * result + (timestamp?.GetHashCode() ?? 0);
            result = 31 * result + (Version?.GetHashCode() ?? 0);
            return result;
        }

        public override string ToString()
        {
            return "MessageHeaders{" +
                    "messageId='" + MessageId + '\'' +
                    ", version='" + Version + '\'' +
                    '}';
        }

    }
}
