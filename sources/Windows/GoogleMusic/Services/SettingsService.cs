﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.Services
{
    using System;
    using System.Reflection;

    using OutcoldSolutions.Diagnostics;
    using OutcoldSolutions.GoogleMusic.Models;

    using Windows.Storage;

    public class SettingsService : ISettingsService
    {
        private const string SettingsContainerKey = "Settings";
        private const string RoamingSettingsContainerKey = "RoamingSettings";

        private static readonly Type DateTimeType = typeof(DateTime);
        private static readonly Type DateTimeNullableType = typeof(DateTime?);

        private readonly IEventAggregator eventAggregator;

        private readonly ApplicationDataContainer settingsContainer;
        private readonly ApplicationDataContainer roamingSettingsContainer;
        
        private readonly ILogger logger;

        public SettingsService(
            ILogManager logManager,
            IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logManager.CreateLogger("SettingsService");
            var localSettings = ApplicationData.Current.LocalSettings;
            var roamingSettings = ApplicationData.Current.RoamingSettings;

            if (localSettings.Containers.ContainsKey(SettingsContainerKey))
            {
                this.settingsContainer = localSettings.Containers[SettingsContainerKey];
            }
            else
            {
                this.settingsContainer = localSettings.CreateContainer(
                    SettingsContainerKey, ApplicationDataCreateDisposition.Always);
            }

            if (roamingSettings.Containers.ContainsKey(RoamingSettingsContainerKey))
            {
                this.roamingSettingsContainer = roamingSettings.Containers[RoamingSettingsContainerKey];
            }
            else
            {
                this.roamingSettingsContainer = localSettings.CreateContainer(
                    RoamingSettingsContainerKey, ApplicationDataCreateDisposition.Always);
            }
        }

        public void SetValue<T>(string key, T value)
        {
            this.logger.Debug("Setting value of key '{0}' to '{1}.'", key, value);
            object oldValue = this.settingsContainer.Values[key];
            this.settingsContainer.Values[key] = this.ToObject(value);
            this.RaiseValueChanged(key, oldValue, value);
        }

        public void RemoveValue(string key)
        {
            this.logger.Debug("Remove value of key '{0}'", key);
            if (this.settingsContainer.Values.ContainsKey(key))
            {
                object oldValue = this.settingsContainer.Values[key];
                this.settingsContainer.Values.Remove(key);
                this.RaiseValueChanged(key, oldValue, null);
            }
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            this.logger.Debug("Getting value of key '{0}'", key);
            if (this.settingsContainer.Values.ContainsKey(key))
            {
                try
                {
                    return this.ParseValue<T>(this.settingsContainer.Values[key]);
                }
                catch (Exception e)
                {
                    this.logger.Error(e, "Cannot get value");
                }
            }

            return defaultValue;
        }

        public void SetRoamingValue<T>(string key, T value)
        {
            this.logger.Debug("Setting roaming value of key '{0}' to '{1}.'", key, value);
            object oldValue = this.roamingSettingsContainer.Values[key];
            object newValue = this.ToObject(value);
            this.roamingSettingsContainer.Values[key] = newValue;
            this.RaiseValueChanged(key, oldValue, newValue);
        }

        public T GetRoamingValue<T>(string key, T defaultValue = default(T))
        {
            this.logger.Debug("Getting roaming value of key '{0}'", key);
            if (this.roamingSettingsContainer.Values.ContainsKey(key))
            {
                try
                {
                    return this.ParseValue<T>(this.roamingSettingsContainer.Values[key]);
                }
                catch (Exception e)
                {
                    this.logger.Error(e, "Cannot get roaming value");
                }
            }

            return defaultValue;
        }

        public void RemoveRoamingValue(string key)
        {
            this.logger.Debug("Removing roaming value of key '{0}'.'", key);
            if (this.roamingSettingsContainer.Values.ContainsKey(key))
            {
                object oldValue = this.roamingSettingsContainer.Values[key];
                this.roamingSettingsContainer.Values.Remove(key);
                this.RaiseValueChanged(key, oldValue, null);
            }
        }

        protected virtual void RaiseValueChanged(string key, object newValue, object oldValue)
        {
            this.eventAggregator.Publish(new SettingsChangeEvent(key, newValue, oldValue));
        }

        private T ParseValue<T>(object value)
        {
            var clrType = typeof(T);
            if (clrType.GetTypeInfo().IsEnum)
            {
                return (T)Enum.Parse(clrType, value.ToString());
            }
            else if (clrType == DateTimeType || clrType == DateTimeNullableType)
            {
                return (T)(object)DateTime.FromBinary((long)value);
            }
            else if (clrType == DateTimeNullableType)
            {
                if (value == null)
                {
                    return (T)(object)null;
                }
                else
                {
                    return (T)(object)DateTime.FromBinary((long)value);
                }
            }
            else
            {
                return (T)value;
            }
        }

        private object ToObject<T>(T value)
        {
            Type type = typeof(T);
            if (type.GetTypeInfo().IsEnum)
            {
                return Convert.ToInt32(value);
            }
            else if (type == DateTimeType)
            {
                long binaryValue = ((DateTime)(object)value).ToBinary();
                return binaryValue;
            }
            else if (type == DateTimeNullableType)
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    long binaryValue = ((DateTime)(object)value).ToBinary();
                    return binaryValue;
                }
            }
            else
            {
                return value;
            }
        }
    }
}