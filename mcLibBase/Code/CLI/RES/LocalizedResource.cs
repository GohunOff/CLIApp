using System;
using System.Globalization;
using System.Resources;

namespace MC.Code.CLI.RES
{
    internal sealed class LocalizedResource
    {
        private readonly ResourceManager _resourceManager;

        public LocalizedResource(ResourceManager resourceManager)
        {
            if (resourceManager == null)
                throw new ArgumentNullException(nameof(resourceManager));

            _resourceManager = resourceManager;
        }

        public string Get(
            string key,
            params object[] args)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(
                    "The resource key cannot be empty.",
                    nameof(key));

            string message =
                _resourceManager.GetString(
                    key,
                    CultureInfo.CurrentUICulture);

            /*
             * Zachowujemy bezpieczne zachowanie:
             * jeśli zasobu nie ma, zwracamy jego klucz.
             */
            if (message == null)
                return key;

            if (args == null || args.Length == 0)
                return message;

            return string.Format(
                CultureInfo.CurrentCulture,
                message,
                args);
        }

        public bool TryGet(
            string key,
            out string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(
                    "The resource key cannot be empty.",
                    nameof(key));

            value =
                _resourceManager.GetString(
                    key,
                    CultureInfo.CurrentUICulture);

            return value != null;
        }
    }
}
