// Code generated by Microsoft (R) AutoRest Code Generator 0.9.6.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication.Models
{
    public partial class AvailablePhoneNumbers
    {
        private string _friendlyName;
        
        /// <summary>
        /// Optional. Friendly Name
        /// </summary>
        public string FriendlyName
        {
            get { return this._friendlyName; }
            set { this._friendlyName = value; }
        }
        
        private string _lata;
        
        /// <summary>
        /// Optional. Local Access Transport Area
        /// </summary>
        public string Lata
        {
            get { return this._lata; }
            set { this._lata = value; }
        }
        
        private string _latitude;
        
        /// <summary>
        /// Optional. Latitude
        /// </summary>
        public string Latitude
        {
            get { return this._latitude; }
            set { this._latitude = value; }
        }
        
        private string _longitude;
        
        /// <summary>
        /// Optional. Longitude
        /// </summary>
        public string Longitude
        {
            get { return this._longitude; }
            set { this._longitude = value; }
        }
        
        private bool? _mMS;
        
        /// <summary>
        /// Optional. Mms Enabled?
        /// </summary>
        public bool? MMS
        {
            get { return this._mMS; }
            set { this._mMS = value; }
        }
        
        private string _phoneNumber;
        
        /// <summary>
        /// Optional. Phone Number
        /// </summary>
        public string PhoneNumber
        {
            get { return this._phoneNumber; }
            set { this._phoneNumber = value; }
        }
        
        private string _postalCode;
        
        /// <summary>
        /// Optional. Postal Code
        /// </summary>
        public string PostalCode
        {
            get { return this._postalCode; }
            set { this._postalCode = value; }
        }
        
        private string _rateCenter;
        
        /// <summary>
        /// Optional. Rate Center
        /// </summary>
        public string RateCenter
        {
            get { return this._rateCenter; }
            set { this._rateCenter = value; }
        }
        
        private string _region;
        
        /// <summary>
        /// Optional. Region
        /// </summary>
        public string Region
        {
            get { return this._region; }
            set { this._region = value; }
        }
        
        private bool? _sMS;
        
        /// <summary>
        /// Optional. Sms Enabled?
        /// </summary>
        public bool? SMS
        {
            get { return this._sMS; }
            set { this._sMS = value; }
        }
        
        private bool? _voice;
        
        /// <summary>
        /// Optional. Voice Enabled?
        /// </summary>
        public bool? Voice
        {
            get { return this._voice; }
            set { this._voice = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the AvailablePhoneNumbers class.
        /// </summary>
        public AvailablePhoneNumbers()
        {
        }
        
        /// <summary>
        /// Deserialize the object
        /// </summary>
        public virtual void DeserializeJson(JToken inputObject)
        {
            if (inputObject != null && inputObject.Type != JTokenType.Null)
            {
                JToken friendlyNameValue = inputObject["friendly_name"];
                if (friendlyNameValue != null && friendlyNameValue.Type != JTokenType.Null)
                {
                    this.FriendlyName = ((string)friendlyNameValue);
                }
                JToken lataValue = inputObject["lata"];
                if (lataValue != null && lataValue.Type != JTokenType.Null)
                {
                    this.Lata = ((string)lataValue);
                }
                JToken latitudeValue = inputObject["latitude"];
                if (latitudeValue != null && latitudeValue.Type != JTokenType.Null)
                {
                    this.Latitude = ((string)latitudeValue);
                }
                JToken longitudeValue = inputObject["longitude"];
                if (longitudeValue != null && longitudeValue.Type != JTokenType.Null)
                {
                    this.Longitude = ((string)longitudeValue);
                }
                JToken mMSValue = inputObject["MMS"];
                if (mMSValue != null && mMSValue.Type != JTokenType.Null)
                {
                    this.MMS = ((bool)mMSValue);
                }
                JToken phoneNumberValue = inputObject["phone_number"];
                if (phoneNumberValue != null && phoneNumberValue.Type != JTokenType.Null)
                {
                    this.PhoneNumber = ((string)phoneNumberValue);
                }
                JToken postalCodeValue = inputObject["postal_code"];
                if (postalCodeValue != null && postalCodeValue.Type != JTokenType.Null)
                {
                    this.PostalCode = ((string)postalCodeValue);
                }
                JToken rateCenterValue = inputObject["rate_center"];
                if (rateCenterValue != null && rateCenterValue.Type != JTokenType.Null)
                {
                    this.RateCenter = ((string)rateCenterValue);
                }
                JToken regionValue = inputObject["region"];
                if (regionValue != null && regionValue.Type != JTokenType.Null)
                {
                    this.Region = ((string)regionValue);
                }
                JToken sMSValue = inputObject["SMS"];
                if (sMSValue != null && sMSValue.Type != JTokenType.Null)
                {
                    this.SMS = ((bool)sMSValue);
                }
                JToken voiceValue = inputObject["voice"];
                if (voiceValue != null && voiceValue.Type != JTokenType.Null)
                {
                    this.Voice = ((bool)voiceValue);
                }
            }
        }
    }
}
