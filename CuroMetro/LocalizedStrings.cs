﻿using Croumetro.Resources;

namespace CrouMetro
{
    /// <summary>
    ///     Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static readonly AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources
        {
            get { return _localizedResources; }
        }
    }
}