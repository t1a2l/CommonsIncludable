using System;

namespace Commons.Utils
{
    public static class PrefabUtils
    {
        /// <summary>
        /// Returns a cleaned-up display name for the given prefab.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <returns>Cleaned display name.</returns>
        public static string GetDisplayName(PrefabInfo prefab)
        {
            // Null check.
            if (!prefab || prefab.name == null)
            {
                return "null";
            }

            // Try getting any localized name first.
            string localizedName = prefab.GetUncheckedLocalizedTitle();

            // Perform cleanup.
            return GetDisplayName(localizedName);
        }

        /// <summary>
        /// Sanitises a raw prefab name for display.
        /// </summary>
        /// <param name="prefabName">Prefab name.</param>
        /// <returns>Cleaned display name.</returns>
        public static string GetDisplayName(string prefabName)
        {
            if (prefabName == null)
            {
                return "null";
            }

            // Workshop prefab names can start with a numeric package ID followed by a dot.
            // A dot elsewhere (for example, PR100.2) is part of the asset name.
            int dotIndex = prefabName.IndexOf('.');

            if (dotIndex > 0)
            {
                bool numericPrefix = true;

                for (int i = 0; i < dotIndex; i++)
                {
                    if (prefabName[i] < '0' || prefabName[i] > '9')
                    {
                        numericPrefix = false;
                        break;
                    }
                }

                if (numericPrefix)
                {
                    prefabName = prefabName.Substring(dotIndex + 1);
                }
            }

            // Remove only the prefab suffix, not an occurrence inside the name.
            const string dataSuffix = "_Data";
            if (prefabName.EndsWith(dataSuffix, StringComparison.Ordinal))
            {
                prefabName = prefabName.Substring(0, prefabName.Length - dataSuffix.Length);
            }

            return prefabName;
        }

        /// <summary>
        /// Checks if this asset is a workshop asset (ie. has a workshop ID associated with it).
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <returns>True if this is a workshop asset, false otherwise.</returns>
        public static bool IsWorkshopAsset(PrefabInfo prefab)
        {
            if (!prefab || string.IsNullOrEmpty(prefab.name))
            {
                return false;
            }

            string name = prefab.name;
            int dotIndex = name.IndexOf('.');

            if (dotIndex <= 0 || dotIndex == name.Length - 1)
            {
                return false;
            }

            for (int i = 0; i < dotIndex; i++)
            {
                if (name[i] < '0' || name[i] > '9')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
